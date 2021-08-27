using Discord.Commands;


namespace Music_user_bot
{
    [Command("goto")]
    class GoToCommand : CommandBase
    {
        [Parameter("song index")]
        public int song_index { get; set; }

        public override void Execute()
        {
            if (Program.TrackLists.TryGetValue(Message.Guild.Id, out var list))
            {
                if (song_index < list.Tracks.Count)
                {
                    TrackQueue.goToIndex = song_index;
                    list.Tracks[0].CancellationTokenSource.Cancel();
                    Program.SendMessage(Message, "Skipped to track **[" + song_index.ToString() + "]**");
                }
                else
                    Program.SendMessage(Message, "Index out of range. See the current queue with **" + Settings.Default.Prefix + "queue**");
            }
            else
            {
                Program.SendMessage(Message, "There's no track to skip to");
            }
        }
    }
}
