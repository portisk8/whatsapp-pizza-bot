using Pizza.Core.Configuration.Interfaces;
using Feature.CluService;
using Feature.QNAService;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using Pizza.Bot.Entities;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Pizza.Bot.Dialogs.DefaultDialogs
{
    public class RootDialog: ComponentDialog
    {
        private const string InitialDialog = "initial-dialog";

        private readonly ConversationState _convesationState;
        protected readonly ICurrentConfiguration _currentConfiguration;
		private readonly IStatePropertyAccessor<UserProfile> _userProfileAccessor;
        private readonly IStatePropertyAccessor<ConversationData> _conversationDataAccessor;
        private readonly UserState _userState;
        private readonly CluServiceRecognizer _cluServiceRecognizer;
        private readonly QNAServiceRecognizer _qnaServiceRecognizer;
        
        public RootDialog(ConversationState conversationState,
            UserState userState,
            ICurrentConfiguration currentConfiguration,
            CluServiceRecognizer cluServiceRecognizer,
            QNAServiceRecognizer qnaServiceRecognizer
            )
            : base(($"{nameof(RootDialog)}-Default"))
        {
            _convesationState = conversationState;
            _userState = userState;
            _currentConfiguration = currentConfiguration;
            _cluServiceRecognizer = cluServiceRecognizer;
            _qnaServiceRecognizer = qnaServiceRecognizer;
            _userProfileAccessor = userState.CreateProperty<UserProfile>(nameof(UserProfile));
            _conversationDataAccessor = conversationState.CreateProperty<ConversationData>(nameof(ConversationData));
            var steps = new WaterfallStep[]
            {
                //GetIntentAsync,
                GetQNAResponseAsync,
            };

            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new ChoicePrompt(nameof(ChoicePrompt)));

            AddDialog(new WaterfallDialog("initStep", steps));

            // The initial child Dialog to run.
            InitialDialogId = "initStep";
        }

        private async Task<DialogTurnResult> GetQNAResponseAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(stepContext.Context?.Activity?.Text))
                return await stepContext.EndDialogAsync(cancellationToken: cancellationToken);

            var result = await _qnaServiceRecognizer.RecognizeAsync(stepContext.Context);
            if(result != null)
            {
                await stepContext.Context.SendActivityAsync(MessageFactory.Text(result.Answer), cancellationToken);
            }

            return await stepContext.EndDialogAsync(cancellationToken: cancellationToken);

        }

        private async Task<DialogTurnResult> GetIntentAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            if(string.IsNullOrEmpty(stepContext.Context?.Activity?.Text)) 
                return await stepContext.EndDialogAsync(cancellationToken: cancellationToken);

            var conversation = await _conversationDataAccessor.GetAsync(stepContext.Context, () => new ConversationData { LastActivity = DateTimeOffset.UtcNow }, cancellationToken: cancellationToken);
            conversation.PreguntarSiNecesitaAlgoMas = true;

            var action = await _cluServiceRecognizer.RecognizeAsync(stepContext.Context, cancellationToken);
            
            await stepContext.Context.SendActivityAsync(MessageFactory.Text($"Esto estas buscando: {action.GetTopScoringIntent().intent}"), cancellationToken);

            return await stepContext.EndDialogAsync(cancellationToken: cancellationToken);

        }
    }
}
