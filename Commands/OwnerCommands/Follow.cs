﻿using Discord.Commands;
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
            if (!Program.isOwner(Message))
            {
                return;
            }
            TrackQueue.Message = Message;

            if (userId == 0 || userId.ToString().Length != 18)
            {
                Program.toFollow = false;
                TrackQueue.isLooping = false;
                TrackQueue.followSongId = null;

                SendMessageAsync("Not following anyone anymore.\n\n" +
                    "Usage: " + CommandHandler.Prefix + "follow [userId]");
            }
            else
            {
                Program.toFollow = true;

                SendMessageAsync("Now following <@" + userId.ToString() + ">");

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
                        var permissions = Client.GetCachedGuild(Message.Guild.Id).ClientMember.GetPermissions(channel.PermissionOverwrites);

                        if (voiceClient.Channel == null)
                        {
                            if (!Program.TrackLists.TryGetValue(Message.Guild.Id, out var list))
                                Program.TrackLists[Message.Guild.Id] = new TrackQueue(Client, Message.Guild.Id);

                            bool isMuted = false;
                            if (TrackQueue.isSilent)
                                isMuted = true;
                            voiceClient.Connect(channel.Id, new Discord.Media.VoiceConnectionProperties {Muted = isMuted, Deafened= false});
                            already_searched = false;
                        }
                        if (voiceClient.Channel.Id != channel.Id || !targetConnected)
                        {
                            voiceClient.Disconnect();
                            already_searched = false;
                            continue;
                        }
                        if (!permissions.Has(DiscordPermission.ConnectToVC) || !permissions.Has(DiscordPermission.SpeakInVC))
                        {
                            Thread.Sleep(100);
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
                            list.Tracks.Add(to_loop.Title);
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
                SendMessageAsync("Be sure to use a valid user ID");
            }
        }
    }
}
