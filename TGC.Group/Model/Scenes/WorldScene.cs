using System.Drawing;
using TGC.Core.Input;
using TGC.Core.Text;
using TGC.Core.Mathematica;
using TGC.Group.Model.Elements;
using TGC.Group.Model.Input;
using TGC.Core.Terrain;
using Microsoft.DirectX;
using TGC.Group.TGCUtils;
using TGC.Group.Model.Resources.Sprites;
using Microsoft.DirectX.Direct3D;
using TGC.Group.Model.Items;
using TGC.Group.Model.Items.Equipment;
using TGC.Group.Model.Player;
using TGC.Group.Model.Elements.RigidBodyFactories;
using TGC.Core.Direct3D;
using Key = Microsoft.DirectX.DirectInput.Key;
using Screen = TGC.Group.Model.Utils.Screen;
using System;
using System.Collections.Generic;
using System.Linq;
using BulletSharp;
using TGC.Core.BoundingVolumes;
using TGC.Group.Model.UI;
using TGC.Group.Model.Utils;
using Chunk = TGC.Group.Model.Chunks.Chunk;
using Element = TGC.Group.Model.Elements.Element;
using Vector3 = BulletSharp.Math.Vector3;
using TGC.Core.SceneLoader;

namespace TGC.Group.Model.Scenes
{
    class WorldScene : GameplayScene
    {
        private readonly TgcText2D DrawText = new TgcText2D();
        private World World { get; }
        private bool BoundingBox { get; set; }
        
        //TODO remove
        private SpawnRate goldRate = new SpawnRate(1, 4);

        string baseDir = "../../../res/";

        public delegate void Callback();
        Callback onPauseCallback = () => {}, onGameOverCallback = () => {};
        TransitionCallback onGetIntoShipCallback = (gameState) => {};

        InventoryScene inventoryScene;

        TgcSkyBox skyBoxUnderwater, skyBoxOutside;
        CustomSprite waterVision, mask, dialogBox;
        Drawer2D drawer = new Drawer2D();
        string dialogName, dialogDescription;
        internal Character Character { get { return this.GameState.character; } }

        private bool gaveOxygenTank = false; //TODO remove
        private bool aimFired = false;

