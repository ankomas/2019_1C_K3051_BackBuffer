using System;
using Microsoft.DirectX.DirectSound;
using TGC.Core.Mathematica;
using TGC.Core.Sound;

namespace TGC.Group.Model.Utils
{
    public class StaticSound
    {
        private string path;
        private TgcStaticSound sound;
        private Device DsDevice;
        private bool loop;

        public StaticSound(string path, Device DsDevice, bool loop)
        {
            this.path = path;
            this.sound = new TgcStaticSound();
            this.sound.loadSound(path, 0, DsDevice);
            this.loop = loop;
            this.DsDevice = DsDevice;
        }
        
        public void play(int volume)
        {
            var clampedVolume = FastMath.Clamp(volume, (int)Volume.Min, (int)Volume.Max);
            
            this.sound.SoundBuffer.Volume = clampedVolume;
            this.sound.play(this.loop);
        }

        public void stop()
        {
            this.sound.stop();
        }

    }
}