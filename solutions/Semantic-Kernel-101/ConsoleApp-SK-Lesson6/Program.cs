using ConsoleApp_SK_Lesson6.Prompts;
using Cosmos.Chat.GPT.Models;
using Cosmos.Chat.GPT.Services;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Newtonsoft.Json.Linq;
using System.Configuration;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using ConsoleApp_SK_Lesson6.Models;
using OpenAI.Chat;


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

var prompt = "How many streams were viewed on amc+ with amazon in us in January 2024?";

var responseExample1 = await Example1TestJSONObjectMode(prompt, kernel);
Console.WriteLine("## Example 1 using json_object mode...\n");
string jsonString = JsonSerializer.Serialize(responseExample1, new JsonSerializerOptions
{
    WriteIndented = true // Makes the JSON output pretty-printed
});
Console.WriteLine(jsonString);
Console.WriteLine("## Example 1 - End - Press any key to continue.\n");
Console.ReadLine();

var responseExample2 = await Example1TestJsonStructuredOutputMode(prompt, kernel);
Console.WriteLine("## Example 2 using json schema, structured output mode...\n");
string jsonString2 = JsonSerializer.Serialize(responseExample2, new JsonSerializerOptions
{
    WriteIndented = true // Makes the JSON output pretty-printed
});
Console.WriteLine(jsonString2);
Console.WriteLine("## Example 2 - End - Press any key to continue.\n");
Console.ReadLine();


static async Task<JSONResponse> Example1TestJSONObjectMode(string prompt, Kernel kernel)
{
    JsonSerializerOptions s_options = new() { WriteIndented = true };
    var executionSettings = new OpenAIPromptExecutionSettings
    {
        ResponseFormat = "json_object"
    };

    var jsonObjectPrompt = ExamplePrompts.GetJSONObjectPrompt(prompt);

    var result = await kernel.InvokePromptAsync(jsonObjectPrompt, new(executionSettings));

    var resultString = result.GetValue<string>();

    JSONResponse? jsonResponseObj = JsonSerializer.Deserialize<JSONResponse>(resultString ?? string.Empty, s_options);

    return jsonResponseObj ?? new JSONResponse();

}

static async Task<JSONResponse> Example1TestJsonStructuredOutputMode(string prompt, Kernel kernel)
{
    JsonSerializerOptions s_options = new() { WriteIndented = true };
    // Initialize ChatResponseFormat object with JSON schema of desired response format.
    ChatResponseFormat chatResponseFormat = ChatResponseFormat.CreateJsonSchemaFormat(
        jsonSchemaFormatName: "test_intent_result",
        jsonSchema: BinaryData.FromString("""
            {
              "$schema": "https://json-schema.org/draft/2020-12/schema",
              "type": "object",
              "properties": {
                "intent": {
                  "type": ["string", "null"],
                  "description": "Retrieval or other intent"
                },
                "subcategory": {
                  "type": ["string", "null"],
                  "description": "SERIES or other subcategories"
                },
                "semanticsearch": {
                  "type": ["string", "null"],
                  "description": "Value to be searched"
                },
                "why": {
                  "type": ["string", "null"],
                  "description": "Explanation for labeling as retrieval or unrelated"
                }
              },
              "required": ["intent", "subcategory", "semanticsearch", "why"],
              "additionalProperties": false
            }
        """),
        jsonSchemaIsStrict: true);
    // Specify response format by setting ChatResponseFormat object in prompt execution settings.
    var executionSettings = new OpenAIPromptExecutionSettings
    {
        ResponseFormat = chatResponseFormat
    };


    var jsonStructuredOutputPrompt = ExamplePrompts.GetJSONStructuredOutputPrompt(prompt);
    // Send a request and pass prompt execution settings with desired response format.
    var result = await kernel.InvokePromptAsync(jsonStructuredOutputPrompt, new(executionSettings));

    var resultString = result.GetValue<string>();

    JSONResponse? jsonResponseObj = JsonSerializer.Deserialize<JSONResponse>(resultString ?? string.Empty, s_options);

    return jsonResponseObj ?? new JSONResponse();

}