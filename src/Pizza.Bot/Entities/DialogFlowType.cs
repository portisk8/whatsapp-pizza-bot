namespace Pizza.Bot.Entities
{
    public static class DialogFlowType
    {
        public static string WhatsappSuffix = "-Whatsapp";
        public static string DefaultSuffix = "-Default";

        public static string Whatsapp(string text)
        {
            return text + WhatsappSuffix;
        }
        public static string Default(string text)
        {
            return text + DefaultSuffix;
        }
    }
}
