﻿
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
    [Command("info")]
    class InfoCommand : CommandBase
    {
        public override void Execute()
        {
            string white_list = "Current whitelist:\n";

            DiscordClient client = new DiscordClient(Program.botToken);

            foreach(string entry_string in Whitelist.white_list)
            {
                ulong entry = ulong.Parse(entry_string);
                string discriminator = "";
                for(int i=0; i < 4 - ((client.GetUser(entry).Discriminator)).ToString().Length; i++)
                {
                    discriminator += "0";
                }
                discriminator += client.GetUser(entry).Discriminator;
                var user_name = client.GetUser(entry).Username + "#" + discriminator;
                white_list += user_name + "\n";
            }
            if (white_list == "**Current whitelist:**\n")
                Message.Channel.SendMessage("**Current whitelist is empty**");
            else
                Message.Channel.SendMessage(white_list);

            string admin_list = "Current admins:\n";

            foreach (string entry_string in Admin.admins)
            {
                ulong entry = ulong.Parse(entry_string);
                string discriminator = "";
                for (int i = 0; i < 4 - ((client.GetUser(entry).Discriminator)).ToString().Length; i++)
                {
                    discriminator += "0";
                }
                discriminator += client.GetUser(entry).Discriminator;
                var user_name = client.GetUser(entry).Username + "#" + discriminator;
                white_list += user_name + "\n";
            }
            if (admin_list == "**Current admins:**\n")
                Message.Channel.SendMessage("**Current admin list is empty**");
            else
                Message.Channel.SendMessage(admin_list);
        }
    }
}
