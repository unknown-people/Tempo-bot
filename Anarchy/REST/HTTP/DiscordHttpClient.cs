using Newtonsoft.Json.Linq;
using System.Threading;
using Newtonsoft.Json;
using System;
using System.Text;
using System.Net.Http;
using Leaf.xNet;
using System.Net;
using System.Threading.Tasks;
using Music_user_bot;

namespace Discord
{
    public class DiscordHttpClient
    {
        private readonly DiscordClient _discordClient;
        public string BaseUrl => DiscordHttpUtil.BuildBaseUrl(_discordClient.Config.ApiVersion, _discordClient.Config.SuperProperties.ReleaseChannel);
        public string _fingerprint { get; set; }
        public DiscordHttpClient(DiscordClient discordClient)
        {
            _discordClient = discordClient;
            if(!(Settings.Default.Fingerprint != null && Settings.Default.Fingerprint != ""))
                Settings.Default.Fingerprint = GetFingerprint().GetAwaiter().GetResult();
            _fingerprint = Settings.Default.Fingerprint;
        }
        public async Task<string> GetFingerprint()
        {
            HttpClient client = new HttpClient();
            var response_fingerprint = await client.SendAsync(new HttpRequestMessage()
            {
                Method = new System.Net.Http.HttpMethod("GET"),
                RequestUri = new Uri("https://discord.com/api/v9/experiments")
            });
            var resp_fingerprint = new DiscordHttpResponse((int)response_fingerprint.StatusCode, response_fingerprint.Content.ReadAsStringAsync().Result);
            var fingerprint = resp_fingerprint.Body.Value<String>("fingerprint");

            return fingerprint;
        }

        /// <summary>
        /// Sends an HTTP request and checks for errors
        /// </summary>
        /// <param name="method">HTTP method to use</param>
        /// <param name="endpoint">API endpoint (fx. /users/@me)</param>
        /// <param name="payload">JSON content</param>
        private async Task<DiscordHttpResponse> SendAsync(Leaf.xNet.HttpMethod method, string endpoint, object payload = null)
        {
            if (!endpoint.StartsWith("https")) 
                endpoint = DiscordHttpUtil.BuildBaseUrl(_discordClient.Config.ApiVersion, _discordClient.Config.SuperProperties.ReleaseChannel) + endpoint;

            string json = "{}";
            if (payload != null)
            {
                if (payload.GetType() == typeof(string))
                    json = (string)payload;
                else
                    json = JsonConvert.SerializeObject(payload);
            }

            uint retriesLeft = _discordClient.Config.RestConnectionRetries;
            bool hasData = method == Leaf.xNet.HttpMethod.POST || method == Leaf.xNet.HttpMethod.PATCH || method == Leaf.xNet.HttpMethod.PUT || method == Leaf.xNet.HttpMethod.DELETE;

            while (true)
            {
                try
                {
                    DiscordHttpResponse resp;

                    if (_discordClient.Proxy == null || _discordClient.Proxy.Type == ProxyType.HTTP)
                    {
                        HttpClient client = new HttpClient(new HttpClientHandler() { Proxy = _discordClient.Proxy == null ? null : new WebProxy(_discordClient.Proxy.Host, _discordClient.Proxy.Port) });
                        if (_discordClient.Token != null)
                            client.DefaultRequestHeaders.Add("Authorization", _discordClient.Token);

                        if (_discordClient.User != null && _discordClient.User.Type == DiscordUserType.Bot)
                            client.DefaultRequestHeaders.Add("User-Agent", "Anarchy/0.8.1.0");
                        else
                        {
                            client.DefaultRequestHeaders.Add("User-Agent", _discordClient.Config.SuperProperties.UserAgent);
                            client.DefaultRequestHeaders.Add("X-Super-Properties", _discordClient.Config.SuperProperties.ToBase64());
                        }

                        var response = await client.SendAsync(new HttpRequestMessage()
                        { 
                            Content = hasData ? new System.Net.Http.StringContent(json, Encoding.UTF8, "application/json") : null, 
                            Method = new System.Net.Http.HttpMethod(method.ToString()), 
                            RequestUri = new Uri(endpoint) 
                        });

                        resp = new DiscordHttpResponse((int)response.StatusCode, response.Content.ReadAsStringAsync().Result);
                    }
                    else
                    {
                        HttpRequest msg = new HttpRequest
                        {
                            IgnoreProtocolErrors = true,
                            UserAgent = _discordClient.User != null && _discordClient.User.Type == DiscordUserType.Bot ? "Anarchy/0.8.1.0" : _discordClient.Config.SuperProperties.UserAgent,
                            Authorization = _discordClient.Token
                        };

                        if (hasData)
                            msg.AddHeader(HttpHeader.ContentType, "application/json");

                        if (_discordClient.User == null || _discordClient.User.Type == DiscordUserType.User) msg.AddHeader("X-Super-Properties", _discordClient.Config.SuperProperties.ToBase64());
                        if (_discordClient.Proxy != null) msg.Proxy = _discordClient.Proxy;

                        var response = msg.Raw(method, endpoint, hasData ? new Leaf.xNet.StringContent(json) : null);

                        resp = new DiscordHttpResponse((int)response.StatusCode, response.ToString());
                    }

                    DiscordHttpUtil.ValidateResponse(resp.StatusCode, resp.Body);
                    return resp;
                }
                catch (Exception ex) when (ex is HttpException || ex is HttpRequestException || ex is TaskCanceledException)
                {
                    if (retriesLeft == 0)
                        throw new DiscordConnectionException();

                    retriesLeft--;
                }
                catch (RateLimitException ex)
                {
                    if (_discordClient.Config.RetryOnRateLimit)
                        Thread.Sleep(ex.RetryAfter);
                    else
                        throw;
                }
            }
        }

