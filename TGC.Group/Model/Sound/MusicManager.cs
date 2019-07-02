using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Microsoft.DirectX.PrivateImplementationDetails;
using TGC.Core.Sound;
using Microsoft.DirectX.DirectInput;
using Microsoft.DirectX.Direct3D;
using Microsoft.DirectX.DirectSound;
using TGC.Core.Input;

namespace TGC.Group.Model.Utils
{   
    public static class MusicManager
    {
        public static readonly TgcMp3Player TitleMusic = new TgcMp3Player {FileName = Game.Default.MediaDirectory + "\\Music\\title.mp3"};
        public static readonly TgcMp3Player SurfaceMusic = new TgcMp3Player {FileName = Game.Default.MediaDirectory + "\\Music\\surface.mp3"};
        public static readonly TgcMp3Player UnderWaterMusic = new TgcMp3Player {FileName = Game.Default.MediaDirectory + "\\Music\\underwater.mp3"};
        public static readonly TgcMp3Player ShipMusic = new TgcMp3Player {FileName = Game.Default.MediaDirectory + "\\Music\\ship.mp3"};

        private static TgcMp3Player lastPlayed;

        public static void play(TgcMp3Player music)
        {
            if (lastPlayed != null && lastPlayed == music)
            {
                return;
            }

            stop(lastPlayed);
            
            lastPlayed = music;
            lastPlayed.play(true);
        }

        public static void stop(TgcMp3Player music)
        {
            music?.stop();
            music?.closeFile();
        }
    }
}