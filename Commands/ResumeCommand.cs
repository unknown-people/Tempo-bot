using Discord.Commands;

namespace Music_user_bot.Commands
{
    [Command("resume")]
    class ResumeCommand : CommandBase
    {
        public override void Execute()
        {
            if (TrackQueue.isPaused)
            {
                TrackQueue.isPaused = false;
                Program.SendMessage(Message, "Track resumed");
            }
            else
            {
                Program.SendMessage(Message, "Track is already playing");
            }
        }
    }
}
