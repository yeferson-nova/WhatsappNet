using Newtonsoft.Json;
using System.Text;
using System.Text.Json.Serialization;

namespace WhatsappNet.Api.Services.WhatsappCloud.SendMessages
{
    public class WhasappCloudSendMessage : IWhasappCloudSendMessage
    {

        public async Task<bool> Execute(object model)
        {
            var client = new HttpClient();
            var byteData = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(model));

            var keyVaultHelper = new KeyVaultHelper("tu-key-vault-name");

            string phoneNumberId = await keyVaultHelper.GetSecretAsync("phoneNumberId");
            string accessToken = await keyVaultHelper.GetSecretAsync("accessToken");

            using (var content = new ByteArrayContent(byteData))
            {
                string endpoint = "https://graph.facebook.com";
                string version = "v20.0";
                string uri = $"{endpoint}/{version}/{phoneNumberId}/messages";

                content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");

                var response = await client.PostAsync(uri, content);

                if (response.IsSuccessStatusCode)
                {
                    return true;
                }
                return false;
            }
        }
    }
}
