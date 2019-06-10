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

namespace TGC.Group.Model.Scenes
{
    class ShipScene : GameplayScene
    {
        TgcSkyBox walls;
        float rotation = 0;
        private readonly TgcText2D drawText = new TgcText2D();
        TGCVector3 viewDirectionStart = new TGCVector3(-1, 0.25f, 0);
        public delegate void Callback();
        private Callback onPauseCallback = () => {};
        private TransitionCallback onGoToWaterCallback = (gameState) => {};
        private InventoryScene inventoryScene;
        private CraftingScene craftingScene;

        Microsoft.DirectX.Direct3D.Effect effect;
        TgcScene crafterTgcScene;
        TgcMesh crafterMesh, shipMesh, hatchMesh;
        List<Thing> selectableThings = new List<Thing>();
        Thing crafter, hatch;
        Drawer2D drawer2D = new Drawer2D();

        TgcText2D DrawText = new TgcText2D();
        string debug;

        TGCVector3 targertPosition;
        TGCVector3 targetLookAt;
        
        public ShipScene(GameState gameState) : base(gameState)
        {
            this.backgroundColor = Color.DarkOrange;

            walls = new TgcSkyBox();
            walls.SkyEpsilon = 0;
            walls.Center = new TGCVector3(0, 500, 0);
            walls.Size = new TGCVector3(500, 500, 1000);

            string baseDir = "../../../res/";
            walls.setFaceTexture(TgcSkyBox.SkyFaces.Back,  baseDir + "wall-1.jpg");
            walls.setFaceTexture(TgcSkyBox.SkyFaces.Down,  baseDir + "wall-1.jpg");
            walls.setFaceTexture(TgcSkyBox.SkyFaces.Front, baseDir + "wall-1.jpg");
            walls.setFaceTexture(TgcSkyBox.SkyFaces.Left,  baseDir + "wall-1.jpg");
            walls.setFaceTexture(TgcSkyBox.SkyFaces.Right, baseDir + "wall-1.jpg");
            walls.setFaceTexture(TgcSkyBox.SkyFaces.Up,    baseDir + "ceiling.jpg");

            string errors;
            effect = Microsoft.DirectX.Direct3D.Effect.FromFile(D3DDevice.Instance.Device,
                Game.Default.ShadersDirectory + "Crafter.fx",
                null, null,
                ShaderFlags.None, null, out errors
                );

            crafterMesh = new TgcSceneLoader()
                .loadSceneFromFile(Game.Default.MediaDirectory + "crafter-v8-TgcScene.xml").Meshes[0];

            shipMesh = new TgcSceneLoader()
                .loadSceneFromFile(Game.Default.MediaDirectory + "new-ship-2-TgcScene.xml").Meshes[0];
            
            hatchMesh = new TgcSceneLoader()
                .loadSceneFromFile(Game.Default.MediaDirectory + "hatch-TgcScene.xml").Meshes[0];

            crafterMesh.Scale = new TGCVector3(.5f, .5f, .5f);
            crafterMesh.Position = new TGCVector3(600, 950, -300);

            shipMesh.Scale = new TGCVector3(16, 12, 16);
            shipMesh.Position = new TGCVector3(-250, 700, -300);

            hatchMesh.Position = new TGCVector3(600, 700, 450);

            crafter = new Thing(crafterMesh, "Crafter", "Start crafting", OpenCrafter);
            hatch = new Thing(hatchMesh, "Hatch", "Exit ship", () => onGoToWaterCallback(this.GameState));
            selectableThings.Add(crafter);
            selectableThings.Add(hatch);

            walls.Init();
            SetCamera();
            AquaticPhysics.Instance.Add(Camera.RigidBody);
            
            inventoryScene = new InventoryScene();

            RegisterSubscene(inventoryScene);
            initCraftingScene();

            TurnExploreCommandsOn();

            pressed[Key.P] = () =>
            {
                onPauseCallback();
            };
            pressed[GameInput.GoBack] = CloseCrafter;
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
            TurnExploreCommandsOn();
            Camera.ConsiderInput();
            inventoryScene.Close();
        }
        private void TellIfCameraIsLookingAtThing(Thing thing)
        {
            TGCVector3 dist = thing.Position - Camera.Position;

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

            shipMesh.Render();
            
            GameState.character.Render();
            
            foreach (var thing in selectableThings)
            {
                thing.Render();
                if (thing.Looked)
                {
                    dialogBox.Clear();
                    if (cursor != null) 
                    {
                        thing.mesh.BoundingBox.Render();
                        dialogBox.AddLineSmall(thing.name);
                        dialogBox.AddLineSmall("------------");
                        dialogBox.AddLineSmall(thing.actionDescription);
                        dialogBox.Render();
                        cursor = hand;
                    }
                }
            }

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
            
            this.drawText.drawText("Pause: P\nInventory: TAB\nExit ship: click the hatch in the floor\nCraft: click the crafter, press ESC to exit crafting",
                300, 300, Color.NavajoWhite);
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
            craftingScene.Open(this.GameState.character, Camera, this.crafter.Position);
        }
        public void CloseCrafter()
        {
            cursor = hand;
            TurnExploreCommandsOn();
            craftingScene.Close();
        }
    }

    class Thing
    {
        public string debug;
        public TgcMesh mesh;
        public bool Looked = false;
        public string name, actionDescription;
        private ShipScene.Callback action = () => {};
        public TGCVector3 Position => mesh.BoundingBox.PMin + TGCVector3.Multiply(mesh.BoundingBox.PMax - mesh.BoundingBox.PMin, 0.5f);

        public Thing(TgcMesh mesh, string name, string actionDescription, ShipScene.Callback action)
        {
            this.mesh = mesh;
            this.name = name;
            this.actionDescription = actionDescription;
            this.action = action;
        }
        public void Render()
        {
            mesh.Render();
        }
        public void ExecuteAction()
        {
            action();
        }
    }
}
