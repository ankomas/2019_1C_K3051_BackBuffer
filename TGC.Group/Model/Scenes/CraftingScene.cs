using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.Mathematica;
using TGC.Group.Model.Player;
using TGC.Group.Model.Items;
using TGC.Group.Model.Input;
using TGC.Core.Text;
using System.Drawing;
using TGC.Core.BoundingVolumes;
using TGC.Group.TGCUtils;
using TGC.Group.Model.Resources.Sprites;
using TGC.Group.Model.UI;
using TGC.Group.Model.Utils;

namespace TGC.Group.Model.Scenes
{
    class CraftingScene : Scene
    {
        TgcText2D text = new TgcText2D();
        Drawer2D drawer = new Drawer2D();
        CustomSprite bubble, cursor, arrow, hand;
        Color cursorDefaultColor;
        DialogBox dialogBox = new DialogBox();
        TGCVector2 bubbleInitialPos;
        ICrafteable crafted = null;
        bool aboutToTake = false;

        private TGCVector2 bubbleDefaultScale = new TGCVector2(.5f, .5f);

        private delegate void Renderer();
        Renderer renderer = () => {};
        private delegate void Updater(float elapsedTime);
        Updater updater = (e) => {};

        private Character Character;
        public Camera ShipCamera;
        private TGCVector3 targetPosition, initialPosition, initialLookAt;
        public ICrafteable ItemHighlighted { get; set; }
        private Items.Crafter crafter = new Items.Crafter();
        private List<bool> selectedItems = new List<bool>();
        private bool opened = false;

        public CraftingScene()
        {
            bubble = BitmapRepository.CreateSpriteFromBitmap(BitmapRepository.Bubble);
            bubble.Scaling = this.bubbleDefaultScale;
            bubbleInitialPos = new TGCVector2(
                Screen.Width / 2 - bubble.Bitmap.Width * bubbleDefaultScale.X / 2,
                Screen.Height / 2 - bubble.Bitmap.Height * bubbleDefaultScale.Y / 2
                );

            arrow = BitmapRepository.CreateSpriteFromBitmap(BitmapRepository.ArrowPointer);
            hand = BitmapRepository.CreateSpriteFromBitmap(BitmapRepository.Hand);
            hand.Scaling = new TGCVector2(0.5f, 0.5f);
            cursor = arrow;

            cursorDefaultColor = cursor.Color;

            foreach (var item in Items.Crafter.Crafteables)
            {
                selectedItems.Add(false);
            }
        }
        private TGCVector2 GetScaleForSpriteByPixels(CustomSprite sprite, int xPixels, int yPixels)
        {
            float pixelWidth = sprite.Bitmap.Width;
            float pixelHeight = sprite.Bitmap.Height;
            float xScale = xPixels / pixelWidth;
            float yScale = yPixels / pixelHeight;
            return new TGCVector2(xScale, yScale);
        }
        private bool cursorOverBubble()
        {
            return System.Windows.Forms.Cursor.Position.X >= this.bubble.Position.X &&
                   System.Windows.Forms.Cursor.Position.X <= this.bubble.Position.X + this.bubble.Bitmap.Width * this.bubble.Scaling.X &&
                   System.Windows.Forms.Cursor.Position.Y >= this.bubble.Position.Y &&
                   System.Windows.Forms.Cursor.Position.Y <= this.bubble.Position.Y + this.bubble.Bitmap.Height * this.bubble.Scaling.Y;
        }
        private void CenterIconToCurrentBubble(CustomSprite icon)
        {
            icon.Position = bubble.Position + new TGCVector2(
                (bubble.Bitmap.Width * bubble.Scaling.X - icon.Bitmap.Width * icon.Scaling.X) / 2,
                (bubble.Bitmap.Height * bubble.Scaling.Y - icon.Bitmap.Height * icon.Scaling.Y) / 2
                );
        }
        public void Render()
        {
            renderer();
        }

        public override void Update(float elapsedTime)
        {
            updater(elapsedTime);
        }

        public override void Render(TgcFrustum frustum)
        {
            this.Render();
        }

