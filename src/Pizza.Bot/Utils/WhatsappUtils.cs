using Microsoft.AspNetCore.Http;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;

namespace Pizza.Bot.Utils
{
    public static class WhatsappUtils
    {
        public static IActivity ImageToActivity(string name, string contentType, string url)
        {
            var attachment = new Attachment
            {
                Name = name,
                ContentType = contentType ?? "image/sticker",
                ContentUrl = url,
            };
            return MessageFactory.Attachment(attachment);
        }


    }
}
