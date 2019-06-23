using Microsoft.DirectX;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.Input;
using TGC.Core.Mathematica;
using TGC.Group.Model.Elements;
using TGC.Group.Model.Items;
using TGC.Group.Model.Resources.Sprites;
using TGC.Group.Model.UI;
using TGC.Group.Model.Utils;
using TGC.Group.TGCUtils;

namespace TGC.Group.Model.Scenes
{
    public struct GameState
    {
        public Player.Character character;
        public GameState(Player.Character character)
        {
            this.character = character;
        }
    }
    abstract class GameplayScene : Scene
    {
        public static GameState InitialGameState => new GameState(new Player.Character());
        protected CustomSprite cursor, aim, hand;
        protected DialogBox dialogBox = new DialogBox();
        public delegate void TransitionCallback(GameState gameState);
        public GameState GameState { get; set; }

        protected StatsIndicators statsIndicators;

        public GameplayScene(GameState gameState) : base()
        {
            this.GameState = gameState;
            InitDialogBox();
            InitStatsIndicator();
            InitAim();
            InitHand();
            cursor = aim;
        }
        public void Render()
        {
            throw new NotImplementedException();
        }

        public override void Update(float elapsedTime)
        {
            AquaticPhysics.Instance.DynamicsWorld.StepSimulation(elapsedTime);
            Camera.Update(elapsedTime);
            UpdateGameplay(elapsedTime);
        }

        public abstract void UpdateGameplay(float elapsedTime);

        public GameplayScene WithGameState(GameState gameState)
        {
            this.GameState = gameState;
            return this;
        }
        private void InitDialogBox()
        {
            dialogBox.color = Color.FromArgb(210, 0, 63, 108);
            dialogBox.borderColor = Color.FromArgb(255, 13, 158, 255);
        }
        private void InitStatsIndicator()
        {
            Vector2 niceOffset = new Vector2(Screen.Width * 0.1f, Screen.Height * 0.05f);
            int baseX0 = (int)(niceOffset.X);
            int baseY0 = (int)(Screen.Height - (StatsIndicators.OxygenMeterSize + niceOffset.Y));

            statsIndicators = new StatsIndicators(baseX0, baseY0);

            this.statsIndicators.init();
        }
        private void InitAim()
        {
            aim = BitmapRepository.CreateSpriteFromBitmap(BitmapRepository.Aim);
            aim.Scaling = new TGCVector2(.5f, .5f);
            Screen.CenterSprite(aim);
        }
        private void InitHand()
        {
            hand = BitmapRepository.CreateSpriteFromBitmap(BitmapRepository.Hand);
            hand.Scaling = new TGCVector2(.5f, .5f);
            Screen.CenterSprite(hand);
        }
    }
}
