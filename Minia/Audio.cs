﻿using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace Minia {
    static class Audio {
        public static AsioOut asioOut;
        static MixingWaveProvider32 mixer;
        public static WaveChannel32 music, miss, hit;
        private static Stopwatch sw = new Stopwatch();
        public static double MusicTime {
            get {
                return sw.Elapsed.TotalMilliseconds + Config.audioOffset;
            }
        }
        public static double Desync {
            get {
                return music.CurrentTime.TotalMilliseconds - sw.Elapsed.TotalMilliseconds;
            }
        }
        public static double DesyncTotal {
            get {
                steps++;
                total += Desync;
                return total / steps;
                //return music.CurrentTime.TotalMilliseconds - sw.Elapsed.TotalMilliseconds;
            }
        }
        public static bool musicCompatible;
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
            hit = new WaveChannel32(new AudioFileReader("hitsound.wav")) {
                Volume = 0.2f,
            };
            miss = new WaveChannel32(new AudioFileReader("miss.wav")) {
                Volume = 0.2f,
            };
            var err = new SignalGenerator() {
                Type = SignalGeneratorType.SawTooth,
                Gain = 0.2,
            }.Take(TimeSpan.FromSeconds(0.1d)).ToWaveProvider();
            mixer.AddInputStream(hit);
            mixer.AddInputStream(miss);
            asioOut.Init(mixer);
            asioOut.Play();
        }

        public static void SetMusic(string musicFile) {
            if (!File.Exists(musicFile)) {
                Console.WriteLine("file not found: " + musicFile);
                return;
            }
            if (music != null) {
                mixer.RemoveInputStream(music);
                music.Dispose();
            }
            var audioFileReader = new AudioFileReader(musicFile);
            if (audioFileReader.WaveFormat.SampleRate != mixer.WaveFormat.SampleRate) {
                audioFileReader.Dispose();
                musicCompatible = false;
                return;
            }
            musicCompatible = true;
            music = new WaveChannel32(audioFileReader) {
                Volume = 0.2f
            };
            mixer.AddInputStream(music);
            sw.Restart();
            //music.CurrentTime = new TimeSpan(0, 0, 30);
        }

        public static void ResetAndSync() {
            mixer.RemoveInputStream(music);
            music.Position = 0;
            mixer.AddInputStream(music);
            sw.Restart();
            //music.CurrentTime = new TimeSpan(0);
            total = 0;
            steps = 0;
        }

        internal static void Abort() {
            asioOut.Stop();
            asioOut.Dispose();
            music.Dispose();
            hit.Dispose();
            miss.Dispose();
        }
    }
}
