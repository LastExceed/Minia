using NAudio.Wave;
using System;

namespace Minia {
    static class Audio {
        static WaveChannel32 miss = new WaveChannel32(new WaveFileReader(Properties.Resources.miss), 1f, 0f) {//new Mp3FileReader("boss.mp3")
            PadWithZeroes = true
        };
        static WaveChannel32 hit = new WaveChannel32(new WaveFileReader(Properties.Resources.hitsound), 0.15f, 0f) {//new Mp3FileReader("boss.mp3")
            PadWithZeroes = true
        };
        static WaveChannel32 music = new WaveChannel32(new Mp3FileReader("audio.mp3"), 0.15f, 0f) {//new Mp3FileReader("boss.mp3")
            PadWithZeroes = true
        };
        static AsioOut asioOut;

        public static void Init() {
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
            while (music.CurrentTime.TotalSeconds < 0.1f) ;//wait for audio playback to become fluent
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
