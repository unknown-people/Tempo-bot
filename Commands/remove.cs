using Discord.Commands;


namespace Music_user_bot
{
    [Command("remove")]
    class RemoveCommand : CommandBase
    {
        [Parameter("song index")]
        public int song_index { get; set; }

        public override void Execute()
        {
            if (!Program.TrackLists.TryGetValue(Message.Guild.Id, out var list)) list = Program.TrackLists[Message.Guild.Id] = new TrackQueue(Client, Message.Guild.Id);
            if (song_index >= list.Tracks.Count)
            {
                Message.Channel.SendMessage("Specified index is not in the current queue");
            }
            else
            {
                list.Tracks.RemoveAt(song_index - 1);
                Message.Channel.SendMessage("Removed song: [" + (song_index).ToString() + "]" + list.Tracks[song_index - 1].Title);
            }
        }
    }
}
