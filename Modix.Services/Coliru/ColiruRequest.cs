using Newtonsoft.Json;

namespace Modix.Services.Coliru
{
    internal sealed class ColiruRequest
    {
        [JsonProperty("cmd")]
        public string Command { get; set; }

        [JsonProperty("src")]
        public string Source { get; set; }
    }
}
