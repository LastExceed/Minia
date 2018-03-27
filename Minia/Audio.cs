using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using System;
using System.Diagnostics;

namespace Minia {
    static class Audio {
        static AsioOut asioOut;
        static MixingWaveProvider32 mixer;
        public static AudioFileReader music, miss, hit;
        public static Stopwatch sw = new Stopwatch();
        public static double Desync {
            get {
                steps++;
                total += music.CurrentTime.TotalMilliseconds - sw.Elapsed.TotalMilliseconds;
                return total / steps;
                //return music.CurrentTime.TotalMilliseconds - sw.Elapsed.TotalMilliseconds;
            }
        }
        static double total = 0;
        static long steps = 0;

        static Audio() {
            var asioDrivers = AsioOut.GetDriverNames();
            if (asioDrivers.Length == 0) {
                Console.WriteLine("please install http://www.asio4all.org/");
                Console.WriteLine("press any key to exit");
                Console.ReadKey();
                Environment.Exit(0);
            }
            asioOut = new AsioOut(asioDrivers[0]);
            mixer = new MixingWaveProvider32();
            asioOut.Init(mixer);
            hit = new AudioFileReader("hitsound.wav") {
                Volume = 0.2f
            };
            miss = new AudioFileReader("miss.wav") {
                Volume = 0.2f
            };
            mixer.AddInputStream(hit);
            mixer.AddInputStream(miss);
            asioOut.Play();
        }

        public static void SetMusic(string musicFile) {
            if (music != null) {
                asioOut.Stop();
                mixer.RemoveInputStream(music);
                music.Dispose();
            }
            music = new AudioFileReader(musicFile) {
                Volume = 0.2f
            };
            try {
                mixer.AddInputStream(music);
            }
            catch (ArgumentException) {
                Console.WriteLine("!");
                return;
            }
            asioOut.Play();
            music.CurrentTime = new TimeSpan(0, 0, 30);
        }

        public static void ResetAndSync() {
            //asioOut.ShowControlPanel();
            while (music.CurrentTime.TotalSeconds <= 2d) ;//wait for audio playback to become 
            music.CurrentTime = new TimeSpan(0);
            sw.Restart();
            total = 0;
            steps = 0;
        }
    }
}
