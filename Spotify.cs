using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using SpotifyAPI;
using SpotifyAPI.Web;
using System.Collections.Generic;

namespace Music_user_bot
{
    class Spotify
    {
        public static SpotifyClient spotify {get;set;}

        public const string clientId = "7c8a47e9ab444aa59af1c4311ca992e7";
        public const string clientSecret = "96ce712677be4fc9b772cb8084368fbe";

        public static async Task Login()
        {
            var config = SpotifyClientConfig.CreateDefault();

            var request = new ClientCredentialsRequest(clientId, clientSecret);
            var response = await new OAuthClient(config).RequestToken(request);

            spotify = new SpotifyClient(config.WithToken(response.AccessToken));
        }
        public static string GetTrack(string track_id)
        {
            if (spotify == null)
                Login().GetAwaiter().GetResult();
            var tracks = spotify.Tracks;
            var track = tracks.Get(track_id).GetAwaiter().GetResult();
            var youtube_query = track.Artists[0].Name + " - " + track.Name;
            return youtube_query;
        }
        public static List<string> GetPlaylist(string playlist_id)
        {
            var playlist = new List<string>() { };
            var playlists = spotify.Playlists;

            var playlistGetItemsRequest = new PlaylistGetItemsRequest();
            playlistGetItemsRequest.Fields.Add("items(track(name,type))");
            var playlistItems = playlists.GetItems(playlist_id).GetAwaiter().GetResult();
            
            foreach(var track in playlistItems.Items)
            {
                var query = ((FullTrack)(track.Track)).Artists[0].Name + " - " + ((FullTrack)(track.Track)).Name;
                playlist.Add(query);
            }
            return playlist;
        }
    }
}
