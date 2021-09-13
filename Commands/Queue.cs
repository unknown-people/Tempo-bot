using Discord;
using Discord.Commands;
using Discord.Gateway;
using System;

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

            canSendEmbed = CanSendEmbed(channel);

            if (canSendEmbed)
            {
                var embed = new EmbedMaker() { Title = "Current queue" };
                try
                {
                    int index = 0;
                    foreach (var song in list.Tracks)
                    {
                        if (index >= 30)
                            break;
                        embed.AddField(song.Title, (song == list.Tracks[0] ? " *(Currently playing)*" : ""));
                        index++;
                    }
                }
                catch (Exception) { }
                Message.Channel.SendMessage(embed);
            }
            else
            {
                string message = "**Current queue:**\n";
                int index = 1;
                foreach(var song in list.Tracks)
                {
                    if (index >= 30)
                        break;
                    message += "**[" + index + "]**" + song.Title + ";\n";
                    index += 1;
                }
                if (message == "**Current queue:**\n")
                    message = "**Current queue is empty**";
                Program.SendMessage(Message, message);
            }
        }
    }
}