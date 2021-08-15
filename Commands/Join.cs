using System;
using Discord.Commands;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Gateway;
using Discord.Media;
using System.Threading;
using YoutubeExplode.Videos.Streams;
using System.IO;
using System.Text.RegularExpressions;

namespace Music_user_bot.Commands
{
    [Command("join")]
    public class JoinCommand : CommandBase
    {
        public override void Execute()
        {
            Client.GetVoiceStates(Message.Author.User.Id).GuildVoiceStates.TryGetValue(Message.Guild.Id, out var theirState);

            var channel = (VoiceChannel)Client.GetChannel(theirState.Channel.Id);
            var voiceClient = Client.GetVoiceClient(Message.Guild.Id);

            if (voiceClient.State < MediaConnectionState.Ready || voiceClient.Channel.Id != channel.Id)
                voiceClient.Connect(channel.Id, new VoiceConnectionProperties() { Deafened = true });
        }
    }
}
