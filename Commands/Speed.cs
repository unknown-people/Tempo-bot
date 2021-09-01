using Discord.Commands;

namespace Music_user_bot.Commands
{
    [Command("speed")]
    class SpeedCommand : CommandBase
    {
        [Parameter("value")]
        public string speed_string { get; set; }
        public override void Execute()
        {
            if (!Program.isOwner(Message) || Program.BlockBotCommand(Message))
            {
                return;
            }
            if (!float.TryParse(speed_string, out var speed))
                speed_string = speed_string.Replace(".", ",");

            if (speed > 2.0f)
                speed /= 10.0f;

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
