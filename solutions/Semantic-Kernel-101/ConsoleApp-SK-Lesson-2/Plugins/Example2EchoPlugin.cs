using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Embeddings;
using System.ComponentModel;
using System.Net.Http.Json;

namespace SKTraining.Plugins
{
    public class Example2EchoPlugin
    {
        private readonly Kernel _kernel;


        public Example2EchoPlugin(Kernel kernel)
        {
            this._kernel = kernel;
        }

        [KernelFunction]
        [Description("Echo the question back")]
        public async Task<string> SendAnEchoAsync([Description("question to echo")] string question)
        {
            var echo = $@"ECHO -- {question} -- ECHO";
            await Task.Delay(1000);

            return echo;
        }
    }
}
