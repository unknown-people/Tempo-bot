using Discord;
using Discord.Commands;
using Discord.Gateway;
using Discord.Media;
using System;

namespace Music_user_bot.Commands
{
    [Command("silence")]
    class SilenceCommand : CommandBase
    {
        public override void Execute()
        {
            if (!Program.isOwner(Message))
            {
                return;
            }

            TrackQueue.isSilent = !TrackQueue.isSilent;

            var voiceClient = Client.GetVoiceClient(Message.Guild.Id);
            var targetConnected = Client.GetVoiceStates(Message.Author.User.Id).GuildVoiceStates.TryGetValue(Message.Guild.Id, out var theirState);

            if (!targetConnected || theirState.Channel == null)
            {
                Program.SendMessage(Message, "You must be in a voice channel to play music");
                return;
            }

            var channel = (VoiceChannel)Client.GetChannel(theirState.Channel.Id);

            try
            {
                if (TrackQueue.isSilent)
                {
                    Program.SendMessage(Message, "You are now in silent mode ;)");

                    voiceClient.Disconnect();
                    voiceClient.Connect(channel.Id, new VoiceConnectionProperties() { Muted = true, Deafened = false });
                }
                else
                {
                    Program.SendMessage(Message, "Silent mode stopped");

                    voiceClient.Disconnect();
                    voiceClient.Connect(channel.Id, new VoiceConnectionProperties() { Muted = false, Deafened = false });
                }
            }
            catch (Exception)
            {
                ;
            }
        }
    }
}
