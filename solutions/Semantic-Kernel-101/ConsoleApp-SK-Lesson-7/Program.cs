using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.Agents.Chat;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel.Agents.OpenAI;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using AzureOpenAISearchConfiguration;
using Plugins;
using System;
using System.Data;
using Azure.Monitor.OpenTelemetry.Exporter;
using OpenTelemetry;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Azure.Identity;
using Azure.Core;
using Microsoft.Extensions.Logging.Console;
using Azure;
using System.Net;
using MyAgent.Models;
using ConsoleApp_SK_Lesson_7.Helper;

var configuration = new Configuration();
new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddEnvironmentVariables()
    .AddJsonFile("local.settings.json")
    .Build()
    .Bind(configuration);

configuration.Validate();

var isDebugMode = configuration.DebugMode;

// This section of Code is only here to allow the testing of AppInsights Logging.
// I only use this if the OpenTelemetry code is having issues and I am not seeing things in App Insights.
// 

#region Testing AppInsights
var checkAppInsights = false;

if (checkAppInsights)
{
    // When this code executes you should see a TestActivity logged to AppInsights
    // This will verify that you logging to AppInsights is working
    string AppInsightsConnection= configuration.AzureAppInsights ?? "";
    using var tracerProvider = Sdk.CreateTracerProviderBuilder()
           .AddSource("DemoSource")
           .AddAzureMonitorTraceExporter(options =>
           {
               options.ConnectionString = AppInsightsConnection;
           })
           .Build();

    // Manually create a trace
    var activitySource = new ActivitySource("DemoSource");
    using (var activity = activitySource.StartActivity("TestActivity"))
    {
        activity?.SetTag("demo", "test");
    }
}
#endregion End Testing AppInsights

// Create an ActivitySource that matches your .AddSource name:
//var manualSource = new ActivitySource("TelemetryMyExample");

#region Enable OpenTelemetry for Semantic Kernel and Logging
var enableOpenTelemtry = false;  // change to true if you want to use OpenTelemtry logging for SLK
// please make sure to have you Application Insights Connection String in your local.settings.json defined!

// Create a resource builder for OpenTelemetry (used if OpenTelemetry is enabled)
var resourceBuilder = ResourceBuilder.CreateDefault().AddService("TelemetryMyExample");

// Initialize AppInsights connection string
string AppInsightsConnectionString = configuration.AzureAppInsights ?? "";

// Define a reusable method to configure OpenTelemetry if enabled
Action<ILoggingBuilder> configureOpenTelemetry = builder =>
{
    builder.AddOpenTelemetry(options =>
    {
        options.SetResourceBuilder(resourceBuilder);
        options.AddAzureMonitorLogExporter(options => options.ConnectionString = AppInsightsConnectionString);
        options.IncludeFormattedMessage = true;
        options.IncludeScopes = true;
    });
};

// Define a reusable method to set console logging
Action<ILoggingBuilder> configureConsoleLogging = builder =>
{
    // Get log level from configuration, default to Information if not specified
    var logLevel = configuration.LogLevel?.ToUpper() switch
    {
        "DEBUG" => LogLevel.Debug,
        "TRACE" => LogLevel.Trace,
        "INFORMATION" => LogLevel.Information,
        "WARNING" => LogLevel.Warning,
        "ERROR" => LogLevel.Error,
        "CRITICAL" => LogLevel.Critical,
        _ => LogLevel.Information // Default to Information if not specified
    };

    // Add console logging if debug mode is enabled or log level is specified
    if (isDebugMode || configuration.LogLevel != null)
    {
        builder.AddSimpleConsole(options =>
        {
            options.IncludeScopes = true;
            options.TimestampFormat = "[HH:mm:ss] ";
            options.SingleLine = true;
            options.UseUtcTimestamp = true;
            options.ColorBehavior = LoggerColorBehavior.Disabled;
        });
    }

    builder.SetMinimumLevel(logLevel);
};

// Create the OpenTelemetry TracerProvider and MeterProvider if OpenTelemetry is enabled
if (enableOpenTelemtry)
{
    AppContext.SetSwitch("Microsoft.SemanticKernel.Experimental.GenAI.EnableOTelDiagnosticsSensitive", true);

    using var traceProvider = Sdk.CreateTracerProviderBuilder()
        .SetResourceBuilder(resourceBuilder)
        .AddSource("Microsoft.SemanticKernel*")
        .AddSource("TelemetryMyExample")
        .AddAzureMonitorTraceExporter(options => options.ConnectionString = AppInsightsConnectionString)
        .Build();

    using var meterProvider = Sdk.CreateMeterProviderBuilder()
        .SetResourceBuilder(resourceBuilder)
        .AddMeter("Microsoft.SemanticKernel*")
        .AddAzureMonitorMetricExporter(options => options.ConnectionString = AppInsightsConnectionString)
        .Build();
}

// Create the logger factory
using var loggerFactory = LoggerFactory.Create(builder =>
{
    if (enableOpenTelemtry)
    {
        configureOpenTelemetry(builder);
    }

    configureConsoleLogging(builder);
});

// Create a logger for agent communication
var agentLogger = loggerFactory.CreateLogger("AgentCommunication\n");

# endregion End of Enable OpenTelemetry for Semantic Kernel and Logger Setup
//agentLogger.LogDebug("Creating the Kernel with Chat Completeion...\n");
//// Initialize kernel with chat completion service
//Kernel kernel = CreateKernelWithChatCompletion();

#region Do you need Azure Credentials for anything?
// Aquire Credentials
// If you need credentials for access various resources in Azure you can use this appraoch to get a token
// Of course there are many ways to get a credential token!
// This code is here for example purposes
var aquireCredentials = false;
if (aquireCredentials)
{
    agentLogger.LogDebug("Retreiving Azure Credentials");
    Console.WriteLine("A browser window will open for authentication. Please select your account...");
    var credential = new InteractiveBrowserCredential(new InteractiveBrowserCredentialOptions
    {
        TokenCachePersistenceOptions = new TokenCachePersistenceOptions()
    });

    // Validate the credential by acquiring a token (one-time prompt)
    await credential.GetTokenAsync(new TokenRequestContext(new[] { "https://management.azure.com/.default" }));
    Console.WriteLine("Authentication successful!");

    //var kernel = Kernel.CreateBuilder()
    //    .AddAzureOpenAIChatCompletion(
    //        configuration.AzureOpenAIDeployment!,
    //        configuration.AzureOpenAIEndpoint!,
    //        configuration.AzureOpenAIApiKey!,
    //        serviceId: "azure-openai"
    //    )
    //    .Build();
}
#endregion

AgentGroupChatHelper agentGroupChat = new AgentGroupChatHelper(configuration);
await agentGroupChat.UseAgentGroupChatWithTwoAgentsAsync();

// See AgentGroupChatPlugins for an example with Plugins but it's still a work in progress.

