using Discord.Gateway;
using System;
using System.Collections.Generic;

namespace Discord.Commands
{
    public abstract class CommandBase
    {
        public DiscordSocketClient Client { get; private set; }
        public DiscordMessage Message { get; private set; }

        internal void Prepare(DiscordSocketClient client, DiscordMessage message)
        {
            Client = client;
            Message = message;
        }
        public bool CanSendEmbed(VoiceChannel channel)
        {
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
        public abstract void Execute();
        public virtual void HandleError(string parameterName, string providedValue, Exception exception) { }
    }
}
