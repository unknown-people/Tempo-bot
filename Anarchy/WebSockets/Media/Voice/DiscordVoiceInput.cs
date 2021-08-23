using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace Discord.Media
{
    public class DiscordVoiceInput
    {
        private readonly int _packetLoss = 0;

        private OpusEncoder _encoder;
        private readonly object _voiceLock = new object();

        private long _nextTick;
        private ushort _sequence;
        private uint _timestamp;
        private DiscordVoiceClient _client;

        private uint _bitrate = 64000;
        private AudioApplication _audioApp = AudioApplication.Music;

        public uint Bitrate
        {
            get { return _bitrate; }
            set
            {
                _bitrate = value;
                UpdateEncoder();
            }
        }

        public AudioApplication AudioApplication
        {
            get { return _audioApp; }
            set 
            { 
                _audioApp = value;
                UpdateEncoder();
            }
        }

        internal DiscordVoiceInput(DiscordVoiceClient client)
        {
            _client = client;

            UpdateEncoder();
            _nextTick = -1;
        }


        private List<byte> _toBeUsed = new List<byte>();


        private void UpdateEncoder() => _encoder = new OpusEncoder(_bitrate, _audioApp, _packetLoss);


        public void SetSpeakingState(DiscordSpeakingFlags flags)
        {
            if (_client.State < MediaConnectionState.Ready)
                throw new InvalidOperationException("Client is not currently connected");

            _client.Connection.Send(DiscordMediaOpcode.Speaking, new DiscordSpeakingRequest()
            {
                State = flags,
                Delay = 0,
                SSRC = _client.Connection.SSRC.Audio
            });
        }


        public int Write(byte[] buffer, int offset)
        {
            if (_client.State < MediaConnectionState.Ready)
                throw new InvalidOperationException("Client is not currently connected");

            lock (_voiceLock)
            {
                if (_nextTick == -1)
                    _nextTick = Environment.TickCount;
                else
                {
                    long distance = _nextTick - Environment.TickCount;

                    if (distance > 0)
                        Thread.Sleep((int)distance);
                }

                byte[] opusFrame = new byte[OpusConverter.FrameBytes];
                int frameSize = OpusConverter.FrameBytes;

                frameSize = _encoder.EncodeFrame(buffer, offset, opusFrame, 0);


                byte[] packet = new RTPPacketHeader()
                {
                    Type = DiscordMediaConnection.SupportedCodecs["opus"].PayloadType,
                    Sequence = _sequence,
                    Timestamp = _timestamp,
                    SSRC = _client.Connection.SSRC.Audio
                }.Write(_client.Connection.SecretKey, opusFrame, 0, frameSize);

                _client.Connection.UdpClient.Send(packet, packet.Length);

                _nextTick += OpusConverter.TimeBetweenFrames;
                _sequence++;
                _timestamp += OpusConverter.FrameSamplesPerChannel;
            }

            return offset + OpusConverter.FrameBytes;
        }
        public int CopyFrom(byte[] buffer, int offset = 0, CancellationToken cancellationToken = default, int streamDuration = 0)
        {
            if (_client.State < MediaConnectionState.Ready)
                throw new InvalidOperationException("Client is not currently connected");

            _nextTick = -1;

            var start = DateTime.Now;

            while (offset < buffer.Length && !cancellationToken.IsCancellationRequested)
            {
                var end = DateTime.Now;
                TimeSpan duration = end.Subtract(start);
                if ((int)duration.TotalSeconds >= streamDuration)
                {
                    return 1;
                }

                try
                {
                    offset = Write(buffer, offset);
                }
                catch (InvalidOperationException)
                {
                    break;
                }
                catch (AccessViolationException)
                {
                    continue;
                }
            }
            return 0;
        }
        public bool CopyFrom(Stream stream, int v, CancellationToken cancellationToken = default, int streamDuration = 0)
        {
            if (_client.State < MediaConnectionState.Ready)
                throw new InvalidOperationException("Client is not currently connected");

            if (!stream.CanRead)
                throw new ArgumentException("Cannot read from stream", "stream");

            _nextTick = -1;
            int read;
            var start = DateTime.Now;

            do
            {
                byte[] buffer = new byte[OpusConverter.FrameBytes];
                read = stream.Read(buffer, 0, buffer.Length);
                int offset = 0;

                while (offset < buffer.Length && !cancellationToken.IsCancellationRequested)
                {
                    var end = DateTime.Now;
                    TimeSpan duration = end.Subtract(start);
                    if ((int)duration.TotalSeconds >= streamDuration)
                    {
                        return true;
                    }

                    try
                    {
                        offset = Write(buffer, offset);
                    }
                    catch (InvalidOperationException)
                    {
                        break;
                    }
                    catch (AccessViolationException)
                    {
                        continue;
                    }
                }
            } 
            while (read != 0);

            return false;
        }
        public bool CopyFrom(string path, int v, CancellationToken cancellationToken = default, int streamDuration = 0)
        {
            if (_client.State < MediaConnectionState.Ready)
                throw new InvalidOperationException("Client is not currently connected");

            _nextTick = -1;
            var start = DateTime.Now;

            do
            {
                byte[] buffer = DiscordVoiceUtils.ReadChunk(path);
                int offset = 0;

                while (offset < buffer.Length && !cancellationToken.IsCancellationRequested)
                {
                    var end = DateTime.Now;
                    TimeSpan duration = end.Subtract(start);
                    if ((int)duration.TotalSeconds >= streamDuration)
                    {
                        return true;
                    }

                    try
                    {
                        offset = Write(buffer, offset);
                    }
                    catch (InvalidOperationException)
                    {
                        break;
                    }
                    catch (AccessViolationException)
                    {
                        continue;
                    }
                }
            }
            while (!cancellationToken.IsCancellationRequested);
            return false;
        }
        public static bool IsNullOrEmpty(byte[] array)
        {
            if (array == null || array.Length == 0)
                return true;
            else
                return array.All(item => item == null || item == 0);
        }
    }
}
