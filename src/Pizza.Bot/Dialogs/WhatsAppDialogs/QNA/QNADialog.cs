using Feature.QNAService;
using Microsoft.Bot.Builder.Dialogs;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.Bot.Builder;
using Pizza.Bot.Entities;
using System.Text.Json;
using System.Linq;
using Pizza.Bot.Utils;
using System.Collections.Generic;
using Whatsapp.Adapter.WhatsAppAdapter.Incomings;

namespace Pizza.Bot.Dialogs.WhatsAppDialogs.QNA
{
    public class QNADialog : ComponentDialog
    {
        private readonly QNAServiceRecognizer _qnaServiceRecognizer;
        private readonly IStatePropertyAccessor<UserProfile> _userProfileAccessor;
        private readonly Serilog.ILogger _logger;

        public QNADialog(QNAServiceRecognizer qnaServiceRecognizer,
            UserState userState,
            Serilog.ILogger logger
            )
            : base(DialogFlowType.Whatsapp(nameof(QNADialog)))
        {
            _qnaServiceRecognizer = qnaServiceRecognizer;
            _userProfileAccessor = userState.CreateProperty<UserProfile>(nameof(UserProfile));
            _logger = logger;
            var steps = new WaterfallStep[]
            {
                GetQNAResponseAsync,
            };
            AddDialog(new WaterfallDialog("_firstStep", steps));
            InitialDialogId = "_firstStep";
        }

        private async Task<DialogTurnResult> GetQNAResponseAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            UserProfile profile = await _userProfileAccessor.GetAsync(stepContext.Context, () => new UserProfile(), cancellationToken);

            if (string.IsNullOrEmpty(stepContext.Context?.Activity?.Text))
                return await stepContext.EndDialogAsync(cancellationToken: cancellationToken);

            var result = await _qnaServiceRecognizer.RecognizeAsync(stepContext.Context);
            
            if (result != null)
            {
                try
                {
                    var qnaResponse = JsonSerializer.Deserialize<QNAResponse>(result.Answer);
                    if(qnaResponse.Title != null)
                    {

                        //activities.Add(MessageFactory.Text($"**{qnaResponse.Title}**"));
                        await stepContext.Context.SendActivityAsync(MessageFactory.Text($"**{qnaResponse.Title}**"), cancellationToken);

                    }
                    if (qnaResponse.Message != null)
                    {
                        //activities.Add(MessageFactory.Text(qnaResponse.Message));
                        await stepContext.Context.SendActivityAsync(MessageFactory.Text(qnaResponse.Message), cancellationToken);

                    }
                    if (qnaResponse.Images.Any())
                    {
                        foreach (var item in qnaResponse.Images)
                        {
                            //activities.Add(WhatsappUtils.ImageToActivity("imagen.png", "image/png", item.Media));
                            await stepContext.Context.SendActivityAsync(WhatsappUtils.ImageToActivity("imagen.png", "image/png", item.Media), cancellationToken);
                        }
                    }
                    if (result.Context.Prompts.Any())
                    {
                        var buttonReply = new List<ButtonReply>();
                        foreach (var btn in result.Context.Prompts)
                        {
                            buttonReply.Add(new ButtonReply
                            {
                                Id = $"qna_{btn.QnaId}",
                                Title = btn.DisplayText
                            });
                        }
                        var activity = stepContext.Context.Activity;
                        activity.Text = null;
                        activity.Properties.Add("buttons", JsonSerializer.Serialize(buttonReply));
                        activity.Properties.Add("buttonText", "Tambien podes consultar:");
                        await stepContext.Context.SendActivityAsync(activity, cancellationToken);
                    }
                }
                catch (System.Exception ex)
                {
                    await stepContext.Context.SendActivityAsync(MessageFactory.Text(result.Answer), cancellationToken);
                }
            }
            return await stepContext.EndDialogAsync(cancellationToken: cancellationToken);
        }
    }
}
