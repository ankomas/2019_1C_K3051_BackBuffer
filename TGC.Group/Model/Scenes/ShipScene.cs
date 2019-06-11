using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BulletSharp;
using TGC.Core.Camara;
using TGC.Core.Input;
using TGC.Core.Mathematica;
using TGC.Core.Terrain;
using Microsoft.DirectX.DirectInput;
using TGC.Core.BoundingVolumes;
using Microsoft.DirectX.Direct3D;
using TGC.Core.Text;
using TGC.Group.Model.Elements.RigidBodyFactories;
using TGC.Group.Model.Input;
using TGC.Core.SceneLoader;
using TGC.Core.Direct3D;
using TGC.Group.Model.Items;
using TGC.Group.Model.Player;
using TGC.Group.TGCUtils;
using TGC.Group.Model.Scenes.Crafter;
using Effect = Microsoft.DirectX.Direct3D.Effect;
using TGC.Group.Model.Things;

namespace TGC.Group.Model.Scenes
{
    class ShipScene : GameplayScene
    {
        float rotation = 0;
        private readonly TgcText2D drawText = new TgcText2D();
        TGCVector3 viewDirectionStart = new TGCVector3(-1, 0.25f, 0);
        public delegate void Callback();
        private Callback onPauseCallback = () => {};
        private TransitionCallback onGoToWaterCallback = (gameState) => {};
        private InventoryScene inventoryScene;
        private CraftingScene craftingScene;

        List<Thing> selectableThings = new List<Thing>();
        Things.Hatch hatch;
        Things.Crafter crafter;
        Things.Ship ship;
        Things.Seat seat;
        Things.LifeBelt lifeBelt;
        Drawer2D drawer2D = new Drawer2D();

        TgcText2D DrawText = new TgcText2D();
        string debug;

        TGCVector3 targertPosition;
        TGCVector3 targetLookAt;
        public ShipScene(GameState gameState) : base(gameState)
        {
            this.backgroundColor = Color.DarkOrange;

            ship = new Ship();
            ship.Scale = new TGCVector3(8, 12, 16);
            ship.Position = new TGCVector3(200, 700, -300);

            //var shipRigidBody = new BoxFactory().Create(shipMesh.BoundingBox);
            //AquaticPhysics.Instance.Add(shipRigidBody);
            lifeBelt = new Things.LifeBelt();
            lifeBelt.Position = new TGCVector3(285, 1000, 250);
            lifeBelt.RotateY(-(float)Math.PI * 0.1f);

            seat = new Things.Seat();
            seat.Position = new TGCVector3(550, 710, 1440);
            seat.RotateY((float)Math.PI / 2);
            seat.Scale = new TGCVector3(2f, 2f, 2f);

            crafter = new Things.Crafter(OpenCrafter);
            crafter.relativeScales = new List<float>(new float[3] {1, .6f, .38f});
            crafter.relativePositions = new List<TGCVector3>(new TGCVector3[3] {
                new TGCVector3(0, 0, 0),
                new TGCVector3(42, 49, 30),
                new TGCVector3(43, 177, 31)
            });
            crafter.Scale = new TGCVector3(.5f, .5f, .5f);
            crafter.Position = new TGCVector3(560, 950, -250);

            hatch = new Things.Hatch(() => onGoToWaterCallback(this.GameState));
            hatch.Position = new TGCVector3(600, 720, 450);

            SetCamera();
            AquaticPhysics.Instance.Add(Camera.RigidBody);
            
            selectableThings.Add(crafter);
            selectableThings.Add(hatch);

            inventoryScene = new InventoryScene();

            RegisterSubscene(inventoryScene);
            initCraftingScene();

            TurnExploreCommandsOn();

            pressed[Key.P] = () =>
            {
                onPauseCallback();
            };
            
            GameState.character.Weapon = new InfinityGauntlet();
        }

        private void TryToInteractWithSelectableThing()
        {
            foreach (Thing thing in selectableThings)
            {
                if (thing.Looked)
                {
                    thing.ExecuteAction();
                }
            }
        }
        private void TurnExploreCommandsOn()
        {
            pressed[GameInput.Inventory] = OpenInventory;
            pressed[GameInput.Accept] = TryToInteractWithSelectableThing;
        }
        private void TurnExploreCommandsOff()
        {
            pressed[GameInput.Inventory] = CloseInventory;
            pressed[GameInput.Accept] = () => {};
        }
        public void ResetCamera()
        {
            AquaticPhysics.Instance.Remove(Camera.RigidBody);
            SetCamera();
            AquaticPhysics.Instance.Add(Camera.RigidBody);
        }

