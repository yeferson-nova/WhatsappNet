using Microsoft.AspNetCore.Mvc;

namespace WhatsappNet.Api.Controllers
{
    [ApiController]
    [Route("api/whatsapp")]
    public class WhtasappController : Controller
    {
        [HttpGet("test")]
        public IActionResult Sample()
        {
            return Ok("ok sample");
        }

        [HttpGet]
        public IActionResult verifyToken()
        {
            string AccessToken = "54ca54f5-866b-4962-826f-b861d8e350f8";
            var token = Request.Query["hub.verify_token"].ToString();
            var challege = Request.Query["hub.challege"].ToString();

            if (challege != null && token != null && token == AccessToken)
            {
                return Ok(challege);
            }
            else { return BadRequest(); }
        }
        [HttpPost]
        public async Task<IActionResult> ReceivedMessage([FromBody] WhatsAppCloudModel body)
        {
            try
            {
                var Message = body.Entry[0]?.Changes[0]?.Value?.Messages[0];
                if (Message != null)
                {
                    var userNumber = Message.From;
                    var UserText = GetUserText(Message);
                }
                return Ok("EVENT_RECEIVED");
            }
            catch (Exception ex)
            {
                return Ok("EVENT_RECEIVED");
            }
        }

        private String GetUserText(Message message)
        {
            string TypeMessage = message.Type;

            switch (TypeMessage.ToUpper())
            {
                case "TEXT":
                    return message.Text.Body;
                    break;
                case "INTERACTIVE":
                    string interactiveType = message.Interactive.Type;
                    if (interactiveType.ToUpper() == "LIST_REPLY")
                    {
                        return message.Interactive.List_Reply.Title;
                    }
                    break;
                case "LIST_REPLY":
                    return message.Interactive.List_Reply.Title;

                case "BUTTON_REPLY":
                    return message.Interactive.Button_Reply.Title;

                default: return string.Empty;
            }
            return string.Empty;
        }
    }
}
