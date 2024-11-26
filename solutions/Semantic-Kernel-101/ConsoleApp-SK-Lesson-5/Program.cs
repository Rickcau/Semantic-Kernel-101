using ConsoleApp_SK_Lesson_5.Helpers;
using Cosmos.Chat.GPT.Models;
using Cosmos.Chat.GPT.Services;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Newtonsoft.Json.Linq;
using System.Configuration;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using ConsoleApp_SK_Lesson_5.Helpers;


Console.WriteLine("Our first Console App - Semantic Kernel Chat Bot\n");

// 5 Simple steps to use the Kernel!

#region Step 1: Create Kernel Builder
// Create a Builder for Creating Kernel Objects
var builder = Kernel.CreateBuilder();
#endregion

#region Step 2: Load AI Endpoint Values

var openAiDeployment = ConfigurationManager.AppSettings.Get("AzureOpenAIModel");
var openAiUri = ConfigurationManager.AppSettings.Get("AzureOpenAIEndpoint");
var openAiApiKey = ConfigurationManager.AppSettings.Get("AzureOpenAIKey");

// Not being used
// int maxConversationTokens = Int32.TryParse(ConfigurationManager.AppSettings.Get("MaxConversationTokens"), out maxConversationTokens) ? maxConversationTokens : 100;
// Double cacheSimilarityScore = Double.TryParse(ConfigurationManager.AppSettings.Get("CacheSimilarityScore"), out cacheSimilarityScore) ? cacheSimilarityScore : 0.99;


#endregion

#region Step 3: Add ChatCompletion Service

builder.Services.AddAzureOpenAIChatCompletion(
   deploymentName: openAiDeployment!,
   endpoint: openAiUri!,
   apiKey: openAiApiKey!);

#endregion

#region Step 4: Construct Kernel, ChatHistory Get instance of ChatCompletion Service
// Construct instance of Kernel using Builder Settings
var kernel = builder.Build();

ChatHistory history = [];

var chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();


#endregion

#region Example of Normalizing Words or Phrases

List<string> phrasesorwords = new List<string>
        {
            "Houses",
            "The Lord of the Rings",
            "Cats and Dogs",
            "Running Quickly",
            "Books, the",
            "the Walking Dead",
            "Zombies"
        };
List<string> records = new List<string>
{
    "Houses",
    "House",
    "Houses of Parliament",
    "Big Houses",
    "The Lord of the Rings",
    "Lord of the Rings",
    "Rings, the Lord of",
    "The Rings Trilogy",
    "Cats and Dogs",
    "Dog and Cat",
    "Running Quickly",
    "Quickly Running",
    "Books, the",
    "The Books",
    "Books",
    "The Walking Dead",
    "Walking Dead, the",
    "The Walking Dead Series",
    "Walking Dead TV Show",
    "Walking Dead",
    "The Walking Dead Season 5",
    "The Walking Dead Universe",
    "Walking Dead Movie",
    "Walking Dead Comics",
    "Walking Dead Video Game"
};

Console.WriteLine("Example of using AI to normalize a word or Phrase...\n");
Console.WriteLine("This is helpful when you need to use fuzzy logic to search for items that match a normalized word or pharse");
Console.WriteLine("You pass the orignal word or phrase to the function, have the LLM normalize it, then you would use this word\n or phrase to find all records with the word or phrase.");
Console.WriteLine("Once you have the list of records you would then pass this to another prompt to perform simalarity match and return the top 3-4 best matches.\n\n");
Console.WriteLine("## Step 1 Normalize the Word or Phrase in the list of words and phrases...\n");

AIHelper aiHelper = new AIHelper(kernel);
foreach (var phraseorword in phrasesorwords)
{
    var normalizedVersion = await aiHelper.GetNormalizedWordOrPhraseAsync(phraseorword);
    Console.WriteLine($"Original: {phraseorword}, Normalized: {normalizedVersion}");
}
Console.WriteLine("Press enter to run the the Simalarity Search example..\n");
Console.ReadLine();
#endregion

#region Example of performing a Simalarity Word Search for the Normalized word or Phrase !!! This is not using embeddings, it's simply performing a search on the normalized word or phrases that are similar to the normalized word or phrase using the LLM.
Console.WriteLine("Now, we will take one of the Words or Phrases normalize and perform a Simalarity Search against a few records...");
Console.WriteLine("The idea here is that you would retreive this the list of records from a backend system, SQL, CosmosDB, Postgres etc..");
Console.WriteLine("In this example we are using a static list of records, in a production solution this set of records would be retreived.");
Console.WriteLine("Ideally, you would have a Vector Index of the records and perform a search on the normalized word or phrase using embeddings with a semantic simalarity search.\n");
Console.WriteLine("Then you take the normalized word and find the closest match in the list of records provided..\n");
Console.WriteLine("## Step 1 Normalize the Word.\n");

var normalizedVersion2 = await aiHelper.GetNormalizedWordOrPhraseAsync(phrasesorwords[5]);
Console.WriteLine($"Original: {phrasesorwords[5]}, Normalized: {normalizedVersion2}\n");
Console.WriteLine("## Step 2 Perform Similarity Search.");
Console.WriteLine($"Searching for: {normalizedVersion2} against the records list..\n");
var result = await aiHelper.Gettop3SemanticMatchesAsync(normalizedVersion2, records);
var resultsofSearch = $"## Results \n {result}\n";
Console.WriteLine(resultsofSearch);


Console.WriteLine("End of Simalarity Word Search example. Press any key...\n\n");
Console.ReadLine();


JsonSerializerOptions s_options = new() { WriteIndented = true };
int[] data = [1, 2, 3];
#pragma warning disable SKEXP0010
#pragma warning disable SKEXP0001


//var deserializedHistory = JsonSerializer.Deserialize<ChatHistory>(chatHistoryJson);
#endregion
