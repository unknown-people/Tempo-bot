using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;

namespace Discord
{
    public class ContextProperties
    {
        [JsonProperty("location")]
        public string location { get; set; } = "Join Guild";

        //[JsonProperty("location_guild_id")]
        //TODO public string Browser { get; set; } = "Chrome";

        //[JsonProperty("location_channel_id")]
        //TODO public string Device { get; set; } = "";

        //[JsonProperty("location_channel_type")]
        //TODO public string SystemLocale { get; set; } = "it-IT";

        public static ContextProperties FromBase64(string base64)
        {
            return JsonConvert.DeserializeObject<ContextProperties>(Encoding.UTF8.GetString(Convert.FromBase64String(base64)));
        }


        public string ToBase64()
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(this)));
        }
    }
}