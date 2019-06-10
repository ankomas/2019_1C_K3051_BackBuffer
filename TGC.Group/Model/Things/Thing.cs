using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.BoundingVolumes;
using TGC.Core.Direct3D;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;
using TGC.Group.Model.Scenes;

namespace TGC.Group.Model.Things
{
    class Thing
    {
        public string debug;
        protected List<TgcMesh> meshes;
        public List<float> relativeScales;
        public List<TGCVector3> relativePositions;
        public bool Looked = false;
        public string name, actionDescription;
        public delegate void Callback();
        private Callback action = () => {};

        protected Effect ambientShader;
        public TGCVector3 Center => meshes[0].BoundingBox.PMin + TGCVector3.Multiply(meshes[0].BoundingBox.PMax - meshes[0].BoundingBox.PMin, 0.5f);

        public TGCVector3 Position
        {
            get
            {
                return meshes[0].Position;
            }
            set
            {
                int i = 0;
                foreach(var mesh in meshes)
                {
                    mesh.Position = value + relativePositions[i++];
                }
            }
        }
        public TGCVector3 Scale
        {
            get
            {
                return meshes[0].Scale;
            }
            set
            {
                int i = 0;
                foreach (var mesh in meshes)
                {
                    mesh.Scale = value * relativeScales[i++];
                }
            }
        }
        public void RotateY(float angle)
        {
            foreach(var mesh in meshes)
            {
                mesh.RotateY(angle);
            }
        }
        public TgcBoundingAxisAlignBox BoundingBox
        {
            get
            {
                return meshes[0].BoundingBox;
            }
        }

        public Thing(List<TgcMesh> meshes) : this(meshes, "Generic Thing", "no action provided", () => {})
        {
        }

        public Thing(List<TgcMesh> meshes, string name, string actionDescription, Callback action)
        {
            InitAmbientShader();
            this.meshes = meshes;
            this.relativeScales = new List<float>(meshes.Select(_ => 1f));
            this.relativePositions = new List<TGCVector3>(meshes.Select(_ => new TGCVector3(0, 0, 0)));
            this.name = name;
            this.actionDescription = actionDescription;
            this.action = action;
        }
        public virtual void Render()
        {
            foreach(var mesh in meshes)
            {
                mesh.Render();
            }
        }
        public void ExecuteAction()
        {
            action();
        }
        protected void SetAmbientShader()
        {
            foreach (var mesh in meshes)
            {
                mesh.Effect = ambientShader;
                mesh.Technique = "ShipAmbient";
            }
        }
        private void InitAmbientShader()
        {
            string e = "";
            try
            {
                ambientShader = Effect.FromFile(D3DDevice.Instance.Device, Game.Default.ShadersDirectory + "ShipAmbient.fx", null, null, ShaderFlags.None, null, out e);
            }
            catch (Exception)
            {
                throw new Exception("CTM" + e);
            }

            if (ambientShader == null)
            {
                throw new Exception("CTM carrajo" + e);
            }
        }
        public void TellCameraPosition(float[] position)
        {
            ambientShader.SetValue("cameraPosition", position);
        }
    }
}
