
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
            if (Message.Author.User.Id != Settings.Default.OwnerId)
            {
                Program.SendMessage(Message, "You must be the owner to use this command");
                return;
            }

            Message.Guild.SetNickname(Settings.Default.Username);

            if (Program.userToCopy != 0)
                Program.SendMessage(Message, "Stopped copying <@" + Program.userToCopy + ">");
            else
            {
                Program.SendMessage(Message, "Not copying anyone yet");
                return;
            }
            Program.userToCopy = 0;
            try
            {
                Client.User.ChangeProfile(new Discord.UserProfileUpdate()
                {
                    Avatar = Image.FromFile(Program.strWorkPath + "\\propic.png"),
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
                    Client.User.ChangeProfile(new Discord.UserProfileUpdate()
                    {
                        Username = Settings.Default.Username,
                        Password = Settings.Default.Password,
                        Biography = "Current owner is " + Program.ownerName + "\n" +
                        "Come check out Tempo user-bot!"
                    });
                }
                catch (DiscordHttpException)
                {
                    ;
                }
            }
        }
    }
}