        private async Task<DiscordHttpResponse> SendAsyncJoin(Leaf.xNet.HttpMethod method, string endpoint, object payload = null)
        {
            if (!endpoint.StartsWith("https"))
                endpoint = DiscordHttpUtil.BuildBaseUrl(_discordClient.Config.ApiVersion, _discordClient.Config.SuperProperties.ReleaseChannel) + endpoint; 
            string inv_code = "https://discord.com/api/v9/invites/";
            if (endpoint.Contains("/invites/"))
                inv_code = endpoint.Substring(inv_code.Length);
            
            string json = "{}";
            if (payload != null)
            {
                if (payload.GetType() == typeof(string))
                    json = (string)payload;
                else
                    json = JsonConvert.SerializeObject(payload);
            }

            uint retriesLeft = _discordClient.Config.RestConnectionRetries;
            bool hasData = method == Leaf.xNet.HttpMethod.POST || method == Leaf.xNet.HttpMethod.PATCH || method == Leaf.xNet.HttpMethod.PUT || method == Leaf.xNet.HttpMethod.DELETE;

            while (true)
            {
                try
                {
                    DiscordHttpResponse resp;

                    if (_discordClient.Proxy == null || _discordClient.Proxy.Type == ProxyType.HTTP)
                    {
                        HttpClient client = new HttpClient(new HttpClientHandler() { Proxy = _discordClient.Proxy == null ? null : new WebProxy(_discordClient.Proxy.Host, _discordClient.Proxy.Port) });
                        var context = ContextProperties.GetContextProperties(inv_code).GetAwaiter().GetResult();

                        if (_discordClient.Token != null)
                            client.DefaultRequestHeaders.Add("authorization", _discordClient.Token);
                        if (_discordClient.User != null && _discordClient.User.Type == DiscordUserType.Bot)
                            client.DefaultRequestHeaders.Add("User-Agent", "Anarchy/0.8.1.0");
                        else
                        {
                            client.DefaultRequestHeaders.Add("accept", "*/*");
                            client.DefaultRequestHeaders.Add("accept-language", "it");
                            client.DefaultRequestHeaders.Add("origin", "https://discord.com");
                            client.DefaultRequestHeaders.Add("referer", "https://discord.com/channels/@me");
                            client.DefaultRequestHeaders.Add("sec-fetch-dest", "empty");
                            client.DefaultRequestHeaders.Add("sec-fetch-mode", "cors");
                            client.DefaultRequestHeaders.Add("sec-fetch-site", "same-origin");
                            client.DefaultRequestHeaders.Add("sec-gpc", "1");
                            client.DefaultRequestHeaders.Add("User-Agent", _discordClient.Config.SuperProperties.UserAgent);
                            client.DefaultRequestHeaders.Add("X-Context-Properties", context);
                            client.DefaultRequestHeaders.Add("X-Debug-Options", "bugReporterEnabled");
                            client.DefaultRequestHeaders.Add("X-Fingerprint", _fingerprint);
                            client.DefaultRequestHeaders.Add("X-Super-Properties", _discordClient.Config.SuperProperties.ToBase64());
                        }

                        var response = await client.SendAsync(new HttpRequestMessage()
                        {
                            Content = hasData ? new System.Net.Http.StringContent(json, Encoding.UTF8, "application/json") : null,
                            Method = new System.Net.Http.HttpMethod(method.ToString()),
                            RequestUri = new Uri(endpoint)
                        });
                        client.DefaultRequestHeaders.Clear();

                        resp = new DiscordHttpResponse((int)response.StatusCode, response.Content.ReadAsStringAsync().Result);
                    }
                    else
                    {
                        HttpRequest msg = new HttpRequest
                        {
                            IgnoreProtocolErrors = true,
                            UserAgent = _discordClient.User != null && _discordClient.User.Type == DiscordUserType.Bot ? "Anarchy/0.8.1.0" : _discordClient.Config.SuperProperties.UserAgent,
                            Authorization = _discordClient.Token
                        };

                        if (hasData)
                            msg.AddHeader(HttpHeader.ContentType, "application/json");

                        if (_discordClient.User == null || _discordClient.User.Type == DiscordUserType.User) msg.AddHeader("X-Super-Properties", _discordClient.Config.SuperProperties.ToBase64());
                        if (_discordClient.Proxy != null) msg.Proxy = _discordClient.Proxy;

                        var response = msg.Raw(method, endpoint, hasData ? new Leaf.xNet.StringContent(json) : null);

                        resp = new DiscordHttpResponse((int)response.StatusCode, response.ToString());
                    }

                    DiscordHttpUtil.ValidateResponse(resp.StatusCode, resp.Body);
                    return resp;
                }
                catch (Exception ex) when (ex is HttpException || ex is HttpRequestException || ex is TaskCanceledException)
                {
                    if (retriesLeft == 0)
                        throw new DiscordConnectionException();

                    retriesLeft--;
                }
                catch (RateLimitException ex)
                {
                    if (_discordClient.Config.RetryOnRateLimit)
                        Thread.Sleep(ex.RetryAfter);
                    else
                        throw;
                }
            }
        }


