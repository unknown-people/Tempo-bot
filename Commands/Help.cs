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

            var message = "**Current command list:**\n\n" +
                "help: prints this message\n" +
                "play/p: play a song in your current channel\n" +
                "join: make me join your current channel\n" +
                "leave: make me leave the current channel and delete the queue\n" +
                "stop: skip all songs in a queue\n" +
                "queue: display all current songs in the queue\n" +
                "remove: remove the specified song from queue\n" +
                "skip/n: skip a single song\n" +
                "loop: loop the current queue\n" +
                "wl: display the current whitelist\n" +
                "\n";
            if (Message.Author.User.Id == Settings.Default.OwnerId)
            {
                message += "**For owner and administrators only:**\n\n" +
                "addw: add the specified user to the whitelist by passing his id as argument\n" +
                "delw: remove the user from the whitelist\n" +
                "follow: follows a user\n" +
                "playfollow: song to play while following\n" +
                "spamdm: spams a message to a specified user / Usage: " + Settings.Default.Prefix +
                "spamdm [userId] [message]\n" +
                "stopspam: stops the spam\n" +
                "savewl: saves the whitelist permanently\n" +
                "prefix: changes the prefix";
            }
            dmChannel.SendMessage(message);
        }
    }
}
