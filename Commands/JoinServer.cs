using System;
using Discord.Commands;
using Discord;
using Discord.Media;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

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
                    DiscordHttpClient.JoinGuild(Client.Token, invite_code, Message);
                    
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

    }
}
