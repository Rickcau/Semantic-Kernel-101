using Azure.AI.OpenAI;
using ConsoleApp_SK_Lesson_2.Helpers;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using System.Configuration;
using SKTraining.Plugins;
using Console_SK_Planner.Plugins;


Console.WriteLine("Our first Console App - Semantic Kernel Chat Bot\n");
var runExercise = 5;
// 5 Simple steps to use the Kernel!

#region Step 1: Create Kernel Builder
// Create a Builder for Creating Kernel Objects
var builder = Kernel.CreateBuilder();
#endregion

#region Step 2: Load AI Endpoint Values

var openAiDeployment = ConfigurationManager.AppSettings.Get("AzureOpenAIModel");
var openAiUri = ConfigurationManager.AppSettings.Get("AzureOpenAIEndpoint");
var openAiApiKey = ConfigurationManager.AppSettings.Get("AzureOpenAIKey");

#endregion

#region Step 3: Add ChatCompletion Service

builder.Services.AddAzureOpenAIChatCompletion(
   deploymentName: openAiDeployment!,
   endpoint: openAiUri!,
   apiKey: openAiApiKey!);

#endregion

#region Step 4: Construct Kernel, ChatHistory Get instance of ChatCompletion Service
// Construct instance of Kernel using Builder Settings
if (runExercise == 5)
{ // lets add the plugins

    builder.Plugins.AddFromType<UniswapV3SubgraphPlugin>();
    builder.Plugins.AddFromType<Example2EchoPlugin>();  
    builder.Plugins.AddFromType<LightOnPlugin>();
    builder.Plugins.AddFromType<WeatherPlugin>();

}

var kernel = builder.Build();

ChatHistory history = [];

var chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();

#endregion



#region Exercise 1 - Inline SK Prompt
// Using Inline SK Prompts
if (runExercise == 1)
{
    Excerises excerises = new Excerises();
    Console.WriteLine("Exercise 1 PigLatin - Inline Prompt");
    Console.Write(">> ");
    var userMessage = Console.ReadLine();
    var result = await excerises.ExerciseOne(userMessage ?? "", kernel);
    Console.WriteLine(result);
    Console.WriteLine("\nPress enter to end.");
    Console.ReadLine();
}
#endregion

#region Exercise 2 - Inline SK Prompt 
// Using Inline SK Prompts
if (runExercise == 2)
{
    Excerises excerises = new Excerises();
    Console.WriteLine("Exercise 2 Write a Story - Inline Prompt");
    Console.WriteLine("Enter only 5 words.");
    Console.Write(">> ");
    var userMessage = Console.ReadLine();
    var result = await excerises.ExerciseTwo(userMessage ?? "", kernel);
    Console.WriteLine(result);
    Console.WriteLine("\nPress enter to end.");
    Console.ReadLine();
}

#endregion

#region Exercise 3 Loading Prompts from File
// Loading Prompts from disk / files
if (runExercise == 3)
{

    Excerises excerises = new Excerises();
    Console.WriteLine("Exercise 3 Summarize Input");
    Console.Write(">> ");
    var userMessage = Console.ReadLine();
    var result = await excerises.ExerciseThree(userMessage ?? "", kernel);
    Console.WriteLine(result);
    Console.WriteLine("\nPress enter to end.");
    Console.ReadLine();
}
#endregion

#region Example Text Input
/*
 The rain poured down in relentless sheets, creating a symphony of water against the metal roof of the old garage. Inside, Sam worked tirelessly on his project car, a vintage Mustang that had seen better days. 
*/
#endregion


#region Exercise 4 A Different way of using prompts Interpolated Strings
// Loading Prompts from disk / files
if (runExercise == 4)
{
    var input = "We like mountain biking, fly fishing and mountains. " +
                "Our travel budget is $12000";
    var prompt = @$"
    The following is a conversation with an AI travel 
    assistant. The assistant is helpful, creative, and 
    very friendly.

    <message role=""user"">Can you give me some travel 
    destination suggestions?</message>

    <message role=""assistant"">Of course! Do you have a 
    budget or any specific activities in mind?</message>

    <message role=""user"">${input}</message>";

    /*
    
    I'm planning an anniversary trip with my wife. We like mountain biking, fly fishing, mountains, and beaches. Our 
    travel budget is $12000
    
    */
    Console.WriteLine("Provide a description of what you like to do.");
    Console.Write(">> ");
    input = Console.ReadLine();
    var result = await kernel.InvokePromptAsync(prompt);
    Console.WriteLine(result);
    Console.WriteLine("\nPress enter to end.");
    Console.ReadLine();
}
#endregion

#region Example Text Input

#region Exercise 5  Lets get into the plugins
if (runExercise == 5)
{

    while (true)
    {
        Console.Write(">> ");
        var userMessage = Console.ReadLine();
        if (userMessage != "Exit")
        {
            // history.AddUserMessage(Console.ReadLine()!);
            history.AddUserMessage(userMessage!);

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
}


// Examples of Calling methods
/*
 var companySearchPluginObj = new CompanySearchPlugin();

var companySearchPlugin = kernel.ImportPluginFromObject(companySearchPluginObj, "CompanySearchPlugin");

var weatherContent = await kernel.InvokeAsync( companySearchPlugin["WeatherSearch"],new(){["text"] = "guangzhou"});

weatherContent.GetValue<string>()
 */

#endregion





#endregion