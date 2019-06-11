using Microsoft.DirectX.Direct3D;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.Direct3D;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;
using TGC.Group.Model.Resources.Meshes;

namespace TGC.Group.Model.Things
{
    
    class Crafter : Thing
    {
        Effect shader;
        public float openingAmplitude = 0, coefficient = 2f;
        public Crafter(Callback callback) : base(MeshesForShip.CrafterMeshes, "Crafter", "Start crafting", callback)
        {
            SetAmbientShader();
        }
        public void Open(float elapsedTime)
        {
            if (openingAmplitude > (float)Math.PI / 2) return;

            float deltaRot = coefficient * elapsedTime;

            var frontCover = meshes[1];
            var topCover = meshes[2];

            frontCover.RotateX(deltaRot);
            frontCover.Position += new TGCVector3(0, deltaRot * 15, 0);

            topCover.RotateX(-deltaRot);
            topCover.Position += new TGCVector3(0, deltaRot * 25, deltaRot * 45);

            openingAmplitude += deltaRot;
        }
        public void Close(float elapsedTime)
        {
            if (openingAmplitude < 0) return;

            float deltaRot = coefficient * elapsedTime;

            var frontCover = meshes[1];
            var topCover = meshes[2];

            frontCover.RotateX(-deltaRot);
            frontCover.Position -= new TGCVector3(0, deltaRot * 15, 0);

            topCover.RotateX(deltaRot);
            topCover.Position -= new TGCVector3(0, deltaRot * 25, deltaRot * 45);

            openingAmplitude -= deltaRot;
        }
    }
}
