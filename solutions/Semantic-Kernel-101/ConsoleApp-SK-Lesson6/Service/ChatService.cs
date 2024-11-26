using Azure.AI.OpenAI;
using Cosmos.Chat.GPT.Models;
using Microsoft.ML.Tokenizers;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;

namespace Cosmos.Chat.GPT.Services
{
    public class ChatService
    {

        private readonly CosmosDbService _cosmosDbService;
        //private readonly OpenAiService _openAiService;
        //private readonly SemanticKernelService _semanticKernelService;
        private readonly Kernel _kernel;
        private readonly int _maxConversationTokens;
        private readonly double _cacheSimilarityScore;

        public ChatService(Kernel kernel, CosmosDbService cosmosDbService, string maxConversationTokens, string cacheSimilarityScore)
        {
            _cosmosDbService = cosmosDbService;
            _kernel = kernel;
            //_openAiService = openAiService;
            //_semanticKernelService = semanticKernelService;

            _maxConversationTokens = Int32.TryParse(maxConversationTokens, out _maxConversationTokens) ? _maxConversationTokens : 100;
            _cacheSimilarityScore = Double.TryParse(cacheSimilarityScore, out _cacheSimilarityScore) ? _cacheSimilarityScore : 0.99;
        }


        public async Task<Message> GetChatCompletionAsync(string? sessionId, string promptText)
        {

            ArgumentNullException.ThrowIfNull(sessionId);

            //Create a message object for the new User Prompt and calculate the tokens for the prompt
            Message chatMessage = await CreateChatMessageAsync(sessionId, promptText);

            //Grab context window from the conversation history up to the maximum configured tokens
            List<Message> contextWindow = await GetChatSessionContextWindow(sessionId);

            ////Perform a cache search to see if this prompt has already been used in the same context window as this conversation
            //(string cachePrompts, float[] cacheVectors, string cacheResponse) = await GetCacheAsync(contextWindow);

            ////Cache hit, return the cached completion
            //if (!string.IsNullOrEmpty(cacheResponse))
            //{
            //    chatMessage.Completion = cacheResponse;
            //    chatMessage.Completion += " (cached response)";
            //    chatMessage.CompletionTokens = 0;

            //    //Persist the prompt/completion, update the session tokens
            //    await UpdateSessionAndMessage(sessionId, chatMessage);

            //    return chatMessage;
            //}
            //else  //Cache miss, send to OpenAI to generate a completion
            //{

            //    //Generate a completion and tokens used from current context window which includes the latest user prompt
            //    //(chatMessage.Completion, chatMessage.CompletionTokens) = await _openAiService.GetChatCompletionAsync(sessionId, contextWindow);
            //    (chatMessage.Completion, chatMessage.CompletionTokens) = await _semanticKernelService.GetChatCompletionAsync(sessionId, contextWindow);

            //    //Cache the prompts in the current context window and their vectors with the generated completion
            //    await CachePutAsync(cachePrompts, cacheVectors, chatMessage.Completion);
            //}

            // Calling the Kernel my self will go back and clean this up
            (chatMessage.Completion, chatMessage.CompletionTokens) = await this.GetChatCompletionAsync(sessionId, contextWindow);

            //Persist the prompt/completion, update the session tokens
            await UpdateSessionAndMessage(sessionId, chatMessage);

            return chatMessage;
        }



        /// <summary>
        /// Add user prompt to a new chat session message object, calculate token count for prompt text.
        /// </summary>
        /// Step 1 - Must create the Session object first so you have a sessionid before calling this
        private async Task<Message> CreateChatMessageAsync(string sessionId, string promptText)
        {

            //Calculate tokens for the user prompt message.  we use this to compare against the _maxConversationTokens to manage the content window for the prompt
            int promptTokens = GetTokens(promptText);

            //Create a new message object.
            Message chatMessage = new(sessionId, promptTokens, promptText, "");

            await _cosmosDbService.InsertMessageAsync(chatMessage);

            return chatMessage;
        }

        private int GetTokens(string userPrompt)
        {
            // Must install the preview version of the library to get the CreateTiktokenForModel function
            // dotnet add package Microsoft.ML.Tokenizers --version 0.22.0-preview.24271.1
            // gpt-4 or gpt-3.5-turbo 4o has not been added to the dictionary yet but this serves the purpose
            Tokenizer _tokenizer = Tokenizer.CreateTiktokenForModel("gpt-4");

            return _tokenizer.CountTokens(userPrompt);

        }

        private async Task<List<Message>> GetChatSessionContextWindow(string sessionId)
        {

            int? tokensUsed = 0;

            List<Message> allMessages = await _cosmosDbService.GetSessionMessagesAsync(sessionId);
            List<Message> contextWindow = new List<Message>();

            //Start at the end of the list and work backwards
            //This includes the latest user prompt which is already cached
            for (int i = allMessages.Count - 1; i >= 0; i--)
            {
                tokensUsed += allMessages[i].PromptTokens + allMessages[i].CompletionTokens;

                if (tokensUsed > _maxConversationTokens)
                    break;

                contextWindow.Add(allMessages[i]);
            }

            //Invert the chat messages to put back into chronological order 
            contextWindow = contextWindow.Reverse<Message>().ToList();

            return contextWindow;

        }

        private async Task UpdateSessionAndMessage(string sessionId, Message chatMessage)
        {

            //Update the tokens used in the session
            Session session = await _cosmosDbService.GetSessionAsync(sessionId);
            session.Tokens += chatMessage.PromptTokens + chatMessage.CompletionTokens;

            //Insert new message and Update session in a transaction
            await _cosmosDbService.UpsertSessionBatchAsync(session, chatMessage);

        }

        public async Task<(string completion, int tokens)> GetChatCompletionAsync(string sessionId, List<Message> chatHistory)
        {
            string _systemPrompt = @"
        You are an AI assistant that helps people find information.
        Provide concise answers that are polite and professional.";
            var skChatHistory = new ChatHistory();
            skChatHistory.AddSystemMessage(_systemPrompt);

            foreach (var message in chatHistory)
            {
                skChatHistory.AddUserMessage(message.Prompt);
                if (message.Completion != string.Empty)
                    skChatHistory.AddAssistantMessage(message.Completion);
            }

            PromptExecutionSettings settings = new()
            {
                ExtensionData = new Dictionary<string, object>()
                {
                    { "Temperature", 0.2 },
                    { "TopP", 0.7 },
                    { "MaxTokens", 1000  }
                }
            };

            var result = await _kernel.GetRequiredService<IChatCompletionService>().GetChatMessageContentAsync(skChatHistory, settings);

            CompletionsUsage completionUsage = (CompletionsUsage)result.Metadata!["Usage"]!;

            string completion = result.Items[0].ToString()!;
            int tokens = completionUsage.CompletionTokens;

            return (completion, tokens);
        }
    }
}