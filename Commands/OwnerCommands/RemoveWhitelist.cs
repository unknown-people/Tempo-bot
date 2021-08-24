
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

namespace Music_user_bot
{
    [Command("delw")]
    class DelWhitelist : CommandBase
    {
        [Parameter("User ID")]
        public ulong IDtoDel { get; private set; }

        public override void Execute()
        {
            try
            {
                if (Message.Author.User.Id != Whitelist.ownerID)
                {
                    Message.Channel.SendMessage("You need to be the owner or an administrator to change the whitelist");
                    return;
                }
                if (IDtoDel.ToString().Length == 18)
                {
                    Whitelist.RemoveFromWL(IDtoDel);
                    Message.Channel.SendMessage("Removed <@" + IDtoDel.ToString() + "> from whitelist");
                }
                else Message.Channel.SendMessage("Usage: delw [userID]");
            }
            catch (Exception)
            {
                Message.Channel.SendMessage("Usage: delw [userID]");
            }
        }
    }
}