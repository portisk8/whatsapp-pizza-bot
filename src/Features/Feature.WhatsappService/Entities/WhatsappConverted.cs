using Feature.WhatsappService.Enums;

namespace Feature.WhatsappService.Entities
{
    public class WhatsappConverted
    {
        public MessageTypeEnum MessageType { get; set; }
        public object Value { get; set; }
    }
}