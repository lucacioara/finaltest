using Azure;
using Azure.Communication.Email;
using WebAPI.Constants;
using WebAPI.Models;

namespace GameAPI.Services
{
    public interface IEmailService
    {
        short SendInvite(EmailData Data);
        short SendSessionResult(Session session);
    }
    public class EmailService : IEmailService
    {
        private IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public short SendInvite(EmailData Data)
        {
            short responseCode = 0;
            try
            {
                var mailClient = new EmailClient(_configuration.GetValue<string>("SMTPSERVER"));
                if (IsValidEmail(Data.Email))
                {
                    var message = new EmailMessage(
                        senderAddress: _configuration.GetValue<string>("SMTP_SENDER"),
                        content: new EmailContent("Invited to game")
                        {
                            Html = @$"<html>
                                    <body>
                                    <p>{Data.PlayerName} invited you to a game!</p>
                                    <a href={Data.InviteLink}>Click here to join!</a>
                                    </body>
                                 </html>"
                        },
                        recipients: new EmailRecipients(new List<EmailAddress> { new EmailAddress(Data.Email) }));
                    EmailSendOperation emailSendOperation = mailClient.Send(
    WaitUntil.Completed,
    message);
                }
            }
            catch (Exception ex)
            {
                responseCode = ErrorCodes.MAIL_ERROR;
            }
            return responseCode;
        }
        public short SendSessionResult(Session session)
        {
            short responseCode = 0;
            try
            {
                var playerEmails = new List<EmailAddress>();
                if (IsValidEmail(session.Player1.Email))
                {
                    playerEmails.Add(new EmailAddress(session.Player1.Email));
                }
                if (IsValidEmail(session.Player2.Email))
                {
                    playerEmails.Add(new EmailAddress(session.Player2.Email));
                }
                if (playerEmails.Any())
                {
                    var mailClient = new EmailClient(_configuration.GetValue<string>("SMTPSERVER"));
                    var message = new EmailMessage(
                        senderAddress: _configuration.GetValue<string>("SMTP_SENDER"),
                        content: new EmailContent("Game Session Result")
                        {
                            PlainText = $"Game Session Result:\n" +
                            $"{session.Player1.Name} {session.Player1.Points} - {session.Player2.Points} {session.Player2.Name}",
                        },
                        recipients: new EmailRecipients(playerEmails));
                    EmailSendOperation emailSendOperation = mailClient.Send(
    WaitUntil.Completed,
    message);
                }
            }
            catch (Exception ex)
            {
                responseCode = ErrorCodes.MAIL_ERROR;
            }
            return responseCode;
        }
        bool IsValidEmail(string email)
        {
            if (!string.IsNullOrEmpty(email))
            {
                var trimmedEmail = email.Trim();

                if (trimmedEmail.EndsWith("."))
                {
                    return false;
                }
                try
                {
                    var addr = new System.Net.Mail.MailAddress(email);
                    return addr.Address == trimmedEmail;
                }
                catch
                {
                    return false;
                }
            }
            return false;
        }
    }
}
