using Microsoft.Extensions.Logging;
using WhatsApp.Adapters.WhatsAppAdapter.Incomings;
using Microsoft.AspNetCore.Http;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using System.Text.Json;
using Feature.WhatsappAdapter.Extensions;
using WhatsappBusiness.CloudApi.Messages.Requests;
using WhatsappBusiness.CloudApi.Response;
using Feature.WhatsappAdapter.Converters;

namespace Feature.WhatsappAdapter
{
    public class WhatsAppAdapter : AdapterBase
    {
        private readonly Feature.WhatsappService.WhatsappService _whatsappService;
        private readonly ILogger _logger;

        public WhatsAppAdapter(
            Feature.WhatsappService.WhatsappService whatsappService,
            ILogger logger)
        {
            _whatsappService = whatsappService;
            _logger = logger;
        }

        public override async Task ProcessAsync(HttpRequest httpRequest, HttpResponse httpResponse, IBot bot, CancellationToken cancellationToken = default)
        {
            string stringifiedBody;

            using (var sr = new StreamReader(httpRequest.Body))
            {
                stringifiedBody = await sr.ReadToEndAsync().ConfigureAwait(false);
            }

            var incomingMessage = JsonSerializer.Deserialize<WhatsAppIncomingRequest>(stringifiedBody);

            //var incomingMessage = stringifiedBody.FromInfobipIncomingMessageJson<InfobipWhatsAppIncomingResult>();
            if (incomingMessage.Entries.FirstOrDefault().Changes.FirstOrDefault().Value.Statuses == null)
            {
                var activities = await incomingMessage.ConvertToActivitiesAsync();
                foreach (var activity in activities)
                {
                    MarkMessageRequest markMessageRequest = new MarkMessageRequest();
                    markMessageRequest.MessageId = activity.Id;
                    markMessageRequest.Status = "read";

                    await _whatsappService.WhatsAppClient.MarkMessageAsReadAsync(markMessageRequest);

                    using (var context = new TurnContext(this, activity))
                    {
                        await RunPipelineAsync(context, bot.OnTurnAsync, cancellationToken).ConfigureAwait(false);
                    }
                }

            }
            else
            {
                Console.WriteLine(incomingMessage);
            }
        }

        public override async Task<ResourceResponse[]> SendActivitiesAsync(ITurnContext turnContext, Activity[] activities, CancellationToken cancellationToken)
        {
            var responses = new List<ResourceResponse>();
            foreach (var activity in activities)
            {
                if (activity.Type == ActivityTypes.Message)
                {
                    var messages = ActivityToWhatsappConvert.Convert(activity);
                    var whatsappResponseList = await _whatsappService.SendAsync(messages);
                    responses.AddRange(whatsappResponseList.Select(x => new ResourceResponse
                    {
                        Id = x.Messages[0].Id,
                    }));
                }
            }
            return responses.ToArray();
        }
    }
}
