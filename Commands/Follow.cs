using Discord.Commands;
using Discord;
using Discord.Gateway;
using Discord.Media;
using System;


namespace Music_user_bot {
    [Command("follow")]
    class FollowCommand : CommandBase
    {
        [Parameter("userId")]
        public static string userId { get; set; }
        public override void Execute()
        {
            if(userId.Length != 18)
            {
                Message.Channel.SendMessage("Usage: " + CommandHandler.Prefix + "follow [userId]");
            }
            else
            {
                try
                {
                    var voiceClient = Client.GetVoiceClient(Message.Guild.Id);
                }
                catch(Exception)
                {
                    Message.Channel.SendMessage("Be sure to use a valid user ID");
                }
            }
        }

        public async void FollowUser(string userId)
        {
            
        }
    }
}
