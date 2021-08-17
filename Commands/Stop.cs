
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
    [Command("stop")]
    class StopCommand : CommandBase
    {
        public override void Execute()
        {
            TrackQueue list = Program.TrackLists[Message.Guild.Id];
            AudioTrack currentSong;
            try
            {
                currentSong = list.Tracks[0];
                Stop();
            }
            catch (IndexOutOfRangeException)
            {
                return;
            }
            currentSong.CancellationTokenSource.Cancel();
        }

        public void Stop()
        {
            Program.TrackLists[Message.Guild.Id] = new TrackQueue(Client, Message.Guild.Id);
        }
    }
}
