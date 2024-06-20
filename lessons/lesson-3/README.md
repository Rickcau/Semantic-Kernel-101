# Lesson 3 - Retrevial Augmentation Generation with Semantic Kernel
## Data Sources (Array) Structure – Why is it so important?
When you use the Azure Open AI Playground to Chat with your Documents, it makes use the **Data Sources** extension to allow the LLM to **Retrieve** data that may be related to the question then it will **Augment** the prompt and **Generate** a response, hence the term **RAG**.  When using the **Data Sources** extension you do not have to write any code to perform the search, you simply let the endpoint do this, it’s a low maintenance approach to **RAG**.  

Now, if you have a need to implement a more robust RAG solution, you can use a customer Semantic Kernel Plugin Approach.  Of course using a custom plugin is additional code, but we will take a look at both and it’s not that complicated.

### Let's take a look at what the raw request body would look like when using the Data Sources Extension
Below you will an example that I captured from a GET request that has a properly formatted **data_sources** array.

![DataSourcesArray](/assets/images/SK-WithDataSourceRAG.png)

## Supported Data Sources
Below is a link to the official supported data sources.
![DataSourcesArray](/assets/images/AzureOpenAI-DataSources.jpg)

[Click here to see a list of the suppored data sources](https://learn.microsoft.com/en-us/azure/ai-services/openai/references/on-your-data?tabs=rest#data-source)

## AzureOpenAIChatCompletionWithDataService Class - Semantic Kernel
In order to leverage the **Data Sources** extension with **Semantic Kernel** we need to use the **AzureOpenAIChatCompletionWithDataService** Class.  Here is an snippet of code that demostrates this. 
![ChatCompletionWithDataService](/assets/images/SK-ChatCompletionWithDataService.jpg)

[🔼 Home ](/README.md) | [Back Lesson 2 ▶](/lessons/lesson-2/README.md)
