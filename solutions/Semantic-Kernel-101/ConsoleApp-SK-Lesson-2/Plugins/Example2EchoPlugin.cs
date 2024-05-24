using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Embeddings;
using System.ComponentModel;
using System.Net.Http.Json;

namespace SKTraining.Plugins
{
    public class Example2EchoPlugin
    {

        [KernelFunction]
        [Description("Echo the question back")]
        public async Task<string> SendAnEchoAsync([Description("question to echo")] string question)
        {
            var echo = $@"ECHO -- {question} ... {question}  -- ECHO";
            await Task.Delay(1000);

            return echo;
        }
    }
}
