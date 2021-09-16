﻿using Discord.Commands;
using System.Text.RegularExpressions;
using System;
using YoutubeExplode.Search;

namespace Music_user_bot
{
    [Command("playfollow")]
    class PlayFollow : CommandBase
    {
        [Parameter("songUrl")]
        public string Url { get; private set; }

        public const string YouTubeVideo = "https://www.youtube.com/watch?v=";

        public override void Execute()
        {
            if (!Program.isOwner(Message))
            {
                SendMessageAsync("You need to be the owner to execute this command!");
                return;
            }
            TrackQueue.Message = Message;
            if (!Program.isOwner(Message))
            {
                return;
            }
            if (!Program.toFollow)
            {
                SendMessageAsync("You need to be following someone to use this command");
                return;
            }
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
            if (Url.Contains("&t="))
            {
                Url = Regex.Replace(Url, "&t=.*", string.Empty, RegexOptions.IgnoreCase);
            }
            if (Url.StartsWith(YouTubeVideo))
                TrackQueue.followSongId = Url.Substring(Url.IndexOf(YouTubeVideo) + YouTubeVideo.Length);
            else
            {
                VideoSearchResult video = Program.YouTubeClient.Search.GetVideo(Url);
                TrackQueue.followSongId = video.Id;
            }
            try
            {
                var track = new AudioTrack(TrackQueue.followSongId);
                TrackQueue.isLooping = true;

                if (!Program.TrackLists.TryGetValue(Message.Guild.Id, out var list)) list = Program.TrackLists[Message.Guild.Id] = new TrackQueue(Client, Message.Guild.Id);
                list.Tracks.Add(track);
                SendMessageAsync("Now playing " + track.Title);
            }
            catch (Exception) {
                SendMessageAsync("Couldn't play the selected track");
            }
        }
    }
}
