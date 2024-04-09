using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.SemanticKernel;
using System.Configuration;


Console.WriteLine("Our first Console App - Semantic Kernel Chat Bot\n");

// 5 Simple steps to use the Kernel!

#region Create Kernel Builder
// Create a Builder for Creating Kernel Objects
var builder = Kernel.CreateBuilder();
#endregion

#region Load AI Endpoint Values

var openAiDeployment = ConfigurationManager.AppSettings.Get("AzureOpenAIModel");
var openAiUri = ConfigurationManager.AppSettings.Get("AzureOpenAIEndpoint");
var openAiApiKey = ConfigurationManager.AppSettings.Get("AzureOpenAIKey");

#endregion

#region Add ChatCompletion Service

builder.Services.AddAzureOpenAIChatCompletion(
   deploymentName: openAiDeployment!,
   endpoint: openAiUri!,
   apiKey: openAiApiKey!);

#endregion

#region Construct Kernel, ChatHistory Get instance of ChatCompletion Service
// Construct instance of Kernel using Builder Settings
var kernel = builder.Build();

ChatHistory history = [];

var chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();

#endregion

#region Send Prompt Get Response - Exercise One

var prompt = "Why is the Sky blue?";
var result = await chatCompletionService.GetChatMessageContentAsync(prompt);
Console.WriteLine(result);
Console.WriteLine("\nPress enter to end.");
Console.ReadLine();

#endregion

#region Chat Loop - Exercise Two

//while (true)
//{
//    Console.Write(">> ");
//    var userMessage = Console.ReadLine();
//    if (userMessage != "Exit")
//    {
//        history.AddUserMessage(userMessage!);

//        // Not really being used in this example but we will use it in future examples
//        OpenAIPromptExecutionSettings openAIPromptExecutionSettings = new()
//        {
//            ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions
//        };

//        try
//        {
//            var result = await chatCompletionService.GetChatMessageContentAsync(
//                history,
//                executionSettings: openAIPromptExecutionSettings,
//                kernel: kernel);

//            Console.WriteLine("<< " + result);

//            if (result.Content != null)
//            {
//                history.AddAssistantMessage(result.Content);
//            }
//        }
//        catch (Exception ex)
//        {
//            Console.WriteLine($"Error: {ex.Message}");
//        }
//    }
//    else break;
//}

#endregion


