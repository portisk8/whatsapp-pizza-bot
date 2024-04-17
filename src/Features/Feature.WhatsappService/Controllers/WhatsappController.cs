using Feature.WhatsappService.Config;
using Microsoft.AspNetCore.JsonPatch.Internal;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WhatsappBusiness.CloudApi.Messages.Requests;

namespace Feature.WhatsappService.Controllers
{
    [ApiController]
    public class WhatsappController : ControllerBase
    {
        private readonly WhatsappService _whatsappService;
        private readonly WhatsappConfig _config;
        private readonly ILogger _logger;
        public WhatsappController(WhatsappService whatsappService, WhatsappConfig config,ILogger logger)
        {
            _whatsappService = whatsappService;
            _config = config;
            _logger = logger;
        }

        [HttpGet]
        [Route("webhook")]
        public async Task<IActionResult> GetWebhook(
            [FromQuery(Name = "hub.mode")] string mode,
            [FromQuery(Name = "hub.challenge")] string challenge,
            [FromQuery(Name = "hub.verify_token")] string verifyToken)
        {
            if (!string.IsNullOrEmpty(mode)
                && !string.IsNullOrEmpty(verifyToken))
            {
                // Check the mode and token sent are correct
                if (mode == "subscribe"
                    && verifyToken == _config.WebHookToken)
                {
                    return Ok(challenge);
                }
                else
                {
                    // Responds with '403 Forbidden' if verify tokens do not match
                    return Forbid();
                }
            }
            return BadRequest();
        }

        [HttpPost]
        [Route("webhook")]
        public async Task<IActionResult> PostWebhook()
        {
            try
            {
                await _whatsappService.ProcessAsync(Request);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"[WhatsappController] Error > {ex.Message}");
                return Ok();
            }
        }

