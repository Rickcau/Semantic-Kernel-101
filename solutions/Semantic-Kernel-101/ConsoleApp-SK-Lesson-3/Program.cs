using Azure.AI.OpenAI;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using System.Configuration;
using static System.Net.Mime.MediaTypeNames;
using System.Runtime.Intrinsics.Arm;
using System.Collections.Immutable;


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

var azureSearchExtensionConfiguration = new AzureSearchChatExtensionConfiguration
{
    SearchEndpoint = new Uri(aiSearchUri ?? ""),
    IndexName = aiSearchIndex,
    Authentication = new OnYourDataApiKeyAuthenticationOptions(aiSearchApiKey),
    QueryType = AzureSearchQueryType.VectorSemanticHybrid,
    ShouldRestrictResultScope = true,
    SemanticConfiguration = aiSearchSemanticConfig,
    Strictness = 3,
    DocumentCount = 5,
    VectorizationSource = new OnYourDataDeploymentNameVectorizationSource("rdc-text-embedding")
};

var  AzureExtensionsOptions = new AzureChatExtensionsOptions()
{
       Extensions = { azureSearchExtensionConfiguration }
};
#pragma warning disable SKEXP0010
var promptExecutionSettings = new OpenAIPromptExecutionSettings { AzureChatExtensionsOptions = AzureExtensionsOptions };

var test = await kernel.InvokePromptAsync("What are my healthcare benefits?  Please respond with a simple list.", new(promptExecutionSettings));
ChatHistory chat = new();
chat.AddUserMessage("What are my healthcare benefits?  Please respond with a simple list.");
var chatMessage = await chatCompletionService.GetChatMessageContentAsync(chat, promptExecutionSettings);

var response = chatMessage.Content!;

var innercontent = chatMessage.InnerContent as Azure.AI.OpenAI.ChatResponseMessage;

// Now let's extract the Citations from the InnerContent as we are using the Azure ChatExtensionsOptions
foreach (var citation in innercontent?.AzureExtensionsContext?.Citations ?? [])
{
    Console.WriteLine($"Citation: {citation.Title}");
}
Console.WriteLine("\n\n");

Console.WriteLine(response);
Console.WriteLine("\nCitations:\n");
var i = 0;
foreach (var citation in innercontent?.AzureExtensionsContext?.Citations ?? [])
{
    i++;
    Console.WriteLine($"[Doc{i}]: {citation.Title}");
}

//Console.WriteLine($@"ToolResponse: {toolResponse}");
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


