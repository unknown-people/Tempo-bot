using Discord.Commands;
using System;

namespace Music_user_bot.Commands
{
    [Command("earrape")]
    class EarrapeCommand : CommandBase
    {
        public override void Execute()
        {
            if (!Program.isOwner(Message) || Program.BlockBotCommand(Message))
            {
                return;
            }
            TrackQueue.isEarrape = !TrackQueue.isEarrape;
            if (TrackQueue.isEarrape)
            {
                Program.SendMessage(Message, "You are now in earrape mode");
            }
            else
            {
                Program.SendMessage(Message, "Earrape mode stopped");
            }
        }
    }
}
