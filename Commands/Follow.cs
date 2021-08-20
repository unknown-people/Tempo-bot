using Discord.Commands;
using Discord;
using Discord.Gateway;
using Discord.Media;
using System;
using System.Threading.Tasks;
using System.Threading;

namespace Music_user_bot 
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
                Message.Channel.SendMessage("You must be the owner to use this command");
                return;
            }
            if(userId == 0 || userId.ToString().Length != 18)
            {
                Program.toFollow = false;
                TrackQueue.isLooping = false;

                Message.Channel.SendMessage("Not following anyone anymore.\n\n" +
                    "Usage: " + CommandHandler.Prefix + "follow [userId]");
            }
            else
            {
                Program.toFollow = true;

                Message.Channel.SendMessage("Now following <@" + userId.ToString() + ">");

                Task.Run(() =>
                {
                    try
                    {
                        FollowUser(userId, Message);
                    }
                    catch (Exception)
                    {
                        Message.Channel.SendMessage("Be sure to use a valid user ID");
                    }
                });
            }
        }

        private void FollowUser(ulong userId, DiscordMessage Message)
        {
            while (Program.toFollow)
            {
                try
                {
                    var voiceClient = Client.GetVoiceClient(Message.Guild.Id);
                    var targetConnected = Client.GetVoiceStates(userId).GuildVoiceStates.TryGetValue(Message.Guild.Id, out var theirState);
                    var channel = (VoiceChannel)Client.GetChannel(theirState.Channel.Id);

                    if (voiceClient.Channel == null || (voiceClient.State < MediaConnectionState.Ready && channel.Id != voiceClient.Channel.Id) || !Program.TrackLists.TryGetValue(Message.Guild.Id, out var list))
                    {
                        if (voiceClient.Channel != null && channel.Id != voiceClient.Channel.Id)
                        {
                            voiceClient.Disconnect();
                        }
                        AudioTrack track = null;

                        while (channel.UserLimit > 0 && Client.GetChannelVoiceStates(channel.Id).Count >= channel.UserLimit)
                        {
                            Thread.Sleep(100);
                        };
                        voiceClient.Connect(channel.Id, new VoiceConnectionProperties() { Deafened = true });

                        if (!Program.TrackLists.TryGetValue(Message.Guild.Id, out list)) 
                            list = Program.TrackLists[Message.Guild.Id] = new TrackQueue(Client, Message.Guild.Id);
                        TrackQueue.isLooping = true;

                        track = new AudioTrack(TrackQueue.followSongId);

                        list.Tracks.Add(track);

                        if (!list.Running)
                            list.Start();
                    }
                }
                catch (Exception)
                {
                    continue;
                }
            }
        }
    }
}
