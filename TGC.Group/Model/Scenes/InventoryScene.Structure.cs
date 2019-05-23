using Microsoft.DirectX.DirectInput;
using TGC.Core.Input;
using System.Drawing;
using TGC.Core.Mathematica;
using TGC.Group.Model.Input;
using TGC.Group.Model.Resources.Sprites;
using TGC.Group.Model.Utils;

namespace TGC.Group.Model.Scenes
{
    partial class InventoryScene
    {
        public delegate void UpdateLogic(float elapsedTime);
        public UpdateLogic updateLogic = time => { };
        enum StateID
        {
            NULL,
            CLOSED,
            IN,
            OPENED,
            OUT,
        }
        struct State
        {
            public StateID onOpenStateID, onCloseStateID;
            public UpdateLogic updateLogic;
            public State(UpdateLogic updateLogic, StateID onOpenStateID, StateID onCloseStateID)
            {
                this.updateLogic = updateLogic;
                this.onOpenStateID = onOpenStateID;
                this.onCloseStateID = onCloseStateID;
            }
        }
        private State[] states = new State[5];
        private StateID stateID, nextStateID;

        private TGCVector2 bubbleDefaultScale = new TGCVector2(.5f, .5f);

        delegate void Callback();
        private Callback onOpen, onClose;
        UpdateLogic emptyLambda = (time) => { };

        public InventoryScene(TgcD3dInput input) : base(input)
        {
            InitPDA();
            InitDarknessCover();
            InitCursor();
            InitBubble();
            InitFish();
            InitPlant();
            
            BindState(StateID.CLOSED, emptyLambda, StateID.IN, StateID.CLOSED);
            BindState(StateID.IN, TakePDAIn, StateID.IN, StateID.OUT);
            BindState(StateID.OPENED, InventoryInteraction, StateID.OPENED, StateID.OUT);
            BindState(StateID.OUT, TakePDAOut, StateID.IN, StateID.OUT);

            SetState(StateID.CLOSED);
            
            this.pressed[GameInput._Enter] = () => itemHighlighted?.Use(character);
        }
        private void SetState(StateID newStateID)
        {
            State newState = states[(int)newStateID];

            this.stateID = newStateID;
            this.updateLogic = newState.updateLogic;
            onOpen  = () => SetNextState(newState.onOpenStateID);
            onClose = () => SetNextState(newState.onCloseStateID);
        }
        private void SetNextState(StateID newStateID)
        {
            nextStateID = newStateID;
        }
        private void BindState(StateID stateID, UpdateLogic stateUpdateLogic, StateID onOpenStateID, StateID onCloseStateID)
        {
            states[(int)stateID] = new State(stateUpdateLogic, onOpenStateID, onCloseStateID);
        }
        private void InitPDA()
        {
            PDA = BitmapRepository.CreateSpriteFromBitmap(BitmapRepository.PDA);
            PDA.Scaling = new TGCVector2(.5f, .35f);
            Screen.CenterSprite(PDA);
            finalPDAPositionX = PDA.Position.X;
            PDAPositionX = GetPDAInitialPosition();
            PDAMoveCoefficient = (finalPDAPositionX - GetPDAInitialPosition()) * 4;
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
            bubble.Scaling = bubbleDefaultScale;
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
                            (finalPDAPositionX - PDAPositionX) / (finalPDAPositionX - GetPDAInitialPosition())
                        )
                ) * limit), 255), 0);
        }
        private int CalculatePDATransparency()
        {
            return CalculateTransparency(140);
        }
        private int CalculaterBlacknessTransparency()
        {
            return CalculateTransparency(188);
        }
    }
}
