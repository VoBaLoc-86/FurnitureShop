using Newtonsoft.Json;

namespace FurnitureShop.Models
{
    public class RecaptchaResponse
    {
        [JsonProperty("success")] 
        public bool Success { get; set; }
        [JsonProperty("challenge_ts")] 
        public DateTime ChallengeTs { get; set; }
        [JsonProperty("hostname")] 
        public string Hostname { get; set; }
    }
}
