using Pizza.Bot.Entities;
using Pizza.Bot.Enums;

namespace Pizza.Bot.EntitiesAdapter
{
    public class UserProfileTelegramAdapter : UserProfile
    {
        public UserProfileTelegramAdapter(ChannelTypeEnum channelTypeId, TelegramData channelData): base(channelTypeId)
        {
            UserName = channelData.message.from.first_name ?? channelData.message.from.username;
            Identifier = channelData.message.from.id.ToString();
        }
    }
}
