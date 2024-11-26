using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp_SK_Lesson6.Prompts
{

    public class ExamplePrompts
    {

        public static string GetJSONObjectPrompt(string prompt) =>
        $$$"""
        ###
        Assistant: 
        You are an expert at detecting if the user question is related to retrieval of data or not.
 
        ###
        Example Retrieval Questions:
        1. Basic Viewing Metrics:
           - User Prompt: 'How many streams were viewed on amc+ with amazon in us in January 2024?'
        2. Watch Time Analytics:
           - User Prompt: 'What is the total watch time for Acorn on Apple in the United States in Q1 2024?'
        3. Latest Season Analysis:
           - User Prompt: 'How many viewers streamed the latest season of the Dark Winds in us in September 2024?'
        4. Cast Performance:
           - User Prompt: 'Provide top 5 series by minutes viewed featuring Andrew Lincoln on AMC + in the United States during 2024?'
        ### 
        Example unrelated retrieval Questions:
        1. Unrelated Request:
           - User Prompt: 'What's the weather like today?'

        2. Unrelated Request:
           - User Prompt: 'Help me with my homework'

        INSTRUCTIONS:
        1. Evaluate the user prompt and categorize it as a retrieval question or as an unrelated question or additional inforamtion.
        2. Identify the subcategory of the user prompt can either be 'BRAND', BRAND_DISTRIBUTOR, 'SERIES', 'SERIES_SYNOPSIS', 'SERIES_CAST_AND_CREW' or 'UNKNOWN'
        3. If subcategory is SERIES set the 'semanticsearch' property to the series that needs to be searched.
        4. Using the JSON structure below, populate the intent property with either retrieval, unrelated or additional
        5. Always return the JSON object in RAW JSON, DO NOT use backticks.

        ### JSON Object
        {
          "intent" : "retrieval",
          "subcategory": "SERIES",
          "semanticsearch": "set to the value that needs to be searched"
          "why" :"populate this property with details about why you labeled this as a retrieval or unrelated question"
        }

        ###
        User Prompt
        {{{prompt}}}
        """;


        public static string GetJSONStructuredOutputPrompt(string prompt) =>
        $$$"""
        ###
        Assistant: 
        You are an expert at detecting if the user question is related to retrieval of data or not.
 
        ###
        Example Retrieval Questions:
        1. Basic Viewing Metrics:
           - User Prompt: 'How many streams were viewed on amc+ with amazon in us in January 2024?'
        2. Watch Time Analytics:
           - User Prompt: 'What is the total watch time for Acorn on Apple in the United States in Q1 2024?'
        3. Latest Season Analysis:
           - User Prompt: 'How many viewers streamed the latest season of the Dark Winds in us in September 2024?'
        4. Cast Performance:
           - User Prompt: 'Provide top 5 series by minutes viewed featuring Andrew Lincoln on AMC + in the United States during 2024?'
        ### 
        Example unrelated retrieval Questions:
        1. Unrelated Request:
           - User Prompt: 'What's the weather like today?'

        2. Unrelated Request:
           - User Prompt: 'Help me with my homework'

        INSTRUCTIONS:
        1. Evaluate the user prompt and categorize it as a retrieval question or as an unrelated question or additional inforamtion.
        2. Identify the subcategory of the user prompt can either be 'BRAND', BRAND_DISTRIBUTOR, 'SERIES', 'SERIES_SYNOPSIS', 'SERIES_CAST_AND_CREW' or 'UNKNOWN'
        3. If subcategory is SERIES set the 'semanticsearch' property to the series that needs to be searched otherwise set it to ""
        4. Populate the 'why' property with details about why you labeled this as a retrieval or unrelated question
        4. Using the JSON structure below, populate the intent property with either retrieval, unrelated or additional
        5. Always return the JSON object in RAW JSON, DO NOT use backticks.

        ###
        User Prompt
        {{{prompt}}}
        """;
    }
}
