using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Business_Logic_Layer.Models.Responses;
using Data_Access_Layer.Entities;
using Data_Access_Layer.Repositories;
using Microsoft.Extensions.Configuration;

namespace Business_Logic_Layer.Services.NotificationService
{
    public class NotificationService : INotificationService
    {
        private readonly IConfiguration _configuration;
        private readonly IAccountRepository _accountRepository;

        public NotificationService(IConfiguration configuration, IAccountRepository accountRepository)
        {
            _configuration = configuration;
            _accountRepository = accountRepository;
        }

        public async Task SendPaymentSuccessEmailAsync(Payment payment, Account account)
        {
            try
            {
                var smtpSettings = _configuration.GetSection("EmailSettings");
                var smtpHost = smtpSettings["SmtpHost"];
                var smtpPort = int.Parse(smtpSettings["SmtpPort"]);
                var smtpUsername = smtpSettings["SmtpUsername"];
                var smtpPassword = smtpSettings["SmtpPassword"];
                var senderEmail = smtpSettings["SenderEmail"];
                var replyToEmail = smtpSettings["ReplyToEmail"] ?? senderEmail;

                using (var client = new SmtpClient(smtpHost, smtpPort))
                {
                    client.Credentials = new NetworkCredential(smtpUsername, smtpPassword);
                    client.EnableSsl = true;

                    var mailMessage = new MailMessage
                    {
                        From = new MailAddress(senderEmail, "BE-MILK-TEA"),
                        Subject = "Your Payment Has Been Confirmed",
                        Body = GeneratePaymentEmailBody(payment, account),
                        IsBodyHtml = true,
                    };

                    // Improve Email Headers
                    mailMessage.Headers.Add("X-Mailer", "BE-MILK-TEA System");
                    mailMessage.Headers.Add("Message-ID", $"<{Guid.NewGuid()}@{smtpHost}>");
                    mailMessage.Headers.Add("List-Unsubscribe", "<mailto:unsubscribe@yourdomain.com>");
                    mailMessage.ReplyToList.Add(new MailAddress(replyToEmail));

                    // Plain-text version
                    var plainTextBody = GeneratePlainTextPaymentBody(payment, account);
                    var plainTextView = AlternateView.CreateAlternateViewFromString(plainTextBody, null, "text/plain");
                    var htmlView = AlternateView.CreateAlternateViewFromString(mailMessage.Body, null, "text/html");
                    mailMessage.AlternateViews.Add(plainTextView);
                    mailMessage.AlternateViews.Add(htmlView);

                    mailMessage.To.Add(account.Email);

                    await client.SendMailAsync(mailMessage);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Email sending failed: {ex.Message}");
            }
        }

        private string GeneratePaymentEmailBody(Payment payment, Account account)
        {
            return $@"
            <html>
            <body style='font-family: Arial, sans-serif; color: #333;'>
                <h2 style='color: #2c3e50;'>Payment Confirmation</h2>
                <p>Dear {account.FirstName} {account.LastName},</p>
                <p>We have successfully received your payment.</p>
                <div style='border: 1px solid #ddd; padding: 10px; background-color: #f9f9f9;'>
                    <h3>Payment Details:</h3>
                    <p><strong>Order ID:</strong> {payment.OrderId}</p>
                    <p><strong>Payment ID:</strong> {payment.Id}</p>
                    <p><strong>Amount Paid:</strong> {payment.AmountPaid:C}</p>
                    <p><strong>Payment Date:</strong> {payment.PaymentDate:F}</p>
                    <p><strong>Payment Method:</strong> {payment.PaymentMethod}</p>
                </div>
                <p>If you have any questions, please contact our support team.</p>
                <p>Best regards,<br/><strong>BE-MILK-TEA Team</strong></p>
            </body>
            </html>";
        }

        private string GeneratePlainTextPaymentBody(Payment payment, Account account)
        {
            return $@"
            Payment Confirmation

            Dear {account.FirstName} {account.LastName},

            We have successfully received your payment.

            Payment Details:
            - Order ID: {payment.OrderId}
            - Payment ID: {payment.Id}
            - Amount Paid: {payment.AmountPaid:C}
            - Payment Date: {payment.PaymentDate:F}
            - Payment Method: {payment.PaymentMethod}

            If you have any questions, please contact our support team.

            Best regards,
            BE-MILK-TEA Team";
        }
    }
}
