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

        public NotificationService(
            IConfiguration configuration,
            IAccountRepository accountRepository
        )
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

                // Create SMTP client
                using (
                    var client = new SmtpClient(smtpHost, smtpPort)
                    {
                        UseDefaultCredentials = false,
                        Credentials = new NetworkCredential(smtpUsername, smtpPassword),
                        EnableSsl = true,
                    }
                )
                {
                    // Prepare email message
                    var mailMessage = new MailMessage
                    {
                        From = new MailAddress(senderEmail, "BE-MILK-TEEEE"),
                        Subject = "Payment Sucesss",
                        Body = GeneratePaymentEmailBody(payment, account),
                        IsBodyHtml = true,
                    };
                    mailMessage.To.Add(account.Email);

                    // Send email
                    await client.SendMailAsync(mailMessage);
                }
            }
            catch (Exception ex)
            {
                // Log the exception
                // You might want to use a proper logging mechanism
                Console.WriteLine($"Email sending failed: {ex.Message}");
            }
        }

        private string GeneratePaymentEmailBody(Payment payment, Account account)
        {
            return $@"
            <html>
            <body>
                <h2>Payment Confirmation</h2>
                <p>Dear {account.FirstName} {account.LastName},</p>
                <p>Your payment has been successfully processed.</p>
                <div style='border: 1px solid #ddd; padding: 10px;'>
                    <h3>Payment Details:</h3>
                    <p><strong>Order ID:</strong> {payment.OrderId}</p>
                    <p><strong>Payment ID:</strong> {payment.Id}</p>
                    <p><strong>Amount Paid:</strong> {payment.AmountPaid:C}</p>
                    <p><strong>Payment Date:</strong> {payment.PaymentDate:F}</p>
                    <p><strong>Payment Method:</strong> {payment.PaymentMethod}</p>
                </div>
                <p>Thank you for your purchase!</p>
                <p>Best regards,<br>Your Company Team</p>
            </body>
            </html>";
        }
    }
}
