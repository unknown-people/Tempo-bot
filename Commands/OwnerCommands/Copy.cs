
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
            if (!Program.isOwner(Message) || Program.BlockBotCommand(Message))
            {
                return;
            }
            if (userId.ToString().Length == 18)
            {
                Program.userToCopy = userId;
                Program.userToCopyDiscrim = Client.GetUser(Program.userToCopy).Discriminator;

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
                        Avatar = avatar_bitmap
                    });
                }
                catch (DiscordHttpException)
                {
                    try
                    {
                        Message.Channel.SendMessage("Could not change username");

                        Client.User.ChangeProfile(new UserProfileUpdate()
                        {
                            Avatar = avatar_bitmap
                        });
                    }
                    catch (DiscordHttpException)
                    {
                        try
                        {
                            Message.Channel.SendMessage("Could not change avatar");
                            Client.User.ChangeProfile(new UserProfileUpdate()
                            {
                                Username = username,
                                Password = Settings.Default.Password,
                            });
                        }
                        catch (DiscordHttpException)
                        {
                            ;
                        }
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