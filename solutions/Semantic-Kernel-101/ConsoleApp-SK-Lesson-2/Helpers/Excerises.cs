using Azure.AI.OpenAI;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.SemanticKernel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#pragma warning disable SKEXP0010
namespace ConsoleApp_SK_Lesson_2.Helpers
{
    internal class Excerises
    {

        public async Task<string> ExerciseOne(string query, Kernel kernel)
        {
            var _promptPiglatin = InlinePromptHelper._promptPiglatin;
            // Exercise 1 - Inline SK Prompt Pig Latin and Write a Story
            var executionSettings = new OpenAIPromptExecutionSettings()
            {
               // ResponseFormat = "json_object", // setting JSON output mode
            };

            KernelArguments arguments = new(executionSettings) { { "query", query } };
            // KernelArguments arguments = new KernelArguments() { { "query", query } });

            string result = "Sorry something went wrong";
            try
            {
                var response = await kernel.InvokePromptAsync(_promptPiglatin, arguments);
                var metadata = response.Metadata;
                if (metadata != null && metadata.ContainsKey("Usage"))
                {
                    var usage = (CompletionsUsage?)metadata["Usage"];
                    Console.WriteLine("");
                    Console.WriteLine($"Token usage. Input tokens: {usage?.PromptTokens}; Output tokens: {usage?.CompletionTokens}");
                    Console.WriteLine("");
                }
                result = response.GetValue<string>() ?? "";
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return result ?? "";
        }


        public async Task<string> ExerciseTwo(string query, Kernel kernel)
        {
            var _promptWriteStory = InlinePromptHelper._promptWriteStory;
            // Exercise 1 - Inline SK Prompt Pig Latin and Write a Story
            var executionSettings = new OpenAIPromptExecutionSettings()
            {
               // ResponseFormat = "json_object", // setting JSON output mode
            };

            KernelArguments arguments = new(executionSettings) { { "query", query } };
            // KernelArguments arguments = new KernelArguments() { { "query", query } });

            // TBD:  Want to load SK Prompts into the plugin collection:   kernel.CreateFunctionFromPrompt(_promptWriteStory, arguments); 


            string result = "Sorry something went wrong";
            try
            {  
                var response = await kernel.InvokePromptAsync(_promptWriteStory, arguments);
                var metadata = response.Metadata;
                if (metadata != null && metadata.ContainsKey("Usage"))
                {
                    var usage = (CompletionsUsage?)metadata["Usage"];
                    Console.WriteLine("");
                    Console.WriteLine($"Token usage. Input tokens: {usage?.PromptTokens}; Output tokens: {usage?.CompletionTokens}");
                    Console.WriteLine("");
                }
                result = response.GetValue<string>() ?? "";
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return result ?? "";
        }

        public async Task<string> ExerciseThree(string query, Kernel kernel)
        {
            var _summarizeFunction = kernel.ImportPluginFromPromptDirectory(Path.Combine("./Plugins/Prompts", "SummarizePlugin"));

            KernelArguments arguments = new KernelArguments() { { "input", query } };

            string result = "Sorry something went wrong";
            try
            {
                var response = await kernel.InvokeAsync(_summarizeFunction["Summarize"], arguments);
                var metadata = response.Metadata;
                if (metadata != null && metadata.ContainsKey("Usage"))
                {
                    var usage = (CompletionsUsage?)metadata["Usage"];
                    Console.WriteLine("");
                    Console.WriteLine($"Token usage. Input tokens: {usage?.PromptTokens}; Output tokens: {usage?.CompletionTokens}");
                    Console.WriteLine("");
                }
                result = response.GetValue<string>() ?? "";
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return result ?? "";
        }


    }
}
