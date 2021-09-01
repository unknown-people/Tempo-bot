﻿using Discord.Commands;

namespace Music_user_bot.Commands
{
    [Command("voicetts")]
    class ChangeVoiceTTSCommand : CommandBase
    {
        [Parameter("voice", true)]
        public string voice { get; set; }

        public override void Execute()
        {
            if (!Program.isOwner(Message) && !Program.isAdmin(Message))
            {
                Program.SendMessage(Message, "You need to be the owner or an administrator to execute this command!");
                return;
            }
            if (voice == null)
            {
                Program.SendMessage(Message, "Currently supported voices:\n**it-IT** - Diego / Isabella\n" + "**en-US** - Cristopher / Michelle");
                return;
            }
            switch (voice.ToLower())
            {
                case "diego":
                    Settings.Default.TTSlang = "it-IT";
                    Settings.Default.TTSvoice = "it-IT-DiegoNeural";
                    Program.SendMessage(Message, "Current voice set to Diego - IT");
                    break;
                case "isabella":
                    Settings.Default.TTSlang = "it-IT";
                    Settings.Default.TTSvoice = "it-IT-IsabellaNeural";
                    Program.SendMessage(Message, "Current voice set to Isabella - IT");
                    break;
                case "michelle":
                    Settings.Default.TTSlang = "en-US";
                    Settings.Default.TTSvoice = "en-US-MichelleNeural";
                    Program.SendMessage(Message, "Current voice set to Michelle - EN");
                    break;
                case "christopher":
                    Settings.Default.TTSlang = "en-US";
                    Settings.Default.TTSvoice = "en-US-ChristopherNeural";
                    Program.SendMessage(Message, "Current voice set to Christopher - EN");
                    break;
            }
        }
    }
}