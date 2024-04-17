using Azure;
using Feature.WhatsappService.Config;
using Feature.WhatsappService.Entities;
using Feature.WhatsappService.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.VisualBasic;
using Newtonsoft.Json.Linq;
using Serilog;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading;
using WhatsApp.Adapters.WhatsAppAdapter.Entities;
using WhatsApp.Adapters.WhatsAppAdapter.Incomings;
using WhatsappBusiness.CloudApi;
using WhatsappBusiness.CloudApi.Configurations;
using WhatsappBusiness.CloudApi.Interfaces;
using WhatsappBusiness.CloudApi.Messages.Requests;
using WhatsappBusiness.CloudApi.Response;

namespace Feature.WhatsappService
{
    public class WhatsappService
    {
        private readonly ILogger _logger;
        public IWhatsAppBusinessClient WhatsAppClient;
        private readonly WhatsappConfig _whatsappConfig;
        public event EventHandler<WhatsAppIncomingRequest> OnMessageReceived;
        private readonly IHttpClientFactory _httpClientFactory;

        public WhatsappService(WhatsappConfig config, IHttpClientFactory httpClientFactory, ILogger logger)
        {
            _whatsappConfig = config;
            _logger = logger;
            _httpClientFactory = httpClientFactory;
            WhatsAppClient = new WhatsAppBusinessClient(new WhatsAppBusinessCloudApiConfig
            {
                AccessToken = config.AccessToken,
                WhatsAppBusinessAccountId = config.AccountId,
                WhatsAppBusinessId = config.WhatsAppBusinessId,
                WhatsAppBusinessPhoneNumberId = config.PhoneNumberId,
                AppName = config.AppName,
                Version = config.Version,
            });
        }

        public async Task ProcessAsync(HttpRequest httpRequest)
        {
            string stringifiedBody;

            using (var sr = new StreamReader(httpRequest.Body))
            {
                stringifiedBody = await sr.ReadToEndAsync().ConfigureAwait(false);
            }

            var incomingMessage = JsonSerializer.Deserialize<WhatsAppIncomingRequest>(stringifiedBody);

            if (incomingMessage!= null 
                && incomingMessage.Entries.FirstOrDefault()?.Changes.FirstOrDefault()?.Value.Statuses == null)
            {
                OnMessageReceived(null, incomingMessage);
            }
            else
            {
                Console.WriteLine("incomingMessage is null");
            }

        }

