using Discord;
using Discord.Commands;

namespace Music_user_bot.Commands
{
    [Command("help")]
    class HelpCommand : CommandBase
    {
        public override void Execute()
        {
            var dmChannel = Client.CreateDM(Message.Author.User.Id);

            dmChannel.SendMessage("**Current command list:**\n\n" +
                "help: prints this message\n" +
                "play/p: used to play a song in your current channel\n" +
                "join: used to make me join your current channel\n" +
                "leave: used to make me leave the current channel and delete the queue\n" +
                "stop: used to skip all songs in a queue\n" +
                "queue: used to display all current songs in the queue\n" +
                "skip/n: used to skip a single song\n" +
                "loop: used to loop the current queue\n" +
                "wl: display the current whitelist\n" +
                "\n" +
                "**For owner and administrators only:**\n\n" +
                "addw: add the specified user to the whitelist by passing his id as argument\n" +
                "delw: remove the user from the whitelist\n" +
                "follow: follows a user\n" +
                "playfollow: song to play while following");
        }
    }
}
