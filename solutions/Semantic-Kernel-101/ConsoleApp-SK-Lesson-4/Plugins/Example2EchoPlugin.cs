using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Embeddings;
using System.ComponentModel;
using System.Net.Http.Json;

namespace SKTraining.Plugins
{
    public class WeatherResponse
    {
        public MainInfo? Weather { get; set; }
    }

    public class MainInfo
    {
        public double temp { get; set; }
        public double feels_like { get; set; }
    }

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

            return echo;
        }
    }
}
