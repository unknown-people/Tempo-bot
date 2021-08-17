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
using YoutubeExplode;

namespace Music_user_bot
{
    public class TrackQueue
    {
        public List<AudioTrack> Tracks { get; private set; }
        public bool Running { get; private set; }

        private DiscordSocketClient _client;
        private ulong _guildId;

        public TrackQueue(DiscordSocketClient client, ulong guildId)
        {
            _client = client;
            _guildId = guildId;
            Tracks = new List<AudioTrack>();
        }

        public void Start()
        {
            Running = true;

            Task.Run(async () =>
            {
                var voiceClient = _client.GetVoiceClient(_guildId);

                while (voiceClient.State == MediaConnectionState.Ready && Tracks.Count > 0)
                {

                    var currentSong = Tracks[0];

                    var manifest = Program.YouTubeClient.Videos.Streams.GetManifestAsync(currentSong.Id).Result;

                    VoiceChannel currentChannel = (VoiceChannel)_client.GetChannel(voiceClient.Channel.Id);

                    var youtube = new YoutubeClient();

                    var video = await youtube.Videos.GetAsync(currentSong.Id);

                    TimeSpan duration = TimeSpan.Zero;
                    if (video.Duration != null)
                    {
                        duration = (TimeSpan)video.Duration;
                    }

                    voiceClient.Microphone.CopyFrom(DiscordVoiceUtils.GetAudio(GetVideoUrl(currentSong.Id, currentChannel.Bitrate)), 0, currentSong.CancellationTokenSource.Token, (int)duration.TotalSeconds);
                    Tracks.RemoveAt(0);
                }
                Running = false;
            });
        }

        private string GetVideoUrl(string videoId, uint channelBitrate)
        {
            var manifest = Program.YouTubeClient.Videos.Streams.GetManifestAsync(videoId).Result;

            AudioOnlyStreamInfo bestStream = null;
            foreach (var stream in manifest.GetAudioOnlyStreams().OrderBy(s => s.Bitrate))
            {
                if (bestStream == null || stream.Bitrate > bestStream.Bitrate)
                {
                    if (stream.AudioCodec == "opus")
                        bestStream = stream;

                    if (stream.Bitrate.BitsPerSecond > channelBitrate)
                        break;
                }
            }

            return bestStream.Url;
        }
    }
}
