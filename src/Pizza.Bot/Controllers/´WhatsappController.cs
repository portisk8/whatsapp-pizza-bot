// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
//
// Generated with Bot Builder V4 SDK Template for Visual Studio EmptyBot v4.15.2

using Feature.WhatsappAdapter;
using Feature.WhatsappService.Config;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Bot.Builder;
using System;
using System.Threading.Tasks;

namespace Pizza.Bot.Controllers
{
    // This ASP Controller is created to handle a request. Dependency Injection will provide the Adapter and IBot
    // implementation at runtime. Multiple different IBot implementations running at different endpoints can be
    // achieved by specifying a more specific type for the bot constructor argument.
    //[Route("api/whatsapp/messages")]
    [ApiController]
    public class WhatsappController : ControllerBase
    {
        private readonly WhatsAppAdapter _adapter;
        private readonly IBot _bot;
        private readonly WhatsappConfig _config;

        public WhatsappController(WhatsAppAdapter adapter, IBot bot, WhatsappConfig config)
        {
            _adapter = adapter;
            _bot = bot;
            _config = config;
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
        //public async Task PostWebhook([FromBody] dynamic messageReceived)
        {
            try
            {
                await _adapter.ProcessAsync(Request, Response, _bot);
                return Ok();
            }
            catch (Exception ex)
            {

                return Ok();
            }
        }

        [HttpPost]
        [HttpGet]
        [Route("api/whatsapp/actions")]
        public async Task<IActionResult> Actions()
        {
            // Delegate the processing of the HTTP POST to the adapter.
            // The adapter will invoke the bot.
            await _adapter.ProcessAsync(Request, Response, _bot);
            return Ok();
        }

        [HttpPost]
        [HttpGet]
        [Route("api/whatsapp/webhooks")]
        public async Task<IActionResult> Webhooks()
        {
            // Delegate the processing of the HTTP POST to the adapter.
            // The adapter will invoke the bot.
            await _adapter.ProcessAsync(Request, Response, _bot);
            return Ok();
        }
    }
}
