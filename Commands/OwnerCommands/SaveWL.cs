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
                if(Message.Author.User.Id != Settings.Default.OwnerId)
                {
                    Program.SendMessage(Message, "You must be the owner to use this command");
                    return;
                }
                Settings.Default.WhiteList = Whitelist.white_list;
                Settings.Default.Save();
                Program.SendMessage(Message, "Whitelist has been saved");
            }
            catch (Exception)
            {
                Program.SendMessage(Message, "Couldn't save whitelist. Try again");
            }
        }
    }
}
