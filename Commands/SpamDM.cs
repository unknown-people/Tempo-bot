
using Discord.Commands;
using System.Collections.Generic;
using Discord;
using Discord.Gateway;
using Discord.Media;
using System.Text.RegularExpressions;
using System;
using YoutubeExplode.Bridge;
using YoutubeExplode.Common;
using YoutubeExplode.Exceptions;
using YoutubeExplode.Utils.Extensions;
using System.Threading.Tasks;
using YoutubeExplode.Search;

namespace Music_user_bot
{
    [Command("spamdm")]
    class SpamDMCommand : CommandBase
    {
        [Parameter("userId")]
        public ulong userId { get; set; }

        public override void Execute()
        {
            if(userId.ToString().Length != 18)
            {
                Message.Channel.SendMessage("Insert a valid user id");
                return;
            }
            var dmChannel = Client.CreateDM(userId);
        }
    }
}
