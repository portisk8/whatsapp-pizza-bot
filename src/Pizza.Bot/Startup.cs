// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
//
// Generated with Bot Builder V4 SDK Template for Visual Studio CoreBot v4.18.1

using Feature.CluService;
using Feature.CluService.Config;
using Feature.Core.Config;
using Feature.QNAService;
using Feature.QNAService.Config;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.ApplicationInsights;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Integration.ApplicationInsights.Core;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Bot.Connector.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Pizza.Bot.Dialogs;
using Pizza.Bot.Entities.Cards;
using Pizza.Bot.ErrorHandlers;
using RepoDb.DbHelpers;
using RepoDb.DbSettings;
using RepoDb.StatementBuilders;
using RepoDb;
using Serilog;
using Serilog.Exceptions;
using Feature.WhatsappService.Config;
using Feature.WhatsappService;
using Feature.WhatsappAdapter;
using Pizza.Core.Configuration.Interfaces;

namespace Pizza.Bot
{
    public class Startup
    {
        public static ICurrentConfiguration CurrentConfiguration { get; set; }
        public IConfiguration Configuration { get; }
        public ConfigBase ConfigBase { get; set; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHttpClient().AddControllers().AddNewtonsoftJson();

            #region Logger
            //Logger
            var logger = new LoggerConfiguration()
                                     .Enrich.WithExceptionDetails()
                                     .ReadFrom.Configuration(Configuration)
                                     .WriteTo.Console()
                                     .CreateLogger();
            services.AddSingleton<Serilog.ILogger>(logger);
            #endregion

            // Create the Bot Framework Authentication to be used with the Bot Adapter.
            services.AddSingleton<BotFrameworkAuthentication, ConfigurationBotFrameworkAuthentication>();

            // Create the Bot Adapter with error handling enabled.
            services.AddSingleton<IBotFrameworkHttpAdapter, AdapterWithErrorHandler>();

            // Create the storage we'll be using for User and Conversation state. (Memory is great for testing purposes.)
            services.AddSingleton<IStorage, MemoryStorage>();

            // Create the User state. (Used in this bot's Dialog implementation.)
            services.AddSingleton<UserState>();

            // Create the Conversation state. (Used by the Dialog system itself.)
            services.AddSingleton<ConversationState>();

            services.AddSingleton<CardFactory>();

            #region Telemetry
            //Region Telemetry
            // Add Application Insights services into service collection
            services.AddApplicationInsightsTelemetry();

            // Create the telemetry client.
            services.AddSingleton<IBotTelemetryClient, BotTelemetryClient>();

            // Add telemetry initializer that will set the correlation context for all telemetry items.
            services.AddSingleton<ITelemetryInitializer, OperationCorrelationTelemetryInitializer>();

            // Add telemetry initializer that sets the user ID and session ID (in addition to other bot-specific properties such as activity ID)
            services.AddSingleton<ITelemetryInitializer, TelemetryBotIdInitializer>();

            // Create the telemetry middleware to initialize telemetry gathering
            services.AddSingleton<TelemetryInitializerMiddleware>();

            // Create the telemetry middleware (used by the telemetry initializer) to track conversation events
            services.AddSingleton<TelemetryLoggerMiddleware>(sp =>
            {
                var telemetryClient = sp.GetService<IBotTelemetryClient>();
                return new TelemetryLoggerMiddleware(telemetryClient, logPersonalInformation: true);
            });
            //Fin Region Telemetry
            #endregion

            #region Configurations
            CurrentConfiguration = Pizza.Core.Configuration.CurrentConfiguration.Build(Configuration);
            services.AddSingleton(CurrentConfiguration);
            #endregion

            services = ConfigureBusinessRepository(services);

            #region Clu
            //https://language.cognitive.azure.com/
            services.AddSingleton(CluServiceConfig.Build(Configuration));
            services.AddSingleton<CluServiceRecognizer>();
            #endregion

            #region QNA
            services.AddSingleton(QNAServiceConfig.Build(Configuration));
            services.AddSingleton<QNAServiceRecognizer>();

            #endregion

            #region WhatsApp
            //Add dependencies for Whatsapp
            services.AddSingleton(WhatsappConfig.Build(Configuration));
            services.AddSingleton<WhatsappService>();
            services.AddSingleton<WhatsAppAdapter, WhatsAppAdapterWithErrorHandler>();

            // Add Infobip Adapter with error handler
            services.AddTransient<Dialogs.WhatsAppDialogs.QNA.QNADialog>();
            
            services.AddSingleton<Dialogs.WhatsAppDialogs.RootDialog>();
            #endregion
            
            
            //Dialogs
            services.AddSingleton<RootDialog>();
            //services.AddSingleton<Dialogs.WhatsAppDialogs.RootDialog>();
            services.AddSingleton<Dialogs.DefaultDialogs.RootDialog>();

            // Create the storage we'll be using for User and Conversation state. (Memory is great for testing purposes.)
            services.AddSingleton<IStorage, MemoryStorage>();

            // Create the User state. (Used in this bot's Dialog implementation.)
            services.AddSingleton<UserState>();
            // Create the Conversation state. (Used by the Dialog system itself.)
            services.AddSingleton<ConversationState>();

            //services.AddSingleton<CardFactory>();

            //Para solucionar issue:  https://stackoverflow.com/questions/67816160/how-to-close-the-qnamker-dialog-while-using-multiturn-qna-kb-and-luis-with-the-h
            //https://github.com/microsoft/BotBuilder-Samples/issues/3194
            ComponentRegistration.Add(new DialogsComponentRegistration());

            // Create the bot as a transient. In this case the ASP Controller is expecting an IBot.
            services.AddTransient<IBot, InitBot<RootDialog>>();
        }

        private IServiceCollection ConfigureBusinessRepository(IServiceCollection services)
        {
            if (!string.IsNullOrEmpty(CurrentConfiguration.General.ConnectionString))
            {
                var dbConnection = new System.Data.SqlClient.SqlConnection(CurrentConfiguration.General.ConnectionString);
                GlobalConfiguration
                    .Setup()
                    .UseSqlServer();
                var dbSetting = new SqlServerDbSetting();

                DbSettingMapper
                    .Add<System.Data.SqlClient.SqlConnection>(dbSetting, true);
                DbHelperMapper
                    .Add<System.Data.SqlClient.SqlConnection>(new SqlServerDbHelper(), true);
                StatementBuilderMapper
                    .Add<System.Data.SqlClient.SqlConnection>(new SqlServerStatementBuilder(dbSetting), true);
            }

            //services.AddSingleton<ChatbotRepository>();
            //services.AddSingleton<ChatbotBusiness>();
            return services;
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }


            app.UseDefaultFiles()
                .UseStaticFiles()
                .UseWebSockets()
                .UseRouting()
                .UseAuthorization()
                .UseEndpoints(endpoints =>
                {
                    endpoints.MapControllers();
                });

            // app.UseHttpsRedirection();
        }
    }
}
