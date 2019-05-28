using System.Drawing;
using TGC.Core.Input;
using TGC.Core.Text;
using TGC.Core.Mathematica;
using TGC.Group.TGCUtils;
using TGC.Group.Model.Utils;
using TGC.Group.Model.Input;
using TGC.Group.Model.Resources.Sprites;

namespace TGC.Group.Model.Scenes
{
    class PauseMenu : Scene
    {
        public delegate void Callback();
        private Callback preRender = () => {};
        private Callback onReturnToGameCallback = () => {}, onGoToStartMenuCallback = () => {};

        private int xTitle, yTitle, yOffsetFirstOption = 250, ySecondOption = 50;

        TgcText2D textBig = new TgcText2D(), textSmall = new TgcText2D();

        enum Pointer
        {
            UP,
            DOWN
        }
        Pointer pointer = Pointer.UP;
        Color[] colors = { Color.White, Color.DarkGray };

        Drawer2D drawer = new Drawer2D();
        CustomSprite darknessCover;
        public PauseMenu() : base()
        {
            darknessCover = BitmapRepository.CreateSpriteFromBitmap(BitmapRepository.BlackRectangle);
            darknessCover.Color = Color.FromArgb(188, 0, 50, 200);
            darknessCover.Scaling = new TGCVector2(.8f, .5f);

            Screen.CenterSprite(darknessCover);

            xTitle = (int)(darknessCover.Position.X + 60);
            yTitle = (int)(darknessCover.Position.Y + 80);

            Uses3DCamera = false;
            textBig.changeFont(new System.Drawing.Font("Arial Black", 40f));
            textSmall.changeFont(new System.Drawing.Font("Arial Black", 20f));

            InitInput();
        }
        private void InitInput()
        {
            pressed[GameInput.Accept] = () => {
                Decide();
                pointer = Pointer.UP;
            };
            pressed[GameInput.GoBack] = () => {
                onReturnToGameCallback();
                pointer = Pointer.UP;
            };
            pressed[GameInput.Up] = () => {
                pointer = Pointer.UP;
            };
            pressed[GameInput.Down] = () => {
                pointer = Pointer.DOWN;
            };
        }
        public override void Update(float elapsedTime)
        {
            
        }
        public override void Render()
        {
            preRender();

            drawer.BeginDrawSprite();
            drawer.DrawSprite(darknessCover);
            drawer.EndDrawSprite();

            textBig.drawText("PAUSE", xTitle + 50, yTitle, Color.DarkGray);
            textSmall.drawText("BACK TO THE GAME", xTitle, yTitle + yOffsetFirstOption, colors[(int)pointer]);
            textSmall.drawText("GO TO START MENU", xTitle, yTitle + yOffsetFirstOption + ySecondOption, colors[(((int)pointer) + 1) % 2]);
            textSmall.drawText("->", xTitle - 45, yTitle + yOffsetFirstOption + (int)pointer * ySecondOption, Color.White);
        }
        public PauseMenu OnReturnToGame(Callback onEnterCallback)
        {
            this.onReturnToGameCallback = onEnterCallback;
            return this;
        }
        public PauseMenu OnGoToStartMenu(Callback onEscapeCallback)
        {
            this.onGoToStartMenuCallback = onEscapeCallback;
            return this;
        }
        private void Decide()
        {
            (pointer == Pointer.UP ? onReturnToGameCallback : onGoToStartMenuCallback)();
        }
        public PauseMenu WithPreRender(Callback preRender)
        {
            this.preRender = preRender;
            return this;
        }
    }
}
