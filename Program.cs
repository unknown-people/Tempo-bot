using System;
using System.Collections.Generic;
using System.Drawing;
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

        public static string ownerName { get; set; }

        public static string botToken { get; set; }

        public static bool CanModifyList(DiscordSocketClient client, DiscordMessage message)
        {
            var voiceClient = client.GetVoiceClient(message.Guild.Id);

            if (voiceClient.State != MediaConnectionState.Ready)
                message.Channel.SendMessage("I am not connected to a voice channel");
            else if (!client.GetVoiceStates(message.Author.User.Id).GuildVoiceStates.TryGetValue(message.Guild.Id, out var state) || state.Channel == null || state.Channel.Id != voiceClient.Channel.Id)
                message.Channel.SendMessage("You must be connected to the same voice channel as me to skip songs");
            else if (!TrackLists.TryGetValue(message.Guild.Id, out var queue) || queue.Tracks.Count == 0)
                message.Channel.SendMessage("The queue is empty");
            else return true;

            return false;
        }

        static void Main(string[] args)
        {
            var random = new string[] { };
            botToken = Settings.Default.Token;
            Whitelist.ownerID = Settings.Default.OwnerId;
            DiscordClient clientNew = new DiscordClient(botToken);
            ownerName = clientNew.GetUser(Whitelist.ownerID).Username + "#" + clientNew.GetUser(Whitelist.ownerID).Discriminator;

            DiscordSocketClient client = new DiscordSocketClient(new DiscordSocketConfig()
            {
                VoiceChannelConnectTimeout = 5000,
                HandleIncomingMediaData = false,
                Intents = DiscordGatewayIntent.Guilds | DiscordGatewayIntent.GuildMessages | DiscordGatewayIntent.GuildVoiceStates
            });

            client.CreateCommandHandler(Settings.Default.Prefix);
            client.OnLoggedIn += Client_OnLoggedIn;
            client.OnJoinedVoiceChannel += Client_OnJoinedVoiceChannel;
            client.Login(botToken);

            Whitelist whitelist = new Whitelist();

            while (true)
            {
                if (NoMuteCommand.noMute)
                {
                    NoMute(client);
                }
                Thread.Sleep(100);
            }
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
            client.User.ChangeProfile(new UserProfileUpdate()
            {
                Username = Settings.Default.Username,
                Password = Settings.Default.Password,
                //Avatar = Image.FromFile("propic.png"),
                Biography = "Current owner is " + ownerName + "\n" +
                    "Come check out Tempo user-bot!"
            });
            Whitelist.white_list = Settings.Default.WhiteList;
        }
    }
}