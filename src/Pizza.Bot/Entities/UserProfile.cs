using Pizza.Bot.Enums;
using System.Text;

namespace Pizza.Bot.Entities
{
    public class UserProfile
    {
        public ChannelTypeEnum ChannelTypeId { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Identifier { get; set; }
        public string? Hash { get; set; }
        public string? ChannelId { get; set; }
        public string Token { get; set; }
        public string UserName { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }

        public UserProfile(ChannelTypeEnum channelTypeId)
        {
            ChannelTypeId = channelTypeId;
        }

        public UserProfile()
        {
        }

        public string GetUserHash()
        {
            return sha256(Hash);
        }

        private string sha256(string randomString)
        {
            var crypt = new System.Security.Cryptography.SHA256Managed();
            var hash = new System.Text.StringBuilder();
            byte[] crypto = crypt.ComputeHash(Encoding.UTF8.GetBytes(randomString));
            foreach (byte theByte in crypto)
            {
                hash.Append(theByte.ToString("x2"));
            }
            return hash.ToString();
        }
        public void Clear()
        {
            Token = string.Empty;
        }
    }
}
