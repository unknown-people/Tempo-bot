using Discord.Commands;
using System;

namespace Music_user_bot.Commands
{
    [Command("earrape")]
    class EarrapeCommand : CommandBase
    {
        public override void Execute()
        {
            if (!Program.isOwner(Message) && !Program.isAdmin(Message))
            {
                Program.SendMessage(Message, "You need to be the owner or an administrator to execute this command!");
                return;
            }

            TrackQueue.isEarrape = !TrackQueue.isEarrape;
            TrackQueue.earrapeChanged = true;
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
