namespace Feature.WhatsappService.Enums
{
    public enum MessageTypeEnum
    {
        TEXT = 1,
        AUDIO = 2,
        DOCUMENT = 3,
        IMAGE = 4,
        STICKER = 5,
        VIDEO = 6,
        CONTACT = 7,
        LOCATION = 8,
        INTERACTIVE_LIST = 9,
        INTERACTIVE_REPLY_BUTTON = 10,
        TEMPLATE = 11,
        TEMPLATE_TEXT_WITH_PARAMETERS = 12,
        TEMPLATE_MEDIA_WITH_PARAMETERS = 13,
        TEMPLATE_INTERACTIVE_WITH_PARAMETERS = 14
    }

    public static class MessageTypeEnumExtensions
    {
        public static MessageTypeEnum GetFromText(string text)
        {
            switch (text)
            {
                case "text":
                    return MessageTypeEnum.TEXT;
                case "audio":
                    return MessageTypeEnum.AUDIO;
                case "document":
                    return MessageTypeEnum.DOCUMENT;
                case "image":
                    return MessageTypeEnum.IMAGE;
                case "sticker":
                    return MessageTypeEnum.STICKER;
                case "video":
                    return MessageTypeEnum.VIDEO;
                case "contact":
                    return MessageTypeEnum.CONTACT;
                case "location":
                    return MessageTypeEnum.LOCATION;
                //Los de abajo hay que revisar que funcionen correctamente
                case "interactive":
                    return MessageTypeEnum.INTERACTIVE_REPLY_BUTTON;
                case "interactive_list":
                    return MessageTypeEnum.INTERACTIVE_LIST;
                case "template":
                    return MessageTypeEnum.TEMPLATE;
                case "template_text":
                    return MessageTypeEnum.TEMPLATE_TEXT_WITH_PARAMETERS;
                case "template_media":
                    return MessageTypeEnum.TEMPLATE_MEDIA_WITH_PARAMETERS;
                case "template_interactive":
                    return MessageTypeEnum.TEMPLATE_INTERACTIVE_WITH_PARAMETERS;
                default:
                    return MessageTypeEnum.TEXT;
            }
        }
    }
}
