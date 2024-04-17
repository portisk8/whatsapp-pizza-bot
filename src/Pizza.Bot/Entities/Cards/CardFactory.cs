using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using System.Collections.Generic;
using System.Linq;

namespace Pizza.Bot.Entities.Cards
{
	public class CardFactory
	{
		private static CardFactory _uniqueInstance;
		public string Title { get; set; }
		public string Text { get; set; }
		public IList<CardAction> Actions { get; set; } = new List<CardAction>();
		public IList<CardImage> Images { get; set; } = new List<CardImage>();

		public CardFactory()
		{
		}
		public CardFactory(string title, string text)
		{
			Title = title;
			Text = text;
		}
		public CardFactory(string title, string text, IList<CardAction> actions)
		{
			Title = title;
			Text = text;
			Actions = actions;
		}
		public CardFactory(string title, string text, IList<CardAction> actions, IList<CardImage> images)
		{
			Title = title;
			Text = text;
			Actions = actions;
			Images = images;
		}

		public static CardFactory GetInstance()
		{
			if(_uniqueInstance == null) _uniqueInstance = new CardFactory();
			return _uniqueInstance;
		}

		public Activity CreateCard()
		{
			var card = new HeroCard();
			card.Title = Title;
			card.Text = Text;
			card.Buttons = Actions;

			var message = (Activity)Activity.CreateMessageActivity();
			message.Attachments.Add(new Attachment
			{
				ContentType = HeroCard.ContentType,
				Content = card
			});
			return message;
		}

		public Activity CreateMenuCard(string title, PromptOptions options)
		{
			var card = new HeroCard();
			card.Title = title;
            card.Buttons = options.Choices.Select(c => new CardAction
			{
				Type = ActionTypes.ImBack,
				Value = c.Value,
				DisplayText = c.Value?.ToString(),
				Text = c.Value?.ToString(),
				Title = c.Value?.ToString()
			}).ToList();

			var message = (Activity)Activity.CreateMessageActivity();
			message.Attachments.Add(new Attachment
			{
				ContentType = HeroCard.ContentType,
				Content = card
			});
			return message;
		}

		public Activity CreateMenuCard(string title, string text, PromptOptions options)
		{
			var card = new HeroCard();
			card.Title = title;
			card.Text = text;
			card.Buttons = options.Choices.Select(c => new CardAction
			{
				Type = ActionTypes.ImBack,
				Value = c.Value,
				DisplayText = c.Value?.ToString(),
				Text = c.Value?.ToString(),
				Title = c.Value?.ToString()
			}).ToList();

			var message = (Activity)Activity.CreateMessageActivity();
			message.Attachments.Add(new Attachment
			{
				ContentType = HeroCard.ContentType,
				Content = card
			});
			return message;
		}

		public Activity CreateMenuCard(string title, string text, List<CardAction> buttons)
		{
			var card = new HeroCard();
			card.Title = title;
			card.Text = text;
			card.Buttons = buttons;

			var message = (Activity)Activity.CreateMessageActivity();
			message.Attachments.Add(new Attachment
			{
				ContentType = HeroCard.ContentType,
				Content = card
			});
			return message;
		}
	}
}