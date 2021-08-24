
using Discord.Commands;
using Discord;
using Discord.Gateway;
using System;
using System.Threading.Tasks;
using System.Threading;

namespace Music_user_bot.Commands
{
    [Command("speak")]
    class SendTTSCommand : CommandBase
    {
        [Parameter("message to tts")]
        public string message { get; set; }

        public override void Execute()
        {
            
        }
    }
}
