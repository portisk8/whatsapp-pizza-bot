using Microsoft.Bot.Schema;
using Pizza.Bot.Entities;
using Pizza.Bot.Enums;

namespace Pizza.Bot.EntitiesAdapter
{
    public class UserProfileWhatsAppAdapter : UserProfile
    {
        public UserProfileWhatsAppAdapter(ChannelTypeEnum channelTypeId, IMessageActivity activity) : base(channelTypeId)
        {
            Identifier = activity.From.Id;
            UserName = activity.From.Name;
            ChannelId = activity.ChannelId;
        }
    }
}