        private void MainUpdate(float elapsedTime)
        {
            cursor.Position = new TGCVector2(System.Windows.Forms.Cursor.Position.X, System.Windows.Forms.Cursor.Position.Y);
        }
        private void MainRender()
        {
            cursor = arrow;

            if (crafted != null)
            {
                var item = Items.Crafter.Crafteables.Find(elem => elem == crafted);
                item.Icon.Scaling = item.DefaultScale * 4;
                item.Icon.Position = new TGCVector2(
                    Screen.Width / 2 - item.Icon.Bitmap.Width * item.Icon.Scaling.X / 2,
                    Screen.Height / 2
                    );

                var cursorPos = System.Windows.Forms.Cursor.Position;


                if(
                    cursorPos.X > item.Icon.Position.X &&
                    cursorPos.X < item.Icon.Position.X + item.Icon.Bitmap.Width * item.Icon.Scaling.X &&
                    cursorPos.Y > item.Icon.Position.Y &&
                    cursorPos.Y < item.Icon.Position.Y + item.Icon.Bitmap.Height * item.Icon.Scaling.Y
                    )
                {
                    cursor = hand;
                    aboutToTake = true;
                }
                else
                {
                    aboutToTake = false;
                }
                

                drawer.BeginDrawSprite();
                drawer.DrawSprite(item.Icon);
                drawer.DrawSprite(cursor);
                drawer.EndDrawSprite();

                return;
            }

            bool hovering = false;
            int i = 0;
            dialogBox.Clear();

            foreach (var item in Items.Crafter.Crafteables)
            {
                item.Icon.Scaling = item.DefaultScale + this.GetScaleForSpriteByPixels(item.Icon, 10, 10);

                bubble.Position = new TGCVector2(bubbleInitialPos.X, bubbleInitialPos.Y + i++ * bubble.Bitmap.Height * bubble.Scaling.Y);
                CenterIconToCurrentBubble(item.Icon);

                if (cursorOverBubble())
                {
                    //bubble.Color = Color.Red;
                    ItemHighlighted = item;
                    selectedItems[Items.Crafter.Crafteables.IndexOf(item)] = true;
                    hovering = true;
                    dialogBox.AddLineBig(item.Name + "       ", this.Character.CanCraft(item) ? Color.FromArgb(255, 0, 255, 0) : Color.Red);
                    dialogBox.AddLineSmall("(" + item.type.ToString() + ")", Color.Gray);
                    dialogBox.AddLineSmall("____________________");
                    dialogBox.AddLineSmall(item.Description);
                    dialogBox.AddLineSmall("____________________");
                    dialogBox.AddLineSmall("Recipe:");
                    foreach (var ingredient in item.Recipe.Ingredients)
                    {
                        bool fulfills = this.Character.Inventory.Items.Where(it => it.Name.Equals(ingredient.Item.Name)).Count() >= ingredient.Quantity;
                        dialogBox.AddLineSmall(ingredient.Item.Name + ": " + ingredient.Quantity, fulfills ? Color.FromArgb(255, 0, 255, 0) : Color.Red);
                    }
                }
                else
                {
                    selectedItems[Items.Crafter.Crafteables.IndexOf(item)] = false;
                }

                this.bubble.Color =
                        this.Character.CanCraft(item)
                            ? Color.White
                            : Color.Black;

                //item.Icon.Color =
                //        this.Character.CanCraft(item)
                //            ? Color.White
                //            : Color.FromArgb(255, 40, 40, 40);

                drawer.BeginDrawSprite();
                drawer.DrawSprite(bubble);
                drawer.DrawSprite(item.Icon);
                drawer.EndDrawSprite();
            }

            if (hovering) dialogBox.Render();

            this.cursor.Color = hovering ? Color.FromArgb(255, 0, 255, 0) : this.cursorDefaultColor;
            drawer.BeginDrawSprite();
            drawer.DrawSprite(cursor);
            drawer.EndDrawSprite();
        }
        public void BeginningAnimation(float elapsedTime)
        {
            var currentPosition = ShipCamera.Position;

            var dist = targetPosition - currentPosition;
            var normalDirection = TGCVector3.Normalize(dist);

            var newPosition =
                dist.Length() < 1 ?
                targetPosition : currentPosition + normalDirection * elapsedTime * 1000;

            ShipCamera.SetCamera(newPosition, ShipCamera.LookAt);

            if(newPosition == targetPosition)
            {
                updater = MainUpdate;
                renderer = MainRender;
                Console.WriteLine("Call 1");
                CraftCommandsOn();
            }
        }
        private void CraftCommandsOn()
        {
            Console.WriteLine("Craft on");

            pressed[GameInput.Accept] = () =>
            {
                Console.WriteLine("Craft");
                int index = this.selectedItems.IndexOf(true);

                Console.WriteLine("index: " + index);

                if (index == -1) return;

                var item = Items.Crafter.Crafteables[index];

                if (!this.Character.CanCraft(item)) return;

                Console.WriteLine("can craft");

                selectedItems = selectedItems.ConvertAll(_ => false);
                StartCrafting(item);
            };
        }
        private void TakeCraftedItemCommandsOn()
        {
            Console.WriteLine("Take on");
            pressed[GameInput.Accept] = () =>
            {
                Console.WriteLine("Take");
                if (true)
                {
                    this.crafter.Craft(crafted, this.Character);
                    this.Character.GiveItem(this.crafter.CraftedItem);
                    this.crafter.CraftedItem = null;
                    this.crafted = null;
                    Console.WriteLine("Call 2");
                    CraftCommandsOn();
                }
            };
        }
        private void StartCrafting(ICrafteable item)
        {
            crafted = item;
            this.cursor.Color = this.cursorDefaultColor;
            TakeCraftedItemCommandsOn();
        }
        public void EndAnimation(float elapsedTime)
        {
            var currentPosition = ShipCamera.Position;

            var dist = initialPosition - currentPosition;
            var normalDirection = TGCVector3.Normalize(dist);

            var newPosition =
                dist.Length() < 1 ?
                targetPosition : currentPosition + normalDirection * elapsedTime * 1000;

            ShipCamera.SetCamera(newPosition, ShipCamera.LookAt);

            if (newPosition == targetPosition)
            {
                ShipCamera.StopUsingManually();
                updater = (e) => {};
                renderer = () => {};
            }
        }

        public void Open(Character character, Camera shipCamera, TGCVector3 crafterPosition)
        {
            Console.WriteLine("Open");

            this.Character = character;
            this.ShipCamera = shipCamera;
            this.initialPosition = shipCamera.Position;
            this.initialLookAt = crafterPosition;
            this.targetPosition = crafterPosition + new TGCVector3(0, 0, 250);

            ShipCamera.UseManually();

            ShipCamera.SetCamera(ShipCamera.Position, crafterPosition);

            updater = BeginningAnimation;

            this.opened = true;
        }
        public void Close()
        {
            if(!this.opened) return;
            ShipCamera.SetCamera(ShipCamera.Position, this.initialLookAt);
            updater = EndAnimation;
            renderer = () => {};
            pressed[GameInput.Accept] = () => {};
        }
    }
}
