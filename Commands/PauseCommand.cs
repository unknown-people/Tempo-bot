
using Discord.Commands;

namespace Music_user_bot.Commands
{
    [Command("pause")]
    class PauseCommand : CommandBase
    {
        public override void Execute()
        {
            if (TrackQueue.isPaused)
            {
                Program.SendMessage(Message, "Current track is already paused");
            }
            else
            {
                TrackQueue.isPaused = true;
                Program.SendMessage(Message, "Paused current track");
            }
        }
    }
}
