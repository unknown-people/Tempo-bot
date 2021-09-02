using Discord.Commands;
using Discord.Gateway;

namespace Music_user_bot.Commands
{
    [Command("speed")]
    class SpeedCommand : CommandBase
    {
        [Parameter("value")]
        public string speed_string { get; set; }
        public override void Execute()
        {
            var voiceClient = Client.GetVoiceClient(Message.Guild.Id);
            var targetConnected = Client.GetVoiceStates(Message.Author.User.Id).GuildVoiceStates.TryGetValue(Message.Guild.Id, out var theirState);

            if (!targetConnected || theirState.Channel == null)
            {
                Program.SendMessage(Message, "You must be in a voice channel to play music");
                return;
            }
            float speed = 0.0f;
            if (speed_string.Contains("."))
            {
                speed = float.Parse(speed_string.Split('.')[1]);
            }
            else if (speed_string.Contains(","))
            {
                speed = float.Parse(speed_string.Split(',')[1]);
            }

            speed /= 10.0f;
            if (speed_string.Contains(".0"))
            {
                speed = float.Parse(speed_string.Split('.')[0]);
            }
            if (speed_string.Contains(",0"))
            {
                speed = float.Parse(speed_string.Split(',')[0]);
            }

            if (speed > 0.0f && speed <= 2.0f)
            {
                TrackQueue.speed = speed;
                TrackQueue.speedChanged = true;
                Program.SendMessage(Message, "Playback speed is now " + TrackQueue.speed.ToString().Replace(",", ".") + "x");
            }
            else
            {
                Program.SendMessage(Message, "You must use a value between 0 and 2.0");
            }
        }
    }
}
