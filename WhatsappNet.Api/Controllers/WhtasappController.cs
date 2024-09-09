using Microsoft.AspNetCore.Mvc;
using WhatsappNet.Api.Services.WhatsappCloud.SendMessages;

namespace WhatsappNet.Api.Controllers
{
    [ApiController]
    [Route("api/whatsapp")]
    public class WhtasappController : Controller
    {
        private readonly IWhasappCloudSendMessage _whasappCloudSendMessage;
        public WhtasappController(IWhasappCloudSendMessage whasappCloudSendMessage)
        {
            _whasappCloudSendMessage = whasappCloudSendMessage;
        }

        [HttpGet("test")]
        public async Task<IActionResult> Sample()
        {
            var data = new
            {
                messaging_product = "whatsapp",
                to = "573012811778",
                type = "text",
                text = new
                {
                    body = "hola app"
                }
            };
            var resut = await _whasappCloudSendMessage.Execute(data);
            return Ok(resut);
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
