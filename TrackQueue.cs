using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Gateway;
using Discord.Media;
using YoutubeExplode.Videos.Streams;
using System;
using YoutubeExplode;
using System.IO;

namespace Music_user_bot
{
    public class TrackQueue
    {
        public List<AudioTrack> Tracks { get; private set; }
        public bool Running { get; set; }
        public static string followSongId { get; set; }
        public static bool isLooping { get; set; }
        public Stream _stream { get; set; }

        private DiscordSocketClient _client;
        private ulong _guildId;

        public TrackQueue(DiscordSocketClient client, ulong guildId)
        {
            _client = client;
            _guildId = guildId;
            Tracks = new List<AudioTrack>();
            isLooping = false;
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

                    voiceClient.Microphone.CopyFrom(GetVideoUrl(currentSong.Id, currentChannel.Bitrate), 2, currentSong.CancellationTokenSource.Token, (int)duration.TotalSeconds);

                    try
                    {
                        if (isLooping)
                        {
                            var track = new AudioTrack(currentSong.Id);
                            Tracks.Add(track);
                        }
                    }
                    catch (Exception)
                    {
                        ;
                    }

                    Tracks.RemoveAt(0);
                }
                Running = false;
            });
        }
        public async ValueTask<bool> IsSleeping(List<AudioTrack> Tracks)
        {
            DateTime last_song = DateTime.Now;
            while (Tracks.Count < 1)
            {
                DateTime now = DateTime.Now;
                TimeSpan since_last_song = now.Subtract(last_song);
                if ((int)since_last_song.TotalSeconds > 300)
                {
                    return true;
                }
            }
            return false;
        }

        private string GetVideoUrl(string videoId, uint channelBitrate)
        {
            var manifest = Program.YouTubeClient.Videos.Streams.GetManifestAsync(videoId).Result;

            AudioOnlyStreamInfo bestStream = null;
            foreach (var stream in manifest.GetAudioOnlyStreams().OrderBy(s => s.Bitrate))
            {
                if (bestStream == null || stream.Bitrate > bestStream.Bitrate)
                {
                    bestStream = stream;

                    if (stream.Bitrate.BitsPerSecond > channelBitrate)
                        break;
                }
            }

            return bestStream.Url;
        }
    }
}
