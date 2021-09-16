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

            if (!Program.TrackLists.TryGetValue(Message.Guild.Id, out var list)) list = Program.TrackLists[Message.Guild.Id] = new TrackQueue(Client, Message.Guild.Id);

            canSendEmbed = CanSendEmbed(theirState);

            if (canSendEmbed)
            {
                var embed = new EmbedMaker() { Title = Client.User.Username, Color = System.Drawing.Color.IndianRed, ThumbnailUrl = "http://unknown-people.it/icon_tempo.png" };
                try
                {
                    int index = 0;
                    if(list.Tracks.Count == 0)
                    {
                        embed.AddField("Current queue:", "Current queue is empty.\nUse " + Settings.Default.Prefix + "play(or p) [TITLE/URL] to play a song!");
                    }
                    foreach (var song in list.Tracks)
                    {
                        if (index >= 20)
                            break;
                        embed.AddField($"[{index + 1}]", song.Title);
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
                SendMessageAsync(message);
            }
        }
    }
}