using Feature.WhatsappAdapter;
using Feature.WhatsappService;
using Feature.WhatsappService.Config;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Integration.ApplicationInsights.Core;
using Microsoft.Bot.Builder.TraceExtensions;
using Microsoft.Extensions.Logging;
using System;

namespace Pizza.Bot.ErrorHandlers
{
    public class WhatsAppAdapterWithErrorHandler : WhatsAppAdapter
    {
        public WhatsAppAdapterWithErrorHandler(WhatsappService whatsapp,
            ILogger<WhatsAppAdapterWithErrorHandler> logger,
            TelemetryInitializerMiddleware telemetryInitializerMiddleware,
            ConversationState conversationState = default)
          : base(whatsapp, logger)
        {
            OnTurnError = async (turnContext, exception) =>
            {
                // Log any leaked exception from the application.
                logger.LogError($"Exception caught : {exception.Message}");

                // Send a catch-all apology to the user.
                await turnContext.SendActivityAsync(exception.Message);
                
                if (conversationState != null)
                {
                    try
                    {
                        // Delete the conversationState for the current conversation to prevent the
                        // bot from getting stuck in a error-loop caused by being in a bad state.
                        // ConversationState should be thought of as similar to "cookie-state" in a Web pages.
                        await conversationState.DeleteAsync(turnContext);
                    }
                    catch (Exception e)
                    {
                        logger.LogError(e, $"Exception caught on attempting to Delete ConversationState : {e.Message}");
                    }
                }
                await turnContext.TraceActivityAsync("OnTurnError Trace", exception.Message, "https://www.botframework.com/schemas/error", "TurnError");
            };
            Use(telemetryInitializerMiddleware);
        }
    }
}
