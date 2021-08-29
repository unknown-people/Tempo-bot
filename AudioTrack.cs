using System.Threading;
using YoutubeExplode.Playlists;
using YoutubeExplode.Videos;

namespace Music_user_bot
{
    public class AudioTrack
    {
        public string Id { get; private set; }
        public string Title { get; private set; }

        public CancellationTokenSource CancellationTokenSource { get; private set; }

        public AudioTrack(string id)
        {
            Id = id;
            
            var video = Program.YouTubeClient.Videos.GetAsyncMinimal(Id);
            Title = video.Title;

            CancellationTokenSource = new CancellationTokenSource();
        }
        public AudioTrack(PlaylistVideoMinimal video)
        {
            Id = video.Id;

            Title = video.Title;

            CancellationTokenSource = new CancellationTokenSource();
        }
    }
}