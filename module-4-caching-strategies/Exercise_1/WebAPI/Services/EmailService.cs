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

                var message = new EmailMessage(
                    senderAddress: _configuration.GetValue<string>("SMTP_SENDER"),
                    content: new EmailContent("Invited to game")
                    {
                        PlainText = $"{Data.PlayerName} invited you to a game!",
                        Html = @$"<html>
                                    <body>
                                    <a href={Data.InviteLink}>Click here to join!</a>
                                    </body>
                                 </html>"
                    },
                    recipients: new EmailRecipients(new List<EmailAddress> { new EmailAddress(Data.Email) }));
                EmailSendOperation emailSendOperation = mailClient.Send(
WaitUntil.Completed,
message);
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
                var mailClient = new EmailClient(_configuration.GetValue<string>("SMTPSERVER"));

                var message = new EmailMessage(
                    senderAddress: _configuration.GetValue<string>("SMTP_SENDER"),
                    content: new EmailContent("Game Session Result")
                    {
                        PlainText = $"Game Session Result:\n" +
                        $"{session.Player1.Name} {session.Player1.Points} - {session.Player2.Points} {session.Player2.Name}",
                    },
                    recipients: new EmailRecipients(new List<EmailAddress> { new EmailAddress(session.Player1.Email), new EmailAddress(session.Player2.Email) }));
                EmailSendOperation emailSendOperation = mailClient.Send(
WaitUntil.Completed,
message);
            }
            catch (Exception ex)
            {
                responseCode = ErrorCodes.MAIL_ERROR;
            }
            return responseCode;
        }
    }
}
