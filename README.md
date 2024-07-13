# Semantic-Kernel-101
## Learn Semantic Kernel using basics to advance concepts
![SK-101-Logo](./assets/images/MSFT_SK-101_Banner.png)

## Goal
- To provide users with simple learning plans that allow you to quickly learn Semantic Kernel starting with the basic to advanced concepts 
- Use a lesson based approach for understanding of concepts that allow you to progressively learn advanced concepts 
- Allow much faster adoption of AI using by leveraging proven practices
- Provide users with reusable building blocks resulting in ability to deliver solutions faster

## Important Note
I am not using DevContainers because I have an issue with Docker that will requirement to reformat my laptop, and I simply have not had time to do that at the moment.

## Lessons
### [Lesson 1 - Why Semantic Kernel? - Building our first App?](./lessons/lesson-1/README.md)
In this lesson we cover the fundimental concepts of Semantic Kernel and what is needed to build your first console app.

### [Lesson 2 - Plugins & Prompts](./lessons/lesson-2/README.md)
In this lesson I reiterate the importance of understanding the ChatCompletion Message structure then we dive right into SK Plugins and how they are the body of your AI solution.  We will drill deeper into SK Prompts and the various ways you can use prompts and how everything is a Plugin and why this is important. 

We are building a foundation of Semantic Kernel Concepts that will set you up for success with your AI engagements.  

### [Lesson 3 - Chat with your Data - RAG](./lessons/lesson-3/README.md)
In this lesson we will look at two options for how to implement a Retrieval Augmentation Generation solution that implement allows us to chat with our documents.  We leverage Azure AI Search for the Vector and Semantic Search capabilities.   You can of course use other systems like CosmosDB for MongoDB vCore.  We will look at using the DataSources extension with the Azure Open AI Completion Endpoint as well as leveraging a custom Semantic Kernel Plugin.  

The most simplistic approach is to use the DataSources Array as you don’t have to write any code for this, you just need to make sure the properties are correctly configured.  Regardless of your approach I would most often still recommend also using an intent recognition pattern to control flow.

## Homework
[Lesson 1 - Homework](/homework/lesson-1/README.md)

[Lesson 2 - Homework](/homework/lesson-2/README.MD)

[Lesson 3 - Homework](/homework/lesson-3/README.md)

## [Solutions](/solutions/README.MD)
Each lesson will have a separate project in the Semantic-Kernel-101.sln.  If you need to see a working example for the lesson you can find them here.  The idea is that you do the homework, put hands on the keyboard and you create the project that that covers the topics outlined in each lesson.  If you need a little help you can use this solution to get you moving.
