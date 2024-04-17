using AdaptiveCards;
using Feature.WhatsappService.Entities;
using Feature.WhatsappService.Enums;
using Microsoft.Bot.Schema;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.RegularExpressions;
using Whatsapp.Adapter.WhatsAppAdapter.Incomings;
using WhatsappBusiness.CloudApi.Messages.Requests;

namespace Feature.WhatsappAdapter.Converters
{
    public class ActivityToWhatsappConvert
    {
        static Regex REGEX_LINK => new Regex(@"(\[((?:\[[^\]]*\]|[^\[\]])*)\]\([ \t]*()<?((?:\([^)]*\)|[^()\s])*?)>?[ \t]*((['\""])(.*?)\6[ \t] *)?\))");

        static Regex REGEX_BOLD_ITALIC => new Regex(@"\*{3}((?=[^\s\*]).*?[^\s\*])\*{3}");
        static Regex REGEX_BOLD => new Regex(@"\*{2}((?=[^\s\*]).*?[^\s\*])\*{2}");
        static Regex REGEX_HEADING_MARKDOWN => new Regex(@"(###\s(.+))");

        public static List<WhatsappConverted> Convert(Activity activity)
        {
            var list = new List<WhatsappConverted>();

            if (!string.IsNullOrEmpty(activity.Text))
            {
                TextMessageRequest textMessageRequest = new TextMessageRequest()
                {
                    To = activity.Recipient.Id,
                    Text = new WhatsAppText()
                    {
                        Body = CleanTextToWhatsapp(activity.Text),
                    }
                };
                list.Add(new WhatsappConverted { MessageType = MessageTypeEnum.TEXT, Value = textMessageRequest });
            }
            if(activity.Properties.ContainsKey("buttons")) 
            {
                var listButton = JsonSerializer.Deserialize<List<ButtonReply>>((string)activity.Properties.GetValue("buttons"));
                var buttonText = (string)activity.Properties.GetValue("buttonText");
                var converted = ConvertToButton(activity.Recipient.Id, listButton, buttonText);
                list.Add(converted);
            }

            if (activity.Attachments != null && activity.Attachments.Any())
            {
                foreach (var item in activity.Attachments)
                {
                    if(item.Content?.GetType() == typeof(HeroCard))
                    {
                        list.AddRange(ConvertFromHeroCard(activity.Recipient.Id, (HeroCard) item.Content));
                    }
                    if(item.Content?.GetType() == typeof(AdaptiveCard))
                    {
                        list.AddRange(ConvertFromAdaptiveCard(activity.Recipient.Id, (AdaptiveCard)item.Content));
                    }
                    if (item.ContentType == "image/png")
                    {
                        list.AddRange(ConvertFromImage(activity.Recipient.Id, item));
                    }
                    if (item.ContentType == "image/sticker")
                    {
                        list.AddRange(ConvertToStickerFromImage(activity.Recipient.Id, item));
                    }
                }
            }

            return list;
        }

        private static WhatsappConverted ConvertToButton(string phoneNumber, List<ButtonReply> listButton, string? buttonText)
        {
            InteractiveReplyButtonMessageRequest interactiveReplyButtonMessage = new InteractiveReplyButtonMessageRequest();
            interactiveReplyButtonMessage.To = phoneNumber;
            interactiveReplyButtonMessage.Interactive = new InteractiveReplyButtonMessage();

            interactiveReplyButtonMessage.Interactive.Body = new ReplyButtonBody();
            interactiveReplyButtonMessage.Interactive.Body.Text = buttonText;

            interactiveReplyButtonMessage.Interactive.Action = new ReplyButtonAction();
            interactiveReplyButtonMessage.Interactive.Action.Buttons = new List<ReplyButton>();
            foreach (ButtonReply buttonReply in listButton)
            {
                interactiveReplyButtonMessage.Interactive.Action.Buttons.Add(new ReplyButton
                {
                    Type = "reply",
                    Reply = new Reply
                    {
                        Id = buttonReply.Id,
                        Title = buttonReply.Title,
                    }
                });
            }
            return new WhatsappConverted
            {
                MessageType = MessageTypeEnum.INTERACTIVE_REPLY_BUTTON,
                Value = interactiveReplyButtonMessage
            };
        }

        private static IEnumerable<WhatsappConverted> ConvertToStickerFromImage(string recipientId, Attachment item)
        {
            var list = new List<WhatsappConverted>();
            StickerMessageByUrlRequest stickerMessage = new StickerMessageByUrlRequest();
            stickerMessage.To = recipientId;
            stickerMessage.Sticker = new MediaStickerUrl();
            stickerMessage.Sticker.Link = item.ContentUrl;


            list.Add(new WhatsappConverted
            {
                MessageType = MessageTypeEnum.STICKER,
                Value = stickerMessage
            });
            return list;
        }

        private static IEnumerable<WhatsappConverted> ConvertFromImage(string recipientId, Attachment item)
        {
            var list = new List<WhatsappConverted>();
            ImageMessageByUrlRequest imageMessage = new ImageMessageByUrlRequest();
            imageMessage.To = recipientId;
            imageMessage.Image = new MediaImageUrl();
            imageMessage.Image.Link = item.ContentUrl;

            list.Add(new WhatsappConverted
            {
                MessageType = MessageTypeEnum.IMAGE,
                Value = imageMessage
            });
            return list;
        }

        private static List<WhatsappConverted> ConvertFromAdaptiveCard(string recipientId, AdaptiveCard adaptiveCard)
        {
            var list = new List<WhatsappConverted>();
            InteractiveListMessageRequest interactiveListMessage = new InteractiveListMessageRequest();
            interactiveListMessage.To = recipientId;
            interactiveListMessage.Interactive = new InteractiveListMessage();
            interactiveListMessage.Interactive.Action = new ListAction();
            interactiveListMessage.Interactive.Action.Button = "Enviar";
            interactiveListMessage.Interactive.Action.Sections = new List<Section>();

            if (!string.IsNullOrEmpty(adaptiveCard.Title))
            {
                interactiveListMessage.Interactive.Header = new Header();
                interactiveListMessage.Interactive.Header.Type = "text";
                interactiveListMessage.Interactive.Header.Text = adaptiveCard.Title;
            }

            if (adaptiveCard.Body.Any())
            {
                foreach (var item in adaptiveCard.Body)
                {
                    if (item.GetType() == typeof(AdaptiveTextBlock))
                    {
                        var adaptiveTextBlock = (AdaptiveTextBlock) item;
                        if(interactiveListMessage.Interactive.Body == null)
                        {
                            interactiveListMessage.Interactive.Body = new ListBody();
                            interactiveListMessage.Interactive.Body.Text = adaptiveTextBlock.Text;
                        }
                        else
                        {
                            interactiveListMessage.Interactive.Body.Text += $" {adaptiveTextBlock.Text}";
                        }
                        
                    }
                    else if (item.GetType() == typeof(AdaptiveChoiceSetInput))
                    {
                        var adaptiveChoiceSetInput = (AdaptiveChoiceSetInput)item;
                        var section = new Section();
                        section.Title = adaptiveChoiceSetInput.Label ?? adaptiveChoiceSetInput.Placeholder;
                        section.Rows = new List<Row>();

                        if (adaptiveChoiceSetInput.Choices.Count > 10)
                            throw new Exception("Whatsapp admite hasta 10 Rows por mensaje. No se puede adaptar a una lista interactiva.");

                        foreach (var choice in adaptiveChoiceSetInput.Choices)
                        {
                            section.Rows.Add(new Row()
                            {
                                Id = choice.Value.ToString(),
                                Title = choice.Title
                            });
                        }
                        interactiveListMessage.Interactive.Action.Sections.Add(section);
                    }
                }
            }
            
            list.Add(new WhatsappConverted
            {
                MessageType = MessageTypeEnum.INTERACTIVE_LIST,
                Value = interactiveListMessage
            });
            return list;
        }

        public static List<WhatsappConverted> ConvertFromHeroCard (string recipientId, HeroCard heroCard)
        {
            const int BUTTON_MAX = 3;
            var list = new List<WhatsappConverted>();
            var firstTime = true;
            var count = 0;
            InteractiveReplyButtonMessageRequest interactiveReplyButtonMessage = null;
            //==> TODO: Hacer un split de mensajes si tiene mas de 3 botones
            if (heroCard.Buttons != null && heroCard.Buttons.Count > BUTTON_MAX)
            {
                
                foreach (var button in heroCard.Buttons)
                {
                    if(count%BUTTON_MAX == 0)
                    {
                        interactiveReplyButtonMessage = new InteractiveReplyButtonMessageRequest()
                        {
                            To = recipientId,
                            Interactive = new InteractiveReplyButtonMessage()
                        };
                        interactiveReplyButtonMessage.Interactive.Action = new ReplyButtonAction();
                        interactiveReplyButtonMessage.Interactive.Action.Buttons = new List<ReplyButton>();

                        if (firstTime)
                        {
                            firstTime = false;
                            interactiveReplyButtonMessage.Interactive.Body = new ReplyButtonBody()
                            {
                                Text = heroCard.Text ?? "Opciones:",
                            };
                        }
                        else
                        {
                            interactiveReplyButtonMessage.Interactive.Body = new ReplyButtonBody()
                            {
                                Text = "Más opciones:",
                            };
                        }
                    }
                    var buttonTitle = button.Title;
                    var buttonId = $"{count}_{button.Title.Replace(" ", "_")}";
                    interactiveReplyButtonMessage.Interactive.Action.Buttons.Add(new ReplyButton()
                    {
                        Type = "reply",
                        Reply = new Reply()
                        {
                            Title = buttonTitle.Length > 20 ? buttonTitle.Substring(0,20) : buttonTitle,
                            Id = buttonId.Length > 20 ? buttonId.Substring(0, 20) : buttonId
                        }
                    });

                    if ((count+1) % BUTTON_MAX == 0 || count+1 == heroCard.Buttons.Count)
                    {
                        list.Add(new WhatsappConverted
                        {
                            MessageType = MessageTypeEnum.INTERACTIVE_REPLY_BUTTON,
                            Value = interactiveReplyButtonMessage
                        });
                    }
                    
                    count++;
                }
                
            }
            else
            {
                interactiveReplyButtonMessage = new InteractiveReplyButtonMessageRequest()
                {
                    To = recipientId,
                    Interactive = new InteractiveReplyButtonMessage()
                };
                interactiveReplyButtonMessage.Interactive.Body = new ReplyButtonBody()
                {
                    Text = heroCard.Title ?? heroCard.Text ?? "Opciones:",
                };
                if (heroCard.Buttons != null)
                {
                    interactiveReplyButtonMessage.Interactive.Action = new ReplyButtonAction();
                    interactiveReplyButtonMessage.Interactive.Action.Buttons = new List<ReplyButton>();
                    foreach (var button in heroCard.Buttons)
                    {
                        count++;
                        var buttonTitle = button.Title;
                        var buttonId = $"{count}_{button.Title.Replace(" ", "_")}";

                        interactiveReplyButtonMessage.Interactive.Action.Buttons.Add(new ReplyButton()
                        {
                            Type = "reply",
                            Reply = new Reply()
                            {
                                Title = buttonTitle.Length > 20 ? buttonTitle.Substring(0, 20) : buttonTitle,
                                Id = buttonId.Length > 20 ? buttonId.Substring(0, 20) : buttonId
                            }
                        });
                    }
                }
                list.Add(new WhatsappConverted
                {
                    MessageType = MessageTypeEnum.INTERACTIVE_REPLY_BUTTON,
                    Value = interactiveReplyButtonMessage
                });
            }
            
            return list;
        }

        public static string CleanTextToWhatsapp (string text)
        {
            var hasLink = REGEX_LINK.Matches(text).ToList();
            if (hasLink.Any())
            {
                foreach (var item in hasLink)
                {
                    var reg = new Regex(@"\(([^\)]+)\)");
                    var content = reg.Match(item.Value);
                    text = text.Replace(item.Value, content.Value.Replace("(", "").Replace(")", ""));
                }
            }
            var hasBoldItalic = REGEX_BOLD_ITALIC.Matches(text).ToList();
            if (hasBoldItalic.Any())
            {
                foreach (var item in hasBoldItalic)
                {
                    var content = item.Value.Replace("***", "");
                    text = text.Replace(item.Value, $"_*{content}*_");
                }
            }
            var hasBold = REGEX_BOLD.Matches(text).ToList();
            if (hasBold.Any())
            {
                foreach (var item in hasBold)
                {
                    var content = item.Value.Replace("**", "");
                    text = text.Replace(item.Value, $"*{content}*");
                }
            }
            var hasHeading = REGEX_HEADING_MARKDOWN.Matches(text).ToList();
            if (hasHeading.Any())
            {
                foreach (var item in hasHeading)
                {
                    var content = item.Value.Replace("### ", "");
                    text = text.Replace(item.Value, $"*{content}*");
                }
            }
            return text;
        }
    }
}