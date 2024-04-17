using Microsoft.Bot.Schema;
using Pizza.Bot.Entities;
using Pizza.Bot.Enums;

namespace Pizza.Bot.EntitiesAdapter
{
    public class UserProfileEmulatorAdapter : UserProfile
    {
        public UserProfileEmulatorAdapter(ChannelTypeEnum channelTypeId, IMessageActivity activity) : base(channelTypeId)
        {
            Identifier = activity.From.Id;
        }
    }
}
