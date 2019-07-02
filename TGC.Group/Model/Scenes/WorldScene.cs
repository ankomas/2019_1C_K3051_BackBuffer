using System.Drawing;
using BulletSharp;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using Microsoft.DirectX.DirectInput;
using TGC.Core.BoundingVolumes;
using TGC.Core.Direct3D;
using TGC.Core.Input;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;
using TGC.Core.Terrain;
using TGC.Core.Text;
using TGC.Group.Model.Elements;
using TGC.Group.Model.Elements.RigidBodyFactories;
using TGC.Group.Model.Input;
using TGC.Group.Model.Items;
using TGC.Group.Model.Player;
using TGC.Group.Model.Resources.Sprites;
using TGC.Group.Model.Elements.RigidBodyFactories;
using TGC.Core.Direct3D;
using Key = Microsoft.DirectX.DirectInput.Key;
using Screen = TGC.Group.Model.Utils.Screen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using BulletSharp;
using TGC.Core.BoundingVolumes;
using TGC.Group.Model.UI;
using TGC.Group.Model.Utils;
using TGC.Group.TGCUtils;
using Element = TGC.Group.Model.Elements.Element;
using Vector3 = BulletSharp.Math.Vector3;
using TGC.Core.SceneLoader;
using TGC.Group.Form;
using TGC.Group.Model.Resources;
using TGC.Group.Model.Chunks;
using Volume = Microsoft.DirectX.DirectSound.Volume;

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
        TransitionCallback onGetIntoShipCallback = gameState => {};

        InventoryScene inventoryScene;

        static OrientationArrow orientationArrow = new OrientationArrow();
        //TgcSkyBox skyBoxUnderwater, skyBoxOutside;
        static FedeSkybox theSkybox = new FedeSkybox("lol2.jpg", 0.0005f);
        static FedeSurface waterSurface = new FedeSurface(50000, 50000);
        CustomSprite waterVision, mask;

        Drawer2D drawer = new Drawer2D();
        string dialogName, dialogDescription;
        internal Character Character { get { return GameState.character; } }

        private bool gaveOxygenTank = false; //TODO remove
        private bool aimFired;

        private float baseCameraSpeed;

        TgcMesh skb;
        private TGCVector3 initialCameraPosition = new TGCVector3(300, -100, 200);

        private bool loaded = false;
        private bool loading = false;

        public WorldScene(GameState gameState) : base(gameState)
        {
            backgroundColor = Color.FromArgb(255, 78, 129, 179);

            SetCamera(Input);
            
            IncrementFarPlane(6f);
            SetClampTextureAddressing();
            InitInventoryScene();
            InitSkyBoxes();
            InitWaterVision();
            InitMask();
            
            this.World = new World(new TGCVector3(0, 0, 0));
            
            pressed[Key.P] = () => {
                onPauseCallback();
            };

            // Die with Q for debugging or god mode
            //pressed[Key.Q] = () => {
            //    onGameOverCallback();
            //};

            pressed[Key.F] = () => {
                BoundingBox = !BoundingBox;
            };

            RegisterSubscene(inventoryScene);

            TurnExploreCommandsOn();

            waterSurface.position = new Microsoft.DirectX.Vector3(0, 0, 0);
            waterSurface.velocity = 3;

            //this.loadIndicator.Init();
        }
        public void ResetCamera()
        {
            AquaticPhysics.Instance.Remove(Camera.RigidBody); 
            SetCamera(Input);
        }
        private void TurnExploreCommandsOn()
        {
            pressed[GameInput.Inventory] = OpenInventory;
            pressed[GameInput.Accept] = () => aimFired = true;
        }
        private void TurnExploreCommandsOff()
        {
            pressed[GameInput.Inventory] = CloseInventory;
            pressed[GameInput.Accept] = () => {};
        }
        private void OpenInventory()
        {
            Camera.IgnoreInput();
            inventoryScene.Open(GameState.character);
            Input.update();
            TurnExploreCommandsOff();
        }
        public void CloseInventory()
        {
            inventoryScene.Close();
            TurnExploreCommandsOn();
            Camera.ConsiderInput();
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
        private void InitSkyBoxes()
        {
            //skyBoxUnderwater = new TgcSkyBox();
            //skyBoxUnderwater.Color = backgroundColor;
            //skyBoxUnderwater.SkyEpsilon = 30;
            //skyBoxUnderwater.Size = new TGCVector3(30000, 8000, 30000);
            //skyBoxUnderwater.Center = new TGCVector3(0, -skyBoxUnderwater.Size.Y / 4, 0);
            //skyBoxUnderwater.setFaceTexture(TgcSkyBox.SkyFaces.Up   , baseDir + "underwater_skybox-up.jpg"    );
            //skyBoxUnderwater.setFaceTexture(TgcSkyBox.SkyFaces.Down , baseDir + "underwater_skybox-down.jpg"  );
            //skyBoxUnderwater.setFaceTexture(TgcSkyBox.SkyFaces.Left , baseDir + "underwater_skybox-left.jpg"  );
            //skyBoxUnderwater.setFaceTexture(TgcSkyBox.SkyFaces.Right, baseDir + "underwater_skybox-right.jpg" );
            //skyBoxUnderwater.setFaceTexture(TgcSkyBox.SkyFaces.Front, baseDir + "underwater_skybox-front.jpg" );
            //skyBoxUnderwater.setFaceTexture(TgcSkyBox.SkyFaces.Back , baseDir + "underwater_skybox-back.jpg"  );
            //skyBoxUnderwater.Init();


            //skyBoxOutside = new TgcSkyBox();
            //skyBoxOutside.Color = Color.FromArgb(255, 71, 96, 164);
            //skyBoxOutside.SkyEpsilon = 50;
            //skyBoxOutside.Size = new TGCVector3(30000, 8000, 30000);
            //skyBoxOutside.Center = new TGCVector3(
            //    skyBoxUnderwater.Center.X,
            //    skyBoxUnderwater.Center.Y + skyBoxUnderwater.Size.Y / 2 + 0,
            //    skyBoxUnderwater.Center.Z
            //    );
            //skyBoxOutside.setFaceTexture(TgcSkyBox.SkyFaces.Up, baseDir + "skybox-up.jpg");
            //skyBoxOutside.setFaceTexture(TgcSkyBox.SkyFaces.Down, baseDir + "skybox-down.jpg");
            //skyBoxOutside.setFaceTexture(TgcSkyBox.SkyFaces.Left, baseDir + "skybox-left-middle.jpg");
            //skyBoxOutside.setFaceTexture(TgcSkyBox.SkyFaces.Right, baseDir + "skybox-right-middle.jpg");
            //skyBoxOutside.setFaceTexture(TgcSkyBox.SkyFaces.Front, baseDir + "skybox-front-middle.jpg");
            //skyBoxOutside.setFaceTexture(TgcSkyBox.SkyFaces.Back, baseDir + "skybox-back-middle.jpg");
            //skyBoxOutside.Init();

            //FedeSkybox s1 = new FedeSkybox(Game.Default.MediaDirectory + "lol6.jpg", 0.0005f);
            //FedeSkybox s2 = new FedeSkybox(Game.Default.MediaDirectory + "lol2.jpg", 0.001f);
            //s = s2;
            theSkybox.size = new Microsoft.DirectX.Vector3(20000, 20000, 20000);
            theSkybox.position = new Microsoft.DirectX.Vector3(0, 0, 0);
        }

        private void SetClampTextureAddressing()
        {
            D3DDevice.Instance.Device.SamplerState[0].AddressU = TextureAddress.Clamp;
            D3DDevice.Instance.Device.SamplerState[0].AddressV = TextureAddress.Clamp;
            D3DDevice.Instance.Device.SamplerState[0].AddressW = TextureAddress.Clamp;
        }
        private void SetCamera(TgcD3dInput input)
        {
            this.Camera = CameraFactory.Create(this.initialCameraPosition, input);
            
            this.baseCameraSpeed = this.Camera.MovementSpeed;
            
            AquaticPhysics.Instance.Add(this.Camera.RigidBody);
        }

        public override void UpdateGameplay(float elapsedTime)
        {
            Game.cameraPosition = Camera.Position;
            if (!loaded)
            {
                if (this.loading)
                {
                    return;
                }

                preload();

                //TODO loading scene
                
                this.loading = true;

                return;
            }

            if (this.GameState.character.IsDead())
            {
                onGameOverCallback();
            }

            MusicManager.play(this.Camera.Position.Y > 0 ? MusicManager.SurfaceMusic : MusicManager.UnderWaterMusic);

            Cheats.ApplyCheats(this.GameState.character);
            
            World.Update(Camera, GameState.character);

            CharacterUpdate(elapsedTime);

            //skyBoxUnderwater.Center = new TGCVector3(Camera.Position.X, skyBoxUnderwater.Center.Y, Camera.Position.Z);
            //skyBoxOutside.Center = new TGCVector3(Camera.Position.X, skyBoxOutside.Center.Y, Camera.Position.Z);
            theSkybox.position = Camera.Position;
            //waterSurface.position = new Microsoft.DirectX.Vector3(Camera.Position.X, 0, Camera.Position.Z);
            waterSurface.Update(elapsedTime);
            
            inventoryScene.Update(elapsedTime);
            aimFired = false;

            orientationArrow.Update(Camera.Position, InitialChunk.ShipInitialPosition, Camera.LookAt);
            dialogBox.Update(elapsedTime);
            waterSurface.position = new Microsoft.DirectX.Vector3(Camera.Position.X, waterSurface.position.Y, Camera.Position.Z);
        }
        
        private void CharacterUpdate(float elapsedTime)
        {
            this.GameState.character.Update(this.Camera);
            this.GameState.character.Attack(this.World, Input);

            propulsion(elapsedTime);

            this.GameState.character.UpdateStats(this.Camera.Position.Y < 0
                ? new Stats(-elapsedTime, 0)
                : new Stats(elapsedTime * (this.GameState.character.MaxStats.Oxygen / 2.5f), 0));
            
            var item = manageSelectableElement(World.SelectableElement); // Imsportant: get this AFTER updating the world

            //TODO refactor
            if (item != null)
            { 
                GameState.character.GiveItem(item);
                if (goldRate.HasToSpawn())
                {
                    GameState.character.GiveItem(new Gold());
                }
            }
        }
        private float propulsionValue = 1;
        private void propulsion(float elapsedTime)
        {
            if (GameInput._Speed.IsDown(Input) && this.Character.ActualStats.Oxygen > 0)
            {
                this.propulsionValue += elapsedTime * 12;
                this.GameState.character.UpdateStats(new Stats(-elapsedTime * 8, 0));
            }
            else
            {
                this.propulsionValue -= elapsedTime * 8;
            }

            var maxValue = 6;
            var variation = (int)Volume.Min / maxValue;
            this.propulsionValue = FastMath.Clamp(this.propulsionValue, 1, maxValue);

            if (this.propulsionValue <= 1)
                SoundManager.Stop(SoundManager.MotorSound);
            else
                SoundManager.Play(SoundManager.MotorSound, (int)Volume.Min - (int)Math.Ceiling(this.propulsionValue * variation));
            
            this.Camera.MovementSpeed = this.baseCameraSpeed * this.propulsionValue;
        }
        
        private IItem manageSelectableElement(Element element)
        {
            if (element == null)
            {
                dialogName = dialogDescription = "";
                cursor = aim;
                return null;
            }
            
            cursor = hand;
            IItem item = null;

            if (element.item != null)
            {
                if(element.item.Name != dialogName)
                {
                    dialogBox.Open();
                }
                dialogName = element.item.Name;
                dialogDescription = element.item.Description;
            }

            if (element.GetType() == typeof(Ship))
            {
                if ("Ship" != dialogName)
                {
                    dialogBox.Open();
                }
                dialogName = "Ship";
                dialogDescription = "Enter to the ship";
            }

            element.Selectable = true;

            if (aimFired)
            {
                if (element.GetType() == typeof(Ship))
                {
                    onGetIntoShipCallback(GameState);
                }
                else
                {
                    var toPlay = element.GetType() == typeof(Coral) ? SoundManager.Coral : SoundManager.Bubble;
                    SoundManager.Play(toPlay);
                    World.Remove(element);
                    item = element.item;
                    aimFired = false;   
                }
            }

            return item;
        }

        private void preload()
        {
            var preloadRadius = World.UpdateRadius;
            
            var preloadWorld = new Task(() =>
            {
                this.World.preLoad(TGCVector3.Empty, preloadRadius);
                this.loaded = true;
            });
            
            preloadWorld.Start();
        }

        //private readonly NumberIndicator loadIndicator = new NumberIndicator(100, (Screen.Width-100)/2, (Screen.Height-100)/2);

        public override void Render(TgcFrustum frustum)
        {
            ClearScreen();
            ShaderRepository.WorldWaterFog.SetValue("cameraPosition", new Vector4(Camera.Position.X, Camera.Position.Y, Camera.Position.Z, 1));
            GameState.character.Render();
            
            if (!this.loaded)
            {
                var oldColor = this.backgroundColor;
                this.backgroundColor = Color.Black;
                ClearScreen();
                //TODO loading screen
                /* indicator
                var max = this.World.generating;
                var progress = this.World.chunks.Count * 100 / (max != 0 ? max : 1);
                
                loadIndicator.Render(progress , 100);
                loadIndicator.RenderText(progress);
                */    
                
                var color = Color.DeepSkyBlue;
                
                this.DrawText.drawText("Loading...", 600, 300, color);
                this.DrawText.drawText("Chunnks: " + this.World.chunks.Count + "/" + this.World.generating, 600, 330, color);
                this.DrawText.drawText("Floors: " + FloorRepository.Floors.Count + "/" + FloorRepository.generating, 600, 360, color);
                this.backgroundColor = Color.Green;
                
                return;
            }

            //if (Camera.Position.Y < 0)
            //{
            //    foreach (var mesh in skyBoxUnderwater.Faces)
            //    {
            //        mesh.Effect = ShaderRepository.WorldWaterFog;
            //        mesh.Technique = "WorldWaterFog";
            //    }
            //    skyBoxUnderwater.Render();
            //}
            //else
            //{
            //    foreach (var mesh in skyBoxOutside.Faces)
            //    {
            //        mesh.Effect = ShaderRepository.WorldWaterFog;
            //        mesh.Technique = "WorldWaterFog";
            //    }
            //   skyBoxOutside.Render();
            //}

            if (Camera.Position.Y < 0)
            {
                theSkybox.Render(new Vector4(Camera.Position.X, Camera.Position.Y, Camera.Position.Z, 1));
            }
            else
            {
                theSkybox.Render();
            }

            var camPos = new Vector4(Camera.Position.X, Camera.Position.Y, Camera.Position.Z, 1);
            
            waterSurface.Render(camPos);
            ShaderRepository.WorldWaterFog.SetValue("cameraPosition", camPos);
            World.Render(Camera, frustum);

            if (BoundingBox)
            {
                World.RenderBoundingBox(Camera);
            }
            orientationArrow.Render();

            drawer.BeginDrawSprite();
            //drawer.DrawSprite(waterVision);
            drawer.DrawSprite(cursor);
            drawer.EndDrawSprite();

            inventoryScene.Render();

            if (dialogName != "")
            {
                dialogBox.Clear();
                dialogBox.AddLineBig(dialogName, Color.FromArgb(255, 204, 234, 255));
                dialogBox.AddLineSmall(dialogDescription, Color.FromArgb(255, 204, 234, 255));
                dialogBox.Render();
            }

            drawer.BeginDrawSprite();
            drawer.DrawSprite(mask);
            drawer.EndDrawSprite();
            statsIndicators.Render(GameState.character);

            this.DrawText.drawText("Camera:", 800, 100, Color.Red);
            this.DrawText.drawText("X: " + Camera.Position.X, 800, 130, Color.White);
            this.DrawText.drawText("Y: " + Camera.Position.Y, 800, 160, Color.White);
            this.DrawText.drawText("Z: " + Camera.Position.Z, 800, 190, Color.White);

        }

        public new void Render()
        {
            if (GameModel.frustum != null) 
                Render(GameModel.frustum);
        }

        public override void Dispose()
        {
            SoundManager.Stop(SoundManager.MotorSound);
            World.Dispose();
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