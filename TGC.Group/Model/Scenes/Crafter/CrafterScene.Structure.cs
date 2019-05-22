using System.Drawing;
using TGC.Core.Input;
using TGC.Core.Mathematica;
using TGC.Group.Model.Input;
using TGC.Group.Model.Resources.Sprites;
using TGC.Group.Model.Utils;

namespace TGC.Group.Model.Scenes.Crafter
{
    partial class CrafterScene
    {
        public delegate void UpdateLogic(float elapsedTime);
        public UpdateLogic updateLogic = time => { };
        private Items.Crafter crafter = new Items.Crafter();
        enum StateID
        {

            IN,
            CRAFTER,
            OUT,
            NULL
        }
        struct State
        {
            public StateID nextStateID;
            public UpdateLogic updateLogic;
            public State(UpdateLogic updateLogic, StateID nextStateID)
            {
                this.updateLogic = updateLogic;
                this.nextStateID = nextStateID;
            }
        }
        private State[] states = new State[3];
        private StateID stateID, nextStateID;

        private TGCVector2 bubbleDefaultScale = new TGCVector2(.5f, .5f);
        public CrafterScene(TgcD3dInput input, GameScene gameScene, ShipScene shipScene) : base(input)
        {
            this.gameScene = gameScene;
            this.shipScene = shipScene;
            
            this.InitPDA();
            this.InitDarknessCover();
            this.InitCursor();
            this.InitBubble();
            this.InitFish();
            this.InitPlant();

            this.BindState(StateID.IN, TakePDAIn, StateID.OUT);
            this.BindState(StateID.CRAFTER, InventoryInteraction, StateID.OUT);
            this.BindState(StateID.OUT, TakePDAOut, StateID.IN);

            this.SetState(StateID.IN);
            
            this.pressed[GameInput._Enter] = () =>
            {
                if (this.itemHighlighted != null)
                {
                    this.crafter.Craft(itemHighlighted, Character);
                    this.Character.GiveItem(this.crafter.CraftedItem);
                }
            };
        }
        private void SetState(StateID newStateID)
        {
            State newState = this.states[(int)newStateID];

            this.stateID = newStateID;
            this.updateLogic = newState.updateLogic;
            pressed[GameInput._Inventory] = () => this.SetNextState(newState.nextStateID);
        }
        private void SetNextState(StateID newStateID)
        {
            this.nextStateID = newStateID;
        }
        private void BindState(StateID stateID, UpdateLogic stateUpdateLogic, StateID nextStateID)
        {
            this.states[(int)stateID] = new State(stateUpdateLogic, nextStateID);
        }
        private void InitPDA()
        {
            PDA = BitmapRepository.CreateSpriteFromBitmap(BitmapRepository.PDA);
            PDA.Scaling = new TGCVector2(.5f, .35f);
            Screen.CenterSprite(PDA);
            finalPDAPositionX = PDA.Position.X;
            PDAPositionX = this.GetPDAInitialPosition();
            PDAMoveCoefficient = (finalPDAPositionX - this.GetPDAInitialPosition()) * 4;
        }
        private void InitDarknessCover()
        {
            darknessCover = BitmapRepository.CreateSpriteFromBitmap(BitmapRepository.BlackRectangle);
            Screen.FitSpriteToScreen(darknessCover);
            darknessCover.Color = Color.FromArgb(0, 0, 0, 0);
        }
        private void InitCursor()
        {
            cursor = BitmapRepository.CreateSpriteFromBitmap(BitmapRepository.ArrowPointer);
            cursorDefaultColor = cursor.Color;
            Screen.CenterSprite(cursor);
        }
        private void InitBubble()
        {
            bubble = BitmapRepository.CreateSpriteFromBitmap(BitmapRepository.Bubble);
            bubble.Scaling = this.bubbleDefaultScale;
        }
        private void InitFish()
        {
            fish = BitmapRepository.CreateSpriteFromBitmap(BitmapRepository.Fish);
            fish.Scaling = new TGCVector2(.1f, .05f);
        }
        private void InitPlant()
        {
            plant = BitmapRepository.CreateSpriteFromBitmap(BitmapRepository.Plant);
            plant.Scaling = new TGCVector2(.1f, .05f);
        }
        private float GetPDAInitialPosition() { return -PDA.Bitmap.Width * PDA.Scaling.X; }
        private int CalculateTransparency(int limit)
        {
            return FastMath.Max(
                FastMath.Min((int)
                ((
                    1 - (
                            (finalPDAPositionX - PDAPositionX) / (finalPDAPositionX - this.GetPDAInitialPosition())
                        )
                ) * limit), 255), 0);
        }
        private int CalculatePDATransparency()
        {
            return this.CalculateTransparency(140);
        }
        private int CalculaterBlacknessTransparency()
        {
            return this.CalculateTransparency(188);
        }
    }
}
