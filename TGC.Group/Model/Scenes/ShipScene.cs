using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.Camara;
using TGC.Core.Input;
using TGC.Core.Mathematica;
using TGC.Core.Terrain;
using Microsoft.DirectX.DirectInput;
using Microsoft.DirectX.Direct3D;
using TGC.Core.Text;
using TGC.Group.Model.Elements.RigidBodyFactories;
using TGC.Group.Model.Input;
using TGC.Core.SceneLoader;
using TGC.Core.Direct3D;
using TGC.Group.Model.Player;
using TGC.Group.TGCUtils;

namespace TGC.Group.Model.Scenes
{
    class ShipScene : GameAbstractScene
    {
        TgcSkyBox walls;
        float rotation = 0;
        private readonly TgcText2D drawText = new TgcText2D();
        TGCVector3 viewDirectionStart = new TGCVector3(-1, 0.25f, 0);
        public delegate void Callback();
        private Callback onPauseCallback = () => {};
        private TransitionCallback onGoToWaterCallback = (gameState) => {};
        private InventoryScene inventoryScene;
        //private Scene subScene;
        //private CrafterScene crafterScene;

        Microsoft.DirectX.Direct3D.Effect effect;
        TgcScene crafterTgcScene;
        TgcMesh crafterMesh, shipMesh, hatchMesh;
        List<Thing> selectableThings = new List<Thing>();
        Drawer2D drawer2D = new Drawer2D(); 

        TgcText2D DrawText = new TgcText2D();
        string debug;

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

            selectableThings.Add(new Thing(crafterMesh, "Crafter", "Start crafting"));
            selectableThings.Add(new Thing(hatchMesh, "Hatch", "Exit ship"));

            walls.Init();
            //Camera = new CameraFPSGravity(walls.Center + new TGCVector3(0, 400, 0), Input);
            SetCamera(Input);
            inventoryScene = new InventoryScene();

            RegisterSubscene(inventoryScene);

            TurnExploreCommandsOn();
        }
        
        private void TurnExploreCommandsOn()
        {
            pressed[GameInput._Inventory] = OpenInventory;
        }
        private void TurnExploreCommandsOff()
        {
            pressed[GameInput._Inventory] = CloseInventory;
        }
        public void ResetCamera()
        {
            SetCamera(Input);
        }
        private void SetCamera(TgcD3dInput input)
        {
            var position = new TGCVector3(675, 1000, 900);
            var rigidBody = new CapsuleFactory().Create(position, 100, 60);
            AquaticPhysics.Instance.Add(rigidBody);
            this.Camera = new Camera(position, input, rigidBody);
        }
        private void OpenInventory()
        {
            TurnExploreCommandsOff();
            ((Camera)Camera).IgnoreInput();
            inventoryScene.Open(this.GameState.character);
        }
        private void CloseInventory()
        {
            TurnExploreCommandsOn();
            ((Camera)Camera).ConsiderInput();
            inventoryScene.Close();
        }
        private void TellIfCameraIsLookingAtThing(Thing thing)
        {
            TGCVector3 dist = thing.Position - Camera.Position;

            bool isClose = Math.Abs(dist.Length()) - D3DDevice.Instance.ZNearPlaneDistance < 600;

            dist.Normalize();

            TGCVector3 normalLookAt = TGCVector3.Normalize(Camera.LookAt - Camera.Position);

            float dot = TGCVector3.Dot(dist, normalLookAt);

            thing.Looked = isClose && dot > 0.985;
        }
        public override void Update(float elapsedTime)
        {
            if(Input.keyPressed(Key.Return))
            {
                onGoToWaterCallback(this.GameState);
            }
            if (Input.keyPressed(Key.Escape))
            {
                onPauseCallback();
            }

            this.GameState.character.UpdateStats(new Stats(elapsedTime * 7, 0));

            inventoryScene.Update(elapsedTime);

            selectableThings.ForEach(TellIfCameraIsLookingAtThing);
        }
        public override void Render()
        {
            ClearScreen();

            shipMesh.Render();

            foreach (var thing in selectableThings)
            {
                thing.Render();
                if (thing.Looked)
                {
                    thing.mesh.BoundingBox.Render();
                    dialogBox.Clear();
                    dialogBox.AddLine(thing.name);
                    dialogBox.AddLine("------------");
                    dialogBox.AddLine(thing.actionDescription);
                    dialogBox.Render();
                    cursor = hand;
                }
            }

            drawer2D.BeginDrawSprite();
            drawer2D.DrawSprite(cursor);
            drawer2D.EndDrawSprite();

            inventoryScene.Render();
            statsIndicators.Render(this.GameState.character);

            cursor = aim;
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
        //private void initCrafterScene()
        //{
        //    this.crafterScene = new CrafterScene(Input, this.GameScene, this);
        //}
        //private void OpenCrafter()
        //{
        //    ((Camera)this.Camera).IgnoreInput();
        //    subScene = this.crafterScene;
        //    Input.update();
        //    TurnExploreCommandsOff();
        //}
        //public void CloseCrafter()
        //{
        //    subScene = Scene.Empty;
        //    TurnExploreCommandsOn();
        //    ((Camera)Camera).ConsiderInput();
        //}
    }

    class Thing
    {
        public string debug;
        public TgcMesh mesh;
        public bool Looked = false;
        public string name, actionDescription;
        public TGCVector3 Position => mesh.BoundingBox.PMin + TGCVector3.Multiply(mesh.BoundingBox.PMax - mesh.BoundingBox.PMin, 0.5f);

        public Thing(TgcMesh mesh, string name, string actionDescription)
        {
            this.mesh = mesh;
            this.name = name;
            this.actionDescription = actionDescription;
        }
        public void Render()
        {
            mesh.Render();
        }
    }
}
