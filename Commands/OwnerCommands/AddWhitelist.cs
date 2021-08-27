
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
                if (Message.Author.User.Id != Whitelist.ownerID)
                {
                    Program.SendMessage(Message, "You need to be the owner or an administrator to change the whitelist");
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
