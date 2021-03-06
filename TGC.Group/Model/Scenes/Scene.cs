﻿using TGC.Core.Input;
using TGC.Core.Camara;
using System.Drawing;
using TGC.Core.Direct3D;
using Microsoft.DirectX.Direct3D;
using Microsoft.DirectX.DirectInput;
using System.Collections.Generic;
using BulletSharp;
using TGC.Core.BoundingVolumes;
using TGC.Core.Mathematica;
using static TGC.Core.Input.TgcD3dInput;

namespace TGC.Group.Model.Scenes
{
    abstract class Scene
    {
        protected CommandList pressed = new CommandList();
        protected CommandList down = new CommandList();
        private List<Scene> subscenes = new List<Scene>();
        private List<CommandList.Command> toExecute = new List<CommandList.Command>();

        private static Scene emptySceneSingleInstance;
        public static Scene Empty => emptySceneSingleInstance ?? (emptySceneSingleInstance = new EmptyScene());
        protected Color backgroundColor = Color.Black;
        private bool _uses3DCamera = true;
        public bool Uses3DCamera { get { return _uses3DCamera; } protected set { _uses3DCamera = value; } }
        private Camera _camera = null;
        public Camera Camera
        {
            set { _camera = value; }
            get { return _camera ?? CameraFactory.Create(TGCVector3.Empty, Input); }
        }
        public static TgcD3dInput Input { get; set; }

        protected Scene()
        {
            if (Input == null) throw new System.Exception("Scene.Input not set yet");
        }
        abstract public void Update(float elapsedTime);
        abstract public void Render(TgcFrustum frustum);
        virtual public void Dispose() {}
        protected void ClearScreen()
        {
            D3DDevice.Instance.Device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, backgroundColor, 1.0f, 0);
        }
        private void ReactToOwnInput()
        {
            foreach (Key key in pressed.keys)
            {
                if (Input.keyPressed(key))
                {
                    toExecute.Add(pressed[key]);
                }
            }
            foreach (Key key in down.keys)
            {
                if (Input.keyDown(key))
                {
                    toExecute.Add(down[key]);
                }
            }
            foreach (MouseButtons button in pressed.mouseButtons)
            {
                if (Input.buttonPressed(button))
                {
                    toExecute.Add(pressed[button]);
                }
            }
            foreach (MouseButtons button in down.mouseButtons)
            {
                if (Input.buttonDown(button))
                {
                    toExecute.Add(down[button]);
                }
            }
            foreach(CommandList.Command command in toExecute)
            {
                command();
            }
            toExecute.Clear();
        }
        public virtual void ReactToInput()
        {
            ReactToOwnInput();
            foreach(Scene subscene in subscenes)
            {
                subscene.ReactToInput();
            }
        }

        protected void RegisterSubscene(Scene subscene)
        {
            subscenes.Add(subscene);
        }

        public void Render()
        {
            this.Render(null);
        }
    }

    class EmptyScene : Scene
    {
        public EmptyScene() : base() {}
        public override void Render(TgcFrustum frustum) {}
        public override void Update(float elapsedTime) {}
    }

    public class CommandList
    {
        private static byte NumberOfKeys = 237, NumberOfMouseButtons = 3;
        public delegate void Command();
        Command[] commandsForKeys = new Command[NumberOfKeys];
        Command[] commandsForMouseButtons = new Command[NumberOfMouseButtons];
        public List<Key> keys = new List<Key>();
        public List<MouseButtons> mouseButtons = new List<MouseButtons>();
        public Command this[Key key]
        {
            get { return commandsForKeys[(int)key]; }
            set
            {
                commandsForKeys[(int)key] = value;
                if(!keys.Contains(key))
                {
                    keys.Add(key);
                }
            }
        }
        public Command this[MouseButtons mouseButton]
        {
            get { return commandsForMouseButtons[(int)mouseButton]; }
            set
            {
                commandsForMouseButtons[(int)mouseButton] = value;
                if (!mouseButtons.Contains(mouseButton))
                {
                    mouseButtons.Add(mouseButton);
                }
            }
        }
        public Command this[List<object> list]
        {
            set
            {
                foreach(object mouseButtonOrKey in list)
                {
                    if( mouseButtonOrKey.GetType() == typeof(Key))
                        this[(Key)mouseButtonOrKey] = value;

                    if (mouseButtonOrKey.GetType() == typeof(MouseButtons))
                        this[(MouseButtons)mouseButtonOrKey] = value;
                }
                
            }
        }
    }
}
