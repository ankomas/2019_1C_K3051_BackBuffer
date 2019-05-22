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
        private Scene subScene;
        private CrafterScene crafterScene;

        public ShipScene(TgcD3dInput input, GameScene gameScene) : base(input)
        {
            this.GameScene = gameScene;
            subScene = Scene.Empty;
            this.initCrafterScene();

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

            TurnExploreCommandsOn();
        }
        
        private void TurnExploreCommandsOn()
        {
            pressed[GameInput._Inventory] = this.OpenCrafter;
        }
        private void TurnExploreCommandsOff()
        {
            pressed[GameInput._Inventory] = () => { };
        }
        private void initCrafterScene()
        {
            this.crafterScene = new CrafterScene(Input, this.GameScene, this);
        }
        private void OpenCrafter()
        {
            ((Camera)this.Camera).IgnoreInput();
            subScene = this.crafterScene;
            Input.update();
            TurnExploreCommandsOff();
        }
        public void CloseCrafter()
        {
            subScene = Scene.Empty;
            TurnExploreCommandsOn();
            ((Camera)Camera).ConsiderInput();
        }
        private void SetCamera(TgcD3dInput input)
        {
            var position = new TGCVector3(0, 1000, 0);
            var rigidBody = new CapsuleFactory().Create(position, 100, 60);
            AquaticPhysics.Instance.Add(rigidBody);
            this.Camera = new Camera(position, input, rigidBody);
        }
        public override void Render()
        {
            ClearScreen();

            walls.Render();

            this.subScene.Render();
            
            drawText.drawText("Press TAB or I to open crafting menu, ENTER to exit ship.", 30, 30, Color.White);
            drawText.drawText("Collect 5 corals to craft potion.", 30, 60, Color.White);
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
            this.subScene.ReactToInput();
            subScene.Update(elapsedTime);
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
    }
}
