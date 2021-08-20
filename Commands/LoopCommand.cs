using Discord.Commands;

namespace Music_user_bot.Commands
{
    [Command("loop")]
    class LoopCommand : CommandBase
    {
        public override void Execute()
        {
            TrackQueue.isLooping = !TrackQueue.isLooping;
            if (TrackQueue.isLooping)
            {
                Message.Channel.SendMessage("Queue is now looping");
            }
            else
            {
                Message.Channel.SendMessage("Queue stopped looping");
            }
        }
    }
}