        TgcMesh skb;
        public WorldScene(GameState gameState) : base(gameState)
        {
            backgroundColor = Color.FromArgb(255, 78, 129, 179);

            this.World = new World(new TGCVector3(0, 0, 0));

            SetCamera(Input);

            IncrementFarPlane(3f);
            SetClampTextureAddressing();
            InitInventoryScene();
            InitSkyBoxes();
            InitWaterVision();
            InitMask();
            InitDialogBox();
            
            World = new World(new TGCVector3(0, 0, 0));
            
            pressed[Key.Escape] = () => {
                onPauseCallback();
            };

            // Die with Q for debugging or god mode
            //pressed[Key.Q] = () => {
            //    onGameOverCallback();
            //};

            pressed[Key.F] = () => {
                this.BoundingBox = !this.BoundingBox;
            };

            RegisterSubscene(inventoryScene);

            TurnExploreCommandsOn();
        }
        public void ResetCamera()
        {
            SetCamera(Input);
        }
        private void TurnExploreCommandsOn()
        {
            pressed[GameInput._Inventory] = OpenInventory;
            pressed[GameInput._Enter] = () => aimFired = true;
        }
        private void TurnExploreCommandsOff()
        {
            pressed[GameInput._Inventory] = CloseInventory;
            pressed[GameInput._Enter] = () => {};
        }
        private void OpenInventory()
        {
            ((Camera)Camera).IgnoreInput();
            inventoryScene.Open(this.GameState.character);
            Input.update();
            TurnExploreCommandsOff();
        }
        public void CloseInventory()
        {
            inventoryScene.Close();
            TurnExploreCommandsOn();
            ((Camera)Camera).ConsiderInput();
        }
        private void IncrementFarPlane(float scale)
        {
            D3DDevice.Instance.Device.Transform.Projection =
                Matrix.PerspectiveFovLH(
                    D3DDevice.Instance.FieldOfView,
                    D3DDevice.Instance.AspectRatio,
                    D3DDevice.Instance.ZNearPlaneDistance,
                    D3DDevice.Instance.ZFarPlaneDistance * scale
                );
        }
        private void SetFOV(int fov)
        {
            D3DDevice.Instance.Device.Transform.Projection =
                Matrix.PerspectiveFovLH(
                    fov,
                    D3DDevice.Instance.AspectRatio,
                    D3DDevice.Instance.ZNearPlaneDistance,
                    D3DDevice.Instance.ZFarPlaneDistance
                );
        }
        private void InitInventoryScene()
        {
            inventoryScene = new InventoryScene();
        }
        private void InitWaterVision()
        {
            waterVision = BitmapRepository.CreateSpriteFromBitmap(BitmapRepository.WaterRectangle);
            Screen.FitSpriteToScreen(waterVision);
            waterVision.Color = Color.FromArgb(120, 10, 70, 164);
        }
        private void InitMask()
        {
            mask = BitmapRepository.CreateSpriteFromBitmap(BitmapRepository.Mask);
            Screen.FitSpriteToScreen(mask);
        }
        private void InitDialogBox()
        {
            dialogBox = BitmapRepository.CreateSpriteFromBitmap(BitmapRepository.BlackRectangle);
            dialogBox.Scaling = new TGCVector2(.35f, .05f);
            dialogBox.Color = Color.FromArgb(188, dialogBox.Color.R, dialogBox.Color.G, dialogBox.Color.B);
            Screen.CenterSprite(dialogBox);
            dialogBox.Position = new TGCVector2(dialogBox.Position.X + 120, dialogBox.Position.Y + 80);
        }
        private void InitSkyBoxes()
        {
            skyBoxUnderwater = new TgcSkyBox();
            skyBoxUnderwater.Color = backgroundColor;
            skyBoxUnderwater.SkyEpsilon = 30;
            skyBoxUnderwater.Size = new TGCVector3(30000, 8000, 30000);
            skyBoxUnderwater.Center = new TGCVector3(0, -skyBoxUnderwater.Size.Y / 4, 0);
            skyBoxUnderwater.setFaceTexture(TgcSkyBox.SkyFaces.Up   , baseDir + "underwater_skybox-up.jpg"    );
            skyBoxUnderwater.setFaceTexture(TgcSkyBox.SkyFaces.Down , baseDir + "underwater_skybox-down.jpg"  );
            skyBoxUnderwater.setFaceTexture(TgcSkyBox.SkyFaces.Left , baseDir + "underwater_skybox-left.jpg"  );
            skyBoxUnderwater.setFaceTexture(TgcSkyBox.SkyFaces.Right, baseDir + "underwater_skybox-right.jpg" );
            skyBoxUnderwater.setFaceTexture(TgcSkyBox.SkyFaces.Front, baseDir + "underwater_skybox-front.jpg" );
            skyBoxUnderwater.setFaceTexture(TgcSkyBox.SkyFaces.Back , baseDir + "underwater_skybox-back.jpg"  );
            skyBoxUnderwater.Init();
            

            skyBoxOutside = new TgcSkyBox();
            skyBoxOutside.Color = Color.FromArgb(255, 71, 96, 164);
            skyBoxOutside.SkyEpsilon = 50;
            skyBoxOutside.Size = new TGCVector3(30000, 8000, 30000);
            skyBoxOutside.Center = new TGCVector3(
                skyBoxUnderwater.Center.X,
                skyBoxUnderwater.Center.Y + skyBoxUnderwater.Size.Y / 2 + 0,
                skyBoxUnderwater.Center.Z
                );
            skyBoxOutside.setFaceTexture(TgcSkyBox.SkyFaces.Up, baseDir +    "skybox-up.jpg");
            skyBoxOutside.setFaceTexture(TgcSkyBox.SkyFaces.Down, baseDir +  "skybox-down.jpg");
            skyBoxOutside.setFaceTexture(TgcSkyBox.SkyFaces.Left, baseDir +  "skybox-left-middle.jpg");
            skyBoxOutside.setFaceTexture(TgcSkyBox.SkyFaces.Right, baseDir + "skybox-right-middle.jpg");
            skyBoxOutside.setFaceTexture(TgcSkyBox.SkyFaces.Front, baseDir + "skybox-front-middle.jpg");
            skyBoxOutside.setFaceTexture(TgcSkyBox.SkyFaces.Back, baseDir + "skybox-back-middle.jpg");
            skyBoxOutside.Init();
            
        }

