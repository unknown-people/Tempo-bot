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
                if (!Program.isOwner(Message))
                {
                    Program.SendMessage(Message, "You need to be the owner to execute this command!");
                    return;
                }
                Settings.Default.Prefix = new_prefix;
                CommandHandler.Prefix = new_prefix;
                Program.SendMessage(Message, "prefix has been changed to: " + new_prefix);
            }
            catch (Exception)
            {
                Program.SendMessage(Message, "Couldn't save new prefix. Try again");
            }
        }
    }
}
