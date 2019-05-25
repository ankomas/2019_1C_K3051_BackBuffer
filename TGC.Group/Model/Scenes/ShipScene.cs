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
        TgcMesh crafter;

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


            crafter = new TgcSceneLoader()
                .loadSceneFromFile(Game.Default.MediaDirectory + "crafter-v8-TgcScene.xml").Meshes[0];

            //crafter.Transform.Translate(0, 500, 0);

            crafter.Scale = new TGCVector3(.5f, .5f, .5f);

            crafter.Position = new TGCVector3(-250, 950, 0);
            crafter.UpdateMeshTransform();
            crafter.RotateY((float)Math.PI / 2);


            //crafter.Effect = effect;
            //crafter.Technique = "CrafterTechnique";
            //crafter.updateBoundingBox();

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
            inventoryScene.Open(this.GameState.character);
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
                onGoToWaterCallback(this.GameState);
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
            crafter.Render();

            inventoryScene.Render();
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
}
