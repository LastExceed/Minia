using NAudio.Wave;
using System;

namespace Minia {
    static class Audio {
        static AsioOut asioOut;
        static MixingWaveProvider32 mixer;
        public static AudioFileReader music, miss, hit;
        public static double time;//in ms

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
            mixer.AddInputStream(music);
            asioOut.Play();
        }

        public static void ResetAndSync() {
            //asioOut.ShowControlPanel();
            while (music.CurrentTime.Ticks == 0) ;//wait for audio playback to become fluent
            music.Position = 0;
            time = 0;
        }

        public static double GetDesync(double time) => music.CurrentTime.TotalMilliseconds - time;
    }
}
