# Homework - Lesson 1
Your homework for Lesson 1 is a follows:

- [Read the overview on Semantic Kernel and understand why a framework is needed](https://learn.microsoft.com/en-us/semantic-kernel/overview/) 

- In the latest version of Semantic Kernel is there such a thing as skills? Do some research to see if you can figuer out the answer before looking at the answer.
   <details>
    <summary><u>Answer</u> (<i>click to expand</i>)</summary>
    <!-- have to be followed by an empty line! -->

       No.  [Skills have been replaced with Plugins.](https://devblogs.microsoft.com/semantic-kernel/road-to-v1-0-for-the-python-semantic-kernel-sdk/)
  </details>

- What are the fundimental steps needed for a Chat implementation using Semantic Kernel?
  <details>
    <summary><u>Answer</u> (<i>click to expand</i>)</summary>
    <!-- have to be followed by an empty line! -->
      
     1. Create a Kernel Builder so you can construct Kernel instances
   
     2. Load the AI Endpoint values so you can access the REST endpoint
   
     3. Add the Chat Completion Service with the Endpoint details
   
     4. Construct the Kernel, Prompt / Chat History, get an instance to the Completion Service
   
     5. Send the Prompt / Chat History and get a response
  </details>

- Build your first Semantic Kernel Console App.
  <details>
    <summary><u>Tips</u> (<i>click to expand</i>)</summary>
    <!-- have to be followed by an empty line! -->
      1. Follow the steps outlined in Lesson 1.
      
     If you need a shortcut you can take look at the Lesson 1 Project found in the [Semantic-Kernel-101.sln file](/solutions/Semantic-Kernel-101/README.md).
      
     **Hint:** Kernel.CreateBuilder, builder.Services.AddAzureOpenAIChatCompletion, builder.Build, kernel.GetRequiredService<IChatCompletionService>(), chatCompletionService.GetChatMessageContentAsync, history.AddAssistantMessage      
  </details>
 
- RAG (Retrieval Augmented Generation) and Generative AI
   The RAG pattern is one of the most important patterns in use today.
     Retrieval  = Retrieve inforamtion from a data source
     Augment    = Inject the retrieved data into the prompt
     Generation = Allow the LLM to generation responses based on the retrieved data
  <details>
    <summary><u>Read and understand RAG with AI Search</u> (<i>click to expand</i>)</summary>
    <!-- have to be followed by an empty line! -->

      
     [RAG with AI Search](https://learn.microsoft.com/en-us/azure/search/retrieval-augmented-generation-overview)
          
  </details>

[🔼 Home ](/README.md) | [Next Homework 2 ▶](/homework/lesson-2/README.md)
