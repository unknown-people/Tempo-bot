using Discord.Commands;
using Discord;

namespace Music_user_bot.Commands
{
    [Command("stopspamdm")]
    class StopSpamDmCommand : CommandBase
    {
        public static bool stopSpamDm { get; set; }
        public override void Execute()
        {
            if (!Program.isOwner(Message) && !Program.isAdmin(Message))
            {
                SendMessageAsync("You need to be the owner or an administrator to execute this command!");
                return;
            }
            stopSpamDm = true;
            if(stopSpamDm)
            {
                Client.CreateDM(Message.Author.User.Id).SendMessage("Stopped spamming dm");
            }
        }
    }
}
