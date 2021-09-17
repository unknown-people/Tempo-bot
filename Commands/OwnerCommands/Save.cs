﻿using Discord.Commands;
using System;

namespace Music_user_bot.Commands
{
    [Command("save")]
    class Save : CommandBase
    {
        public override void Execute()
        {
            try
            {
                if (!Program.isOwner(Message) || Program.BlockBotCommand(Message))
                {
                    SendMessageAsync("You need to be the owner to execute this command!");
                    return;
                }
                Settings.Default.WhiteList = Whitelist.white_list;
                Settings.Default.Admins = Admin.admins;
                Settings.Default.Save();
                Settings.Default.Reload();
                Program.SaveSettings();
                SendMessageAsync("All current settings have been saved");
            }
            catch (Exception)
            {
                SendMessageAsync("Couldn't save current settings. Try again");
            }
        }
    }
}
