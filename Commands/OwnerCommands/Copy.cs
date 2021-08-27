
using Discord;
using Discord.Commands;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace Music_user_bot.Commands
{
    [Command("copy")]
    class CopyCommand : CommandBase
    {
        [Parameter("userId")]
        public ulong userId { get; set; }
        public override void Execute()
        {
            if(Message.Author.User.Id != Settings.Default.OwnerId)
            {
                Program.SendMessage(Message, "You must be the owner to use this command");
                return;
            }
            if(userId.ToString().Length == 18)
            {
                Program.userToCopy = userId;
                Program.userToCopyName = Client.GetUser(Program.userToCopy).Username + "#" + Client.GetUser(Program.userToCopy).Discriminator;

                var avatar = Client.GetUser(userId).Avatar;
                var username = Client.GetUser(userId).Username;
                var guild_username = Message.Guild.GetMember(userId).Nickname;
                var path = Program.strWorkPath + "\\avatar_new.png";

                path = path.Replace('\\', '/');
                Bitmap avatar_bitmap = null;
                try
                {
                    avatar_bitmap = SaveImage(avatar.Url, ImageFormat.Png);
                }
                catch (ExternalException)
                {
                    ;
                }
                catch (ArgumentNullException)
                {
                    ;
                }

                try
                {
                    Client.User.ChangeProfile(new UserProfileUpdate()
                    {
                        Username = username,
                        Password = Settings.Default.Password,
                        Biography = "Current owner is " + Program.ownerName + "\n" +
                        "Come check out Tempo user-bot!",
                    });
                }
                catch (DiscordHttpException)
                {
                    try
                    {
                        Message.Channel.SendMessage("Could not change username");

                        Client.User.ChangeProfile(new UserProfileUpdate()
                        {
                            Biography = "Current owner is " + Program.ownerName + "\n" +
                            "Come check out Tempo user-bot!",
                            Avatar = avatar_bitmap
                        });
                    }
                    catch (DiscordHttpException)
                    {
                        Message.Channel.SendMessage("Could not change avatar");
                    }
                }
                try
                {
                    Message.Guild.SetNickname(guild_username);
                }
                catch (DiscordHttpException)
                {
                    Program.SendMessage(Message, "Could not change guild username");
                }
                Program.SendMessage(Message, "Now copying <@" + Program.userToCopy + ">");
            }
        }

        public Bitmap SaveImage(string imageUrl, ImageFormat format)
        {
            WebClient client = new WebClient();
            Stream stream = client.OpenRead(imageUrl);
            Bitmap bitmap; bitmap = new Bitmap(stream);

            stream.Flush();
            stream.Close();
            client.Dispose();
            return bitmap;
        }
    }
}