using Feature.CluService;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Pizza.Bot.Dialogs.WhatsAppDialogs.QNA;
using Pizza.Bot.Entities;
using Pizza.Core.Configuration.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace Pizza.Bot.Dialogs.WhatsAppDialogs
{
    public class RootDialog : ComponentDialog
    {
        private const string InitialDialog = "initial-dialog";

        private readonly ConversationState _convesationState;
        private readonly IStatePropertyAccessor<UserProfile> _userProfileAccessor;
        private readonly IStatePropertyAccessor<ConversationData> _conversationDataAccessor;
        private readonly UserState _userState;
        private readonly ICurrentConfiguration _currentConfiguration;
        private readonly CluServiceRecognizer _cluServiceRecognizer;

        public RootDialog(ConversationState conversationState,
            UserState userState,
            ICurrentConfiguration currentConfiguration,
            CluServiceRecognizer cluServiceRecognizer,
            QNADialog qnaDialog)
            : base($"{nameof(RootDialog)}-WhatsApp")
        {
            _convesationState = conversationState;
            _userState = userState;
            _currentConfiguration = currentConfiguration;
            _cluServiceRecognizer = cluServiceRecognizer;

            _userProfileAccessor = userState.CreateProperty<UserProfile>(nameof(UserProfile));
            _conversationDataAccessor = conversationState.CreateProperty<ConversationData>(nameof(ConversationData));
            var steps = new WaterfallStep[]
            {
                HelloPrompt,
            };

            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new ChoicePrompt(nameof(ChoicePrompt)));

            AddDialog(qnaDialog);
            AddDialog(new WaterfallDialog("initStep", steps));

            // The initial child Dialog to run.
            InitialDialogId = "initStep";
        }
        private async Task<DialogTurnResult> HelloPrompt(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            UserProfile profile = await _userProfileAccessor.GetAsync(stepContext.Context, () => new UserProfile(), cancellationToken);
            await stepContext.Context.SendActivityAsync(MessageFactory.Text(_currentConfiguration.BotSettings.WelcomeMessage.Replace("[NOMBRE]", profile.UserName)), cancellationToken);
           
            return await stepContext.EndDialogAsync(cancellationToken: cancellationToken);
        }
    }
}