        public async Task<List<WhatsAppResponse>> SendAsync (List<WhatsappConverted> messages)
        {
            var responses = new List<WhatsAppResponse>();
            WhatsAppResponse whatsappResponse;

            foreach (var item in messages)
            {
                try
                {
                    switch (item.MessageType)
                    {
                        case MessageTypeEnum.TEXT:
                            var textMessage = (TextMessageRequest)item.Value;
                            whatsappResponse = await WhatsAppClient.SendTextMessageAsync(textMessage);

                            if (whatsappResponse.Messages.Any())
                            {
                                responses.Add(whatsappResponse);
                            }
                            break;
                        case MessageTypeEnum.AUDIO:
                            var audio = (AudioMessageByUrlRequest)item.Value;
                            _logger.Information(JsonSerializer.Serialize(audio));
                            whatsappResponse = await WhatsAppClient.SendAudioAttachmentMessageByUrlAsync(audio);
                            if (whatsappResponse.Messages.Any() && responses.Count < 1)
                            {
                                responses.Add(whatsappResponse);
                            }
                            break;
                        case MessageTypeEnum.DOCUMENT:
                            break;
                        case MessageTypeEnum.IMAGE:
                            var imageMessage = (ImageMessageByUrlRequest)item.Value;
                            _logger.Information(JsonSerializer.Serialize(imageMessage));

                            whatsappResponse = await WhatsAppClient.SendImageAttachmentMessageByUrlAsync(imageMessage);

                            if (whatsappResponse.Messages.Any() && responses.Count < 1)
                            {
                                responses.Add(whatsappResponse);
                            }

                            break;
                        case MessageTypeEnum.STICKER:
                            //https://www.tyntec.com/docs/whatsapp-business-api-tutorials-creating-and-sending-whatsapp-stickers
                            var stickerMessage = (StickerMessageByUrlRequest)item.Value;
                            _logger.Information(JsonSerializer.Serialize(stickerMessage));
                            whatsappResponse = await WhatsAppClient.SendStickerMessageByUrlAsync(stickerMessage);

                            if (whatsappResponse.Messages.Any() && responses.Count < 1)
                            {
                                responses.Add(whatsappResponse);
                            }
                            break;
                        case MessageTypeEnum.VIDEO:
                            break;
                        case MessageTypeEnum.CONTACT:
                            break;
                        case MessageTypeEnum.LOCATION:
                            var location = (LocationMessageRequest)item.Value;
                            _logger.Information(JsonSerializer.Serialize(location));
                            whatsappResponse = await WhatsAppClient.SendLocationMessageAsync(location);
                            if (whatsappResponse.Messages.Any() && responses.Count < 1)
                            {
                                responses.Add(whatsappResponse);
                            }
                            break;
                        case MessageTypeEnum.INTERACTIVE_LIST:
                            var interactiveList = (InteractiveListMessageRequest)item.Value;
                            _logger.Information(JsonSerializer.Serialize(interactiveList));
                            whatsappResponse = await WhatsAppClient.SendInteractiveListMessageAsync(interactiveList);
                            if (whatsappResponse.Messages.Any() && responses.Count < 1)
                            {
                                responses.Add(whatsappResponse);
                            }
                            break;
                        case MessageTypeEnum.INTERACTIVE_REPLY_BUTTON:
                            var interactiveReply_Button = (InteractiveReplyButtonMessageRequest)item.Value;

                            _logger.Information(JsonSerializer.Serialize(interactiveReply_Button));

                            whatsappResponse = await WhatsAppClient.SendInteractiveReplyButtonMessageAsync(interactiveReply_Button);

                            if (whatsappResponse.Messages.Any() && responses.Count < 1)
                            {
                                responses.Add(whatsappResponse);
                            }
                            break;
                        case MessageTypeEnum.TEMPLATE:
                            break;
                        case MessageTypeEnum.TEMPLATE_TEXT_WITH_PARAMETERS:
                            break;
                        case MessageTypeEnum.TEMPLATE_MEDIA_WITH_PARAMETERS:
                            break;
                        case MessageTypeEnum.TEMPLATE_INTERACTIVE_WITH_PARAMETERS:
                            break;
                        default:
                            break;
                    }

                }
                catch (Exception ex)
                {
                    _logger.Error(ex, $"Error en SendActivitiesAsync > {ex.Message}");
                }
            }
            return responses;
        }

        public async Task<MediaFile> GetMediaAsync(WhatsApp.Adapters.WhatsAppAdapter.Incomings.Message message)
        {

            switch (MessageTypeEnumExtensions.GetFromText(message.Type))
            {
                case MessageTypeEnum.TEXT:
                    break;
                case MessageTypeEnum.AUDIO:
                    break;
                case MessageTypeEnum.DOCUMENT:
                    break;
                case MessageTypeEnum.IMAGE:
                    var mediaUrl = await WhatsAppClient.GetMediaUrlAsync(message.Image.Id);
                    var media = await WhatsAppClient.DownloadMediaAsync(mediaUrl.Url);
                    return new MediaFile
                    {
                        ContentType = mediaUrl.MimeType,
                        FileArray = media
                    };
                case MessageTypeEnum.STICKER:
                    break;
                case MessageTypeEnum.VIDEO:
                    break;
                case MessageTypeEnum.CONTACT:
                    break;
                case MessageTypeEnum.LOCATION:
                    break;
                case MessageTypeEnum.INTERACTIVE_LIST:
                    break;
                case MessageTypeEnum.INTERACTIVE_REPLY_BUTTON:
                    break;
                case MessageTypeEnum.TEMPLATE:
                    break;
                case MessageTypeEnum.TEMPLATE_TEXT_WITH_PARAMETERS:
                    break;
                case MessageTypeEnum.TEMPLATE_MEDIA_WITH_PARAMETERS:
                    break;
                case MessageTypeEnum.TEMPLATE_INTERACTIVE_WITH_PARAMETERS:
                    break;
                default:
                    break;
            }
            return null;
        }


    }
}
