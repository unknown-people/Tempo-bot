using Discord;
using Discord.Commands;
using Discord.Gateway;

namespace Music_user_bot
{
    [Command("queue")]
    public class QueueCommand : CommandBase
    {
        public bool canSendEmbed { get; set; }
        public override void Execute()
        {
            var targetConnected = Client.GetVoiceStates(Message.Author.User.Id).GuildVoiceStates.TryGetValue(Message.Guild.Id, out var theirState);

            var channel = (VoiceChannel)Client.GetChannel(theirState.Channel.Id);

            if (!Program.TrackLists.TryGetValue(Message.Guild.Id, out var list)) list = Program.TrackLists[Message.Guild.Id] = new TrackQueue(Client, Message.Guild.Id);

            var embed = new EmbedMaker() { Title = "Current queue" };
            foreach (var song in list.Tracks)
                embed.AddField(song.Title, song.ChannelName + (song == list.Tracks[0] ? " *(Currently playing)*" : ""));

            var x = channel.PermissionOverwrites;

            foreach (var entry in channel.PermissionOverwrites)
            {
                if (entry.AffectedId == Message.Author.User.Id)
                {
                    canSendEmbed = entry.GetPermissionState(DiscordPermission.EmbedLinks) == OverwrittenPermissionState.Allow;
                }
            }
            if(canSendEmbed)
                Message.Channel.SendMessage(embed);
            else
            {
                string message = "**Current queue:**";
                int index = 1;
                foreach(var song in list.Tracks)
                {
                    message += "[" + index + "]" + song.Title + ";\n";
                    index += 1;
                }
                if (message == "**Current queue:**")
                    message = "**Current queue is empty**";
                Message.Channel.SendMessage(message);
            }
        }
    }
}