using System;
using Discord.Commands;
using Discord;
using Discord.Gateway;

namespace Music_user_bot.Commands
{
    [Command("leave")]
    public class LeaveCommand : CommandBase
    {
        public override void Execute()
        {
            Client.GetVoiceStates(Message.Author.User.Id).GuildVoiceStates.TryGetValue(Message.Guild.Id, out var theirState);

            var channel = (VoiceChannel)Client.GetChannel(theirState.Channel.Id);
            var voiceClient = Client.GetVoiceClient(Message.Guild.Id);

            if (voiceClient.Channel.Id == channel.Id)
            {
                try
                {
                    voiceClient.Disconnect();
                }
                catch (Exception) { }
            }
        }
    }
}
