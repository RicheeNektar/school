using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace Morser
{
    public class Audio
    {
        private const short CHANNELS = 1;
        private const int SAMPLE_RATE = 96000;
        private const short BITS_PER_SAMPLE = 16;
        
        private static readonly short FRAME_SIZE = CHANNELS * ((BITS_PER_SAMPLE + 7) / 8);
        private static readonly int BYTES_PER_SECOND = SAMPLE_RATE * FRAME_SIZE;

        private int duration;
        private int frequency;
        
        private readonly string path;
        public int Duration
        {
            get => duration;
            set
            {
                Delete();
                duration = value;
                Write();
            }
        }
        public int Frequency
        {
            get => frequency;
            set
            {
                Delete();
                frequency = value;
                Write();
            }
        }

        private string File => path + Duration + Frequency + ".wav";

        public void Dispose()
        {
            Delete();
        }
        
        public Audio()
        {
            path = Assembly.GetExecutingAssembly().Location;
            path = path.Substring(0, path.LastIndexOf(Path.DirectorySeparatorChar) + 1);
        }

        private void Delete()
        {
            System.IO.File.Delete(File);
        }

        public void Play()
        {
            if (Environment.OSVersion.Platform == PlatformID.Unix)
            {
                PlayLinux();
            }
            else
            {
                Console.Beep(Frequency, Duration);
            }
        }

        private void PlayLinux()
        {
            Process p = new Process()
            {
                EnableRaisingEvents = false,
                StartInfo = new ProcessStartInfo()
                {
                    FileName = "aplay",
                    Arguments = "-q -t wav " + File
                }
            };
            
            p.Start();
            p.WaitForExit(duration);
        }
        
        public void Write()
        {
            MemoryStream ms = new MemoryStream();
            BinaryWriter w = new BinaryWriter(ms);

            int totalSamples = (int)(SAMPLE_RATE * (duration / 1000.0f) * 2);
            double f = 22;
            
            for (float i = 0f; i < totalSamples || (int) Math.Ceiling(f) != 0; i++)
            {
                f = Math.Sin(++i / SAMPLE_RATE * frequency * 2 * Math.PI);
                short d = (short) (10000 * f);
                w.Write(d);
            }

            int length = (int) ms.Length;
            
            Stream s = new FileStream(File, FileMode.Create);
            w = new BinaryWriter(s);

            w.Write("RIFF".ToCharArray());
            w.Write(38 + length);
            w.Write("WAVEfmt ".ToCharArray());
            w.Write(16);
            w.Write((short) 1);
            w.Write(CHANNELS);
            w.Write(SAMPLE_RATE);
            w.Write(BYTES_PER_SECOND);
            w.Write(FRAME_SIZE);
            w.Write(BITS_PER_SAMPLE);
            w.Write("data".ToCharArray());
            w.Write(length);
            w.Write(ms.ToArray());
            
            w.Close();
            ms.Close();
        }
    }
}