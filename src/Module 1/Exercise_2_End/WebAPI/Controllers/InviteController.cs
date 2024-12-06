using Microsoft.AspNetCore.Mvc;
using System.Net.Mail;
using System.Net;
using System.Runtime;
using System.Text.Json;
using WebAPI.Constants;
using WebAPI.Models;
using Microsoft.VisualBasic;
using Microsoft.Extensions.Options;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InviteController : ControllerBase
    {
        private IConfiguration _configuration;
        public InviteController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost]
        [Route("SendInvite")]
        public async Task<short> SendInvite([FromBody] EmailData emailData)
        {
            short responseCode = 0;
            try
            {
                var mailClient = new SmtpClient(_configuration.GetValue<string>("SMTPSERVER"))
                {
                    UseDefaultCredentials = false,
                    Port = Int32.Parse(_configuration.GetValue<string>("SMTPSSLPORT")),
                    Credentials = new NetworkCredential(
                   _configuration.GetValue<string>("SMTPUSERNAME"),
                   _configuration.GetValue<string>("SMTPPASSWORD")),
                    EnableSsl = false

                };

                MailAddress from = new MailAddress("cbara@keyticket.eu", emailData.PlayerName); 
                var message = new MailMessage();

                MailAddress toSender = new MailAddress(emailData.Email, emailData.Name);
                message.From = from;
                message.To.Add(toSender);
                message.Subject = $"{emailData.Name} Sent you an invite to Play!";
                message.IsBodyHtml = true;
                message.Body = $"Come and let's play on {_configuration.GetValue<string>("TRUSTEDORIGINS")}";
                await mailClient.SendMailAsync(message);
            }
            catch (Exception ex)
            {
                responseCode = ErrorCodes.MAIL_ERROR;
            }
            return responseCode;
        }
    }
}
