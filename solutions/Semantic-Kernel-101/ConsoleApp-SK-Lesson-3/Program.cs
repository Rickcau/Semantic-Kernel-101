using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using System.Configuration;


Console.WriteLine("Your first Semantic Kernel Chat with Your Data App \n");

// 5 Simple steps to use the Kernel!

#region Step 1: Create Kernel Builder
// Create a Builder for Creating Kernel Objects
var builder = Kernel.CreateBuilder();
#endregion

#region Step 2: Load AI Endpoint Values

var openAiDeployment = ConfigurationManager.AppSettings.Get("AzureOpenAIModel");
var openAiUri = ConfigurationManager.AppSettings.Get("AzureOpenAIEndpoint");
var openAiApiKey = ConfigurationManager.AppSettings.Get("AzureOpenAIKey");
var aiSearchUri = ConfigurationManager.AppSettings.Get("AzureSearchEndpoint");
var aiSearchApiKey = ConfigurationManager.AppSettings.Get("AzureSearchApiKey");
var aiSearchIndex = ConfigurationManager.AppSettings.Get("AzureSearchIndex");
var aiSearchSemanticConfig = ConfigurationManager.AppSettings.Get("AzureSearchSemanticConfig");

#endregion

#region Step 3: Add ChatCompletion Service

builder.Services.AddAzureOpenAIChatCompletion(
   deploymentName: openAiDeployment!,
   endpoint: openAiUri!,
   apiKey: openAiApiKey!);

#endregion

#region Step 4: Construct Kernel, ChatHistory Get instance of ChatCompletion Service
// Construct instance of Kernel using Builder Settings
var kernel = builder.Build();

ChatHistory history = [];

var chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();

#endregion

// Important Breaking Changes:  https://learn.microsoft.com/en-us/azure/ai-services/openai/references/on-your-data?tabs=python
// In this example we are going to use the ChatCompletion Extension endpoint to allow us to chat with our data.
// For simple cases this allows uss to allow the AI to call the AI Search without us having to use the AI Search Client.

// 1st we need to properly setup the OpenAIPrmptExecutionSettings object for this.
#pragma warning disable SKEXP0010
var dataConfig = new AzureOpenAIChatCompletionWithDataConfig
{
    CompletionModelId = openAiDeployment ?? "",
    CompletionEndpoint = openAiUri ?? "",
    CompletionApiKey = openAiApiKey ?? "",
    DataSourceEndpoint = aiSearchUri ?? "",
    DataSourceApiKey = aiSearchApiKey ?? "",
    DataSourceIndex = aiSearchIndex ?? ""
};

// Why can I not simply use ChatCompletionServiceExtensions.GetChatMessageContentAsync()
// I am simply trying to replicate what I can do via Postman and passing the data_sources in the request body
// If I can get the same type of results using this, then I don't have to use an AI Search client


// var test = ChatCompletionServiceExtensions.GetChatCompletionTest(dataConfig);

var chatCompletion = new AzureOpenAIChatCompletionWithDataService(dataConfig);

var dataSources = new[]
    {
        new
        {
            type = "azure_search",
            parameters = new
            {
                endpoint = aiSearchUri,
                index_name = aiSearchIndex,
                semantic_configuration = aiSearchSemanticConfig,
                query_type = "vectorSemanticHybrid",
                fields_mapping = new { },
                in_scope = true,
                role_information = "You are an AI assistant that helps people find information.  Format the output in a nice bullet list",
                filter = "",
                strictness = 3,
                top_n_documents = 5,
                authentication = new
                    {
                        type = "api_key",
                        key = aiSearchApiKey
                    },
                embedding_dependency = new
                    {
                    type = "deployment_name",
                    deployment_name = "rdc-text-embedding"
                    },
                key = aiSearchApiKey,
                indexName = aiSearchIndex
            }
        }
    };
var settings = new OpenAIPromptExecutionSettings
{
    ChatSystemPrompt = "You are an AI assistant that helps people find information.",

    ModelId = openAiDeployment,
    Temperature = 0,
    TopP = 1,
    FrequencyPenalty = 0,
    PresencePenalty = 0,

    ExtensionData = new Dictionary<string, object>
        {
            { "dataSources", dataSources }
        },
};

ChatHistory chat = new();
chat.AddUserMessage("What are my healthcare benefits?  Please respond with a simple list.");


// Chat Completion example
var chatMessage = (AzureOpenAIWithDataChatMessageContent)await chatCompletion.GetChatMessageContentAsync(chat, settings);

var response = chatMessage.Content!;
var toolResponse = chatMessage.ToolContent;

Console.WriteLine(response);
Console.WriteLine("\n\n");
Console.WriteLine($@"ToolResponse: {toolResponse}");
Console.WriteLine("Press any key to end.");
Console.ReadLine();



// Examples of Calling methods
/*
 var companySearchPluginObj = new CompanySearchPlugin();

var companySearchPlugin = kernel.ImportPluginFromObject(companySearchPluginObj, "CompanySearchPlugin");

var weatherContent = await kernel.InvokeAsync( companySearchPlugin["WeatherSearch"],new(){["text"] = "guangzhou"});

weatherContent.GetValue<string>()
 */


//static void RunPluginExamples(Kernel kernel)
//{
//    kernel.ImportPluginFromPromptDirectory(Path.Combine("./Plugins/Prompts", "SummarizePlugin"));
//}


