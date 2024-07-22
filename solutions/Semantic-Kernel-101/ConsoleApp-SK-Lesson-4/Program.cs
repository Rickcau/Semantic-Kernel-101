using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Newtonsoft.Json.Linq;
using System.Configuration;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using Cosmos.Chat.GPT.Services;
using Cosmos.Chat.GPT.Models;


Console.WriteLine("Our first Console App - Semantic Kernel Chat Bot\n");

// 5 Simple steps to use the Kernel!

#region Step 1: Create Kernel Builder
// Create a Builder for Creating Kernel Objects
var builder = Kernel.CreateBuilder();
#endregion

#region Step 2: Load AI Endpoint Values

var openAiDeployment = ConfigurationManager.AppSettings.Get("AzureOpenAIModel");
var openAiUri = ConfigurationManager.AppSettings.Get("AzureOpenAIEndpoint");
var openAiApiKey = ConfigurationManager.AppSettings.Get("AzureOpenAIKey");
var cosmosDbEndPoint = ConfigurationManager.AppSettings.Get("CosmosDb_Endpoint") ?? "";
var cosmosDbName = ConfigurationManager.AppSettings.Get("CosmosDb_Name") ?? "";
var cosmosDbChatContainer = ConfigurationManager.AppSettings.Get("CosmosDb_ChatContainerName") ?? "";
var cosmosDbCacheContainer = ConfigurationManager.AppSettings.Get("CosmosDb_CacheContainerName") ?? "";

int maxConversationTokens = Int32.TryParse(ConfigurationManager.AppSettings.Get("MaxConversationTokens"), out maxConversationTokens) ? maxConversationTokens : 100;
Double cacheSimilarityScore = Double.TryParse(ConfigurationManager.AppSettings.Get("CacheSimilarityScore"), out cacheSimilarityScore) ? cacheSimilarityScore : 0.99;


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

#region Example of Serializing Message and DeSerializing
// This section of code demostrations how you can serialize and deserialize the chatHistory
// Basically, you could serialize it to JSON then store it in CosmosDB
// Retreive it then Deserialize into a C# object.

JsonSerializerOptions s_options = new() { WriteIndented = true };
int[] data = [1, 2, 3];
#pragma warning disable SKEXP0010
#pragma warning disable SKEXP0001
var message = new ChatMessageContent(AuthorRole.User, "Describe the factors contributing to climate change.")
{
    Items =
           [
               new TextContent("Discuss the potential long-term consequences for the Earth's ecosystem as well."),
               new ImageContent(new Uri("https://fake-random-test-host:123")),
               new BinaryContent(new BinaryData(data), "application/octet-stream"),
               new AudioContent(new BinaryData(data), "application/octet-stream")
           ]
};

var chatHistory = new ChatHistory([message]);

var chatHistoryJson = JsonSerializer.Serialize(chatHistory, s_options);

var deserializedHistory = JsonSerializer.Deserialize<ChatHistory>(chatHistoryJson);

var deserializedMessage = deserializedHistory!.Single();

Console.WriteLine($"Content: {deserializedMessage.Content}");
Console.WriteLine($"Role: {deserializedMessage.Role.Label}");
#endregion

var promptText = "Why is the Sky Blue?";

CosmosDbService cosmosDbService = new CosmosDbService(cosmosDbEndPoint, cosmosDbName, cosmosDbChatContainer, cosmosDbCacheContainer);
// first create a new session object
Session session = new();
List<Session> ChatSessions = new();
ChatService chatservice = new(cosmosDbService, "100","0.99");
var messageFromChatService = await chatservice.GetChatCompletionAsync(session.Id, promptText); 
ChatSessions.Add(messageFromChatService);

// var theSession = await cosmosDbService.InsertSessionAsync(session);
theSession.Name = "My Test Session";  // You can use GenAI to get a short descrption for the prompt and use that for the name...

// simulate ChatService.GetChatCompletionAsync(
// this function calls the following:
//  Message chatMessage = await CreateChatMessageAsync(sessionId, promptText);
//  which gets the token count for the prompt and it calls cosmosDbService.InsertSessionAsync()
// it then calls GetChatSessionContextWindow() and it returns a List<Messages> in chronological order using the _maxConversationTokens to keep the context window in check
// next it performs a cache search if a match is found it returns the message which has the cached completion and prompt no need to call LLM

#region Step 5: Send Prompt Get Response - Exercise One

// var prompt = "Why is the Sky blue?";
//var result = await chatCompletionService.GetChatMessageContentAsync(prompt);
//Console.WriteLine(result);
//Console.WriteLine("\nPress enter to end.");
//Console.ReadLine();

#endregion

#region Chat Loop - Exercise Two

while (true)
{
    Console.Write(">> ");
    var userMessage = Console.ReadLine();
    if (userMessage != "Exit")
    {
        history.AddUserMessage(userMessage!);

        // Not really being used in this example but we will use it in future examples
        OpenAIPromptExecutionSettings openAIPromptExecutionSettings = new()
        {
            ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions
        };

        try
        {
            var result = await chatCompletionService.GetChatMessageContentAsync(
                history,
                executionSettings: openAIPromptExecutionSettings,
                kernel: kernel);

            Console.WriteLine("<< " + result);

            if (result.Content != null)
            {
                history.AddAssistantMessage(result.Content);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }
    else break;
}

#endregion


static void RunPluginExamples(Kernel kernel)
{
    kernel.ImportPluginFromPromptDirectory(Path.Combine("./Plugins/Prompts", "SummarizePlugin"));
}


