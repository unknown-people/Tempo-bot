using Discord;
using Discord.Commands;

namespace Music_user_bot.Commands
{
    [Command("changename")]
    class ChangeNameCommand : CommandBase
    {
        [Parameter("new name")]
        public string new_name { get; set; }
        public override void Execute()
        {
            if (!Program.isOwner(Message))
            {
                Program.SendMessage(Message, "You need to be the owner to execute this command!");
                return;
            }
            if (Program.BlockBotCommand(Message))
            {
                Program.SendMessage(Message, "You need to use a user token to execute this command!");
                return;
            }
            try
            {
                Client.User.ChangeProfile(new UserProfileUpdate()
                {
                    Username = new_name,
                    Password = Settings.Default.Password
                });
                Program.SendMessage(Message, "Name correctly set");
            }
            catch (DiscordHttpException)
            {
                Program.SendMessage(Message, "Couldn't set name, try restarting the program and check the password, or you may have been rate limited, so please try again later");
            }
        }
    }
}
