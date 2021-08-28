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
using System.Threading;

namespace Music_user_bot
{
    public class TrackQueue
    {
        public List<AudioTrack> Tracks { get; private set; }
        public bool Running { get; set; }
        public static DiscordMessage Message { get; set; }
        public static int seekTo { get; set; }
        public static int FFseconds { get; internal set; }
        public static string followSongId { get; set; }
        public static int goToIndex { get; set; }
        public static bool isLooping { get; set; }
        public static bool isPaused { get; set; }
        public static DateTime pauseTime { get; set; }
        public static int pauseTimeSec { get; set; }
        public DateTime start_time { get; set; }
        public DiscordMessage last_message { get; set; }
        public Stream _stream { get; set; }
        public static int stream_volume { get; set; }
        public static AudioTrack currentSong { get; set; }

        private DiscordSocketClient _client;
        private ulong _guildId;

        public TrackQueue(DiscordSocketClient client, ulong guildId)
        {
            _client = client;
            _guildId = guildId;
            Tracks = new List<AudioTrack>();
            isLooping = false;
            stream_volume = 100;
        }

        public void Start()
        {
            Running = true;

            Thread track_queue = new Thread(async () =>
            {
                FFseconds = 0;
                seekTo = 0;

                var voiceClient = _client.GetVoiceClient(_guildId);

                last_message = null;

                while (voiceClient.State == MediaConnectionState.Ready && Tracks.Count > 0)
                {
                    if (Program.TrackLists.TryGetValue(_guildId, out var list) && goToIndex > 0)
                    {
                        for (int i = 0; i <= list.Tracks.Count; i++)
                        {
                            list.Tracks.RemoveAt(0);
                        }
                        goToIndex = 0;
                    }

                    currentSong = Tracks[0];

                    VoiceChannel currentChannel = (VoiceChannel)_client.GetChannel(voiceClient.Channel.Id);

                    var youtube = new YoutubeClient();

                    var video = await youtube.Videos.GetAsync(currentSong.Id);

                    TimeSpan duration = TimeSpan.Zero;
                    if (video.Duration != null)
                    {
                        duration = (TimeSpan)video.Duration;
                    }

                    if (last_message != null)
                        last_message.Delete();
                    last_message = Message.Channel.SendMessage("**Now playing:**\n" + video.Title + "\n**By:**    " + video.Author);

                    start_time = DateTime.Now;
                    pauseTimeSec = 0;
                    string url = GetVideoUrl(currentSong.Id, currentChannel.Bitrate);
                    while (voiceClient.Microphone.CopyFrom( url, (int)duration.TotalSeconds, currentSong.CancellationTokenSource.Token))
                    {
                        pauseTime = DateTime.Now;
                        while (isPaused)
                        {
                            await Task.Delay(10);
                        }
                        pauseTimeSec += (int)(pauseTime - start_time).TotalSeconds;
                        start_time = DateTime.Now;
                        FFseconds = 0;
                        seekTo = 0;
                    }

                    try
                    {
                        if (isLooping)
                        {
                            Tracks.Add(new AudioTrack(video.Id));
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
            track_queue.Start();
        }
        public static TimeSpan StringToTimeSpan(string input)
        {
            var string_split = input.Split(':');
            var result = "";
            var return_value = TimeSpan.Zero;
            if(string_split.Length == 2)
            {
                var buffer = new string[] { "00" };
                buffer = buffer.Concat(string_split).ToArray();
                result = string.Join(":", buffer);
                return_value = TimeSpan.Parse(result);
            }
            else if(string_split.Length == 3)
            {
                return_value = TimeSpan.Parse(result);
            }
            else if(string_split.Length == 1)
            {
                var buffer0 = new string[] { "00", "00" };
                var buffer = buffer0.Concat(string_split).ToArray();
                result = string.Join(":", buffer);
                return_value = TimeSpan.Parse(result);
            }
            return return_value;
        }
        public async ValueTask<bool> IsSleeping(bool running)
        {
            DateTime last_song = DateTime.Now;
            while (!running)
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
