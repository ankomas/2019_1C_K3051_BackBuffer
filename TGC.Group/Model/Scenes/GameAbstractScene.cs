using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.Input;
using TGC.Group.Model.Elements;
using TGC.Group.Model.Items;
using TGC.Group.TGCUtils;

namespace TGC.Group.Model.Scenes
{
    public struct GameState
    {
        public Player.Character character;
        public GameState(Player.Character character)
        {
            this.character = character;
        }
    }
    abstract class GameAbstractScene : Scene
    {
        public static GameState InitialGameState = new GameState(new Player.Character());
        CustomSprite cursor, aim, hand;
        string dialogName, dialogDescription;
        public delegate void TransitionCallback(GameState gameState);
        public GameState GameState { get; set; }

        public GameAbstractScene(GameState gameState) : base()
        {
            this.GameState = gameState;
        }
        public override void Render()
        {
            throw new NotImplementedException();
        }

        public override void Update(float elapsedTime)
        {
            throw new NotImplementedException();
        }

        public GameAbstractScene WithGameState(GameState gameState)
        {
            this.GameState = gameState;
            return this;
        }
    }
}
