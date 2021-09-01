
using Discord;
using Discord.Commands;
using System.Drawing;
using System.IO;

namespace Music_user_bot.Commands
{
    [Command("stopcopy")]
    class StopCopyCommand : CommandBase
    {
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

            Message.Guild.SetNickname(Settings.Default.Username);

            if (Program.userToCopy != 0)
                Program.SendMessage(Message, "Stopped copying <@" + Program.userToCopy + ">");
            else
            {
                Program.SendMessage(Message, "Not copying anyone yet");
            }
            Program.userToCopy = 0;
            var path = Program.strWorkPath + "\\propic.png";
            path = path.Replace('\\', '/');
            Bitmap bitmap = new Bitmap(path);
            try
            {
                Client.User.ChangeProfile(new UserProfileUpdate()
                {
                    Avatar = bitmap,
                    Username = Settings.Default.Username,
                    Password = Settings.Default.Password,
                    Biography = "Current owner is " + Program.ownerName + "\n" +
                    "Come check out Tempo user-bot!"
                });
            }
            catch (DiscordHttpException)
            {
                try
                {
                    Client.User.ChangeProfile(new UserProfileUpdate()
                    {
                        Username = Settings.Default.Username,
                        Password = Settings.Default.Password,
                        Biography = "Current owner is " + Program.ownerName + "\n" +
                        "Come check out Tempo user-bot!"
                    });
                }
                catch (DiscordHttpException)
                {
                    Client.User.ChangeProfile(new UserProfileUpdate()
                    {
                        Avatar = bitmap
                    });
                }
            }
        }
    }
}