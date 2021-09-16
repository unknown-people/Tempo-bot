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
                try
                {
                    TrackQueue.currentSong.CancellationTokenSource.Cancel();
                }
                catch (IndexOutOfRangeException)
                {
                    SendMessageAsync("The queue is empty");
                    return;
                }
            }
        }
    }
}