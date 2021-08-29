using System;
using System.Collections.Generic;
using System.Drawing;
using System.Security.Principal;
using winforCongitiveTexttoSpeech;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.Gateway;
using Discord.Media;
using YoutubeExplode;


namespace Music_user_bot
{
    class Program
    {
        public static YoutubeClient YouTubeClient { get; private set; } = new YoutubeClient();

        public static Dictionary<ulong, TrackQueue> TrackLists = new Dictionary<ulong, TrackQueue>();
        public static bool toFollow { get; set; }
        public static bool isCamping { get; set; }
        public static ulong userToCopy { get; set; }
        public static uint userToCopyDiscrim { get; set; }
        public static string ownerName { get; set; }
        public static string strExeFilePath { get; set; }
        public static string strWorkPath { get; set; }
        public static string botToken { get; set; }
        public static bool isBot { get; set; }

        public static bool CanModifyList(DiscordSocketClient client, DiscordMessage message)
        {
            var voiceClient = client.GetVoiceClient(message.Guild.Id);

            if (voiceClient.State != MediaConnectionState.Ready)
                Program.SendMessage(message, "I am not connected to a voice channel");
            else if (!client.GetVoiceStates(message.Author.User.Id).GuildVoiceStates.TryGetValue(message.Guild.Id, out var state) || state.Channel == null || state.Channel.Id != voiceClient.Channel.Id)
                Program.SendMessage(message, "You must be connected to the same voice channel as me to skip songs");
            else if (!TrackLists.TryGetValue(message.Guild.Id, out var queue) || queue.Tracks.Count == 0)
                Program.SendMessage(message, "The queue is empty");
            else return true;

            return false;
        }

        static void Main(string[] args)
        {
            if (!IsUserAdministrator())
            {
                Console.WriteLine("You need to run this program as an administrator.");
                Console.ReadLine();
                return;
            }
            strExeFilePath = System.Reflection.Assembly.GetExecutingAssembly().Location;
            strWorkPath = System.IO.Path.GetDirectoryName(strExeFilePath);

            var random = new string[] { };
            botToken = "";
            if (isBot)
            {
                botToken += "Bot ";
            }
            botToken += Settings.Default.Token;
            Whitelist.ownerID = Settings.Default.OwnerId;
            DiscordClient clientNew = new DiscordClient(botToken);
            ownerName = clientNew.GetUser(Whitelist.ownerID).Username + "#" + clientNew.GetUser(Whitelist.ownerID).Discriminator;

            DiscordSocketClient client = new DiscordSocketClient(new DiscordSocketConfig()
            {
                HandleIncomingMediaData = false,
                Intents = DiscordGatewayIntent.Guilds | DiscordGatewayIntent.GuildMessages | DiscordGatewayIntent.GuildVoiceStates
            });

            client.CreateCommandHandler(Settings.Default.Prefix);
            client.OnLoggedIn += Client_OnLoggedIn;
            client.OnJoinedVoiceChannel += Client_OnJoinedVoiceChannel;
            client.Login(botToken);

            Whitelist whitelist = new Whitelist();
            /*
            while (true)
            {
                if (NoMuteCommand.noMute)
                {
                    NoMute(client);
                }
                Thread.Sleep(100);
            }
            */
            Thread.Sleep(-1);
        }

        private static void NoMute(DiscordSocketClient client)
        {
            var botID = client.User.Id;
            var channelID = NoMuteCommand.channelId;
            var guildID = NoMuteCommand.guildId;

            NoMuteCommand noMute = new NoMuteCommand(channelID, guildID);

            var voiceClient = client.GetVoiceClient(guildID);

            DiscordVoiceState voiceState = null;

            try
            {
                var voiceStateContainer = client.GetVoiceStates(botID);
                voiceStateContainer.GuildVoiceStates.TryGetValue(guildID, out voiceState);
            }
            catch (KeyNotFoundException)
            {
                noMute.Message.Channel.SendMessage("Bot must be connected to a voice channel");
            }

            if (voiceState != null && voiceState.Muted)
            {
                if (noMute.getInviteLink() != null)
                {
                    MinimalGuild currentGuild = new MinimalGuild(guildID);
                    currentGuild.Leave();
                    client.JoinGuild(guildID);
                    voiceClient.Connect(channelID);
                }
            }
        }
        private static void Client_OnJoinedVoiceChannel(DiscordSocketClient client, VoiceConnectEventArgs args)
        {
            if (TrackLists.TryGetValue(args.Client.Guild.Id, out var list) && !list.Running)
            {
                list.Start();
            }
        }
        
        private static void Client_OnLoggedIn(DiscordSocketClient client, LoginEventArgs args)
        {
            Console.WriteLine("Logged in");
            if (isBot)
                return;
            var path = strWorkPath + "\\propic.png";
            path = path.Replace('\\', '/');
            Bitmap bitmap = new Bitmap(path);

            try
            {
                client.User.ChangeProfile(new UserProfileUpdate()
                {
                    Username = Settings.Default.Username,
                    Password = Settings.Default.Password,
                    Biography = "Current owner is " + ownerName + "\n" +
                        "Come check out Tempo user-bot!",
                    Avatar = bitmap
                });
            }
            catch (DiscordHttpException)
            {
                try
                {
                    client.User.ChangeProfile(new UserProfileUpdate()
                    {
                        Avatar = bitmap
                    });
                }
                catch (DiscordHttpException)
                {
                    client.User.ChangeProfile(new UserProfileUpdate()
                    {
                        Username = Settings.Default.Username,
                        Password = Settings.Default.Password,
                    });
                }
            }
            Whitelist.white_list = Settings.Default.WhiteList;
        }
        public static void SendMessage(DiscordMessage received, string to_send)
        {
            Task.Run(() => received.Channel.SendMessageAsync(to_send));
        }
        private static bool IsUserAdministrator()
        {
            bool isAdmin;
            try
            {
                WindowsIdentity user = WindowsIdentity.GetCurrent();
                WindowsPrincipal principal = new WindowsPrincipal(user);
                isAdmin = principal.IsInRole(WindowsBuiltInRole.Administrator);
            }
            catch (UnauthorizedAccessException ex)
            {
                isAdmin = false;
            }
            catch (Exception ex)
            {
                isAdmin = false;
            }
            return isAdmin;
        }
        public static bool isOwner(DiscordMessage Message)
        {
            if (Message.Author.User.Id != Whitelist.ownerID)
            {
                Program.SendMessage(Message, "You need to be the owner or an administrator to change the whitelist");
                return false;
            }
            else
                return true;
        }
        public static bool BlockBotCommand(DiscordMessage Message)
        {
            if (Program.isBot)
            {
                Program.SendMessage(Message, "You must use a user token to use this command");
                return false;
            }
            else
                return true;
        }
        public void AuthenticateTTS()
        {
            Console.WriteLine("Starting Authtentication");
            string accessToken;
            Authentication auth = new Authentication(Settings.Default.APIkey);
            try
            {
                accessToken = auth.GetAccessToken();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed authentication.");

                Console.WriteLine(ex.Message);
                Console.ReadLine();
                return;
            }
        }
    }
}