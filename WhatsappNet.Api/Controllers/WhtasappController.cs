using Microsoft.AspNetCore.Mvc;
using WhatsappNet.Api.Services.WhatsappCloud.SendMessages;
using WhatsappNet.Api.Util;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace WhatsappNet.Api.Controllers
{
    [ApiController]
    [Route("api/whatsapp")]
    public class WhtasappController : Controller
    {
        private readonly IWhasappCloudSendMessage _whasappCloudSendMessage;
        private readonly IUtil _util;
        public WhtasappController(IWhasappCloudSendMessage whasappCloudSendMessage, IUtil util)
        {
            _whasappCloudSendMessage = whasappCloudSendMessage;
            _util = util;
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

                    object objectMesage;

                    switch (UserText.ToUpper())
                    {
                        case "TEXT":
                            objectMesage = _util.TextMessage("este es un ejemplo de texto", userNumber);
                            break;
                        case "IMAGE":
                            objectMesage = _util.ImageMessage("https://img.freepik.com/foto-gratis/belleza-otonal-abstracta-patron-venas-hoja-multicolor-generado-ia_188544-9871.jpg?w=1380&t=st=1725874435~exp=1725875035~hmac=270eb4e943a973d6941b932551882541d75379ebf115b11a329dadbc0f6e2c39", userNumber);
                            break;
                        case "AUDIO":
                            objectMesage = _util.AudioMessage("https://biostoragecloud.blob.core.windows.net/resource-udemy-whatsapp-node/audio_whatsapp.mp3", userNumber);
                            break;
                        case "VIDEO":
                            objectMesage = _util.VideoMessage("https://biostoragecloud.blob.core.windows.net/resource-udemy-whatsapp-node/video_whatsapp.mp4", userNumber);
                            break;
                        case "DOCUMENT":
                            objectMesage = _util.DocumentMessage("https://biostoragecloud.blob.core.windows.net/resource-udemy-whatsapp-node/document_whatsapp.pdf", userNumber);
                            break;
                        case "LOCATION":
                            objectMesage = _util.LocationMessage(userNumber);
                            break;
                        case "BUTTON":
                            objectMesage = _util.ButtonsMessage(userNumber);
                            break;
                        default:
                            objectMesage = _util.TextMessage("NINGUNA OPCION1", userNumber);
                            break;

                    }
                    await _whasappCloudSendMessage.Execute(objectMesage);

                }
                return Ok("EVENT_RECEIVED");
            }
            catch (Exception ex)
            {
                return Ok("EVENT_RECEIVED");
            }
        }

        private string GetUserText(Message message)
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
