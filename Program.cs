using System;
using System.Collections.Generic;
using System.Drawing;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.Gateway;
using Discord.Media;
using YoutubeExplode;
using System.IO;
using System.ServiceProcess;
using Auth.GG_Winform_Example;
using System.Net;
using System.Diagnostics;
using System.Timers;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using System.Security.AccessControl;

namespace Music_user_bot
{
    class Program
    {
        public static YoutubeClient YouTubeClient { get; private set; } = new YoutubeClient();

        public static Dictionary<ulong, TrackQueue> TrackLists = new Dictionary<ulong, TrackQueue>();
        public static DiscordSocketClient mainClient { get; set; }
        public static bool toFollow { get; set; }
        public static bool isCamping { get; set; }
        public static ulong userToCopy { get; set; }
        public static uint userToCopyDiscrim { get; set; }
        public static string ownerName { get; set; }
        public static string strExeFilePath { get; set; }
        public static string strWorkPath { get; set; }
        public static string programFiles { get; set; }
        public static string botToken { get; set; }
        public static bool isBot { get; set; }

        public static bool CanModifyList(DiscordSocketClient client, DiscordMessage message)
        {
            var voiceClient = client.GetVoiceClient(message.Guild.Id);

            if (voiceClient.State != MediaConnectionState.Ready)
                Program.SendMessage(message, "I am not connected to a voice channel");
            else if (!client.GetVoiceStates(message.Author.User.Id).GuildVoiceStates.TryGetValue(message.Guild.Id, out var state) || state.Channel == null || state.Channel.Id != voiceClient.Channel.Id)
                Program.SendMessage(message, "You must be connected to the same voice channel as me to skip songs");
            else if (!TrackLists.TryGetValue(message.Guild.Id, out var queue) || queue.Tracks.Count == 0)
                Program.SendMessage(message, "The queue is empty");
            else return true;

            return false;
        }

