using System;
using Discord.Commands;
using Discord;
using Discord.Gateway;
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
                    var invite_code = Regex.Replace(invite, "https://discord.gg/.*", string.Empty, RegexOptions.IgnoreCase);
                    invite_code = Regex.Replace(invite, "discord.gg/.*", string.Empty, RegexOptions.IgnoreCase);
                    var inviteNew = Join(invite_code);
                    if (inviteNew.Guild.Name != null)
                    {
                        Program.SendMessage(Message, "Joined guild");
                    }
                    else
                    {
                        throw new IndexOutOfRangeException();
                    }
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
        public async Task<GuildInvite> JoinAsync(string Code)
        {
            return await Client.JoinGuildAsync(Code);
        }
        public GuildInvite Join(string Code)
        {
            return JoinAsync(Code).GetAwaiter().GetResult();
        }
    }
}
