using System.Collections.Generic;
using System.Linq;
using Microsoft.DirectX.DirectInput;
using TGC.Core.Input;

using static TGC.Core.Input.TgcD3dInput;

namespace TGC.Group.Model.Input
{
    public class GameInput
    {
        public static readonly GameInput _Up = new GameInput(new List<Key> {Key.Up, Key.W});
        public static readonly GameInput _Down = new GameInput(new List<Key>{Key.Down, Key.S});
        public static readonly GameInput _Left = new GameInput(new List<Key>{Key.Left, Key.A});
        public static readonly GameInput _Right = new GameInput(new List<Key>{Key.Right, Key.D});
        public static readonly GameInput _Speed = new GameInput(new List<Key>{Key.LeftShift});
        public static readonly GameInput _Float = new GameInput(new List<Key>{Key.Space});
        public static readonly GameInput _Escape = new GameInput(new List<Key>{Key.Escape});
        public static readonly GameInput _Enter = new GameInput(new List<Key>{Key.Return}, new List<MouseButtons> {MouseButtons.BUTTON_LEFT});
        public static readonly GameInput _Inventory = new GameInput(new List<Key>{Key.I, Key.Tab});
        public static readonly GameInput _Statistic = new GameInput(new List<Key>{Key.F});
        public static readonly GameInput _Attack =
            new GameInput(new List<Key> {Key.K}, new List<MouseButtons>{MouseButtons.BUTTON_RIGHT});
        
        public static readonly List<object> Up = new List<object> { Key.Up, Key.W };
        public static readonly List<object> Down = new List<object> { Key.Down, Key.S };
        public static readonly List<object> Left = new List<object> { Key.Left, Key.L };
        public static readonly List<object> Right = new List<object> { Key.Right, Key.D };
        public static readonly List<object> Accept = new List<object> { Key.Return, MouseButtons.BUTTON_LEFT };
        public static readonly List<object> Inventory = new List<object> { Key.I, Key.Tab };
        public static readonly List<object> GoBack = new List<object> { Key.Escape };
        public static readonly List<object> Pause = new List<object> { Key.P };

        private readonly IEnumerable<Key> keys;
        private readonly IEnumerable<MouseButtons> buttons;


        private GameInput(IEnumerable<Key> keys, IEnumerable<MouseButtons> buttons)
        {
            this.keys = keys;
            this.buttons = buttons;
        }

        private GameInput(IEnumerable<Key> keys) : this(keys, Enumerable.Empty<MouseButtons>())
        {

        }


        public bool IsPressed(TgcD3dInput input)
        {
            return this.keys.Any(input.keyPressed) ||
                   this.buttons.Any(input.buttonPressed);
        }

        public bool IsDown(TgcD3dInput input)
        {
            return this.keys.Any(input.keyDown) ||
                   this.buttons.Any(input.buttonDown);
        }

        public bool IsUp(TgcD3dInput input)
        {
            return this.keys.Any(input.keyUp) ||
                   this.buttons.Any(input.buttonUp);
        }
    }
}