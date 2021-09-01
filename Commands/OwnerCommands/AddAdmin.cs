
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
    [Command("adda")]
    class AddAdmin : CommandBase
    {
        [Parameter("User ID")]
        public ulong IDtoAdd { get; private set; }

        public override void Execute()
        {
            try
            {
                if (!Program.isOwner(Message))
                {
                    Program.SendMessage(Message, "You need to be the owner to execute this command!");
                    return;
                }
                if (Program.BlockBotCommand(Message))
                {
                    Program.SendMessage(Message, "You need to use a user token to execute this command!");
                    return;
                }

                if (IDtoAdd.ToString().Length == 18)
                {
                    Admin.AddToAl(IDtoAdd);
                    Program.SendMessage(Message, "Added <@" + IDtoAdd.ToString() + "> to the Admins");
                }
                else Program.SendMessage(Message, "Usage: adda [userID]");
            }
            catch (Exception)
            {
                Program.SendMessage(Message, "Usage: adda [userID]");
            }
        }
    }
}
