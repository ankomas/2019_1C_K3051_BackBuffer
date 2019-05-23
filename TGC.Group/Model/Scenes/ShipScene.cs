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
using TGC.Core.Text;
using TGC.Group.Model.Elements.RigidBodyFactories;
using TGC.Group.Model.Input;
using TGC.Group.Model.Scenes.Crafter;

namespace TGC.Group.Model.Scenes
{
    class ShipScene : Scene
    {
        TgcSkyBox walls;
        float rotation = 0;
        private readonly TgcText2D drawText = new TgcText2D();
        TGCVector3 viewDirectionStart = new TGCVector3(-1, 0.25f, 0);
        public delegate void Callback();
        private Callback onGoToWaterCallback = () => {}, onPauseCallback = () => {};
        private GameScene GameScene;
        private InventoryScene inventoryScene;
        //private Scene subScene;
        //private CrafterScene crafterScene;

        public ShipScene(TgcD3dInput input, GameScene gameScene) : base(input)
        {
            this.GameScene = gameScene;

            this.backgroundColor = Color.DarkOrange;

            walls = new TgcSkyBox();
            walls.SkyEpsilon = 0;
            walls.Center = new TGCVector3(0, 500, 0);
            walls.Size = new TGCVector3(500, 500, 1000);

            string baseDir = "../../../res/";
            walls.setFaceTexture(TgcSkyBox.SkyFaces.Back, baseDir +  "wall-1.jpg");
            walls.setFaceTexture(TgcSkyBox.SkyFaces.Down, baseDir +  "wall-1.jpg");
            walls.setFaceTexture(TgcSkyBox.SkyFaces.Front, baseDir + "wall-1.jpg");
            walls.setFaceTexture(TgcSkyBox.SkyFaces.Left, baseDir +  "wall-1.jpg");
            walls.setFaceTexture(TgcSkyBox.SkyFaces.Right, baseDir + "wall-1.jpg");
            walls.setFaceTexture(TgcSkyBox.SkyFaces.Up, baseDir +    "ceiling.jpg");

            walls.Init();
            //Camera = new CameraFPSGravity(walls.Center + new TGCVector3(0, 400, 0), Input);
            SetCamera(Input);
            inventoryScene = new InventoryScene(Input);

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
        private void SetCamera(TgcD3dInput input)
        {
            var position = new TGCVector3(0, 1000, 0);
            var rigidBody = new CapsuleFactory().Create(position, 100, 60);
            AquaticPhysics.Instance.Add(rigidBody);
            this.Camera = new Camera(position, input, rigidBody);
        }
        private void OpenInventory()
        {
            TurnExploreCommandsOff();
            ((Camera)Camera).IgnoreInput();
            inventoryScene.Open(this.GameScene.Character);
        }
        private void CloseInventory()
        {
            TurnExploreCommandsOn();
            ((Camera)Camera).ConsiderInput();
            inventoryScene.Close();
        }
        public override void Update(float elapsedTime)
        {
            if(Input.keyPressed(Key.Return))
            {
                onGoToWaterCallback();
            }
            if (Input.keyPressed(Key.Escape))
            {
                onPauseCallback();
            }
            inventoryScene.Update(elapsedTime);
        }
        public override void Render()
        {
            ClearScreen();

            walls.Render();

            inventoryScene.Render();
        }
        public ShipScene OnGoToWater(Callback onGoToWaterCallback)
        {
            this.onGoToWaterCallback = onGoToWaterCallback;
            return this;
        }
        public ShipScene OnPause(Callback onPauseCallback)
        {
            this.onPauseCallback = onPauseCallback;
            return this;
        }
        public override void ReactToInput()
        {
            base.ReactToInput();
            inventoryScene.ReactToInput();
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
}
