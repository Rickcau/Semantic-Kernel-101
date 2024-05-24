# Lesson 2 - Prompts and Plugins
## Chat Completion Message Structure – Why is it so important?
Before we get into the prompts it’s important to understand the Chat Completion API structures, specifically the messages array.  1st – I’d recommend that you take a close look at the [Azure OpenAI REST API reference document](https://learn.microsoft.com/en-us/azure/ai-services/openai/reference).  2nd – I would recommend that you play with the Chat Completion endpoints using Postman.  You can find a Postman Chat Completion Collection in my [AI Fundamentals repo](https://github.com/Rickcau/AI-Fundamentals).
![ChatCompletions](/assets/images/ChatCompletion.png)

## Plugins
Basically, everything is a Plugin!  Plugins are the building blocks of your AI solution.  It allows you to define the tasks the Kernel should complete and allows you to augment the capabilities of the LLM and your solution.  Plugins give you the ability to add native code.  You can use built-in plugins or build your own, you can pass arguments and the functions of your plugins can be auto invoked.
![Plugins](/assets/images/Plugins.png)




[🔼 Home ](/README.md) | [Next Lesson 3 ▶](/lessons/lesson-3/README.md)
