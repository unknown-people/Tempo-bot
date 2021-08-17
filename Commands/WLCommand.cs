
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
    [Command("wl")]
    class WLCommand : CommandBase
    {
        public override void Execute()
        {
            string white_list = "Current whitelist:\n";
            foreach(ulong entry in Whitelist.white_list)
            {
                white_list += "<@" + entry.ToString() + ">\n";
            }
            if (white_list == "Current whitelist:\n")
                Message.Channel.SendMessage("Current whitelist is empty");
            else
                Message.Channel.SendMessage(white_list);
        }
    }
}
