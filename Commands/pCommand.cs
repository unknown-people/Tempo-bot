using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Music_user_bot.Commands
{
    [Command("p")]
    class pCommand : CommandBase
    {
        [Parameter("Url")]
        public string Url { get; set; }
        public override void Execute()
        {
            ;
        }
    }
}
