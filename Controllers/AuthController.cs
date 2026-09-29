using Microsoft.AspNetCore.Mvc;
using TravelBook.Api.Services;

namespace TravelBook.Api.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public sealed class AuthController : ControllerBase
    {
        private readonly OtpService _otpService;
        private readonly ResendEmailService _emailService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(
            OtpService otpService,
            ResendEmailService emailService,
            ILogger<AuthController> logger)
        {
            _otpService = otpService;
            _emailService = emailService;
            _logger = logger;
        }

        // POST:
        // /api/auth/request-code
        [HttpPost("request-code")]
        public async Task<IActionResult> RequestCode(
            [FromBody] RequestCodeRequest request)
        {
            var email = OtpService.NormalizeEmail(request.Email);

            if (string.IsNullOrWhiteSpace(email))
            {
                return BadRequest(new
                {
                    ok = false,
                    message = "Введіть коректну email-адресу."
                });
            }

            if (!OtpService.IsBestEmail(email))
            {
                return BadRequest(new
                {
                    ok = false,
                    message =
                        "Доступ до розділу «Традиції» дозволено лише для пошти @best-eu.org."
                });
            }

            try
            {
                // 1. Генеруємо OTP.
                // У БД зберігається тільки hash.
                var code =
                    await _otpService.CreateOtpAsync(email);

                // 2. Надсилаємо реальний код через Resend.
                await _emailService.SendOtpAsync(
                    email,
                    code);

                return Ok(new
                {
                    ok = true,
                    email,
                    expiresInSeconds = 600,
                    message =
                        "Код підтвердження надіслано на BEST-пошту."
                });
            }
            catch (InvalidOperationException exception)
            {
                _logger.LogWarning(
                    exception,
                    "OTP request rejected for {Email}.",
                    email);

                return BadRequest(new
                {
                    ok = false,
                    message = exception.Message
                });
            }
            catch (Exception exception)
            {
                _logger.LogError(
                    exception,
                    "Failed to send OTP to {Email}.",
                    email);

                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    new
                    {
                        ok = false,
                        message =
                            "Не вдалося надіслати код підтвердження."
                    });
            }
        }
    }

    public sealed class RequestCodeRequest
    {
        public string Email { get; set; } =
            string.Empty;
    }
}