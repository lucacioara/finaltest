using GameAPI.Services;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Models;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InviteController : ControllerBase
    {
        private IConfiguration _configuration;
        private IEmailService _emailService;
        public InviteController(IConfiguration configuration, IEmailService emailService)
        {
            _configuration = configuration;
            _emailService = emailService;
        }

        [HttpPost]
        [Route("SendInvite")]
        public async Task<short> SendInvite([FromBody] EmailData emailData)
        {
            return _emailService.SendInvite(emailData);
        }
    }
}
