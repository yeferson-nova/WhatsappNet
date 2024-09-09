namespace WhatsappNet.Api.Services.WhatsappCloud.SendMessages
{
    public interface IWhasappCloudSendMessage
    {
        Task<bool> Execute(object model);

    }
}
