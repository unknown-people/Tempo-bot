using Discord.Commands;
using Discord;
using Discord.Gateway;
using Discord.Media;
using System;

namespace Music_user_bot.Commands
{
    [Command("join")]
    public class JoinCommand : CommandBase
    {
        public override void Execute()
        {
            Client.GetVoiceStates(Message.Author.User.Id).GuildVoiceStates.TryGetValue(Message.Guild.Id, out var theirState);

            try
            {
                var channel = (VoiceChannel)Client.GetChannel(theirState.Channel.Id);
                var voiceClient = Client.GetVoiceClient(Message.Guild.Id);

                if (voiceClient.State < MediaConnectionState.Ready || voiceClient.Channel.Id != channel.Id)
                    voiceClient.Connect(channel.Id);
            }
            catch (Exception)
            {
                Program.SendMessage(Message, "You need to be in a voice channel for me to join you :tired_face:");
            }
        }
    }
}
