using Pizza.Bot.Entities.Cards;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using Microsoft.Bot.Schema;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Pizza.Bot.Entities.Prompts
{
	public class MenuPrompt : Prompt<FoundChoice>
	{
		public CardFactory Card;
		public FindChoicesOptions RecognizerOptions { get; set; }
		private string _titleMenu;
		private string _textMenu;
		private bool _permittedNotMatch;
		private List<CardAction> _actions;
		public MenuPrompt(string dialogId, CardFactory cards, PromptValidator<FoundChoice> validator = null, string titleMenu = null, string textMenu = null, bool permittedNotMatch = false) : base(dialogId, validator)
		{
			Card = cards;
			_titleMenu = titleMenu;
			_textMenu = textMenu;
			_permittedNotMatch = permittedNotMatch;
		}
		public MenuPrompt(string dialogId, List<CardAction> actions, PromptValidator<FoundChoice> validator = null, string titleMenu = null, string textMenu = null) : base(dialogId, validator)
		{
			_actions = actions;
			_titleMenu = titleMenu;
			_textMenu = textMenu;
		}

		protected override async Task OnPromptAsync(ITurnContext turnContext, IDictionary<string, object> state, PromptOptions options, bool isRetry, CancellationToken cancellationToken = new CancellationToken())
		{
			if(_actions== null)
            {
				var card = Card.CreateMenuCard(_titleMenu,_textMenu, options);
				await turnContext.SendActivityAsync(card, cancellationToken);
            }
            else
			{
				var card = Card.CreateMenuCard(_titleMenu, _textMenu, _actions);
				await turnContext.SendActivityAsync(card, cancellationToken);
			}
		}
        protected override Task<bool> OnPreBubbleEventAsync(DialogContext dc, DialogEvent e, CancellationToken cancellationToken)
        {
            return base.OnPreBubbleEventAsync(dc, e, cancellationToken);
        }

        protected override async Task<PromptRecognizerResult<FoundChoice>> OnRecognizeAsync(ITurnContext turnContext, IDictionary<string, object> state, PromptOptions options, CancellationToken cancellationToken = new CancellationToken())
		{
			var choices = options.Choices ?? new List<Choice>();

			var result = new PromptRecognizerResult<FoundChoice>();
			if (turnContext.Activity.Type == ActivityTypes.Message)
			{
				var activity = turnContext.Activity;
				var value = activity.Value?.ToString() ?? activity.Text;
				var opt = RecognizerOptions ?? new FindChoicesOptions();
				var results = ChoiceRecognizers.RecognizeChoices(value, choices, opt);
				if (results != null && results.Count > 0)
				{
					result.Succeeded = true;
					result.Value = results[0].Resolution;
                }
                if (!string.IsNullOrEmpty(activity.Text) && _permittedNotMatch)
				{
					result.Succeeded = true;
					result.Value = new FoundChoice() { Value= activity.Text};
				}
			}

			return result;
		}


	}
}