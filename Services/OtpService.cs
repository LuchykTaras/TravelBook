using System.Net.Mail;
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using TravelBook.Api.Data;
using TravelBook.Api.Models;

namespace TravelBook.Api.Services
{
    public sealed class OtpService
    {
        private const int OtpLifetimeMinutes = 10;
        private const int MaxFailedAttempts = 5;
        private const int Pbkdf2Iterations = 100_000;

        private readonly ServerDbContext _db;

        public OtpService(ServerDbContext db)
        {
            _db = db;
        }

        public static string NormalizeEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return string.Empty;
            }

            email = email.Trim();

            if (!MailAddress.TryCreate(email, out var address))
            {
                return string.Empty;
            }

            return address.Address.ToLowerInvariant();
        }

        public static bool IsBestEmail(string email)
        {
            var normalized = NormalizeEmail(email);

            if (string.IsNullOrWhiteSpace(normalized))
            {
                return false;
            }

            if (!MailAddress.TryCreate(normalized, out var address))
            {
                return false;
            }

            return address.Host.Equals(
                "best-eu.org",
                StringComparison.OrdinalIgnoreCase);
        }

        public async Task<string> CreateOtpAsync(string email)
        {
            var normalizedEmail = NormalizeEmail(email);

            if (!IsBestEmail(normalizedEmail))
            {
                throw new InvalidOperationException(
                    "Доступ дозволено лише для пошти @best-eu.org.");
            }

            var now = DateTime.UtcNow;

            // Робимо всі старі активні коди недійсними.
            var previousCodes = await _db.OtpCodes
                .Where(x =>
                    x.Email == normalizedEmail &&
                    x.UsedAtUtc == null &&
                    x.ExpiresAtUtc > now)
                .ToListAsync();

            foreach (var previousCode in previousCodes)
            {
                previousCode.UsedAtUtc = now;
            }

            var code = RandomNumberGenerator
                .GetInt32(0, 1_000_000)
                .ToString("D6");

            var otp = new OtpCode
            {
                Email = normalizedEmail,
                CodeHash = HashCode(code),
                CreatedAtUtc = now,
                ExpiresAtUtc = now.AddMinutes(OtpLifetimeMinutes),
                UsedAtUtc = null,
                FailedAttempts = 0
            };

            _db.OtpCodes.Add(otp);

            await _db.SaveChangesAsync();

            return code;
        }

        public async Task<bool> VerifyOtpAsync(
            string email,
            string code)
        {
            var normalizedEmail = NormalizeEmail(email);

            if (!IsBestEmail(normalizedEmail))
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(code))
            {
                return false;
            }

            code = code.Trim();

            var now = DateTime.UtcNow;

            var otp = await _db.OtpCodes
                .Where(x =>
                    x.Email == normalizedEmail &&
                    x.UsedAtUtc == null &&
                    x.ExpiresAtUtc > now)
                .OrderByDescending(x => x.CreatedAtUtc)
                .FirstOrDefaultAsync();

            if (otp is null)
            {
                return false;
            }

            if (otp.FailedAttempts >= MaxFailedAttempts)
            {
                return false;
            }

            if (!VerifyCode(code, otp.CodeHash))
            {
                otp.FailedAttempts++;

                if (otp.FailedAttempts >= MaxFailedAttempts)
                {
                    otp.UsedAtUtc = now;
                }

                await _db.SaveChangesAsync();

                return false;
            }

            otp.UsedAtUtc = now;

            await _db.SaveChangesAsync();

            return true;
        }

        private static string HashCode(string code)
        {
            var salt = RandomNumberGenerator.GetBytes(16);

            var hash = Rfc2898DeriveBytes.Pbkdf2(
                code,
                salt,
                Pbkdf2Iterations,
                HashAlgorithmName.SHA256,
                32);

            return
                $"{Convert.ToBase64String(salt)}." +
                $"{Convert.ToBase64String(hash)}";
        }

        private static bool VerifyCode(
            string code,
            string storedValue)
        {
            try
            {
                var parts = storedValue.Split('.');

                if (parts.Length != 2)
                {
                    return false;
                }

                var salt = Convert.FromBase64String(parts[0]);
                var expectedHash =
                    Convert.FromBase64String(parts[1]);

                var actualHash = Rfc2898DeriveBytes.Pbkdf2(
                    code,
                    salt,
                    Pbkdf2Iterations,
                    HashAlgorithmName.SHA256,
                    expectedHash.Length);

                return CryptographicOperations.FixedTimeEquals(
                    actualHash,
                    expectedHash);
            }
            catch
            {
                return false;
            }
        }
    }
}