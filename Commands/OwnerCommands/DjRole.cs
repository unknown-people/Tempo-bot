using Discord.Commands;
using System;

namespace Music_user_bot.Commands
{
    [Command("djrole")]
    class SetDjRoleCommand : CommandBase
    {
        [Parameter("new dj role")]
        public string new_dj_role_id { get; private set; }
        public override void Execute()
        {
            try
            {
                if (!Program.isOwner(Message))
                {
                    Program.SendMessage(Message, "You need to be the owner to execute this command!");
                    return;
                }
                Settings.Default.Dj_role = ulong.Parse(new_dj_role_id);
                Program.SendMessage(Message, "prefix has been changed to: " + new_dj_role_id);
            }
            catch (Exception)
            {
                Program.SendMessage(Message, "Couldn't save new prefix. Try again");
            }
        }
    }
}

