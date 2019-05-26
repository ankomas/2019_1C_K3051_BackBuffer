using System.Drawing;
using System.Windows.Forms;
using TGC.Core.Mathematica;
using TGC.Core.Text;
using TGC.Group.Model.Items;
using TGC.Group.Model.Player;
using TGC.Group.TGCUtils;

namespace TGC.Group.Model.Scenes.Crafter
{
    partial class CrafterScene : Scene
    {
        private GameScene gameScene;
        private ShipScene shipScene;
        private int count;

        private TgcText2D text = new TgcText2D();
        private Drawer2D drawer = new Drawer2D();
        private CustomSprite PDA, darknessCover, cursor, bubble, fish, plant;
        private Color cursorDefaultColor;
        float PDAPositionX, finalPDAPositionX, PDAMoveCoefficient;
        int PDATransparency;

        private Character Character => this.gameScene.Character;

        public ICrafteable itemHighlighted { get; set; }

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

        public override void Render()
        {
            this.drawer.BeginDrawSprite();
            //this.drawer.DrawSprite(this.PDA);
            this.drawer.DrawSprite(this.darknessCover);
            this.drawer.EndDrawSprite();
            if (stateID == StateID.CRAFTER)
            {
                bool hovering = false;

                TGCVector2 baseVector = this.PDA.Position + new TGCVector2(100, 175);

                this.drawer.BeginDrawSprite();
                byte xOffset = 110;
                byte yOffset = 110;
                byte maxItemsPerLine = 8;
                byte i = 0;

                this.itemHighlighted = null;

                foreach (var item in Items.Crafter.Crafteables)
                {
                    int x = i % maxItemsPerLine;
                    int y = i / maxItemsPerLine;
                    //text.drawText("-" + i++ + ": " + item.Name + " | " + item.Description + " | " + item.type.ToString(), 500, 300 + 30 * i, Color.White);
                    this.bubble.Position = baseVector + new TGCVector2(xOffset * x, yOffset * y);
                    if (this.cursorOverBubble())
                    {
                        this.bubble.Scaling = bubbleDefaultScale + this.GetScaleForSpriteByPixels(this.bubble, 10, 10);
                        item.Icon.Scaling = item.DefaultScale + this.GetScaleForSpriteByPixels(item.Icon, 10, 10);
                        hovering = true;
                        this.itemHighlighted = item;
                    }
                    else {
                        this.bubble.Scaling = bubbleDefaultScale;
                        item.Icon.Scaling = item.DefaultScale;
                    }

                    item.Icon.Position = this.bubble.Position + new TGCVector2(7, 19);
                    this.bubble.Color =
                        this.Character.CanCraft(item)
                            ? Color.White
                            : Color.Black;
                    this.drawer.DrawSprite(this.bubble);
                    this.drawer.DrawSprite(item.Icon);
                    ++i;
                }

                this.cursor.Color = hovering ? Color.Yellow : this.cursorDefaultColor;
                this.drawer.DrawSprite(this.cursor);
                this.drawer.EndDrawSprite();
            }

            //if (stateID == StateID.INVENTORY)
            //{
            //    //text.drawText("count: " + count, 500, 270, Color.White);
            //    drawer.BeginDrawSprite();
            //    //int i = 1;
            //    foreach (var item in gameScene.Character.Inventory.Items)
            //    {
            //        //text.drawText("-" + i++ + ": " + item.Name + " | " + item.Description + " | " + item.type.ToString(), 500, 300 + 30 * i, Color.White);
            //        drawer.DrawSprite(bubble);
            //    }
            //    drawer.DrawSprite(PDA);
            //}
            if(this.itemHighlighted != null)
                this.drawText.drawText(this.itemHighlighted.Recipe.ToString(), 300, 300, 
                    this.Character.CanCraft(this.itemHighlighted) ?
                        Color.Aquamarine : Color.Red);
        }

        private bool cursorOverBubble()
        {
            return Cursor.Position.X >= this.bubble.Position.X &&
                   Cursor.Position.X <= this.bubble.Position.X + this.bubble.Bitmap.Width * this.bubble.Scaling.X &&
                   Cursor.Position.Y >= this.bubble.Position.Y &&
                   Cursor.Position.Y <= this.bubble.Position.Y + this.bubble.Bitmap.Height * this.bubble.Scaling.Y;
        }

        public void TakePDAIn(float elapsedTime)
        {
            this.PDAPositionX += this.PDAMoveCoefficient * elapsedTime;
            this.PDATransparency = CalculatePDATransparency();

            if (this.PDAPositionX > this.finalPDAPositionX)
            {
                this.PDAPositionX = this.finalPDAPositionX;
                SetNextState(StateID.CRAFTER);
            }

            this.PDA.Position = new TGCVector2(this.PDAPositionX, this.PDA.Position.Y);
            this.PDA.Color = Color.FromArgb(this.PDATransparency, this.PDA.Color.R, this.PDA.Color.G, this.PDA.Color.B);
            this.darknessCover.Color = Color.FromArgb(CalculaterBlacknessTransparency(), this.darknessCover.Color.R,
                this.darknessCover.Color.G, this.darknessCover.Color.B);
        }

        public void InventoryInteraction(float elapsedTime)
        {
            this.count = Items.Crafter.Crafteables.Count;
            this.cursor.Position = new TGCVector2(Cursor.Position.X, Cursor.Position.Y);
        }

        public void TakePDAOut(float elapsedTime)
        {
            this.PDAPositionX -= this.PDAMoveCoefficient * elapsedTime;
            this.PDATransparency = CalculatePDATransparency();

            if (this.PDAPositionX + this.PDA.Bitmap.Width * this.PDA.Scaling.X < 0)
            {
                this.PDAPositionX = GetPDAInitialPosition();
                SetNextState(StateID.IN);
                //this.shipScene.CloseCrafter();
            }

            this.PDA.Position = new TGCVector2(this.PDAPositionX, this.PDA.Position.Y);
            this.PDA.Color = Color.FromArgb(this.PDATransparency, this.PDA.Color.R, this.PDA.Color.G, this.PDA.Color.B);
            this.darknessCover.Color = Color.FromArgb(CalculaterBlacknessTransparency(), this.darknessCover.Color.R,
                this.darknessCover.Color.G, this.darknessCover.Color.B);
        }
    }
}