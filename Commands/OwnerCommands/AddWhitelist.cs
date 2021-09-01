
using Discord.Commands;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Gateway;
using Discord.Media;
using System.Threading;
using YoutubeExplode.Videos.Streams;
using System.IO;
using System.Text.RegularExpressions;
using System;

namespace Music_user_bot.Commands
{
    [Command("addw")]
    class AddWhitelist : CommandBase
    {
        [Parameter("User ID")]
        public ulong IDtoAdd { get; private set; }

        public override void Execute()
        {
            try
            {
                if (!Program.isOwner(Message) && !Program.isAdmin(Message) )
                {
                    Program.SendMessage(Message, "You need to be the owner or an administrator to execute this command!");
                    return;
                }
                if (Program.BlockBotCommand(Message))
                {
                    Program.SendMessage(Message, "You need to use a user token to execute this command!");
                    return;
                }

                if (IDtoAdd.ToString().Length == 18)
                {
                    Whitelist.AddToWL(IDtoAdd);
                    Program.SendMessage(Message, "Added <@" + IDtoAdd.ToString() + "> to whitelist");
                }
                else Program.SendMessage(Message, "Usage: addw [userID]");
            }
            catch (Exception)
            {
                Program.SendMessage(Message, "Usage: addw [userID]");
            }
        }
    }
}
