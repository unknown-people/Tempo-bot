﻿using Discord.Gateway;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Discord.Commands
{
    public abstract class CommandBase
    {
        public DiscordSocketClient Client { get; private set; }
        public DiscordMessage Message { get; private set; }
        public static Dictionary<ulong, bool> isAdminDict { get; set; }
        public List<ulong> admin_roles { get; private set; }

        internal void Prepare(DiscordSocketClient client, DiscordMessage message)
        {
            Client = client;
            Message = message;
            foreach(var role in Client.GetCachedGuild(Message.Guild.Id).Roles)
            {
                if (role.Permissions == DiscordPermission.Administrator)
                {
                    admin_roles.Add(role.Id);
                }
            }
        }
        public bool CanSendEmbed(DiscordVoiceState theirState)
        {
            var channel = (VoiceChannel)Client.GetChannel(theirState.Channel.Id);

            if (channel.PermissionOverwrites.Count == 0)
                return true;

            if (isAdmin())
            {
                return true;
            }

            foreach (var entry in channel.PermissionOverwrites)
            {
                if (entry.AffectedId == Message.Author.User.Id)
                {
                    var result = entry.GetPermissionState(DiscordPermission.EmbedLinks) == OverwrittenPermissionState.Allow;
                    if (result)
                        return true;
                }
            }
            return false;
        }
        public bool isAdmin()
        {
            try
            {
                if (isAdminDict[Message.Guild.Id] == true)
                    return true;
                else
                    return false;
            }
            catch
            {
                foreach (var role in Client.GetCachedGuild(Message.Guild.Id).GetMember(Client.User.Id).Roles)
                {
                    foreach (var admin in admin_roles)
                    {
                        if (role == admin)
                        {
                            isAdminDict[Message.Guild.Id] = true;
                            return true;
                        }
                    }
                }

                isAdminDict[Message.Guild.Id] = false;
                return false;
            }
        }
        public void SendMessageAsync(string to_send)
        {
            Client.GetVoiceStates(Message.Author.User.Id).GuildVoiceStates.TryGetValue(Message.Guild.Id, out var theirState);

            try
            {
                if (CanSendEmbed(theirState))
                {
                    var embed = new EmbedMaker() { Title = Client.User.Username, TitleUrl = "https://discord.gg/DWP2AMTWdZ", Color = System.Drawing.Color.IndianRed, ThumbnailUrl = Client.User.Avatar.Url };
                    embed.AddField("", to_send);
                    Task.Run(() => Message.Channel.SendMessageAsync(embed));
                }
            }
            catch
            {
                CommandBase.isAdminDict[Message.Guild.Id] = false;

                if (CanSendEmbed(theirState))
                {
                    var embed = new EmbedMaker() { Title = Client.User.Username, TitleUrl = "https://discord.gg/DWP2AMTWdZ", Color = System.Drawing.Color.IndianRed, ThumbnailUrl = Client.User.Avatar.Url };
                    embed.AddField("", to_send);
                    Task.Run(() => Message.Channel.SendMessageAsync(embed));
                }
            }
            Task.Run(() => Message.Channel.SendMessageAsync(to_send));
        }
        public abstract void Execute();
        public virtual void HandleError(string parameterName, string providedValue, Exception exception) { }
    }
}
