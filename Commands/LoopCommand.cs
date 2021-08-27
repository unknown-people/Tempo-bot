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
                Program.SendMessage(Message, "Queue is now looping");
            }
            else
            {
                Program.SendMessage(Message, "Queue stopped looping");
            }
        }
    }
}