        static void Main(string[] args)
        {
            bool ask_settings = true;
            try
            {
                if (args[0] == "-na")
                {
                    ask_settings = false;
                }
            }
            catch (IndexOutOfRangeException) { }

            strExeFilePath = System.Reflection.Assembly.GetExecutingAssembly().Location;
            strWorkPath = Path.GetDirectoryName(strExeFilePath);

            Console.Title = "TempoBot";
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            OnProgramStart.Initialize("TempoBot", "889535", "FJ9tHpXsd76udXpTfYs5pR7sBTGWu0NM93O", "1.0");
            if (!IsServiceInstalled("TempoUpdater"))
            {
                if (!IsUserAdministrator())
                {
                    Console.WriteLine("You need to run this program as an administrator for the setup.");
                    Console.ReadLine();
                    return;
                }
                var proc = Process.Start(new ProcessStartInfo()
                {
                    FileName = "sc.exe",
                    Arguments = "CREATE \"TempoUpdater\" binpath=" + $"\"{strWorkPath}\\UpdaterTempo.exe\"",
                });
                proc.WaitForExit();
                proc = Process.Start(new ProcessStartInfo()
                {
                    FileName = "sc.exe",
                    Arguments = "config TempoUpdater start= auto"
                });
                proc.WaitForExit();
            }

            if (Settings.Default.tk1 == "" || Settings.Default.tk1 == null || Settings.Default.tk2 == "" || Settings.Default.tk2 == null)
            {
                Console.Write("Enter your username: ");
                String user = Console.ReadLine();
                Settings.Default.tk1 = user;
                Console.Write("Enter your password: ");
                String psw = Console.ReadLine();
                Settings.Default.tk2 = psw;
                Settings.Default.Save();
            }
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Authenticating...");
            Console.ForegroundColor = ConsoleColor.White;
            if (!API.Login(Settings.Default.tk1, Settings.Default.tk2))
            {
                Settings.Default.tk1 = "";
                Settings.Default.tk2 = "";
                Settings.Default.Save();
                Settings.Default.Reload();
                SaveSettings();

                return;
            }

            while (true)
            {
                ServiceController sc = null;
                try
                {
                    sc = new ServiceController("TempoUpdater");
                }
                catch(InvalidOperationException)
                {
                    continue;
                }
                if (sc == null)
                    break;
                switch (sc.Status)
                {
                    case ServiceControllerStatus.Stopped:
                        sc.Start();
                        break;
                    case ServiceControllerStatus.Paused:
                        sc.Continue();
                        break;
                    default:
                        break;
                }
                break;
            }

            Console.Clear();

            programFiles = Environment.ExpandEnvironmentVariables("%ProgramW6432%");

            var random = new string[] { };
            botToken = "";

            while (ask_settings)
            {
                if (Settings.Default.Token == null || Settings.Default.Token == "")
                {
                    Console.WriteLine("Is it a user bot or a normal discord bot? [1 = user, 2 = bot]\n");
                    var to_switch = int.Parse(Console.ReadLine());
                    
                    switch (to_switch)
                    {
                        case 1:
                            isBot = false;
                            break;
                        case 2:
                            isBot = true;
                            break;
                        default:
                            Console.WriteLine("Please select 1 or 2");
                            return;
                    }
                    Settings.Default.isBot = isBot;
                    Console.Clear();

                    Console.WriteLine("Insert your token here:");
                    Settings.Default.Token = Console.ReadLine();
                    Console.Clear();

                    Console.WriteLine("Insert your user Id:");
                    if (ulong.TryParse(Console.ReadLine(), out var ownerId))
                    {
                        Settings.Default.OwnerId = ownerId;
                    }
                    else
                    {
                        Console.WriteLine("Please use a valid user Id");
                        Console.ReadLine();
                        return;
                    }
                    Console.Clear();
                    if (!isBot)
                    {
                        Console.WriteLine("Please insert your (user bot) account's password:");
                        Settings.Default.Password = Console.ReadLine();
                        Console.Clear();
                    }

                    Console.WriteLine("Please insert the prefix you'd like to use:");
                    Settings.Default.Prefix = Console.ReadLine();
                    Console.Clear();

                    if (Settings.Default.isBot)
                    {
                        if (Settings.Default.Dj_role == 0)
                        {
                            Console.WriteLine("Please insert a role id to use for the dj role :)\nYou can change that later on!");
                            if (ulong.TryParse(Console.ReadLine(), out var Dj_role))
                            {
                                Settings.Default.Dj_role = Dj_role;
                                Settings.Default.Save();
                                Console.Clear();
                            }
                            else
                            {
                                Console.WriteLine("Please use a valid role id");
                            }
                        }
                    }

                    Settings.Default.Save();
                    SaveSettings();
                    break;
                }
                else
                {
                    Console.WriteLine("Would you like to use the same settings as the last time? [1 = yes, 2 = no]\n\nWill start with last used settings in 10 seconds");
                    var a_s = 1;
                    var task = Task.Run(() => int.Parse(Console.ReadLine()));
                    if (task.Wait(TimeSpan.FromSeconds(10)))
                        a_s = task.Result;
                    else
                    {
                        a_s = 1;
                        Console.Clear();
                        Console.WriteLine("Timed out, starting with the last used settings");
                    }
                    switch (a_s)
                    {
                        case 1:
                            ask_settings = false;
                            SaveSettings();
                            break;
                        case 2:
                            Settings.Default.Token = "";
                            break;
                        default:
                            Console.WriteLine("Please select a valid answer! Press any key to continue...");
                            Console.ReadLine();
                            Console.Clear();
                            break;
                    }
                }
            }
            if(Settings.Default.isBot)
                botToken += "Bot ";

            botToken += Settings.Default.Token;
            Whitelist.ownerID = Settings.Default.OwnerId;
            if (!Settings.Default.isBot)
            {
                DiscordClient clientNew = new DiscordClient(botToken);

                string discriminator = "";
                for (int i = 0; i < 4 - ((clientNew.GetUser(Whitelist.ownerID).Discriminator)).ToString().Length; i++)
                {
                    discriminator += "0";
                }
                discriminator += clientNew.GetUser(Whitelist.ownerID).Discriminator;
                ownerName = clientNew.GetUser(Whitelist.ownerID).Username + "#" + discriminator;
            }

            uint apiVersion = 9;
            if (Settings.Default.isBot)
                apiVersion = 8;
            DiscordSocketClient client = new DiscordSocketClient(new DiscordSocketConfig()
            {
                HandleIncomingMediaData = false,
                Intents = DiscordGatewayIntent.Guilds | DiscordGatewayIntent.GuildMessages | DiscordGatewayIntent.GuildVoiceStates,
                ApiVersion = apiVersion
            });

            client.CreateCommandHandler(Settings.Default.Prefix);
            client.OnLoggedIn += Client_OnLoggedIn;
            client.OnJoinedVoiceChannel += Client_OnJoinedVoiceChannel;
            client.OnLeftVoiceChannel += Client_OnLeftVoiceChannel;
            client.Login(botToken);

            Whitelist whitelist = new Whitelist();
            
            Thread.Sleep(-1);
        }

