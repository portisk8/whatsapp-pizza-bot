using Feature.WhatsappService.Enums;
using Microsoft.Bot.Schema;
using Newtonsoft.Json.Linq;
using WhatsApp.Adapters.WhatsAppAdapter.Incomings;

namespace Feature.WhatsappAdapter.Extensions
{
    public static class MessageExtensions
    {
        public static Activity ConvertToActivity(this Message message)
        {
            var activity = new Activity
            {
                Id = message.Id,
                ChannelData = JObject.FromObject(message),
                ChannelId = "whatsapp",
                Entities = new List<Entity>(),
                Attachments = new List<Attachment>(),
                Type = ActivityTypes.Message,
                From = new ChannelAccount { Id = message.From },
                Conversation = new ConversationAccount { IsGroup = false, Id = message.From },
                Timestamp = DateTimeOffset.FromUnixTimeSeconds(long.Parse(message.Timestamp))
            };

            switch (MessageTypeEnumExtensions.GetFromText(message.Type))
            {
                case MessageTypeEnum.TEXT:
                    activity.Text = message.Text.Text;
                    break;
                case MessageTypeEnum.INTERACTIVE_REPLY_BUTTON:
                    activity.Text = message.Interactive.ButtonReply.Title;
                    activity.Value = message.Interactive.ButtonReply.Id;
                    activity.Name = "REPLY_BUTTON";
                    break;
                case MessageTypeEnum.LOCATION:
                    activity.Properties.Add("latitude", message.Location.Latitude.ToString(System.Globalization.CultureInfo.InvariantCulture));
                    activity.Properties.Add("longitude", message.Location.Longitude.ToString(System.Globalization.CultureInfo.InvariantCulture));
                    activity.Text = "Ubicación compartida";
                    break;
                default:
                    activity.Text = message.Text.Text;
                    break;
            }

            return activity;
        }
    }
}
