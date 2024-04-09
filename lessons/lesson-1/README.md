# Lesson 1 - Why Semantic Kernel? - Building our first App?

## What will you learn in this lession?
- Why is an SDK like Semantic Kernel Needed?
- What is required to build a basic Chat Bot with Semantic Kernel?
  
## Why is this important?
It's important to understand why a framework is needed for AI based solutions and what are the fundemental building blocks needed for a basic Semantic Kernel Chat Bot.  Once you understand this basics, you are start to leverage the more complex features.

## Why Semantic Kernel? 
![WhySK](/assets/images/WhySK.png)

## What is needed to build our first App?

### 5 Simple steps are needed
1. Create a Kernel Builder

   ~~~
         var builder = Kernel.CreateBuilder();
   ~~~

2. Load you AI Endpoint Details

   ~~~
        var openAiDeployment = ConfigurationManager.AppSettings.Get("AzureOpenAIModel");
        var openAiUri = ConfigurationManager.AppSettings.Get("AzureOpenAIEndpoint");
        var openAiApiKey = ConfigurationManager.AppSettings.Get("AzureOpenAIKey");
   ~~~

3. Add the Chat Completion Service

   ~~~
       builder.Services.AddAzureOpenAIChatCompletion(
            deploymentName: openAiDeployment!,
            endpoint: openAiUri!,
            apiKey: openAiApiKey!);
   ~~~

4. Construct the Kernel, ChatHistory, get instance of the ChatCompletion Service

   ~~~
        var kernel = builder.Build();
        ChatHistory history = [];
        var chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();
   ~~~

5. Send a prompt or ChatHistory and get a response from LLM

   ~~~
       var prompt = "Why is the Sky blue?";
       var result = await chatCompletionService.GetChatMessageContentAsync(prompt);
       Console.WriteLine(result);
   ~~~

## Let's build your first .NET Core SK Console App

1. Create a new .NET Core Console App in Visual Studio and name it SK-Lesson-1
2. Add the Semantic Kernel Package to the project.
   - Click on **Dependecies->Manage NuGet Packages** and inssue the package below 

    <details>
    <summary><u>Packages</u> (<i>click to expand</i>)</summary>
    <!-- have to be followed by an empty line! -->

        Microsoft.SemanticKernel 1.6.3 or better
    </details>

3. Come up with some way to store and read the Model Name, AI Key and AI Endpoint.  Here is an example of using the System.Configuration class and an App.Config file.
   - Create an App.Config file in the root of the project using the following format and replace the values to point to your AI Endpoint

   ~~~
      <?xml version="1.0" encoding="utf-8" ?>
      <configuration>
	      <appSettings>
		      <add key="AzureOpenAIEndpoint" value="AzureOpenAI-Endpoint-URI" />
	          <add key="AzureOpenAIKey" value="AzureOpenAI KEY" />  
	          <add key="AzureOpenAIModel" value="AzureOpenAI Model Name" />
	      </appSettings>
      </configuration>
   ~~~

   - Add a reference to the System.Configuration assembly to the program.cs file

4. Using the steps outlined in the **5 Simple steps** build and test your first app.
   - If needed you can take a look at the soltion to see a fishined project, but ideally you should put hands on keyboard and work through any issues you come across.

## [Finish your homework before the next lesson](/homework/lesson-1/README.md)
[🔼 Home ](/README.md) | [Next Lesson 2 ▶](/lessons/lesson-2/README.md)
