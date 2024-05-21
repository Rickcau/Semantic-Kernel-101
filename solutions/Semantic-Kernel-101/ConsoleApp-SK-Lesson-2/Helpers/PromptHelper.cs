using System.Configuration;

internal static class InlinePromptHelper
{
    public const string _promptPiglatin = @"
        +++++
        Convert the follow to Pig Latin: 
        {{$query}}
        +++++     
      
        Pig Latin Translation:";

    public const string _promptWriteStory = @"
        +++++
        Write a 5-paragraph story that includes the following words: 
        {{$query}}
        If more than 5 words are provided only use 5 words.
        +++++";




}
