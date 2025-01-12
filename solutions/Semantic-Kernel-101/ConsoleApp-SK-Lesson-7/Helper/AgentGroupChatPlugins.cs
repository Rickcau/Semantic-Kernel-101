using AzureOpenAISearchConfiguration;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.Agents.Chat;
using Microsoft.SemanticKernel.Agents.OpenAI;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using OpenAI.Assistants;
using OpenAI.Chat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using ChatMessageContent = Microsoft.SemanticKernel.ChatMessageContent;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using Plugins;

namespace ConsoleApp_SK_Lesson_7.Helper;
/// <summary>
/// Warning, this code is not comnplete yet, it's a work in progress.
/// Check here if you need to see Agents with Plugins examples 
/// https://github.com/microsoft/semantic-kernel/tree/main/dotnet/samples/GettingStartedWithAgents
/// </summary>
public class AgentGroupChatPluginsHelper
{
    private readonly Kernel _kernel;
    private readonly Configuration _configuration;
    private const string ShoppingAssistantName = "ShoppingAssistant";

    private const string ShoppingAssistantInstructions =
       """
                You are a Shopping Assistant Agent that helps customers find and purchase items.

                When handling a NEW customer query:
                1. Confirm receipt of the query by saying "Let me check the availability for you."
                2. Forward the query to the Inventory Manager in the format: "CHECK_AVAILABILITY: [item details]"
                3. After receiving a response:
                    - If the item is available, respond with "Yes, [item details] is available. Would you like to add it to your cart?"
                    - If the item is unavailable, suggest alternatives by saying "Unfortunately, [item details] is out of stock. However, we have [alternative item details] available."
                4. End the interaction with either "ITEM_ADDED_TO_CART" or "ALTERNATIVE_SUGGESTED."

                When confirming a purchase:
                1. Confirm by saying "Your purchase has been processed successfully."
                2. Notify the Inventory Manager with "UPDATE_STOCK: [item details and quantity]"
                3. Ask "Is there anything else I can assist you with?"
                4. End with "DONE!"

                For queries unrelated to shopping:
                1. Respond with "I am here to assist with shopping-related queries only."
                2. End with "DONE!"

                Important:
                - Never re-check availability for the same query unless requested by the customer.
                - Always be polite and ensure clarity in responses.
               """;
    private const string InventoryManagerName = "InventoryManager";
    private const string InventoryManagerInstructions =
        """
                You are an Inventory Manager Agent responsible for managing stock levels and responding to availability queries.

                When handling an AVAILABILITY CHECK:
                1. Confirm receipt of the query by saying "Checking inventory for [item details]."
                2. Check the stock status:
                   - If the item is in stock, respond with "IN_STOCK: [item details] - [quantity available]."
                   - If the item is out of stock, respond with "OUT_OF_STOCK: [item details]."
                3. End the response with "CHECK_COMPLETE."

                When handling a STOCK UPDATE:
                1. Confirm receipt by saying "Processing stock update for [item details]."
                2. Update the stock database and confirm by responding "STOCK_UPDATED: [item details] - [new quantity]."
                3. End with "UPDATE_COMPLETE."

                For unrecognized requests:
                1. Respond with "This request is not valid for an Inventory Manager."
                2. End with "INVALID_REQUEST."

                Important:
                - Always confirm receipt of any query or update request before taking action.
                - Ensure accuracy when providing stock status or updating stock levels.
                """;
    private readonly KernelFunction _selectionFunction; 

    // ApprovalTerminationStrategyAgentGroupChatPlugins

