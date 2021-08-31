using Discord.Commands;
using System;

namespace Music_user_bot.Commands
{
    [Command("savewl")]
    class SaveWL : CommandBase
    {
        public override void Execute()
        {
            try
            {
                if (!Program.isOwner(Message) || Program.BlockBotCommand(Message))
                {
                    return;
                }
                Settings.Default.WhiteList = Whitelist.white_list;
                Settings.Default.Save();
                Settings.Default.Reload();
                Program.SendMessage(Message, "Whitelist has been saved");
            }
            catch (Exception)
            {
                Program.SendMessage(Message, "Couldn't save whitelist. Try again");
            }
        }
    }
}
