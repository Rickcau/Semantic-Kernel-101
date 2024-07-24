using Azure.AI.OpenAI;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using System.Configuration;
using static System.Net.Mime.MediaTypeNames;
using System.Runtime.Intrinsics.Arm;
using System.Collections.Immutable;
using ConsoleApp_SK_Lesson_3.Models;
using System.Text.Json;


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
    VectorizationSource = new OnYourDataDeploymentNameVectorizationSource("rdc-text-embedding"),
    FieldMappingOptions = new AzureSearchIndexFieldMappingOptions
    {
        FilepathFieldName = "metadata_storage_path",
        TitleFieldName = "title"
    }
};

var  AzureExtensionsOptions = new AzureChatExtensionsOptions()
{
       Extensions = { azureSearchExtensionConfiguration }
};
#pragma warning disable SKEXP0010
var promptExecutionSettings = new OpenAIPromptExecutionSettings { AzureChatExtensionsOptions = AzureExtensionsOptions };
// just showing you that you can use InvokePromptAsync() for this and still get access to the Citation data
// var testResponseInvokeAsync = await kernel.InvokePromptAsync("What are my healthcare benefits?  Please respond with a simple list.", new(promptExecutionSettings));
// "Why do we need a No Retaliation Policy"
var testResponseInvokeAsync = await kernel.InvokePromptAsync("Why do we need a No Retaliation Policy", new(promptExecutionSettings));
var testInnnercontent = testResponseInvokeAsync.GetValue<ChatMessageContent>()!.InnerContent as Azure.AI.OpenAI.ChatResponseMessage;
var test5 = testInnnercontent?.AzureExtensionsContext;
var i = 0;
foreach (var citation in testInnnercontent?.AzureExtensionsContext?.Citations ?? [])
{
    i++;
    Console.WriteLine($"[Doc{i}]: {citation.Title}"); 
}


ChatHistory chat = new();
// chat.AddUserMessage("What are my healthcare benefits?  Please respond with a simple list.");
// Why do we need a No Retaliation Policy
chat.AddSystemMessage("If the requested information is not found in the retreived data, simply reponse with 'I am unable to answer query, please try another query or topic.'");
chat.AddUserMessage("Why is the sky blue?");
//chat.AddUserMessage("What is the Northwindws no retaliation policy?");
//chat.AddUserMessage("Why do we need a No Retaliation Policy");
var chatMessage = await chatCompletionService.GetChatMessageContentAsync(chat, promptExecutionSettings);
// For the "Why is the sky blue question?" The AI should response with the following: "{The requested information is not found in the retrieved data. Please try another query or topic.}"

// Create an instance of the ChatResponse object 
// I am testing out different ways to manage the response and citations and how a structured might be returned to a client via API.
var chatResponse = new ChatResponse
                    {
                        Content = chatMessage.Content!,
                        Citations = new List<Citation>()
                    };
i = 0;
var innercontent = chatMessage.InnerContent as Azure.AI.OpenAI.ChatResponseMessage;
foreach (var citation in innercontent?.AzureExtensionsContext?.Citations ?? [])
{
    i++;
    Console.WriteLine($"[Doc{i}]: {citation.Title}");
    chatResponse.Citations.Add(new Citation
    {
        Title = $"[doc{i}]: {citation.Title}",
        Link = citation.Filepath
    });
}

Console.WriteLine(chatMessage.Content!);
Console.WriteLine("\nCitations:\n");
i = 0; // reset i
// Now let's extract the Citations from the InnerContent as we are using the Azure ChatExtensionsOptions and print them out
foreach (var citation in innercontent?.AzureExtensionsContext?.Citations ?? [])
{
    i++;
    Console.WriteLine($"[Doc{i}]: {citation.Title}");
}

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


