using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.Input;
using TGC.Core.Text;
using TGC.Group.Model.Input;
using TGC.Group.Model.Resources.Sprites;
using TGC.Group.Model.Utils;
using TGC.Group.TGCUtils;

namespace TGC.Group.Model.Scenes
{
    class GameOverScene : Scene
    {
        public delegate void Callback();
        Callback onGoToStartScreenCallback;
        Callback preRender;
        TgcText2D DrawTextBig = new TgcText2D(), DrawTextSmall = new TgcText2D();
        Drawer2D drawer2D = new Drawer2D();
        CustomSprite darknessCover;
        float darknessTransparency = 0f, maxDarknessTransparency = 220f;
        float letterTransparency = 0f;

        public GameOverScene() : base()
        {
            Uses3DCamera = false;

            pressed[GameInput.Accept] = () => {
                onGoToStartScreenCallback();
            };

            InitDarknessCover();
            
            DrawTextBig.changeFont(new Font("Arial Black", 30f));
            DrawTextSmall.changeFont(new Font("Arial Black", 20f));
        }
        private void InitDarknessCover()
        {
            darknessCover = BitmapRepository.CreateSpriteFromBitmap(BitmapRepository.BlackRectangle);

            Screen.FitSpriteToScreen(darknessCover);

            darknessCover.Color = Color.FromArgb(0, 0, 0, 0);
        }
        public override void Render()
        {
            preRender();

            drawer2D.BeginDrawSprite();
            drawer2D.DrawSprite(darknessCover);
            drawer2D.EndDrawSprite();

            if(darknessTransparency == maxDarknessTransparency)
            {
                int x1 = Screen.Width / 2 - 110;
                int y1 = Screen.Height / 2 + 20;
                int x2 = Screen.Width / 2 - 260;
                int y2 = Screen.Height / 2 + 35;
                DrawTextBig.drawText("YOU DIED", x1, y1, Color.FromArgb((int)letterTransparency, 175, 0, 0));
                DrawTextSmall.drawText("(Press Enter to go back to main menu)", x2, y2 + 60, Color.FromArgb((int)letterTransparency, 200, 200, 200));
            }
        }

        public override void Update(float elapsedTime)
        {
            darknessTransparency = Math.Min(darknessTransparency + 100f * elapsedTime, maxDarknessTransparency);

            if (darknessTransparency == maxDarknessTransparency)
                letterTransparency = Math.Min(letterTransparency + 200f * elapsedTime, 255);

            darknessCover.Color = Color.FromArgb((int)darknessTransparency, 255, 0, 0);
        }
        public GameOverScene OnGoToStartScreen(Callback onGoToStartScreenCallback)
        {
            this.onGoToStartScreenCallback = onGoToStartScreenCallback;
            return this;
        }
        public GameOverScene WithPreRender(Callback preRender)
        {
            this.preRender = preRender;
            return this;
        }
    }
}
