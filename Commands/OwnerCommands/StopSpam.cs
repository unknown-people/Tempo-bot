using Discord.Commands;
using Discord;

namespace Music_user_bot.Commands
{
    [Command("stopspam")]
    class StopSpamCommand : CommandBase
    {
        public static bool stopSpam { get; set; }
        public override void Execute()
        {
            if (SpamDMCommand.isSpamming == false)
            {
                Client.CreateDM(Message.Author.User.Id).SendMessage("Not spamming to anyone yet.");
                return;
            }
            stopSpam = true;
            if(stopSpam)
            {
                Client.CreateDM(Message.Author.User.Id).SendMessage("Stopped spamming");
            }
        }
    }
}
