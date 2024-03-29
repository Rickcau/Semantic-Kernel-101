# Lesson 1 - Why Semantic Kernel and What is needed to build our first App?

## What will you learn in this lession?
- Why is an SDK like Semantic Kernel Needed?
- What is required to build a basic Chat Bot with Semantic Kernel?
  
## Why is this important?
It's important to understand why and SDK is needed 

## Module 1 - Why Semantic Kernel? 
Today's AI models can easily generate messages and images for users. While this is helpful when building a simple chat app, it is not enough to build fully automated AI agents that can automate business processes and empower users to achieve more. To do so, you would need a framework that can take the responses from these models and use them to call existing code to actually do something productive.

Microsoft has created an SDK that allows you to easily describe your existing code to AI models so they can request that they be called. Afterwards, Semantic Kernel does the heavy lifting of translating the model's response into a call to your code.

It allows you to do cool things like call a Weather API to get the current weather, or perform some fancy features using native code, like querying a database or searching an index.

## Module 2 - What is needed to build our first App?
1. Let's start by build a .Net Core Console App in Visual Studio.

2. Next, let's add the Semantic Kernel Package to the project.

    <details>
    <summary><u>Packages</u> (<i>click to expand</i>)</summary>
    <!-- have to be followed by an empty line! -->

        Microsoft.SemanticKernel 1.6.3 or better
  </details>

4. Come up with some way to store and read the Model Name, AI Key and AI Endpoint.  I use System.Configuration and App.Config for Console Apps, just because these are for learning purposes.

5. Implement logic that does the following:

    <details>
    <summary><u>Details</u> (<i>click to expand</i>)</summary>
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