        public async Task<DiscordHttpResponse> GetAsync(string endpoint)
        {
            return await SendAsync(Leaf.xNet.HttpMethod.GET, endpoint);
        }


        public async Task<DiscordHttpResponse> PostAsync(string endpoint, object payload = null)
        {
            return await SendAsync(Leaf.xNet.HttpMethod.POST, endpoint, payload);
        }

        public async Task<DiscordHttpResponse> PostAsyncJoin(string endpoint, object payload = null)
        {
            return await SendAsyncJoin(Leaf.xNet.HttpMethod.POST, endpoint, payload);
        }


        public async Task<DiscordHttpResponse> DeleteAsync(string endpoint, object payload = null)
        {
            return await SendAsync(Leaf.xNet.HttpMethod.DELETE, endpoint, payload);
        }


        public async Task<DiscordHttpResponse> PutAsync(string endpoint, object payload = null)
        {
            return await SendAsync(Leaf.xNet.HttpMethod.PUT, endpoint, payload);
        }


        public async Task<DiscordHttpResponse> PatchAsync(string endpoint, object payload = null)
        {
            return await SendAsync(Leaf.xNet.HttpMethod.PATCH, endpoint, payload);
        }
    }
    public class Fingerprint
    {
        [JsonProperty("assignments")]
        public Array assignments { get; set; }
        [JsonProperty("fingerprint")]
        public string fingerprint { get; set; }
    }
}