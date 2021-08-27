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
                    Program.SendMessage(Message, "The queue is empty");
                    return;
                }
                currentSong.CancellationTokenSource.Cancel();

                Program.SendMessage(Message, "Skipped the current song.");
            }
        }
    }
}