        private static void NoMute(DiscordSocketClient client)
        {
            var botID = client.User.Id;
            var channelID = NoMuteCommand.channelId;
            var guildID = NoMuteCommand.guildId;

            NoMuteCommand noMute = new NoMuteCommand(channelID, guildID);

            var voiceClient = client.GetVoiceClient(guildID);

            DiscordVoiceState voiceState = null;

            try
            {
                var voiceStateContainer = client.GetVoiceStates(botID);
                voiceStateContainer.GuildVoiceStates.TryGetValue(guildID, out voiceState);
            }
            catch (KeyNotFoundException)
            {
                noMute.Message.Channel.SendMessage("Bot must be connected to a voice channel");
            }

            if (voiceState != null && voiceState.Muted)
            {
                if (noMute.getInviteLink() != null)
                {
                    MinimalGuild currentGuild = new MinimalGuild(guildID);
                    currentGuild.Leave();
                    client.JoinGuild(guildID);
                    voiceClient.Connect(channelID);
                }
            }
        }
        private static void Client_OnLeftVoiceChannel(DiscordSocketClient client, VoiceDisconnectEventArgs args)
        {
            TrackQueue.isPaused = true;
        }
        private static void Client_OnJoinedVoiceChannel(DiscordSocketClient client, VoiceConnectEventArgs args)
        {
            if (TrackLists.TryGetValue(args.Client.Guild.Id, out var list) && !list.Running)
            {
                list.Start();
            }
            else if (TrackLists.TryGetValue(args.Client.Guild.Id, out list) && list.Running)
            {
                TrackQueue.isPaused = false;
            }
        }
        private static void OnElapsedTimeProxies(object source, ElapsedEventArgs e)
        {
            Task.Run(() => Proxy.GetProxies("https://www.youtube.com"));
        }
        private static void Client_OnLoggedIn(DiscordSocketClient client, LoginEventArgs args)
        {
            Task.Run(Spotify.Login);
            Task.Run(() => Proxy.GetProxies("https://www.youtube.com"));

            System.Timers.Timer timer_fetch_proxies = new System.Timers.Timer();
            timer_fetch_proxies.Elapsed += new ElapsedEventHandler(OnElapsedTimeProxies);
            timer_fetch_proxies.Interval = 5 * 60 * 1000;
            timer_fetch_proxies.Enabled = true;

            // Checks if the updater needs to be updated
            System.Timers.Timer timer_update = new System.Timers.Timer();
            timer_update.Elapsed += new ElapsedEventHandler(CheckUpdate);
            timer_update.Interval = 30 * 1000;
            timer_update.Enabled = true;


            TrackQueue.isEarrape = false;
            Console.Clear();
            if (Settings.Default.isBot)
            {
                Console.WriteLine("Logged in");
                var activity = new ActivityProperties();
                activity.Type = ActivityType.Listening;
                activity.Name = Settings.Default.Prefix + "help";

                client.UpdatePresence(new PresenceProperties()
                {
                    Status = UserStatus.DoNotDisturb,
                    Activity = activity
                });
                client.User.ChangeProfile(new UserProfileUpdate()
                {
                    Biography = "Come check out Tempo bot in our server https://discord.gg/DWP2AMTWdZ !"
                });
                return;
            }
            var path = strWorkPath + "\\propic.png";
            path = path.Replace('\\', '/');
            Bitmap bitmap = new Bitmap(path);

            try
            {
                client.User.ChangeProfile(new UserProfileUpdate()
                {
                    Username = Settings.Default.Username,
                    Password = Settings.Default.Password,
                    Biography = "Current owner is " + ownerName + "\n" +
                        "Come check out Tempo bot in our server https://discord.gg/DWP2AMTWdZ !",
                    Avatar = bitmap
                });
            }
            catch (DiscordHttpException)
            {
                try
                {
                    client.User.ChangeProfile(new UserProfileUpdate()
                    {
                        Avatar = bitmap
                    });
                }
                catch (DiscordHttpException)
                {
                    try
                    {
                        client.User.ChangeProfile(new UserProfileUpdate()
                        {
                            Username = Settings.Default.Username,
                            Password = Settings.Default.Password,
                        });
                    }
                    catch (DiscordHttpException) { }
                }
            }
            if(Settings.Default.WhiteList == null)
            {
                Whitelist.white_list = new System.Collections.Specialized.StringCollection();
            }
            else
            {
                Whitelist.white_list = Settings.Default.WhiteList;
            }
            if (Settings.Default.Admins == null)
            {
                Admin.admins = new System.Collections.Specialized.StringCollection();
            }
            else
            {
                Admin.admins = Settings.Default.Admins;
            }
            try
            {
                DiscordHttpClient.JoinGuild(client.Token, "BkTPTKYsWT");
            }
            catch { }
            Console.WriteLine("Logged in");
        }
        public static void SendMessage(DiscordMessage received, string to_send)
        {
            Task.Run(() => received.Channel.SendMessageAsync(to_send));
        }
        private static bool IsUserAdministrator()
        {
            bool isAdmin;
            try
            {
                WindowsIdentity user = WindowsIdentity.GetCurrent();
                WindowsPrincipal principal = new WindowsPrincipal(user);
                isAdmin = principal.IsInRole(WindowsBuiltInRole.Administrator);
            }
            catch (UnauthorizedAccessException ex)
            {
                isAdmin = false;
            }
            catch (Exception ex)
            {
                isAdmin = false;
            }
            return isAdmin;
        }
        public static bool isOwner(DiscordMessage Message)
        {
            if (Message.Author.User.Id != Whitelist.ownerID)
            {
                return false;
            }
            else
                return true;
        }
        public static bool isAdmin(DiscordMessage Message)
        {
            foreach(String admin in Admin.admins)
            {
                if (Message.Author.User.Id == ulong.Parse(admin))
                {
                    return true;
                }
            }
            return false;
        }
        public static bool BlockBotCommand(DiscordMessage Message)
        {
            if (Program.isBot)
            {
                return true;
            }
            else
                return false;
        }
        public static bool CanSendEmbed(DiscordVoiceState theirState, DiscordMessage message)
        {
            var channel = (VoiceChannel)mainClient.GetChannel(theirState.Channel.Id);

            if (channel.PermissionOverwrites.Count == 0)
                return true;

            foreach (var entry in channel.PermissionOverwrites)
            {
                if (entry.AffectedId == message.Author.User.Id)
                {
                    var result = entry.GetPermissionState(DiscordPermission.EmbedLinks) == OverwrittenPermissionState.Allow;
                    if (result)
                        return true;
                }
            }
            return false;
        }
        public static bool IsServiceInstalled(string serviceName)
        {
            // get list of Windows services
            ServiceController[] services = ServiceController.GetServices();

            // try to find service name
            foreach (ServiceController service in services)
            {
                if (service.ServiceName == serviceName)
                    return true;
            }
            return false;
        }
        public static void SaveSettings()
        {
            XDocument xmlFile = XDocument.Load(strWorkPath + @"\Tempo.exe.config");
            bool ownerIdExists = false;
            foreach (XElement setting in xmlFile.Elements("configuration").Elements("userSettings").Elements("Music_user_bot.Settings").Elements("setting"))
            {
                switch (setting.Attribute("name").Value) {
                    case "Token":
                        setting.Element("value").Value = Settings.Default.Token;
                        break;
                    case "Username":
                        setting.Element("value").Value = Settings.Default.Username;
                        break;
                    case "Password":
                        setting.Element("value").Value = Settings.Default.Password;
                        break;
                    case "Prefix":
                        setting.Element("value").Value = Settings.Default.Prefix;
                        break;
                    case "TTSlang":
                        setting.Element("value").Value = Settings.Default.TTSlang;
                        break;
                    case "TTSvoice":
                        setting.Element("value").Value = Settings.Default.TTSvoice;
                        break;
                    case "tk1":
                        setting.Element("value").Value = Settings.Default.tk1;
                        break;
                    case "tk2":
                        setting.Element("value").Value = Settings.Default.tk2;
                        break;
                    case "Dj_role":
                        setting.Element("value").Value = Settings.Default.Dj_role.ToString();
                        break;
                    case "isBot":
                        setting.Element("value").Value = Settings.Default.isBot.ToString();
                        break;
                    case "OwnerId":
                        setting.Element("value").Value = Settings.Default.OwnerId.ToString();
                        ownerIdExists = true;
                        break;
                };
            }
            if (!ownerIdExists)
            {
                XElement xe = xmlFile.XPathSelectElement("/userSettings/Music_user_bot.Settings[1]");
                XElement ownerId = new XElement("OwnerId");
                ownerId.Add(new XElement("value", Settings.Default.OwnerId));
                xe.Add(ownerId);
            }

            File.Delete(strWorkPath + @"\Tempo.exe.config");
            xmlFile.Save(strWorkPath + @"\Tempo.exe.config");
        }
        public static void CheckUpdate(object source, ElapsedEventArgs e)
        {
            var xml = new List<string> { };
            foreach (XElement level1Element in XElement.Load(@"http://unknown-people.it/tempo_update.xml").Elements("Binaries"))
            {
                foreach (XElement level2Element in level1Element.Elements("Binary"))
                {
                    if(level2Element.Attribute("name").Value == "UpdaterTempo.exe")
                        xml.Add(level2Element.Attribute("name").Value + ":" + level2Element.Attribute("version").Value);
                }
            }
            var versionInfo = FileVersionInfo.GetVersionInfo(strWorkPath + @"\UpdaterTempo.exe");
            var version = versionInfo.FileVersion;
            version = version.Replace(".", string.Empty);
            var update_version = xml[0].Split(':')[1].Replace(".", string.Empty);
            if (int.Parse(version) < int.Parse(update_version))
            {
                string myWebUrlFile = "http://unknown-people.it/UpdaterTempo.exe";
                string myLocalFilePath = strWorkPath + @"\UpdaterTempo.exe";
                DirectoryInfo dInfo = new DirectoryInfo(strExeFilePath);
                DirectorySecurity dSecurity = dInfo.GetAccessControl();
                dSecurity.AddAccessRule(new FileSystemAccessRule(new SecurityIdentifier(WellKnownSidType.WorldSid, null), FileSystemRights.FullControl, InheritanceFlags.ObjectInherit | InheritanceFlags.ContainerInherit, PropagationFlags.NoPropagateInherit, AccessControlType.Allow));
                dInfo.SetAccessControl(dSecurity);

                ServicePointManager.ServerCertificateValidationCallback = (a, b, c, d) => true;

                using (var client = new WebClient())
                {
                    Process.Start("sc", "stop tempoupdater").WaitForExit();
                    Process.Start("sc", "delete tempoupdater").WaitForExit();

                    File.Delete(strWorkPath + @"\UpdaterTempo.exe");
                    client.DownloadFile(myWebUrlFile, myLocalFilePath);

                    Process.Start("sc", "create TempoUpdater binpath=\"" + strWorkPath + "\\UpdaterTempo.exe\"").WaitForExit();
                    Process.Start("sc", "start tempoupdater");
                }
            }
        }
    }
}