
using Discord.Commands;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Gateway;
using Discord.Media;
using System.Threading;
using YoutubeExplode.Videos.Streams;
using System.IO;
using System.Text.RegularExpressions;
using System;

namespace Music_user_bot
{
    [Command("play")]
    public class PlayCommand : CommandBase
    {
        [Parameter("YouTube video URL")]
        public string Url { get; private set; }

        public override void Execute()
        {
            const string YouTubeVideo = "https://www.youtube.com/watch?v=";

            var targetConnected = Client.GetVoiceStates(Message.Author.User.Id).GuildVoiceStates.TryGetValue(Message.Guild.Id, out var theirState);

            if (!targetConnected || theirState.Channel == null)
            {
                Message.Channel.SendMessage("You must be in a voice channel to play music");
                return;
            }

            var channel = (VoiceChannel)Client.GetChannel(theirState.Channel.Id);
            var voiceClient = Client.GetVoiceClient(Message.Guild.Id);

            List<AudioTrack> empty_list = new List<AudioTrack> { };

            try
            {
                if (voiceClient.Channel != null && voiceClient.Channel.Id != channel.Id)
                {
                    voiceClient.Disconnect();
                }
            }
            catch (Exception)
            {
                ;
            }
            if (voiceClient.State < MediaConnectionState.Ready || voiceClient.Channel.Id != channel.Id)
            {
                var permissions = Client.GetCachedGuild(Message.Guild.Id).ClientMember.GetPermissions(channel.PermissionOverwrites);

                if (!permissions.Has(DiscordPermission.ConnectToVC) || !permissions.Has(DiscordPermission.SpeakInVC))
                {
                    Message.Channel.SendMessage("I lack permissions to play music in this channel");
                    return;
                }

                else if (channel.UserLimit > 0 && Client.GetChannelVoiceStates(channel.Id).Count >= channel.UserLimit)
                {
                    Message.Channel.SendMessage("Your channel is full");
                    return;
                }
            }
            // Substitutes all occurences of m.youtube with youtube due to the link being previously broken af
            if (Url.Contains("m.youtube"))
            {
                Url = Url.Replace("m.youtube", "www.youtube");
            }
            // Fixes url taken from playlists to fit in the next if statement
            if (Url.Contains("&list="))
            {
                Url = Regex.Replace(Url, "&list=.*", string.Empty, RegexOptions.IgnoreCase);
            }
            if (Url.Contains("&ab_channel="))
            {
                Url = Regex.Replace(Url, "&ab_channel=.*", string.Empty, RegexOptions.IgnoreCase);
            }
            if (Url.StartsWith(YouTubeVideo))
            {
                string id = Url.Substring(Url.IndexOf(YouTubeVideo) + YouTubeVideo.Length); // lazy

                AudioTrack track = null;
                try
                {
                    track = new AudioTrack(id);
                }
                catch (ArgumentException)
                {
                    Message.Channel.SendMessage("Please enter a valid YouTube video URL");
                    return;
                }
                if (!Program.TrackLists.TryGetValue(Message.Guild.Id, out var list)) list = Program.TrackLists[Message.Guild.Id] = new TrackQueue(Client, Message.Guild.Id);

                list.Tracks.Add(track);

                Message.Channel.SendMessage($"Song \"{track.Title}\" has been added to the queue");

                if (voiceClient.State < MediaConnectionState.Ready || voiceClient.Channel.Id != channel.Id)
                    voiceClient.Connect(channel.Id, new VoiceConnectionProperties() { Deafened = true });
                else if (!list.Running)
                    list.Start();
            }
            else Message.Channel.SendMessage("Please enter a valid YouTube video URL");
        }
    }
}