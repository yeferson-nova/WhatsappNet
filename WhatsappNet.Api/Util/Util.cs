namespace WhatsappNet.Api.Util
{
    public class Util : IUtil
    {
        public object TextMessage(string message, string numberTo)
        {
            return new
            {
                messaging_product = "whatsapp",
                to = numberTo,
                type = "text",
                text = new
                {
                    body = message
                }
            };
        }

        public object ImageMessage(string url, string numberTo)
        {
            return new
            {
                messaging_product = "whatsapp",
                to = numberTo,
                type = "image",
                image = new
                {
                    link = url
                }
            };
        }
        public object AudioMessage(string url, string numberTo)
        {
            return new
            {
                messaging_product = "whatsapp",
                to = numberTo,
                type = "audio",
                audio = new
                {
                    link = url
                }
            };
        }
        public object VideoMessage(string url, string numberTo)
        {
            return new
            {
                messaging_product = "whatsapp",
                to = numberTo,
                type = "video",
                video = new
                {
                    link = url
                }
            };
        }
        public object DocumentMessage(string url, string numberTo)
        {
            return new
            {
                messaging_product = "whatsapp",
                to = numberTo,
                type = "document",
                document = new
                {
                    link = url
                }
            };
        }
        public object LocationMessage(string numberTo)
        {
            return new
            {
                messaging_product = "whatsapp",
                to = numberTo,
                type = "location",
                location = new
                {
                    latitude = "4.603468371191371",
                    longitud = "-74.07223344125866",
                    name = "Museo del Oro",
                    address = "Cra. 6 #15-88, Santa Fé, Bogotá, Cundinamarca"
                }
            };
        }
        public object ButtonsMessage(string numberTo)
        {
            return new
            {
                messaging_product = "whatsapp",
                to = numberTo,
                type = "interactive",
                interactive = new
                {
                    type = "button",
                    body = new
                    {
                        text = "Seleccione"
                    },
                    action = new
                    {
                        buttons = new List<object>
                        {
                            new
                            {
                                type = "reply",
                                reply = new
                                {
                                    id = "01",
                                    title = "Aprobar"
                                }
                            },
                            new
                            {
                                type = "reply",
                                reply = new
                                {
                                    id = "02",
                                    title = "Rechazar"
                                }
                            }
                        }
                    }
                }
            };
        }
    }
}
