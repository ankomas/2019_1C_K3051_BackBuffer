using TGC.Core.Direct3D;
using TGC.Core.Example;
using TGC.Core.Textures;
using System;
using System.Runtime.CompilerServices;
using TGC.Group.Model.Scenes;
using TGC.Group.Form;
using System.Windows.Forms;
using TGC.Core.BoundingVolumes;
using TGC.Group.TGCUtils;
using TGC.Group.Model.Resources.Sprites;

namespace TGC.Group.Model
{
    /// <summary>
    ///     Ejemplo para implementar el TP.
    ///     Inicialmente puede ser renombrado o copiado para hacer m�s ejemplos chicos, en el caso de copiar para que se
    ///     ejecute el nuevo ejemplo deben cambiar el modelo que instancia GameForm <see cref="Form.GameForm.InitGraphics()" />
    ///     line 97.
    /// </summary>
    public class GameModel : TgcExample
    {
        public static float GlobalElapsedTime;
        public static float GlobalTime;
        private WorldScene gameScene;
        private StartMenu startMenu;
        private PauseMenu pauseMenu;
        private ShipScene shipScene;
        private GameOverScene gameOverScene;

        private Scene _curentScene = null;
        private Scene CurrentScene
        {
            set
            {
                _curentScene = value;
                Camara = _curentScene.Camera;
                nextScene = null;
            }
            get { return _curentScene; }
        }

        private Scene nextScene;

        public static TgcFrustum frustum;

        /// <summary>
        ///     Constructor del juego.
        /// </summary>
        /// <param name="mediaDir">Ruta donde esta la carpeta con los assets</param>
        /// <param name="shadersDir">Ruta donde esta la carpeta con los shaders</param>
        public GameModel(string mediaDir, string shadersDir) : base(mediaDir, shadersDir)
        {
            Console.WriteLine("Mediadir: " + mediaDir);
            Category = Game.Default.Category;
            Name = Game.Default.Name;
            Description = Game.Default.Description;
        }

        public override void Init()
        {
            //note(fede): Only at this point the Input field has been initialized by the form

            Scene.Input = Input;

            startMenu = new StartMenu()
                    .onGameStart(() => SetNextScene(this.shipScene))
                    .onGameExit(StopGame);

            pauseMenu = new PauseMenu()
                .OnGoToStartMenu(() => {
                    ResetGame();
                    SetNextScene(startMenu);
                });

            ResetGame();

            //SetNextScene(new ShipScene(GameplayScene.InitialGameState));
            //SetNextScene(new TrainingScene());
            SetNextScene(startMenu);
        }

        public override void Update()
        {
            GlobalElapsedTime = ElapsedTime;
            GlobalTime += ElapsedTime;
            
            if (hasToChangeScene()) CurrentScene = nextScene;

            PreUpdate();


            CurrentScene.ReactToInput();

            CurrentScene.Update(this.ElapsedTime);

            if(CurrentScene.Uses3DCamera)
            {
                PostUpdate();
            }
        }

        public override void Render()
        {
            D3DDevice.Instance.Device.BeginScene();
            TexturesManager.Instance.clearAll();

            CurrentScene.Render(this.Frustum);
            GameModel.frustum = this.Frustum;

            PostRender();
        }

        private bool hasToChangeScene() { return nextScene != null; }

        public override void Dispose()
        {
            CurrentScene.Dispose();
        }


        /**
         * This methods anounces the INTENTION of changing the current scene, but doesn't actually changes it
         * The ChangeScene() method will change the scene at a safe time and should not be called by any 
         * other than the GameModel
         */
        private void SetNextScene(Scene newScene)
        {
            nextScene = newScene;
        }

        private void StopGame()
        {
            GameForm.Stop();
            Application.Exit();
        }

        private void ResetGame()
        {
            this.gameScene?.Dispose();
            
            gameScene = new WorldScene(GameplayScene.InitialGameState)
                .OnPause(() => {
                    PauseScene(gameScene);
                })
                .OnGetIntoShip((gameState) => {
                    shipScene.ResetCamera();
                    SetNextScene(shipScene.WithGameState(gameState));
                })
                .OnGameOver(() => {
                    SetNextScene(gameOverScene);
                    ResetGame();
                });

            shipScene = new ShipScene(GameplayScene.InitialGameState)
                .OnGoToWater((gameState) => {
                    gameScene.ResetCamera();
                    SetNextScene(gameScene.WithGameState(gameState));
                })
                .OnPause(() => {
                    PauseScene(shipScene);
                });

            gameOverScene = new GameOverScene()
                .WithPreRender(gameScene.Render)
                .OnGoToStartScreen(() => SetNextScene(startMenu));
        }

        private void PauseScene(Scene scene)
        {
            SetNextScene(pauseMenu
                    .WithPreRender(() => scene.Render())
                    .OnReturnToGame(() => SetNextScene(scene))
                 );
        }
    }
}