using System;
using Discord.Commands;
using Discord;

using Discord.Media;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace Music_user_bot.Commands
{
    [Command("joinserver")]
    class JoinServerCommand : CommandBase
    {
        [Parameter("Guild invite")]
        public string invite { get; set; }
        public override void Execute()
        {
            if (Message.Author.User.Id == Whitelist.ownerID)
            {
                try
                {
                    var invite_code = Regex.Replace(invite, "https://discord.gg/", string.Empty, RegexOptions.IgnoreCase);

                    DiscordHttpClient.JoinGuild(Client.Token, invite_code , Message);

                    var guildId = ulong.Parse(GetInviteGuildAsync(invite_code).GetAwaiter().GetResult());

                    if (IsInGuild(Client, guildId))
                        Program.SendMessage(Message, "Succesfully joined the guild");
                    else
                        throw new DiscordHttpException(new DiscordHttpError());
                }
                catch(Exception) {
                    Program.SendMessage(Message, "Couldn't join guild.\n\nUsage: " + CommandHandler.Prefix + "joinserver [invite/code]");
                }
            }
            else
            {
                Program.SendMessage(Message, "You must be the owner to join guilds");
            }
        }
        public async Task<string> GetInviteGuildAsync(string inv_code)
        {
            string request_url = "https://discord.com/api/v9/invites/" + inv_code + "?inputValue=" + inv_code + "&with_counts=true&with_expiration=true";
            HttpClient client = new HttpClient();
            var response_context = await client.SendAsync(new HttpRequestMessage()
            {
                Method = new HttpMethod("GET"),
                RequestUri = new Uri(request_url)
            });
            var resp_context = new DiscordHttpResponse((int)response_context.StatusCode, response_context.Content.ReadAsStringAsync().Result);
            var json = JObject.Parse(resp_context.Body.ToString());

            return json["guild"].Value<string>("id");
        }
        public bool IsInGuild(DiscordClient Client, ulong guildId)
        {
            foreach (var guild in Client.GetGuilds())
            {
                if (guildId == guild.Id)
                    return true;
            }
            return false;
        }
    }
}
