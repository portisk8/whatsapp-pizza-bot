using Feature.CluService.Clu;
using Feature.CluService.Config;
using Microsoft.Bot.Builder;
//https://learn.microsoft.com/en-us/azure/ai-services/language-service/conversational-language-understanding/tutorials/bot-framework
namespace Feature.CluService
{
    public class CluServiceRecognizer
    {
        private readonly CluServiceConfig _cluServiceConfig;
        private readonly CluRecognizer _recognizer;

        public CluServiceRecognizer(CluServiceConfig cluServiceConfig)
        {
            _cluServiceConfig = cluServiceConfig;
            var cluIsConfigured = !string.IsNullOrEmpty(_cluServiceConfig.ProjectName) 
                                    && !string.IsNullOrEmpty(_cluServiceConfig.DeploymentName) 
                                    && !string.IsNullOrEmpty(_cluServiceConfig.APIKey) 
                                    && !string.IsNullOrEmpty(_cluServiceConfig.HostName);
            if (cluIsConfigured)
            {
                var cluApplication = new CluApplication(
                    _cluServiceConfig.ProjectName,
                    _cluServiceConfig.DeploymentName,
                    _cluServiceConfig.APIKey,
                    _cluServiceConfig.HostName);
                // Set the recognizer options depending on which endpoint version you want to use.
                var recognizerOptions = new CluOptions(cluApplication)
                {
                    Language = _cluServiceConfig.Language ?? "es",
                };

                _recognizer = new CluRecognizer(recognizerOptions);
            }
        }

        // Returns true if clu is configured in the appsettings.json and initialized.
        public virtual bool IsConfigured => _recognizer != null;

        public virtual async Task<RecognizerResult> RecognizeAsync(ITurnContext turnContext, CancellationToken cancellationToken)
            => await _recognizer.RecognizeAsync(turnContext, cancellationToken);

        public virtual async Task<T> RecognizeAsync<T>(ITurnContext turnContext, CancellationToken cancellationToken)
            where T : IRecognizerConvert, new()
            => await _recognizer.RecognizeAsync<T>(turnContext, cancellationToken);
    }
}
