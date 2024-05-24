# Solutions
In this section you will find solutions for each Lesson.  There is a master Visual Studio Solution which has a project for each lesson.  If you get stuck and cannot write the necesscary code to complete the lesson you can take a look at the project for the lesson.  You will find the master solution by [clicking here](/solutions/Semantic-Kernel-101/README.md).

# Lesson 1 - Solution
The idea is that you are to use the knowledge shared with you in lesson 1 to build your first basic Semantic Kernel Console App.  Your program.cs file should look something like this:

   ~~~
        using Microsoft.SemanticKernel.ChatCompletion;
        using Microsoft.SemanticKernel.Connectors.OpenAI;
        using Microsoft.SemanticKernel;
        using System.Configuration;
        
        
        Console.WriteLine("Our first Console App - Semantic Kernel Chat Bot");
        
        var builder = Kernel.CreateBuilder();
        
        var openAiDeployment = ConfigurationManager.AppSettings.Get("AzureOpenAIModel");
        var openAiUri = ConfigurationManager.AppSettings.Get("AzureOpenAIEndpoint");
        var openAiApiKey = ConfigurationManager.AppSettings.Get("AzureOpenAIKey");
        
        if (openAiDeployment != null && openAiUri != null && openAiApiKey != null)
        {
            builder.Services.AddAzureOpenAIChatCompletion(
            deploymentName: openAiDeployment,
            endpoint: openAiUri,
            apiKey: openAiApiKey);
        
        }
        
        var kernel = builder.Build();
        
        ChatHistory history = [];
        
        var chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();
        
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
   ~~~

You will find all the Lessons included as projects in the Semantic-Kernel-101.sln.  It's recommended that you clone this repo locally as this will give you access to the master solution and projects if you need them.

# Lesson 2 - Solution
In this solution we dive into Plugins and Prompts.  The goal here is to demostrate the various ways to use prompts and plugins and the power of Auto Invoke.  We start out with prompts then work our way into plugins.  Basically, everything is a plugin. I would encourage you to try to build this from scratch using the examples.  If you want make a fast start, you can simple use the [Lesson-2 solution](/solutions/Semantic-Kernel-101/ConsoleApp-SK-Lesson-2). 

[🔼 Home ](/README.md)
