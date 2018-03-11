using NAudio.Wave;
using System;

namespace Minia {
    static class Audio {
        static AudioFileReader music, miss, hit;
        static AsioOut asioOut;

        public static void Init(string musicFile) {
            hit = new AudioFileReader("hitsound.wav") {
                Volume = 0.2f
            };
            miss = new AudioFileReader("miss.wav") {
                Volume = 0.2f
            };
            music = new AudioFileReader(musicFile) {
                Volume = 0.2f
            };

            var asioDrivers = AsioOut.GetDriverNames();
            if (asioDrivers.Length == 0) {
                Console.WriteLine("please install http://www.asio4all.org/");
                Console.WriteLine("press any key to exit");
                Console.ReadKey();
                Environment.Exit(0);
            }
            asioOut = new AsioOut(asioDrivers[0]);
            //asioOut.ShowControlPanel();
            asioOut.Init(new WaveMixerStream32(new WaveStream[3] { music, hit, miss }, false));
        }

        public static void StartAndSync() {
            asioOut.Play();
            while (music.CurrentTime.Ticks == 0) ;//wait for audio playback to become fluent
            music.Position = 0;
        }

        public static void Play(string sound) {
            switch (sound) {
                case "miss":
                    miss.Position = 0;
                    break;
                case "hit":
                    hit.Position = 0;
                    break;
                default:
                    throw new InvalidOperationException("unknown sound");
            }
        }

        public static double GetDesync(double time) => music.CurrentTime.TotalMilliseconds - time;
    }
}
