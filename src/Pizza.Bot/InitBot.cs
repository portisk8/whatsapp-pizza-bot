// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using Pizza.Bot.Entities;
using Pizza.Bot.EntitiesAdapter;
using Pizza.Core.Configuration.Interfaces;

namespace Pizza.Bot
{
    public class InitBot<T> : ActivityHandler where T : Microsoft.Bot.Builder.Dialogs.Dialog
    {
        private readonly ConversationState _conversationState;
        private readonly UserState _userState;
        protected readonly Microsoft.Bot.Builder.Dialogs.Dialog _dialog;
        private readonly IStatePropertyAccessor<ConversationData> _conversationDataAccessor;
        private readonly IStatePropertyAccessor<UserProfile> _userProfileAccessor;
        private readonly IStatePropertyAccessor<DialogState> _dialogStateProfileAccessor;
        private readonly ICurrentConfiguration _currentConfigurations;

        public InitBot(ConversationState conversationState, UserState userState, T dialog, ICurrentConfiguration currentConfigurations)
        {
            _conversationState = conversationState;
            _dialog = dialog;
            _currentConfigurations = currentConfigurations;
            _conversationDataAccessor = conversationState.CreateProperty<ConversationData>(nameof(ConversationData));
            _userProfileAccessor = userState.CreateProperty<UserProfile>(nameof(UserProfile));
            _dialogStateProfileAccessor = _conversationState.CreateProperty<DialogState>(nameof(DialogState));
            _userState = userState;
        }

        protected override async Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            foreach (var member in membersAdded)
            {
                if (member.Id != turnContext.Activity.Recipient.Id)
                {
                    await _conversationState.DeleteAsync(turnContext, cancellationToken: cancellationToken);
                    await _userState.DeleteAsync(turnContext, cancellationToken: cancellationToken);
                    var name = string.Empty;
                    switch (turnContext.Activity.ChannelId)
                    {
                        case "telegram":
                            var telegramProfileData = turnContext.Activity.GetChannelData<TelegramData>();
                            name = telegramProfileData.message.from.first_name ?? telegramProfileData.message.from.username;
                            break;
                        default:
                            break;
                    }
                    var saludo = string.Format(_currentConfigurations.BotSettings.WelcomeMessage.Replace("$NAME", name), GetGretting(turnContext.Activity.LocalTimestamp));
                    await turnContext.SendActivityAsync(MessageFactory.Text(saludo), cancellationToken);
                    
                    
                    var imagePath = Path.Combine(Environment.CurrentDirectory, @"Resources", "Bot.png");
                    var imageData = Convert.ToBase64String(File.ReadAllBytes(imagePath));
                    var attachment =  new Attachment
                    {
                        Name = @"Resources\Bot.png",
                        ContentType = "image/png",
                        ContentUrl = $"data:image/png;base64,{imageData}",
                    };
                    await turnContext.SendActivityAsync(MessageFactory.Attachment(attachment));
                    await _dialog.RunAsync(turnContext, _dialogStateProfileAccessor, cancellationToken);

                }
            }
        }

        public override async Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken = default)
        {
            //await turnContext.SendActivityAsync(new Activity { Type = ActivityTypes.Typing }, cancellationToken);
            await  base.OnTurnAsync(turnContext, cancellationToken);
            await _conversationState.SaveChangesAsync(turnContext, cancellationToken: cancellationToken); 
        }

        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            var conversation = await _conversationDataAccessor.GetAsync(turnContext, () => new ConversationData { LastActivity = DateTimeOffset.UtcNow }, cancellationToken: cancellationToken);
            if (DateTimeOffset.UtcNow.Subtract(conversation.LastActivity).TotalMinutes > _currentConfigurations.BotSettings.ConversationTimeoutInMinutes)
            {
                await _conversationState.DeleteAsync(turnContext, cancellationToken: cancellationToken);
                await _userState.DeleteAsync(turnContext, cancellationToken: cancellationToken);
            }
            conversation.LastActivity = DateTimeOffset.UtcNow;
            var profile = await _userProfileAccessor.GetAsync(turnContext, () => new UserProfile(), cancellationToken);
            switch (turnContext.Activity.ChannelId)
            {
                case "telegram":
                    var telegramProfileData = new UserProfileTelegramAdapter(Enums.ChannelTypeEnum.BOT_TELEGRAM, turnContext.Activity.GetChannelData<TelegramData>());
                    profile.Identifier = telegramProfileData.Identifier;
                    profile.PhoneNumber = telegramProfileData.PhoneNumber;
                    profile.ChannelId = telegramProfileData.ChannelId;
                    profile.UserName = telegramProfileData.UserName;
                    profile.ChannelTypeId = Enums.ChannelTypeEnum.BOT_TELEGRAM;
                    break;
                case "webchat":
                    var webchatProfileData = new UserProfileEmulatorAdapter(Enums.ChannelTypeEnum.BOT_WEB, turnContext.Activity);
                    profile.Identifier = webchatProfileData.Identifier;
                    profile.ChannelId = webchatProfileData.ChannelId;
                    profile.ChannelTypeId = Enums.ChannelTypeEnum.BOT_WEB;
                    break;
                case "whatsapp":
                    var whatsappProfileData = new UserProfileWhatsAppAdapter(Enums.ChannelTypeEnum.BOT_WHATSAPP, turnContext.Activity);
                    profile.Identifier = whatsappProfileData.Identifier;
                    profile.ChannelId = whatsappProfileData.ChannelId;
                    profile.UserName= whatsappProfileData.UserName;
                    profile.PhoneNumber = whatsappProfileData.Identifier;
                    profile.ChannelTypeId = Enums.ChannelTypeEnum.BOT_WHATSAPP;
                    break;
                default: //Emulator
                    var emaulatorProfileData = new UserProfileEmulatorAdapter(Enums.ChannelTypeEnum.BOT_EMULADOR, turnContext.Activity);
                    profile.Identifier = emaulatorProfileData.Identifier;
                    profile.ChannelId = emaulatorProfileData.ChannelId;
                    profile.ChannelTypeId = Enums.ChannelTypeEnum.BOT_EMULADOR;
                    break;
            }

            await _userProfileAccessor.SetAsync(turnContext, profile);
            await _userState.SaveChangesAsync(turnContext);
            
            var message = turnContext.Activity.Text?.ToLower();

            conversation.UserProfile = profile;
            await _dialog.RunAsync(turnContext, _dialogStateProfileAccessor, cancellationToken);

        }
        protected override Task OnEndOfConversationActivityAsync(ITurnContext<IEndOfConversationActivity> turnContext, CancellationToken cancellationToken)
        {
            turnContext.SendActivityAsync(MessageFactory.Text("¿Deseas consultar algo mas?"), cancellationToken);
            return base.OnEndOfConversationActivityAsync(turnContext, cancellationToken);
        }

        private string GetGretting(DateTimeOffset? dateTimeOffset)
        {
            if(!dateTimeOffset.HasValue) 
                return "Hola";
            if (dateTimeOffset.Value.Hour > 12 & dateTimeOffset.Value.Hour <= 17)
            {
                return "Buenas tardes";
            }
            if (dateTimeOffset.Value.Hour > 17 & dateTimeOffset.Value.Hour <= 24)
            {
                return "Buenas noches";
            }
            return "Buenos días";
        }

    }
}
