﻿using System;
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
using TGC.Core.SceneLoader;

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
        public CameraFPSGravity ShipCamera;
        private TGCVector3 targetPosition, targetLookAt, initialPosition, initialLookAt;
        public ICrafteable ItemHighlighted { get; set; }
        private Items.Crafter logicalCrafter = new Items.Crafter();
        private List<bool> hoveredItems = new List<bool>();
        private Things.Crafter physicalCrafter;

        List<TgcMesh> crafted3DModel;

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
                hoveredItems.Add(false);
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
        public new void Render()
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
            physicalCrafter.Open(elapsedTime);
            cursor.Position = new TGCVector2(System.Windows.Forms.Cursor.Position.X, System.Windows.Forms.Cursor.Position.Y);
        }
        private void MainRender()
        {
            cursor = arrow;

            if (crafted != null)
            {
                var item = Items.Crafter.Crafteables.Find(elem => elem == crafted);

                foreach(var mesh in crafted3DModel)
                {
                    mesh.RotateY(0.025f);
                    mesh.Render();
                }

                item.Icon.Scaling = item.DefaultScale * 4;
                item.Icon.Position = new TGCVector2(
                    Screen.Width / 2 - item.Icon.Bitmap.Width * item.Icon.Scaling.X / 2,
                    Screen.Height / 2
                    );

                var cursorPos = System.Windows.Forms.Cursor.Position;

                if(
                    cursorPos.X > Screen.Width  * 0.4f &&
                    cursorPos.X < Screen.Width  * 0.6f &&
                    cursorPos.Y > Screen.Height * 0.3f &&
                    cursorPos.Y < Screen.Height * 0.8f
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
                //drawer.DrawSprite(item.Icon);
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
                    hoveredItems[Items.Crafter.Crafteables.IndexOf(item)] = true;
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
                    hoveredItems[Items.Crafter.Crafteables.IndexOf(item)] = false;
                }

                this.bubble.Color =
                        this.Character.CanCraft(item)
                            ? Color.White
                            : Color.Black;

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

            ShipCamera.SetCamera(newPosition, targetLookAt);
            physicalCrafter.Open(elapsedTime);

            if (newPosition == targetPosition)
            {
                updater = MainUpdate;
                renderer = MainRender;
                CraftCommandsOn();
                if (crafted != null) TakeCraftedItemCommandsOn();
            }
        }
        private void CraftCommandsOn()
        {
            pressed[GameInput.Accept] = () =>
            {
                int index = this.hoveredItems.IndexOf(true);

                if (index == -1) return;

                var item = Items.Crafter.Crafteables[index];

                if (!this.Character.CanCraft(item)) return;

                hoveredItems = hoveredItems.ConvertAll(_ => false);

                TGCVector3 size = item.Meshes[0].BoundingBox.calculateSize();
                float scale = 100f / size.Y;
                crafted3DModel = item.Meshes;
                foreach (var mesh in crafted3DModel)
                {
                    mesh.Scale = new TGCVector3(scale, scale, scale);
                    mesh.Position = physicalCrafter.Center + new TGCVector3(0, -50, 40);
                    mesh.RotateZ((float)Math.PI / 8);
                }

                StartCrafting(item);
            };
        }
        private void TakeCraftedItemCommandsOn()
        {
            pressed[GameInput.Accept] = () =>
            {
                if (aboutToTake)
                {
                    this.logicalCrafter.Craft(crafted, this.Character);
                    this.Character.GiveItem(this.logicalCrafter.CraftedItem);
                    this.logicalCrafter.CraftedItem = null;
                    this.crafted = null;
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
                initialPosition : currentPosition + normalDirection * elapsedTime * 1000;

            ShipCamera.SetCamera(newPosition, targetLookAt);
            physicalCrafter.Close(elapsedTime);

            if (newPosition == initialPosition)
            {
                ShipCamera.StopUsingManually();
                updater = physicalCrafter.Close;
                renderer = () => {};
            }
        }

        public void Open(Character character, CameraFPSGravity shipCamera, Things.Crafter crafter)
        {
            this.physicalCrafter = crafter;
            this.Character = character;
            this.ShipCamera = shipCamera;
            this.initialPosition = shipCamera.Position;
            this.initialLookAt = shipCamera.LookAt;
            this.targetPosition = crafter.Center + new TGCVector3(0, 0, 250);
            this.targetLookAt = crafter.Center;

            ShipCamera.UseManually();

            ShipCamera.SetCamera(ShipCamera.Position, crafter.Center);

            updater = BeginningAnimation;
        }
        public void Close()
        {
            ShipCamera.SetCamera(ShipCamera.Position, this.initialLookAt);
            updater = EndAnimation;
            renderer = () => {};
            pressed[GameInput.Accept] = () => {};
        }
    }
}
