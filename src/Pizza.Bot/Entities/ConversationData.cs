using System;

namespace Pizza.Bot.Entities
{
    public class ConversationData
	{
		public DateTimeOffset LastActivity { get; set; }
		public UserProfile UserProfile { get; set; }
        public bool PreguntarSiNecesitaAlgoMas { get; set; }
		public bool GoToMenuPrincipal { get; set; }

	}
}
