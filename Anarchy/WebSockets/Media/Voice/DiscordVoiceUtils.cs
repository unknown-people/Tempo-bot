using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Discord.Media
{
    public static class DiscordVoiceUtils
    {
        public static Stream GetAudioStream(string path)
        {
            if (!File.Exists("ffmpeg.exe"))
                throw new FileNotFoundException("ffmpeg.exe was not found");

            var process = Process.Start(new ProcessStartInfo
            {
                FileName = "ffmpeg.exe",
                Arguments = $"-nostats -loglevel 0 -i \"{path}\" -ac 2 -f s16le -ar 48000 pipe:1",
                UseShellExecute = false,
                RedirectStandardOutput = true
            });

            return process.StandardOutput.BaseStream;
        }
        public static void SaveAudioToFile(string url, string full_path)
        {
            {
                if (!File.Exists("ffmpeg.exe"))
                    throw new FileNotFoundException("ffmpeg.exe was not found");

                if (File.Exists(full_path))
                {
                    DeleteFile(full_path);
                }
                var process = Process.Start(new ProcessStartInfo
                {
                    FileName = "ffmpeg.exe",
                    Arguments = $"-nostats -loglevel 0 -i \"{url}\" -ac 2 -f s16le -ar 48000 \"{full_path}\"",
                    UseShellExecute = false,
                    RedirectStandardOutput = true
                });
            }
        }
        public static void DeleteFile(string full_path)
        {
            File.Delete(full_path);
        }
        public static Stream GetFileAudioStream(string full_path, string url)
        {
            SaveAudioToFile(url, full_path);
            FileStream fs = File.OpenRead(full_path);
            DeleteFile(full_path);
            return fs;
        }
        public static byte[] GetAudio(string path)
        {
            using (var memStream = new MemoryStream())
            {
                GetAudioStream(path).CopyTo(memStream);
                return memStream.ToArray();
            }
        }

        public static MemoryStream GetAudioMemoryStream(string path)
        {
            using (var memStream = new MemoryStream())
            {
                GetAudioStream(path).CopyTo(memStream);
                return memStream;
            }
        }

        [Obsolete("This method is obsolete. Please use GetAudioStream instead", true)]
        public static byte[] ReadFromFile(string path)
        {
            return null;
        }

        private static List<int> FindNALUnitIndexes(byte[] byteStream)
        {
            List<int> indexes = new List<int>();

            byte[] startCode = new byte[] { 0, 0, 0, 1 };
            int currentStartCodeChar = 0;

            for (int i = 0; i < byteStream.Length; i++)
            {
                if (byteStream[i] == startCode[currentStartCodeChar])
                {
                    currentStartCodeChar++;

                    if (currentStartCodeChar == startCode.Length)
                        indexes.Add(i + 1);
                    else
                        continue; // don't wanna reset till we're done lel
                }
                
                currentStartCodeChar = 0;
            }

            return indexes;
        }

        // used for DiscordLiveStream.Write(). WIP
        private static byte[][] NALBytestreamToPackets(byte[] byteStream)
        {
            List<int> startIndexes = FindNALUnitIndexes(byteStream);

            byte[][] nalUnits = new byte[startIndexes.Count][];

            for (int i = 0; i < startIndexes.Count; i++)
            {
                int count;

                if (i == startIndexes.Count - 1)
                    count = byteStream.Length - startIndexes[i];
                else
                    count = startIndexes[i + 1] - startIndexes[i] - 4;

                nalUnits[i] = new byte[count];
                Buffer.BlockCopy(byteStream, startIndexes[i], nalUnits[i], 0, count);
            }

            byte[] starts = new byte[nalUnits.Length];

            for (int i = 0; i < starts.Length; i++)
                starts[i] = nalUnits[i][0];

            return nalUnits;
        }
    }
}