        [HttpGet]
        [Route("test")]
        public async Task<IActionResult> PostWebhook([FromQuery] string phoneNumber, [FromQuery] string type)
        {
            try
            {
                switch (type.ToLower())
                {
                    case "message":
                        await SendMessage(phoneNumber);
                        break;
                    case "audio":
                        await SendAudio(phoneNumber);
                        break;
                    case "image":
                        await SendImage(phoneNumber);
                        break;
                    case "document":
                        await SendDocument(phoneNumber);
                        break;
                    case "sticker":
                        await SendSticker(phoneNumber);
                        break;
                    case "video":
                        await SendVideo(phoneNumber);
                        break;
                    case "contact":
                        await SendContact(phoneNumber);
                        break;
                    case "location":
                        await SendLocation(phoneNumber);
                        break;
                    case "list":
                        await SendInteractiveList(phoneNumber);
                        break;
                    case "button":
                        await SendInteractiveReplyButton(phoneNumber);
                        break;
                    default:
                        break;
                }
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"[WhatsappController] Error > {ex.Message}");
                return Ok();
            }
        }

        #region Test
        private Task SendDocument(string phoneNumber)
        {
            throw new NotImplementedException();
        }

        private Task SendVideo(string phoneNumber)
        {
            throw new NotImplementedException();
        }

        private async Task SendMessage(string phoneNumber)
        {
            TextMessageRequest textMessageRequest = new TextMessageRequest();
            textMessageRequest.To = phoneNumber;
            textMessageRequest.Text = new WhatsAppText();
            textMessageRequest.Text.Body = "Message Body";
            textMessageRequest.Text.PreviewUrl = false;

            var results = await _whatsappService.WhatsAppClient.SendTextMessageAsync(textMessageRequest);
        }
        private async Task SendAudio(string phoneNumber)
        {
            var urlAudio = $"{HttpContext.Request.Host}/assets/audios/audio-test.mp3";
            AudioMessageByUrlRequest audioMessage = new AudioMessageByUrlRequest();
            audioMessage.To = phoneNumber;
            audioMessage.Audio = new MediaAudioUrl();
            audioMessage.Audio.Link = urlAudio;

            var results = await _whatsappService.WhatsAppClient.SendAudioAttachmentMessageByUrlAsync(audioMessage);
        }

        private async Task SendInteractiveReplyButton(string phoneNumber)
        {
            InteractiveReplyButtonMessageRequest interactiveReplyButtonMessage = new InteractiveReplyButtonMessageRequest();
            interactiveReplyButtonMessage.To = phoneNumber;
            interactiveReplyButtonMessage.Interactive = new InteractiveReplyButtonMessage();

            interactiveReplyButtonMessage.Interactive.Body = new ReplyButtonBody();
            interactiveReplyButtonMessage.Interactive.Body.Text = "Reply Button Body";

            interactiveReplyButtonMessage.Interactive.Action = new ReplyButtonAction();
            interactiveReplyButtonMessage.Interactive.Action.Buttons = new List<ReplyButton>()
            {
                new ReplyButton()
                {
                    Type = "reply",
                    Reply = new Reply()
                    {
                        Id = "SAMPLE_1_CLICK",
                        Title = "CLICK ME!!!"
                    }
                },

                new ReplyButton()
                {
                    Type = "reply",
                    Reply = new Reply()
                    {
                        Id = "SAMPLE_2_CLICK",
                        Title = "LATER"
                    }
                }
            };

            var results = await _whatsappService.WhatsAppClient.SendInteractiveReplyButtonMessageAsync(interactiveReplyButtonMessage);
        }

        private async Task SendInteractiveList(string phoneNumber)
        {
            InteractiveListMessageRequest interactiveListMessage = new InteractiveListMessageRequest();
            interactiveListMessage.To = phoneNumber;
            interactiveListMessage.Interactive = new InteractiveListMessage();

            interactiveListMessage.Interactive.Header = new Header();
            interactiveListMessage.Interactive.Header.Type = "text";
            interactiveListMessage.Interactive.Header.Text = "List Header Sample Test";

            interactiveListMessage.Interactive.Body = new ListBody();
            interactiveListMessage.Interactive.Body.Text = "List Message Body";

            interactiveListMessage.Interactive.Footer = new Footer();
            interactiveListMessage.Interactive.Footer.Text = "List Footer Sample Test";

            interactiveListMessage.Interactive.Action = new ListAction();
            interactiveListMessage.Interactive.Action.Button = "Send";
            interactiveListMessage.Interactive.Action.Sections = new List<Section>()
            {
                new Section()
                {
                    Title = "Category A",
                    Rows = new List<Row>()
                    {
                        new Row()
                        {
                            Id = "Item_A1",
                            Title = "Apples",
                            //Description = "Enjoy fruits for free"
                        },
                        new Row()
                        {
                            Id = "Item_A2",
                            Title = "Tangerines",
                            Description = "Enjoy fruits for free"
                        },
                    },
                },
                new Section()
                {
                    Title = "Category B",
                    Rows = new List<Row>()
                    {
                        new Row()
                        {
                            Id = "Item_B1",
                            Title = "2JZ",
                            Description = "Engine discounts"
                        },
                        new Row()
                        {
                            Id = "Item_2",
                            Title = "1JZ",
                            Description = "Engine discounts"
                        },
                    }
                }
            };

            var results = await _whatsappService.WhatsAppClient.SendInteractiveListMessageAsync(interactiveListMessage);
        }

        private async Task SendLocation(string phoneNumber)
        {
            LocationMessageRequest locationMessageRequest = new LocationMessageRequest();
            locationMessageRequest.To = phoneNumber;
            locationMessageRequest.Location = new Location();
            locationMessageRequest.Location.Name = "Google México";
            locationMessageRequest.Location.Address = "C. Montes Urales 445";
            locationMessageRequest.Location.Longitude = -99.20670385327475;
            locationMessageRequest.Location.Latitude = 19.42855975927536;

            var results = await _whatsappService.WhatsAppClient.SendLocationMessageAsync(locationMessageRequest);
        }

        private async Task SendContact(string phoneNumber)
        {
            ContactMessageRequest contactMessageRequest = new ContactMessageRequest();
            contactMessageRequest.To = phoneNumber;
            contactMessageRequest.Contacts = new List<ContactData>()
            {
                new ContactData()
                {
                    Addresses = new List<Address>()
                    {
                        new Address()
                        {
                            State = "State Test",
                            City = "City Test",
                            Zip = "Zip Test",
                            Country = "Country Test",
                            CountryCode = "Country Code Test",
                            Type = "Home"
                        }
                    },
                    Name = new Name()
                    {
                        FormattedName = "Augusto",
                        FirstName = "Augusto",
                        LastName = "Chatbot",
                        MiddleName = "MName",

                    }
                }
            };

            var results = await _whatsappService.WhatsAppClient.SendContactAttachmentMessageAsync(contactMessageRequest);
        }

        private async Task SendSticker(string phoneNumber)
        {
            var urlSticker = $"{HttpContext.Request.Host}/assets/images/test-sticker.webp";
            //https://www.tyntec.com/docs/whatsapp-business-api-tutorials-creating-and-sending-whatsapp-stickers
            StickerMessageByUrlRequest stickerMessage = new StickerMessageByUrlRequest();
            stickerMessage.To = phoneNumber;
            stickerMessage.Sticker = new MediaStickerUrl();
            stickerMessage.Sticker.Link = urlSticker;
            var results = await _whatsappService.WhatsAppClient.SendStickerMessageByUrlAsync(stickerMessage);
        }

        private async Task SendImage(string phoneNumber)
        {
            ImageMessageByUrlRequest imageMessage = new ImageMessageByUrlRequest();
            imageMessage.To = phoneNumber;
            imageMessage.Image = new MediaImageUrl();
            imageMessage.Image.Link = "https://cdn.pixabay.com/photo/2017/02/09/21/08/wings-2053515_640.png";
            imageMessage.Image.Caption = "Messaging Caption";
            var results = await _whatsappService.WhatsAppClient.SendImageAttachmentMessageByUrlAsync(imageMessage);
        }
        #endregion
    }
}
