using Discord.Commands;
using System;

namespace Music_user_bot.Commands
{
    [Command("prefix")]
    class ChangePrefix : CommandBase
    {
        [Parameter("new prefix")]
        public string new_prefix { get; private set; }
        public override void Execute()
        {
            try
            {
                if(Message.Author.User.Id != Settings.Default.OwnerId)
                {
                    Message.Channel.SendMessage("You must be the owner to use this command");
                    return;
                }
                Settings.Default.Prefix = new_prefix;
                Message.Channel.SendMessage("prefix has been changed to: " + new_prefix);
            }
            catch (Exception)
            {
                Message.Channel.SendMessage("Couldn't save new prefix. Try again");
            }
        }
    }
}
