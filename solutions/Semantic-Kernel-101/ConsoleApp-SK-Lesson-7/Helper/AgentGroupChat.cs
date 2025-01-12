using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.Agents.Chat;
using Microsoft.SemanticKernel.ChatCompletion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using ConsoleApp_SK_Lesson_7.Helper;
using Microsoft.Extensions.DependencyInjection;
using AzureOpenAISearchConfiguration;
using Microsoft.SemanticKernel.Agents.OpenAI;
using OpenAI.Assistants;
using OpenAI.Chat;
using ChatMessageContent = Microsoft.SemanticKernel.ChatMessageContent;

namespace ConsoleApp_SK_Lesson_7.Helper
{
   
    public class AgentGroupChatHelper
    {
        private readonly Kernel _kernel;
        private readonly Configuration _configuration;
        private const string ReviewerName = "ArtDirector";
        private const string ReviewerInstructions =
            """
        You are an art director who has opinions about copywriting born of a love for David Ogilvy.
        The goal is to determine if the given copy is acceptable to print.
        If so, state that it is approved.
        If not, provide insight on how to refine suggested copy without example.
        """;

        private const string CopyWriterName = "CopyWriter";
        private const string CopyWriterInstructions =
            """
        You are a copywriter with ten years of experience and are known for brevity and a dry humor.
        The goal is to refine and decide on the single best copy as an expert in the field.
        Only provide a single proposal per response.
        You're laser focused on the goal at hand.
        Don't waste time with chit chat.
        Consider suggestions when refining an idea.
        """;

        public AgentGroupChatHelper(Configuration configuration)
        {
            _configuration = configuration;
            _kernel = CreateKernelWithChatCompletion();
        }

        Kernel CreateKernelWithChatCompletion()
        {
            // Very important to enable logging with the loggerFactory otherwise the telemetry will not be logged
            // AppInsights the line of code that most folks miss is: builder.Services.AddSingleton(loggerFactory); which requires the use of dependency injection
            var builder = Kernel.CreateBuilder();
            builder.AddAzureOpenAIChatCompletion(
                _configuration.AzureOpenAIDeployment!,
                _configuration.AzureOpenAIEndpoint!,
                _configuration.AzureOpenAIApiKey!,
                serviceId: "azure-openai"
            );
            return builder.Build();
            //        }
        }

        public async Task UseAgentGroupChatWithTwoAgentsAsync()
        {
            // Define the agents
            ChatCompletionAgent agentReviewer =
                new()
                {
                    Instructions = ReviewerInstructions,
                    Name = ReviewerName,
                    Kernel = this.CreateKernelWithChatCompletion(),
                };

            ChatCompletionAgent agentWriter =
                new()
                {
                    Instructions = CopyWriterInstructions,
                    Name = CopyWriterName,
                    Kernel = this.CreateKernelWithChatCompletion(),
                };

            // Create a chat for agent interaction.
            AgentGroupChat chat =
                new(agentWriter, agentReviewer)
                {
                    ExecutionSettings =
                        new()
                        {
                            // Here a TerminationStrategy subclass is used that will terminate when
                            // an assistant message contains the term "approve".
                            TerminationStrategy =
                                new ApprovalTerminationStrategyAgentGroupChat()
                                {
                                    // Only the art-director may approve.
                                    Agents = [agentReviewer],
                                    // Limit total number of turns
                                    MaximumIterations = 10,
                                }
                        }
                };

            // Invoke chat and display messages.
            ChatMessageContent input = new(AuthorRole.User, "concept: maps made out of egg cartons.");
            chat.AddChatMessage(input);
            this.WriteAgentChatMessage(input);

            await foreach (ChatMessageContent response in chat.InvokeAsync())
            {
                this.WriteAgentChatMessage(response);
            }

            Console.WriteLine($"\n[IS COMPLETED: {chat.IsComplete}]");
        }

        public void WriteAgentChatMessage(ChatMessageContent message)
        {
            // Include ChatMessageContent.AuthorName in output, if present.
            string authorExpression = message.Role == AuthorRole.User ? string.Empty : $" - {message.AuthorName ?? "*"}";
            // Include TextContent (via ChatMessageContent.Content), if present.
            string contentExpression = string.IsNullOrWhiteSpace(message.Content) ? string.Empty : message.Content;
            bool isCode = message.Metadata?.ContainsKey(OpenAIAssistantAgent.CodeInterpreterMetadataKey) ?? false;
            string codeMarker = isCode ? "\n  [CODE]\n" : " ";
            Console.WriteLine($"\n# {message.Role}{authorExpression}:{codeMarker}{contentExpression}");

            // Provide visibility for inner content (that isn't TextContent).
            foreach (KernelContent item in message.Items)
            {
                if (item is AnnotationContent annotation)
                {
                    Console.WriteLine($"  [{item.GetType().Name}] {annotation.Quote}: File #{annotation.FileId}");
                }
                else if (item is FileReferenceContent fileReference)
                {
                    Console.WriteLine($"  [{item.GetType().Name}] File #{fileReference.FileId}");
                }
                else if (item is ImageContent image)
                {
                    Console.WriteLine($"  [{item.GetType().Name}] {image.Uri?.ToString() ?? image.DataUri ?? $"{image.Data?.Length} bytes"}");
                }
                else if (item is FunctionCallContent functionCall)
                {
                    Console.WriteLine($"  [{item.GetType().Name}] {functionCall.Id}");
                }
                else if (item is FunctionResultContent functionResult)
                {
                    Console.WriteLine(value: $"  [{item.GetType().Name}] {functionResult.CallId} - {SerializeResult(functionResult.Result) ?? "*"}");
                }
            }

            if (message.Metadata?.TryGetValue("Usage", out object? usage) ?? false)
            {
                if (usage is RunStepTokenUsage assistantUsage)
                {
                    WriteUsage(assistantUsage.TotalTokenCount, assistantUsage.InputTokenCount, assistantUsage.OutputTokenCount);
                }
                else if (usage is ChatTokenUsage chatUsage)
                {
                    WriteUsage(chatUsage.TotalTokenCount, chatUsage.InputTokenCount, chatUsage.OutputTokenCount);
                }
            }

            void WriteUsage(int totalTokens, int inputTokens, int outputTokens)
            {
                Console.WriteLine($"  [Usage] Tokens: {totalTokens}, Input: {inputTokens}, Output: {outputTokens}");
            }
        }

        private static string? SerializeResult(object? result)
        {
            try
            {
                // Serialize to JSON if result is not null
                return result != null ? JsonSerializer.Serialize(result) : "*";
            }
            catch (Exception ex)
            {
                // Handle serialization errors gracefully
                return $"Serialization Error: {ex.Message}";
            }
        }
    }

}

