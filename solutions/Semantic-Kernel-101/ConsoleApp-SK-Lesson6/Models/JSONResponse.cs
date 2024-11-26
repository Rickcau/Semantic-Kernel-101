using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;


namespace ConsoleApp_SK_Lesson6.Models
{
    
    public class JSONResponse
    {
        [JsonPropertyName("intent")]
        public string?   Intent { get; set; } // Retrieval or other intent

        [JsonPropertyName("subcategory")]
        public string? Subcategory { get; set; } // SERIES or other subcategories

        [JsonPropertyName("semanticsearch")]
        public string? SemanticSearch { get; set; } // Value to be searched

        [JsonPropertyName("why")]
        public string? Why { get; set; } // Explanation for labeling as retrieval or unrelated
    }
}
