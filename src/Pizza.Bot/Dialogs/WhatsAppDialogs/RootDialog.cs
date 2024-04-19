using Feature.CluService;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Pizza.Bot.Dialogs.WhatsAppDialogs.QNA;
using Pizza.Bot.Entities;
using Pizza.Core.Configuration.Interfaces;
using System;
using System.Text;
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
            var stepsAnalyzeConversation = new WaterfallStep[]
            {
                GetIntentAsync,
            };

            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new ChoicePrompt(nameof(ChoicePrompt)));

            AddDialog(qnaDialog);
            AddDialog(new WaterfallDialog("initStep", stepsAnalyzeConversation));

            // The initial child Dialog to run.
            InitialDialogId = "initStep";
        }

        private async Task<DialogTurnResult> GetIntentAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            UserProfile profile = await _userProfileAccessor.GetAsync(stepContext.Context, () => new UserProfile(), cancellationToken);
            if (string.IsNullOrEmpty(stepContext.Context?.Activity?.Text))
                return await stepContext.EndDialogAsync(cancellationToken: cancellationToken);

            var conversation = await _conversationDataAccessor.GetAsync(stepContext.Context, () => new ConversationData { LastActivity = DateTimeOffset.UtcNow }, cancellationToken: cancellationToken);
            conversation.PreguntarSiNecesitaAlgoMas = true;

            var action = await _cluServiceRecognizer.RecognizeAsync<ActionIntentType>(stepContext.Context, cancellationToken);

            switch (action.GetTopIntent().intent)
            {
                case ActionIntentType.Intent.Saludar:
                    stepContext.Context.Activity.Text = string.Empty;
                    stepContext.Context.Activity.Value = string.Empty;
                    await stepContext.Context.SendActivityAsync(MessageFactory.Text(_currentConfiguration.BotSettings.WelcomeMessage.Replace("[NOMBRE]", profile.UserName)), cancellationToken);
                    stepContext.Context.Activity.Text = string.Empty;
                    stepContext.Context.Activity.Value = string.Empty;
                    return await stepContext.EndDialogAsync(cancellationToken: cancellationToken);
                case ActionIntentType.Intent.PedirPizza:
                    stepContext.Context.Activity.Text = string.Empty;
                    stepContext.Context.Activity.Value = string.Empty;
                    var textPedirPizza = new StringBuilder();
                    textPedirPizza.AppendLine($"Tu intencion es **{action.GetTopIntent().intent}**");
                    if(action.Entities?.Entities != null)
                    {
                        textPedirPizza.AppendLine($"Estas son las entidades:");
                        foreach (var item in action.Entities.Entities)
                        {
                            textPedirPizza.AppendLine($"* {item.Text} [{item.Category}]");    
                        }
                    }
                    await stepContext.Context.SendActivityAsync(MessageFactory.Text(textPedirPizza.ToString()), cancellationToken);
                    return await stepContext.EndDialogAsync(cancellationToken: cancellationToken);
                case ActionIntentType.Intent.None:
                default:
                    return await stepContext.BeginDialogAsync(DialogFlowType.Whatsapp(nameof(QNADialog)), cancellationToken: cancellationToken);
            }

        }

        private async Task<DialogTurnResult> HelloPrompt(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            UserProfile profile = await _userProfileAccessor.GetAsync(stepContext.Context, () => new UserProfile(), cancellationToken);
            await stepContext.Context.SendActivityAsync(MessageFactory.Text(_currentConfiguration.BotSettings.WelcomeMessage.Replace("[NOMBRE]", profile.UserName)), cancellationToken);
           
            return await stepContext.EndDialogAsync(cancellationToken: cancellationToken);
        }
    }
}
