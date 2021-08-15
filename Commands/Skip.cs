using Discord.Gateway;
using Discord.Commands;
using System;

namespace Music_user_bot
{
    [Command("skip")]
    public class SkipCommand : CommandBase
    {
        public override void Execute()
        {
            if (Program.CanModifyList(Client, Message))
            {
                var list = Program.TrackLists[Message.Guild.Id];
                AudioTrack currentSong;
                try
                {
                    currentSong = list.Tracks[0];
                }
                catch (IndexOutOfRangeException)
                {
                    Message.Channel.SendMessage("The queue is empty");
                    return;
                }
                currentSong.CancellationTokenSource.Cancel();

                Message.Channel.SendMessage("Skipped the current song.");
            }
        }
    }
}