        private void SetClampTextureAddressing()
        {
            D3DDevice.Instance.Device.SamplerState[0].AddressU = TextureAddress.Clamp;
            D3DDevice.Instance.Device.SamplerState[0].AddressV = TextureAddress.Clamp;
            D3DDevice.Instance.Device.SamplerState[0].AddressW = TextureAddress.Clamp;
        }
        private void SetCamera(TgcD3dInput input)
        {
            var position = new TGCVector3(30, 30, 200);
            var rigidBody = new CapsuleFactory().Create(position, 100, 60);
            AquaticPhysics.Instance.Add(rigidBody);
            Camera = new Camera(position, input, rigidBody);
        }
        private IItem manageSelectableElement(Element element)
        {
            dialogName = dialogDescription = "";

            if (element == null)
            {
                cursor = aim;
                return null;
            }
            
            cursor = hand;
            IItem item = null;

            if (element.item != null)
            {
                dialogName = element.item.Name;
                dialogDescription = element.item.Description;
            }

            if (element.GetType() == typeof(Ship))
            {
                dialogName = "Ship";
                dialogDescription = "Enter to the ship";
            }

            element.Selectable = true;

            if (aimFired)
            {
                if (element.GetType() == typeof(Ship))
                {
                    onGetIntoShipCallback(this.GameState);
                }
                else
                {
                    this.World.Remove(element);
                    item = element.item;
                    aimFired = false;   
                }
            }

            return item;
        }

        public override void Update(float elapsedTime)
        {
            if (this.GameState.character.IsDead())
            {
                onGameOverCallback();
            }
            
            AquaticPhysics.Instance.DynamicsWorld.StepSimulation(elapsedTime);

            this.World.Update((Camera) this.Camera, this.GameState.character);

            var item = manageSelectableElement(this.World.SelectableElement); // Imsportant: get this AFTER updating the world

            //TODO refactor
            if (item != null)
            { 
                this.GameState.character.GiveItem(item);
                if (this.goldRate.HasToSpawn())
                {
                    this.GameState.character.GiveItem(new Gold());
                }
            }

            skyBoxUnderwater.Center = new TGCVector3(Camera.Position.X, skyBoxUnderwater.Center.Y, Camera.Position.Z);
            skyBoxOutside.Center = new TGCVector3(Camera.Position.X, skyBoxOutside.Center.Y, Camera.Position.Z);

            this.GameState.character.UpdateStats(this.Camera.Position.Y < 0
                ? new Stats(-elapsedTime, 0)
                : new Stats(elapsedTime * (this.GameState.character.MaxStats.Oxygen/2.5f), 0));

            inventoryScene.Update(elapsedTime);
            aimFired = false;
        }

        public override void Render(TgcFrustum frustum)
        {
            ClearScreen();

            if (Camera.Position.Y < 0)
            {
                this.skyBoxUnderwater.Render();
            }
            else
            {
                this.skyBoxOutside.Render();
            }

            this.World.Render(this.Camera, frustum);

            if (this.BoundingBox)
            {
                this.World.RenderBoundingBox(this.Camera);
            }

            drawer.BeginDrawSprite();
            //drawer.DrawSprite(waterVision);
            drawer.DrawSprite(cursor);
            if (dialogName != "")
            {
                drawer.DrawSprite(dialogBox);
            }
            drawer.EndDrawSprite();

            inventoryScene.Render();

            if (dialogName != "")
            {
                DrawText.drawText(dialogName, (int)dialogBox.Position.X, (int)dialogBox.Position.Y, Color.White);
                DrawText.drawText(dialogDescription, (int)dialogBox.Position.X, (int)dialogBox.Position.Y + 15, Color.White);
            }

            drawer.BeginDrawSprite();
            drawer.DrawSprite(mask);
            drawer.EndDrawSprite();
            
            this.statsIndicators.Render(this.GameState.character);
        }

        public new void Render()
        {
            if (GameModel.frustum != null) 
                this.Render(GameModel.frustum);
        }

        public override void Dispose()
        {
            this.World.Dispose();
        }

        public WorldScene OnPause(Callback onPauseCallback)
        {
            this.onPauseCallback = onPauseCallback;
            return this;
        }
        public WorldScene OnGetIntoShip(TransitionCallback onGetIntoShipCallback)
        {
            this.onGetIntoShipCallback = onGetIntoShipCallback;
            return this;
        }
        public WorldScene OnGameOver(Callback onGameOverCallback)
        {
            this.onGameOverCallback = onGameOverCallback;
            return this;
        }
    }
}