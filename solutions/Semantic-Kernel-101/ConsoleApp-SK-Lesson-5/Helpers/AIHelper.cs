
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using System.Net;

namespace ConsoleApp_SK_Lesson_5.Helpers
{
    public class AIHelper
    {
        private Kernel _kernel;
        private string _promptSummarize = @"SUMMARIZE THE CONVERSATION IN 20 BULLET POINTS OR LESS

        SUMMARY MUST BE:
        - WORKPLACE / FAMILY SAFE NO SEXISM, RACISM OR OTHER BIAS/BIGOTRY
        - G RATED
        - IF THERE ARE ANY  INCONSISTENCIES IN THE TRANSCRIPT, DO YOUR BEST TO CALL THOSE OUT

        {{$input}}";

        private string _promptTranslation = @"Translate the input below into {{$language}}

        MAKE SURE YOU ONLY USE {{$language}}.

        {{$input}}

        Translation:";

        private string _promptWordOrPhraseNormalization = @"You are an expert at normalizing words or phrases.  When given a word or pharse, you should normalize it and respond with the Normalized version using the your knowledge and the additional instructions below.
        
        
        Normalization is the process of converting a word or phrase into a standard or canonical form. It often involves removing variations, redundancies, or unnecessary details to make the term consistent for comparison, storage, or further processing. This can include:        

        ## Singularization: Converting plural forms into their singular counterparts (e.g., ""Ducks"" → ""Duck"").
        ## Removing Articles or Prepositions: Stripping words like ""the,"" ""of,"" or ""in"" if they are not essential to the meaning (e.g., ""The Walking Dead"" → ""Walking Dead"").
        ## Standardizing Case: Ensuring consistent letter casing (e.g., lowercase or title case).
        ## Simplifying Structure: Reordering words or removing non-critical ones to clarify the core meaning (e.g., ""Walking Dead, the"" → ""Walking Dead"").
        ## Lemmatization: Converting inflected forms of words to their base or dictionary form (e.g., ""running"" → ""run"").
        
        ## Goal
        The goal is to produce a version of the word or phrase that is concise and semantically clear while preserving the original intent or meaning.
        MAKE SURE YOU ONLY USE {{$language}}.
        
        Provided Word or Phrase: {{$input}}

        Normalizataion:";

        private string _promptForTop3SemanticMatchesJSON = @"
        You are an expert at determining the semantic similarity between words or phrases. When given a normalized word or phrase, you should compare it to the following list of records and return a JSON object with the top 3 records that best match the normalized version based on semantic similarity.

        ### Provided Normalized Word or Phrase: {{$normalizedWord}}

        ### Records to Compare:
        {{$records}}

        ### Instructions:
        1. Determine the semantic similarity between the normalized word or phrase and each record in the provided list.
        2. Rank the records based on their semantic similarity to the normalized word or phrase.
        3. Return the top 3 records with the highest similarity in a raw JSON format now backticks.

        The raw JSON object should have the following structure:
        {
          'matches': [
            {
              'record': 'Record 1',
              'similarityScore': <similarity_score_1>
            },
            {
              'record': 'Record 2',
              'similarityScore': <similarity_score_2>
            },
            {
            'record': 'Record 3',
              'similarityScore': < similarity_score_3 >
            }
          ]
        }
        ";


        public AIHelper(Kernel kernel)
        {
            this._kernel = kernel;
        }

        public async Task<string> GetSummaryAsync(string transcript)
        {
            var summarizeFunction = _kernel.CreateFunctionFromPrompt(_promptSummarize, executionSettings: new OpenAIPromptExecutionSettings { MaxTokens = 100 });

            var response = await _kernel.InvokeAsync(summarizeFunction, new() { ["input"] = transcript });

            return response.GetValue<string>() ?? "";
        }

        public async Task<string> GetNormalizedWordOrPhraseAsync(string wordorphrase)
        {
            var normalizedWordOrPhraseFunction = _kernel.CreateFunctionFromPrompt(_promptWordOrPhraseNormalization, executionSettings: new OpenAIPromptExecutionSettings { MaxTokens = 100 });

            var response = await _kernel.InvokeAsync(normalizedWordOrPhraseFunction, new() { ["input"] = wordorphrase });

            return response.GetValue<string>() ?? "";
        }

        public async Task<string> Gettop3SemanticMatchesAsync(string normalizedword, List<string> records)
        {
            var top3SemanticMathcesFunction = _kernel.CreateFunctionFromPrompt(_promptForTop3SemanticMatchesJSON, executionSettings: new OpenAIPromptExecutionSettings { MaxTokens = 100 });
            string joinedString = string.Join(", ", records);

            var response = await _kernel.InvokeAsync(top3SemanticMathcesFunction, new() { ["normalizedWord"] = normalizedword, ["records"] = joinedString });

            return response.GetValue<string>() ?? "";
        }

        public async Task<string> GetTranslationAsync(string content, string language)
        {
            var translationFunction = _kernel.CreateFunctionFromPrompt(_promptTranslation, executionSettings: new OpenAIPromptExecutionSettings { MaxTokens = 2000, Temperature = 0.7, TopP = 0.0 });

            var response = await _kernel.InvokeAsync(translationFunction, new() { ["input"] = content, ["language"] = language });

            return response.GetValue<string>() ?? "";
        }


    }
}