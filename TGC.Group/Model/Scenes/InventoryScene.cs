using System.ComponentModel.Design;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.DirectX.DirectInput;
using TGC.Core.BoundingVolumes;
using TGC.Core.Mathematica;
using TGC.Core.Text;
using TGC.Group.Model.Input;
using TGC.Group.Model.Items;
using TGC.Group.TGCUtils;

namespace TGC.Group.Model.Scenes
{
    partial class InventoryScene : Scene
    {
        private int count;
        private Player.Character character;

        private TgcText2D text = new TgcText2D();
        private Drawer2D drawer = new Drawer2D();
        private CustomSprite PDA, darknessCover, cursor, bubble, fish, plant;
        private Color cursorDefaultColor;
        float PDAPositionX, finalPDAPositionX, PDAMoveCoefficient;
        int PDATransparency;

        public IItem itemHighlighted { get; set; }

        private TGCVector2 GetScaleForSpriteByPixels(CustomSprite sprite, int xPixels, int yPixels)
        {
            float pixelWidth = sprite.Bitmap.Width;
            float pixelHeight = sprite.Bitmap.Height;
            float xScale = xPixels / pixelWidth;
            float yScale = yPixels / pixelHeight;
            return new TGCVector2(xScale, yScale);
        }
        public override void Update(float elapsedTime)
        {
            if (nextStateID != StateID.NULL)
            {
                SetState(nextStateID);
                nextStateID = StateID.NULL;
            }
            updateLogic(elapsedTime);

        }
        public override void Render(TgcFrustum frustum)
        {
            if (stateID == StateID.CLOSED) return;

            drawer.BeginDrawSprite();
            drawer.DrawSprite(darknessCover);
            drawer.DrawSprite(PDA);
            drawer.EndDrawSprite();
            if (stateID == StateID.OPENED)
            {
                bool hovering = false;
                TGCVector2 baseVector = PDA.Position + new TGCVector2(375, 175);
                drawer.BeginDrawSprite();
                byte xOffset = 110;
                byte yOffset = 110;
                byte maxItemsPerLine = 5;
                byte i = 0;
                foreach (var item in character.Inventory.Items)
                {
                    int x = i % maxItemsPerLine;
                    int y = i / maxItemsPerLine;
                    //text.drawText("-" + i++ + ": " + item.Name + " | " + item.Description + " | " + item.type.ToString(), 500, 300 + 30 * i, Color.White);
                    bubble.Position = baseVector + new TGCVector2(xOffset * x, yOffset * y);
                    if(cursorOverBubble())
                    {
                        bubble.Scaling = bubbleDefaultScale + GetScaleForSpriteByPixels(bubble, 10, 10);
                        item.Icon.Scaling = item.DefaultScale + GetScaleForSpriteByPixels(item.Icon, 10, 10);
                        hovering = true;
                        this.itemHighlighted = item;
                    }
                    else
                    {
                        bubble.Scaling = bubbleDefaultScale;
                        item.Icon.Scaling = item.DefaultScale;
                    }
                    item.Icon.Position = bubble.Position + new TGCVector2(7, 19);
                    drawer.DrawSprite(bubble);
                    drawer.DrawSprite(item.Icon);
                    ++i;
                }

                cursor.Color = hovering ? Color.Yellow : cursorDefaultColor;
                drawer.DrawSprite(cursor);
                drawer.EndDrawSprite();
            }
        }

        private bool cursorOverBubble()
        {
            return Cursor.Position.X >= this.bubble.Position.X &&
                   Cursor.Position.X <= this.bubble.Position.X + this.bubble.Bitmap.Width * this.bubble.Scaling.X &&
                   Cursor.Position.Y >= this.bubble.Position.Y &&
                   Cursor.Position.Y <= this.bubble.Position.Y + this.bubble.Bitmap.Height * this.bubble.Scaling.Y;
        }

        private void TakePDAIn(float elapsedTime)
        {
            PDAPositionX += PDAMoveCoefficient * elapsedTime;
            PDATransparency = CalculatePDATransparency();

            if (PDAPositionX > finalPDAPositionX)
            {
                PDAPositionX = finalPDAPositionX;
                SetNextState(StateID.OPENED);
            }

            PDA.Position = new TGCVector2(PDAPositionX, PDA.Position.Y);
            PDA.Color = Color.FromArgb(PDATransparency, PDA.Color.R, PDA.Color.G, PDA.Color.B);
            darknessCover.Color = Color.FromArgb(CalculaterBlacknessTransparency(), darknessCover.Color.R, darknessCover.Color.G, darknessCover.Color.B);
        }
        private void InventoryInteraction(float elapsedTime)
        {
            count = character.Inventory.Items.Count;
            cursor.Position = new TGCVector2(Cursor.Position.X, Cursor.Position.Y);
        }
        private void TakePDAOut(float elapsedTime)
        {
            PDAPositionX -= PDAMoveCoefficient * elapsedTime;
            PDATransparency = CalculatePDATransparency();

            if (PDAPositionX + PDA.Bitmap.Width * PDA.Scaling.X < 0)
            {
                PDAPositionX = GetPDAInitialPosition();
                SetNextState(StateID.CLOSED);
            }

            PDA.Position = new TGCVector2(PDAPositionX, PDA.Position.Y);
            PDA.Color = Color.FromArgb(PDATransparency, PDA.Color.R, PDA.Color.G, PDA.Color.B);
            darknessCover.Color = Color.FromArgb(CalculaterBlacknessTransparency(), darknessCover.Color.R, darknessCover.Color.G, darknessCover.Color.B);
        }
        public void Open(Player.Character character)
        {
            this.character = character;
            SetNextState(this.states[(int)this.stateID].onOpenStateID);
        }
        public void Close()
        {
            SetNextState(this.states[(int)this.stateID].onCloseStateID);
        }
    }
}