    public AgentGroupChatPluginsHelper(Configuration configuration)
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
    }

    public async Task UseAgentGroupChatWithTwoAgentsAsync()
    {
        // Define the agents
        ChatCompletionAgent agentShoppingAssistant =
            new()
            {
                Instructions = ShoppingAssistantInstructions,
                Name = ShoppingAssistantInstructions,
                Kernel = this.CreateKernelWithChatCompletion(),
                Arguments = new KernelArguments(
                new OpenAIPromptExecutionSettings()
                {
                    ServiceId = "azure-openai",
                })
            };

        ChatCompletionAgent agentInventoryManager =
            new()
            {
                Instructions = InventoryManagerInstructions,
                Name = InventoryManagerName,
                Kernel = this.CreateKernelWithChatCompletion(),
                Arguments = new KernelArguments(
                new OpenAIPromptExecutionSettings()
                {
                    ServiceId = "azure-openai",
                })
            };

        // Create a chat for agent interaction.
        AgentGroupChat chat =
            new(agentShoppingAssistant, agentInventoryManager)
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
                                Agents = [agentShoppingAssistant],
                                // Limit total number of turns
                                MaximumIterations = 10,
                            },
                        SelectionStrategy =
                        new KernelFunctionSelectionStrategy(CreateSelectionFunction(ShoppingAssistantName,InventoryManagerName), CreateKernelWithChatCompletion())
                        {
                            // Returns the entire result value as a string.
                            ResultParser = (result) => result.GetValue<string>() ?? "ShoppingAssistantAgentName",
                            // The prompt variable name for the agents argument.
                            AgentsVariableName = "agents",
                            // The prompt variable name for the history argument.
                            HistoryVariableName = "chatHistory2",
                        },
                    }
            };


        // Create plugins with their loggers
        // Step 1: Create a logger factory
        ILogger<InventoryPlugin> inventoryLogger = LoggerFactory
            .Create(builder =>
            {
                builder
                    .SetMinimumLevel(LogLevel.Debug) // Set log level
                    .AddConsole(); // Add console output
            })
            .CreateLogger<InventoryPlugin>();

        // Step 2: Create a logger for InventoryPlugin
        KernelPlugin inventoryplugin = agentInventoryManager.Kernel.ImportPluginFromObject(new InventoryPlugin(_configuration, inventoryLogger));
        agentInventoryManager.Kernel.Plugins.Add(inventoryplugin);

        // Invoke chat and display messages.
        ChatMessageContent input = new(AuthorRole.User, "Do you have jeans in stock?");
        chat.AddChatMessage(input);
        this.WriteAgentChatMessage(input);

        await foreach (ChatMessageContent response in chat.InvokeAsync())
        {
            this.WriteAgentChatMessage(response);
        }

        Console.WriteLine($"\n[IS COMPLETED: {chat.IsComplete}]");
    }

    private KernelFunction CreateSelectionFunction(string shoppingAssistantAgentName, string inventoryManagerAgentName)
    {
        var ShoppingAssistantAgentName = shoppingAssistantAgentName!;
        var InventoryManagerAgentName = inventoryManagerAgentName!;
        var selectionFunction = KernelFunctionFactory.CreateFromPrompt(
            $$$"""
            Your job is to determine which participant takes the next turn in a conversation.
            Return only the name: "{{{ShoppingAssistantAgentName}}}" or "{{{InventoryManagerAgentName}}}".

            Rules (in strict priority order):
            1. If this is a NEW user query about an item, return "{{{ShoppingAssistantAgentName}}}".
            2. If the last message was "CHECK_AVAILABILITY: [item details]" from {{{ShoppingAssistantAgentName}}}, return "{{{InventoryManagerAgentName}}}" to provide stock information.
            3. If the last message was "IN_STOCK" or "OUT_OF_STOCK" from {{{InventoryManagerAgentName}}}, return "{{{ShoppingAssistantAgentName}}}" to inform the user or suggest alternatives.
            4. If the last message was "UPDATE_STOCK: [item details]" from {{{ShoppingAssistantAgentName}}}, return "{{{InventoryManagerAgentName}}}" to update the stock levels.
            5. After "STOCK_UPDATED", return "{{{ShoppingAssistantAgentName}}}" to ask the customer if they need anything else.

            Check carefully:
            - Is this a NEW user query about an item?
            - Is this a response from the Inventory Manager regarding stock availability?
            - Is this a stock update request from the Shopping Assistant?

            History:
            {{$chatHistory}}

            Return only the exact agent name, no explanation.
            """);
        return selectionFunction;
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




