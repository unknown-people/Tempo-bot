using Discord.Commands;
using Discord;
using Discord.Gateway;
using System;
using System.Threading.Tasks;
using System.Threading;

namespace Music_user_bot.Commands
{
    [Command("follow")]
    class FollowCommand : CommandBase
    {
        [Parameter("userId", optional: true)]
        public ulong userId { get; set; }
        public override void Execute()
        {
            if(Message.Author.User.Id != Whitelist.ownerID)
            {
                Program.SendMessage(Message, "You must be the owner to use this command");
                return;
            }
            if(userId == 0 || userId.ToString().Length != 18)
            {
                Program.toFollow = false;
                TrackQueue.isLooping = false;
                TrackQueue.followSongId = null;

                Program.SendMessage(Message, "Not following anyone anymore.\n\n" +
                    "Usage: " + CommandHandler.Prefix + "follow [userId]");
            }
            else
            {
                Program.toFollow = true;

                Program.SendMessage(Message, "Now following <@" + userId.ToString() + ">");

                Thread follow = new Thread(() => FollowUser(userId, Message));
                follow.Start();
            }
        }

        private void FollowUser(ulong userId, DiscordMessage Message)
        {
            try
            {
                bool already_searched = false;
                AudioTrack to_loop = null;
                while (Program.toFollow)
                {
                    try
                    {
                        var voiceClient = Client.GetVoiceClient(Message.Guild.Id);
                        var targetConnected = Client.GetVoiceStates(userId).GuildVoiceStates.TryGetValue(Message.Guild.Id, out var theirState);
                        var channel = (VoiceChannel)Client.GetChannel(theirState.Channel.Id);

                        if (voiceClient.Channel == null)
                        {
                            if (!Program.TrackLists.TryGetValue(Message.Guild.Id, out var list))
                                Program.TrackLists[Message.Guild.Id] = new TrackQueue(Client, Message.Guild.Id);
                            voiceClient.Connect(channel.Id);
                            already_searched = false;
                        }
                        if (voiceClient.Channel.Id != channel.Id || !targetConnected)
                        {
                            voiceClient.Disconnect();
                            already_searched = false;
                            continue;
                        }
                        while (channel.UserLimit > 0 && Client.GetChannelVoiceStates(channel.Id).Count >= channel.UserLimit)
                        {
                            Thread.Sleep(100);
                            if (Client.GetChannelVoiceStates(channel.Id).Count <= channel.UserLimit)
                                throw new InvalidOperationException("Channel is full");
                        };
                        if (TrackQueue.followSongId != null && !already_searched)
                        {
                            if (!Program.TrackLists.TryGetValue(Message.Guild.Id, out var list))
                                list = Program.TrackLists[Message.Guild.Id] = new TrackQueue(Client, Message.Guild.Id);
                            TrackQueue.isLooping = true;
                            if (to_loop == null)
                                to_loop = new AudioTrack(TrackQueue.followSongId);
                            list.Tracks.Add(to_loop);
                            if (!list.Running)
                            {
                                list.Start();
                                already_searched = true;
                            }
                        }
                    }
                    catch (Exception)
                    {
                        continue;
                    }
                }
            }
            catch (Exception)
            {
                Program.SendMessage(Message, "Be sure to use a valid user ID");
            }
        }
    }
}
