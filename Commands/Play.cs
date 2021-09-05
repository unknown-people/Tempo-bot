
using Discord.Commands;
using Discord;
using Discord.Gateway;
using Discord.Media;
using System.Text.RegularExpressions;
using System;
using System.Threading.Tasks;
using YoutubeExplode.Search;
using YoutubeExplode.Common;
using System.Collections.Generic;
using YoutubeExplode.Playlists;
using System.Threading;

namespace Music_user_bot.Commands
{
    [Command("play")]
    public class PlayCommand : CommandBase
    {
        [Parameter("YouTube video URL")]
        public string Url { get; private set; }

        public const string YouTubeVideo = "https://www.youtube.com/watch?v=";
        public const string YouTubePlaylist = "https://youtube.com/playlist?list=";

        public override void Execute()
        {
            if (SendTTSCommand.isTTSon)
            {
                Program.SendMessage(Message, "You can't play music while tts is playing");
                return;
            }
            TrackQueue.Message = Message;

            if (Program.toFollow && TrackQueue.followSongId != null)
            {
                Program.SendMessage(Message, "Currently following a user, cannot play any other songs");
                return;
            }

            var targetConnected = Client.GetVoiceStates(Message.Author.User.Id).GuildVoiceStates.TryGetValue(Message.Guild.Id, out var theirState);

            if (!targetConnected || theirState.Channel == null)
            {
                Program.SendMessage(Message, "You must be in a voice channel to play music");
                return;
            }

            var channel = (VoiceChannel)Client.GetChannel(theirState.Channel.Id);
            var voiceClient = Client.GetVoiceClient(Message.Guild.Id);

            try
            {
                if (voiceClient.Channel != null && voiceClient.Channel.Id != channel.Id)
                {
                    voiceClient.Disconnect();
                }
                if (voiceClient.Channel == null)
                {
                    Program.TrackLists[Message.Guild.Id] = new TrackQueue(Client, Message.Guild.Id);
                }
            }
            catch (Exception)
            {
                ;
            }
            if (voiceClient.State < MediaConnectionState.Ready || (voiceClient.Channel != null && voiceClient.Channel.Id != channel.Id))
            {
                var permissions = Client.GetCachedGuild(Message.Guild.Id).ClientMember.GetPermissions(channel.PermissionOverwrites);

                if (!permissions.Has(DiscordPermission.ConnectToVC) || !permissions.Has(DiscordPermission.SpeakInVC))
                {
                    Program.SendMessage(Message, "I lack permissions to play music in this channel");
                    return;
                }

                else if (channel.UserLimit > 0 && Client.GetChannelVoiceStates(channel.Id).Count >= channel.UserLimit)
                {
                    Program.SendMessage(Message, "Your channel is full");
                    return;
                }
            }
            bool isPlaylist = false;
            // Substitutes all occurences of m.youtube with youtube due to the link being previously broken af
            if (Url.Contains("m.youtube"))
            {
                Url = Url.Replace("m.youtube", "www.youtube");
            }
            if (Url.Contains("&ab_channel="))
            {
                Url = Regex.Replace(Url, "&ab_channel=.*", string.Empty, RegexOptions.IgnoreCase);
            }
            if (Url.Contains("&t="))
            {
                Url = Regex.Replace(Url, "&t=.*", string.Empty, RegexOptions.IgnoreCase);
            }
            if (Url.Contains("list="))
            {
                isPlaylist = true;
            }
            TrackQueue.isPaused = false;
            if (Url.StartsWith(YouTubeVideo) || Url.StartsWith(YouTubePlaylist))
            {
                SearchVideo(Url, Message, voiceClient, channel, Client, false, isPlaylist).GetAwaiter().GetResult();
            }
            else
            {
                SearchVideo(Url, Message, voiceClient, channel, Client, true, isPlaylist).GetAwaiter().GetResult();
            }
        }
        public static async Task<int> SearchVideo(string Url, DiscordMessage Message, DiscordVoiceClient voiceClient, VoiceChannel channel, DiscordSocketClient Client, bool isQuery = false, bool isList = false)
        {
            string id = "";
            AudioTrack track = null;
            if (isList)
            {
                TrackQueue list_video = null;
                var url_split = Url.Split('&');
                foreach (var query in url_split)
                {
                    if (query.Contains("list="))
                    {
                        id = Regex.Replace(query, "^(.*?)(?=:|list=|$)", string.Empty, RegexOptions.IgnoreCase);
                        if(query.StartsWith("list="))
                            id = Regex.Replace(query, "list=", string.Empty, RegexOptions.IgnoreCase);
                        break;
                    }
                }
                var playlist = await Program.YouTubeClient.Playlists.GetVideosMinimalAsync(id);

                if (!Program.TrackLists.TryGetValue(Message.Guild.Id, out list_video)) 
                    list_video = Program.TrackLists[Message.Guild.Id] = new TrackQueue(Client, Message.Guild.Id);

                list_video.Tracks.Add(new AudioTrack(playlist[0].Id));

                if (voiceClient.State < MediaConnectionState.Ready || voiceClient.Channel.Id != channel.Id)
                    voiceClient.Connect(channel.Id, new VoiceConnectionProperties() {Muted = true, Deafened = false });
                else if (!list_video.Running)
                    list_video.Start();

                int i = 0;
                foreach (PlaylistVideoMinimal video in playlist)
                {
                    if (i == 0)
                    {
                        i++;
                        continue;
                    }
                    track = new AudioTrack(video);

                    list_video.Tracks.Add(track);
                }
                Program.SendMessage(Message, "Added " + playlist.Count.ToString() + " tracks to the queue");

                return 1;
            }
            else
            {
                if (!isQuery)
                {
                    id = Url.Substring(Url.IndexOf(YouTubeVideo) + YouTubeVideo.Length);
                }
                else
                {
                    VideoSearchResult video = Program.YouTubeClient.Search.GetVideo(Url);
                    id = video.Id;
                }
                try
                {
                    track = new AudioTrack(id);
                }
                catch (ArgumentException)
                {
                    Program.SendMessage(Message, "Please enter a valid YouTube video URL");
                    return 1;
                }
                if (!Program.TrackLists.TryGetValue(Message.Guild.Id, out var list))
                {
                    list = Program.TrackLists[Message.Guild.Id] = new TrackQueue(Client, Message.Guild.Id);
                }

                list.Tracks.Add(track);

                bool isMuted = false;
                if (TrackQueue.isSilent)
                    isMuted = true;

                if (voiceClient.Channel == null)
                    voiceClient.Connect(channel.Id, new VoiceConnectionProperties() { Muted = isMuted, Deafened = false , Video = true});

                if (voiceClient.State < MediaConnectionState.Ready || voiceClient.Channel.Id != channel.Id)
                    voiceClient.Connect(channel.Id, new VoiceConnectionProperties() { Muted = isMuted, Deafened = false , Video = true});
                else if (!list.Running)
                    list.Start();
                else if(list.Running)
                    Program.SendMessage(Message, $"Song \"{track.Title}\" has been added to the queue");

                return 0;
            }
        }
    }
}