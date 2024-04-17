using Pizza.Core.Configuration.Interfaces;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Pizza.Bot.Entities;
using System.Threading;
using System.Threading.Tasks;

namespace Pizza.Bot.Dialogs
{
    public class RootDialog: ComponentDialog
    {
        /// <summary>
        /// QnA Maker initial dialog
        /// </summary>
        private const string InitialDialog = "initial-dialog";

        private readonly WhatsAppDialogs.RootDialog _whatsAppRootDialog;
        private readonly DefaultDialogs.RootDialog _defaultRootDialog;

        private readonly ConversationState _conversationState;
		private readonly IStatePropertyAccessor<UserProfile> _userProfileAccessor;
        private WaterfallStep[] _finalizarConversacionStep;
        private readonly IStatePropertyAccessor<ConversationData> _conversationDataAccessor;
        private readonly UserState _userState;
        private readonly ICurrentConfiguration _currentConfiguration;

        public RootDialog(ConversationState conversationState,
            UserState userState,
            WhatsAppDialogs.RootDialog whatsAppRootDialog,
            DefaultDialogs.RootDialog defaultRootDialog
            )
            : base(nameof(RootDialog))
        {
            _conversationState = conversationState;
            _userState = userState;
            _userProfileAccessor = userState.CreateProperty<UserProfile>(nameof(UserProfile));
            _conversationDataAccessor = conversationState.CreateProperty<ConversationData>(nameof(ConversationData));

            _whatsAppRootDialog = whatsAppRootDialog;
            _defaultRootDialog = defaultRootDialog;

            AddDialog(new WaterfallDialog("_dispatchStep", new WaterfallStep[]
           {
                DispatchStepAsync
           }));

            // The initial child Dialog to run.
            InitialDialogId = "_dispatchStep";
        }

        private async Task<DialogTurnResult> DispatchStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var profile = await _userProfileAccessor.GetAsync(stepContext.Context, () => new UserProfile(), cancellationToken);
            switch (profile.ChannelId)
            {
                case "whatsapp":
                    AddDialog(_whatsAppRootDialog);
                    return await stepContext.BeginDialogAsync($"{nameof(RootDialog)}-WhatsApp", cancellationToken: cancellationToken);
                default:
                    AddDialog(_defaultRootDialog);
                    return await stepContext.BeginDialogAsync($"{nameof(RootDialog)}-Default", cancellationToken: cancellationToken);
            }
        }
    }
}
