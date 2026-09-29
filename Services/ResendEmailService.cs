using Resend;

namespace TravelBook.Api.Services
{
    public sealed class ResendEmailService
    {
        private readonly IResend _resend;
        private readonly IConfiguration _configuration;

        public ResendEmailService(
            IResend resend,
            IConfiguration configuration)
        {
            _resend = resend;
            _configuration = configuration;
        }

        public async Task SendOtpAsync(
            string email,
            string code)
        {
            var from = _configuration["Resend:From"];

            if (string.IsNullOrWhiteSpace(from))
            {
                throw new InvalidOperationException(
                    "Resend:From is not configured.");
            }

            var message = new EmailMessage
            {
                From = from,
                Subject = "TravelBook — код доступу до традицій",

                HtmlBody = $"""
                    <div style="
                        background:#080A10;
                        color:#F4EFE0;
                        padding:32px;
                        font-family:Arial,sans-serif;
                        border:1px solid #C8962E;
                        border-radius:10px;
                    ">
                        <h1 style="
                            color:#E5B558;
                            margin-bottom:8px;
                        ">
                            TRAVELBOOK
                        </h1>

                        <p>
                            Підтвердження доступу до розділу
                            <strong>«Традиції»</strong>.
                        </p>

                        <p>
                            Ваш одноразовий код:
                        </p>

                        <div style="
                            font-size:32px;
                            font-weight:bold;
                            letter-spacing:8px;
                            color:#FFD54A;
                            margin:24px 0;
                        ">
                            {code}
                        </div>

                        <p>
                            Код дійсний протягом 10 хвилин.
                        </p>

                        <p style="
                            color:#9F9A88;
                            font-size:12px;
                            margin-top:28px;
                        ">
                            Якщо ви не запитували цей код,
                            просто проігноруйте лист.
                        </p>
                    </div>
                    """,

                TextBody =
                    $"TravelBook\n\n" +
                    $"Код доступу до розділу Традиції: {code}\n\n" +
                    $"Код дійсний протягом 10 хвилин."
            };

            message.To.Add(email);

            await _resend.EmailSendAsync(message);
        }
    }
}