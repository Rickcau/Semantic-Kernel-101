# Homework - Lesson 1
Your homework for Lesson 1 is a follows:

- [Read the overview on Semantic Kernel and understand why a framework is needed](https://learn.microsoft.com/en-us/semantic-kernel/overview/) 
- In the latest version of Semantic Kernel is there such a thing as skills? Do some research to see if you can figuer out the answer before looking at the answer.
   <details>
    <summary><u>Answer</u> (<i>click to expand</i>)</summary>
    <!-- have to be followed by an empty line! -->

       No.  Skills have been replaced with Plugins.
  </details>
- What are the fundimental steps needed for a Chat implementation using Semantic Kernel?
  <details>
    <summary><u>Answer</u> (<i>click to expand</i>)</summary>
    <!-- have to be followed by an empty line! -->

      1. Create an Azure Open AI Service in Azure, you will need to store the Model Name, Endpoint and API Key for later use
      2. Create a Kernel Builder so you can construct Kernel instances
      3. Add the OpenAIChatCompletion service using the details from step 1
      4. Create an install of the Kernel
      5. Create a ChatHistory instance to store the Chat History
      6. Create an instance of the ChatCompletionService
      7. Read the user input
      8. Call the Chat Completion Service with the prompt history / user input
      9. Display the result of the chat completion
      10. Repeat
  </details>
- Build your first Semantic Kernel Console App.
  <details>
    <summary><u>Tips</u> (<i>click to expand</i>)</summary>
    <!-- have to be followed by an empty line! -->

      1. Install the >=1.4.0 version of the Semantic Kernel .NET Package
      2. I like to use the System.Configuration package to read the Model Name, Endpoint and API Key from an App.Config file
      3. Using the details outlined in the fundimental steps try and implement the necesscary code to create a simple chat bot.

      Hint: Kernel.CreateBuilder, builder.Services.AddAzureOpenAIChatCompletion, builder.Build, kernel.GetRequiredService<IChatCompletionService>(), chatCompletionService.GetChatMessageContentAsync, history.AddAssistantMessage      
  </details>

<details>
    <summary><u>Answer</u> (<i>click to expand</i>)</summary>
    <!-- have to be followed by an empty line! -->

      1. Create an Azure Open AI Service in Azure, you will need to store the Model Name, Endpoint and API Key for later use
      2. Create a Kernel Builder so you can construct Kernel instances
      3. Add the OpenAIChatCompletion service using the details from step 1
      4. Create an install of the Kernel
      5. Create a ChatHistory instance to store the Chat History
      6. Create an instance of the ChatCompletionService
      7. Read the user input
      8. Call the Chat Completion Service with the prompt history / user input
      9. Display the result of the chat completion
      10. Repeat
  </details>

[🔼 Home ](/README.md) | [Next Homework 2 ▶](/homework/lesson-2/README.md)
