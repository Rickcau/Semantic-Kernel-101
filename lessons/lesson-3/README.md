# Lesson 3 - Retrevial Augmentation Generation with Semantic Kernel
## Data Sources (Array) Structure – Why is it so important?
When you use the Azure Open AI Playground to Chat with your Documents, it makes use the **Data Sources** extension to allow the LLM to **Retrieve** data that may be related to the question then it will **Augment** the prompt and **Generate** a response, hence the term **RAG**.  When using the **Data Sources** extension you do not have to write any code to perform the search, you simply let the endpoint do this, it’s a low maintenance approach to **RAG**.  

Now, if you have a need to implement a more robust RAG solution, you can use a customer Semantic Kernel Plugin Approach.  Of course using a custom plugin is additional code, but we will take a look at both and it’s not that complicated.

### Let's take a look at what the raw request body would look like when using the Data Sources Extension
Below you will an example that I captured from a GET request that has a properly formatted **data_sources** array.

![DataSourcesArray](/assets/images/SK-WithDataSourceRAG.png)

Before we get into the prompts it’s important to understand the Chat Completion API structures, specifically the messages array.  1st – I’d recommend that you take a close look at the [Azure OpenAI REST API reference document](https://learn.microsoft.com/en-us/azure/ai-services/openai/reference).  2nd – I would recommend that you play with the Chat Completion endpoints using Postman.  You can find a Postman Chat Completion Collection in my [AI Fundamentals repo](https://github.com/Rickcau/AI-Fundamentals).
![ChatCompletions](/assets/images/ChatCompletion.png)

## Supported Data Sources
Below is a link to the official supported data sources.
![DataSourcesArray](/assets/images/AzureOpenAI-DataSources.jpg)

[Click here to see a list of the suppored data sources](https://learn.microsoft.com/en-us/azure/ai-services/openai/references/on-your-data?tabs=rest#data-source)




Basically, everything is a Plugin!  Plugins are the building blocks of your AI solution.  It allows you to define the tasks the Kernel should complete and allows you to augment the capabilities of the LLM and your solution.  Plugins give you the ability to add native code.  You can use built-in plugins or build your own, you can pass arguments and the functions of your plugins can be auto invoked.
![Plugins](/assets/images/Plugins.png)

[🔼 Home ](/README.md) | [Back Lesson 2 ▶](/lessons/lesson-2/README.md)
