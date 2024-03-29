# Solutions
In this section you will find solutions for each Lesson.

# Lesson 1 Solution
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
