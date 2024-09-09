namespace WhatsappNet.Api.Util
{
    public interface IUtil
    {
        object TextMessage(string message, string numberTo);
        object ImageMessage(string url, string numberTo);
        object AudioMessage(string url, string numberTo);
        object VideoMessage(string url, string numberTo);
        object DocumentMessage(string url, string numberTo);
        object LocationMessage(string numberTo);
        object ButtonsMessage(string numberTo);
    }
}
