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
            if (!Program.TrackLists.TryGetValue(Message.Guild.Id, out var list)) list = Program.TrackLists[Message.Guild.Id] = new TrackQueue(Client, Message.Guild.Id);

            canSendEmbed = CanSendEmbed();

            if (canSendEmbed)
            {
                var embed = new EmbedMaker() { Title = Client.User.Username, TitleUrl = "https://discord.gg/DWP2AMTWdZ", Color = System.Drawing.Color.IndianRed, ThumbnailUrl = Client.User.Avatar.Url, Description = to_send };
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