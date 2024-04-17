using Feature.QNAService.Config;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.AI.QnA;
using Microsoft.Bot.Builder.AI.QnA.Models;

namespace Feature.QNAService
{
    public class QNAServiceRecognizer
    {
        private readonly QNAServiceConfig _qnaServiceConfig;
        private readonly IQnAMakerClient _qna;
        public QNAServiceRecognizer(QNAServiceConfig qnaServiceConfig, IBotTelemetryClient telemetryClient)
        {
            _qnaServiceConfig = qnaServiceConfig;
            var qnaIsConfigured = !string.IsNullOrEmpty(_qnaServiceConfig.APIKey)
                                    && !string.IsNullOrEmpty(_qnaServiceConfig.HostName)
                                    && !string.IsNullOrEmpty(_qnaServiceConfig.KnowledgebaseId);
            if (qnaIsConfigured)
            {
                // QnA configurations
                _qna = new CustomQuestionAnswering(new QnAMakerEndpoint
                {
                    KnowledgeBaseId = _qnaServiceConfig.KnowledgebaseId,
                    EndpointKey = _qnaServiceConfig.APIKey,
                    Host = _qnaServiceConfig.HostName,
                    QnAServiceType = ServiceType.Language
                }, null, null, telemetryClient);
            }
            else
            {
                throw new Exception("Faltan configuraciones de QNA, revise: APIKey, HostName, KnowledgebaseId.");
            }
        }

        public async Task<QueryResult?> RecognizeAsync(ITurnContext turnContext)
        {
            var options = new QnAMakerOptions { Top = 1, EnablePreciseAnswer = _qnaServiceConfig.EnablePreciseAnswer };
            if(turnContext.Activity.Value != null
                && turnContext.Activity.Name == "REPLY_BUTTON"
                && ((string)turnContext.Activity.Value).Contains("qna_")
                )
            {
                options.QnAId = int.Parse(((string)turnContext.Activity.Value).Substring(4));
            }
            var response = await _qna.GetAnswersAsync(turnContext, options,null);
            if (response.Length > 0)
                return response[0];
            else
                return null;
        }
    }
}