        private void SetCamera()
        {
            this.Camera = CameraFactory.Create(new TGCVector3(675, 1000, 900), Input);

        }

        private void OpenInventory()
        {
            cursor = null;
            TurnExploreCommandsOff();
            Camera.IgnoreInput();
            inventoryScene.Open(this.GameState.character);
        }
        private void CloseInventory()
        {
            cursor = aim;
            TurnExploreCommandsOn();
            Camera.ConsiderInput();
            inventoryScene.Close();
        }
        private void TellIfCameraIsLookingAtThing(Thing thing)
        {
            TGCVector3 dist = thing.Center - Camera.Position;

            bool isClose = Math.Abs(dist.Length()) - D3DDevice.Instance.ZNearPlaneDistance < 500;

            dist.Normalize();

            TGCVector3 normalLookAt = TGCVector3.Normalize(Camera.LookAt - Camera.Position);

            float dot = TGCVector3.Dot(dist, normalLookAt);

            thing.Looked = isClose && dot > 0.985;
        }

        public override void UpdateGameplay(float elapsedTime)
        {
            
            this.GameState.character.UpdateStats(new Stats(elapsedTime * this.GameState.character.MaxStats.Oxygen/3, 0));
            inventoryScene.Update(elapsedTime);
            craftingScene.Update(elapsedTime);
            selectableThings.ForEach(TellIfCameraIsLookingAtThing);
            
            GameState.character.Update(Camera);
        }

        public override void Render(TgcFrustum tgcFrustum)
        {
            ClearScreen();

            GameState.character.Render();
            ship.TellCameraPosition(new float[4] { Camera.Position.X, Camera.Position.Y, Camera.Position.Z, 1 });
            ship.Render();

            foreach (var thing in selectableThings)
            {
                thing.TellCameraPosition(new float[4]{ Camera.Position.X, Camera.Position.Y, Camera.Position.Z, 1});
                thing.Render();
                if (thing.Looked)
                {
                    dialogBox.Clear();
                    if (cursor != null)
                    {
                        thing.BoundingBox.Render();
                        dialogBox.AddLineSmall(thing.name);
                        dialogBox.AddLineSmall("------------");
                        dialogBox.AddLineSmall(thing.actionDescription);
                        dialogBox.Render();
                        cursor = hand;
                    }
                }
            }

            lifeBelt.Position = new TGCVector3(285, 1000, 250);
            lifeBelt.Render();
            lifeBelt.Position = new TGCVector3(330, 1000, 100);
            lifeBelt.Render();
            lifeBelt.Position = new TGCVector3(375, 1000, -50);
            lifeBelt.Render();

            seat.Render();

            if(cursor != null)
            {
                drawer2D.BeginDrawSprite();
                drawer2D.DrawSprite(cursor);
                drawer2D.EndDrawSprite();

                cursor = aim;
            }

            inventoryScene.Render();
            craftingScene.Render();

            statsIndicators.Render(this.GameState.character);
            //this.drawText.drawText("Pause: P\nInventory: TAB\nExit ship: click the hatch in the floor\nCraft: click the crafter, press ESC to exit crafting",
            //    300, 300, Color.NavajoWhite);

            //this.drawText.drawText("Camera:", 800, 100, Color.Red);
            //this.drawText.drawText("X: " + Camera.Position.X, 800, 130, Color.White);
            //this.drawText.drawText("Y: " + Camera.Position.Y, 800, 160, Color.White);
            //this.drawText.drawText("Z: " + Camera.Position.Z, 800, 190, Color.White);
        }
        public ShipScene OnGoToWater(TransitionCallback onGoToWaterCallback)
        {
            this.onGoToWaterCallback = onGoToWaterCallback;
            return this;
        }
        public ShipScene OnPause(Callback onPauseCallback)
        {
            this.onPauseCallback = onPauseCallback;
            return this;
        }
        private void initCraftingScene()
        {
            craftingScene = new CraftingScene();
            RegisterSubscene(craftingScene);
        }
        private void OpenCrafter()
        {
            cursor = null;
            TurnExploreCommandsOff();
            craftingScene.Open(this.GameState.character, Camera, this.crafter);
            pressed[GameInput.GoBack] = CloseCrafter;
        }
        public void CloseCrafter()
        {
            cursor = hand;
            pressed[GameInput.GoBack] = () => {};
            TurnExploreCommandsOn();
            craftingScene.Close();
        }
    }
}
