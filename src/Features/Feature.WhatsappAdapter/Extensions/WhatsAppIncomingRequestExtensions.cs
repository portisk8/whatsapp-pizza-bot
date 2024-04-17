using Feature.WhatsappService.Enums;
using Microsoft.Bot.Schema;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WhatsApp.Adapters.WhatsAppAdapter.Incomings;

namespace Feature.WhatsappAdapter.Extensions
{
    public static class WhatsAppIncomingRequestExtensions
    {
        public static async Task<IEnumerable<Activity>> ConvertToActivitiesAsync(this WhatsAppIncomingRequest incomingMessage)
        {
            if (incomingMessage == null) throw new ArgumentNullException(nameof(incomingMessage));

            var changeValue = incomingMessage.Entries.FirstOrDefault()?.Changes.FirstOrDefault()?.Value;
            if (changeValue == null) return Enumerable.Empty<Activity>();

            var messages = changeValue.Messages;
            var result = new List<Activity>();
            foreach (var message in messages)
            {
                try
                {
                    var activity = message.ConvertToActivity();
                    if (activity != null)
                        result.Add(activity);

                }
                catch
                {
                    return null;
                }
            }
            return result;
        }

    }
}
