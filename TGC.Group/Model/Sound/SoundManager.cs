using System.Collections.Generic;
using Microsoft.DirectX.DirectSound;
using TGC.Core.Mathematica;
using TGC.Core.Sound;
using TGC.Group.Model.Chunks;

namespace TGC.Group.Model.Utils
{
    public class SoundManager
    {
        public static readonly string Bubble = Game.Default.MediaDirectory + "\\Sound\\bubble.wav";
        public static readonly string Motor = Game.Default.MediaDirectory + "\\Sound\\motor.wav";
        public static readonly string CrafterOpen = Game.Default.MediaDirectory + "\\Sound\\crafterOpen.wav";
        public static readonly string CrafterClose = Game.Default.MediaDirectory + "\\Sound\\crafterClose.wav";
        public static readonly string Eat = Game.Default.MediaDirectory + "\\Sound\\eat.wav";
        public static readonly string Drink = Game.Default.MediaDirectory + "\\Sound\\drink.wav";
        public static readonly string Hit = Game.Default.MediaDirectory + "\\Sound\\hit.wav";
        public static readonly string Death = Game.Default.MediaDirectory + "\\Sound\\death.wav";
        public static readonly string Inevitable = Game.Default.MediaDirectory + "\\Sound\\inevitable.wav";
        public static readonly string Oxygen = Game.Default.MediaDirectory + "\\Sound\\oxygen.wav";

        public static TgcDirectSound DirectSound { get; set; }

        public static StaticSound MotorSound;

        private static readonly List<TgcStaticSound> SoundsPlaying = new List<TgcStaticSound>();

        public static void init(TgcDirectSound directSound)
        {
            DirectSound = directSound;
            MotorSound = new StaticSound(Motor, DirectSound.DsDevice, false);
        }
        public static void CleanUpTasks()
        {
            var toDelete = SoundsPlaying
                .FindAll(sound => !sound.SoundBuffer.Status.Playing);
            toDelete.ForEach(sound => Stop(sound));
        }
        
        public static void Play(string soundPath)
        {
            var temp = new TgcStaticSound();
            temp.loadSound(soundPath, DirectSound.DsDevice);

            Play(temp);
        }
        private static void Play(TgcStaticSound sound)
        {
            sound.play(false);
            SoundsPlaying.Add(sound);
        }

        public static void Play(StaticSound staticSound, int volume)
        {
            staticSound.play(volume);
        }

        public static void Stop(TgcStaticSound sound)
        {
            sound.dispose();
            SoundsPlaying.Remove(sound);
        }

        public static void Stop(StaticSound sound)
        {
            sound.stop();
        }
    }
}