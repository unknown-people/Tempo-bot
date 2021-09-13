using Discord.Commands;

namespace Music_user_bot.Commands
{
    [Command("volume")]
    class VolumeCommand : CommandBase
    {
        [Parameter("volume level")]
        public int volume { get; set; }
        public override void Execute()
        {
            if(volume > 0 && volume <= 200)
            {
                TrackQueue.stream_volume = volume;
                TrackQueue.isVolumeChanged = true;
                Program.SendMessage(Message, "Current volume is now set to " + volume.ToString() + "%");
            }
            else
            {
                Program.SendMessage(Message, "Please insert a value between 1 and 200");
            }
        }
    }
}
