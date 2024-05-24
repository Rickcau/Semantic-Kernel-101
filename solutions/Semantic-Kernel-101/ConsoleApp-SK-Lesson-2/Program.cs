using Azure.AI.OpenAI;
using ConsoleApp_SK_Lesson_2.Helpers;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using System.Configuration;


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

var runExercise = 5;

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

    var vacationRecommendation = kernel.CreateFunctionFromPrompt(prompt);
    // vacationRecommendation.
    var myPlugin = kernel.CreatePluginFromFunctions("VacationRecommendation", (IEnumerable<KernelFunction>?)vacationRecommendation);
    KernelFunctionFactory.CreateFromPrompt

    var vacationContent = kernel.InvokeAsync(myPlugin["VactionRecommendation"], new() { ["input"] = input });

    Console.WriteLine(vacationContent);
    Console.WriteLine("\nPress enter to end.");
    Console.ReadLine();
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