﻿using Discord;
using Discord.Commands;

namespace Music_user_bot.Commands
{
    [Command("help")]
    class HelpCommand : CommandBase
    {
        public override void Execute()
        {
            var dmChannel = Client.CreateDM(Message.Author.User.Id);

            var message = "**Current command list:**\n\n" +
                "help: prints this message\n" +
                "play/p: play a song in your current channel\n" +
                "join: make me join your current channel\n" +
                "leave: make me leave the current channel and delete the queue\n" +
                "stop: skip all songs in a queue\n" +
                "queue: display all current songs in the queue\n" +
                "remove: remove the specified song from queue\n" +
                "skip/n: skip a single song\n" +
                "loop: loop the current queue\n" +
                "wl: display the current whitelist\n" +
                "goto: skips to the specified song in the queue\n" +
                "seek: seeks the current song to the specified time\n" +
                "ff: skips the specified amount of seconds\n" +
                "say/s: says the specified message with tts\n" +
                "\n";
            if (Message.Author.User.Id == Settings.Default.OwnerId)
            {
                message += "**For owner and administrators only:**\n\n" +
                "addw: add the specified user to the whitelist by passing his id as argument[User token only]\n" +
                "delw: remove the user from the whitelist[User token only]\n" +
                "adda: add the specified user to the admin list[Bot token only]\n" +
                "dela: remove the user from the admin list[Bot token only]\n" +
                "follow: follows a user\n" +
                "playfollow: song to play while following\n" +
                "spamdm: spams a message to a specified user / Usage: " + Settings.Default.Prefix +
                "spamdm [userId] [message]\n" +
                "stopspam: stops the spam\n" +
                "savewl: saves the whitelist permanently\n" +
                "prefix: changes the prefix\n" +
                "camp: camps the specified channel, same as follow but with a channel id\n" +
                "joinserver: joins the specified server by passing an invite code or link as parameter[User token only]\n" +
                "djrole: specify the dj role id [Bot token only]\n" +
                "silence: silences the bot ;)\n" +
                "changename: changes the default name the bot will have on each login\n";
            }
            dmChannel.SendMessage(message);
        }
    }
}
