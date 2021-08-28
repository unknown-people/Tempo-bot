
using Discord.Commands;
using System;

namespace Music_user_bot.Commands
{
    [Command("stop")]
    class StopCommand : CommandBase
    {
        public override void Execute()
        {
            TrackQueue list = Program.TrackLists[Message.Guild.Id];
            AudioTrack currentSong = TrackQueue.currentSong;
            try
            {
                currentSong.CancellationTokenSource.Cancel();
                Stop();
            }
            catch (IndexOutOfRangeException)
            {
                Message.Channel.SendMessage("Queue is already empty");
                return;
            }
        }

        public void Stop()
        {
            Program.TrackLists[Message.Guild.Id] = new TrackQueue(Client, Message.Guild.Id);
        }
    }
}
