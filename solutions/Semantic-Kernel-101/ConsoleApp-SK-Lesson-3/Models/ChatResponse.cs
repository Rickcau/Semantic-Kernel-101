using System;
using System.Collections.Generic;

namespace ConsoleApp_SK_Lesson_3.Models
{
    public class ChatResponse
    {
        public string? Content { get; set; }
        public List<Citation> ?Citations { get; set; }
    }

    public class Citation
    {
        public string? Title { get; set; }
        public string? Link { get; set; }
    }
}
