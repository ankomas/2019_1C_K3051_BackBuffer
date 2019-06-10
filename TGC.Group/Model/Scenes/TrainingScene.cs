using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.BoundingVolumes;
using TGC.Core.Direct3D;
using TGC.Core.Mathematica;
using TGC.Group.Model.UI;

namespace TGC.Group.Model.Scenes
{
    class TrainingScene : Scene
    {
        OrientationArrow orientationArrow = new OrientationArrow();
        CustomVertex.PositionColored[] vertexBuffer;
        Matrix itsTransform;
        Effect shader;
        public TrainingScene() : base()
        {
            vertexBuffer = new CustomVertex.PositionColored[3];
            vertexBuffer[0] = new CustomVertex.PositionColored(new Vector3(-1, -1, 0), 0xFF0000);
            vertexBuffer[1] = new CustomVertex.PositionColored(new Vector3(+1, -1, 0), 0x00FF00);
            vertexBuffer[2] = new CustomVertex.PositionColored(new Vector3(+0, +1, 0), 0x0000FF);

            Vector3 Translation = new Vector3(0, 0, 0);
            Vector3 Scale = new Vector3(1, 1, 1);
            Vector3 Rotation = new Vector3(0, 0, (float)Math.PI);

            Matrix T = Matrix.Translation(Translation);
            Matrix S = Matrix.Scaling(Scale);
            Matrix Rx = Matrix.RotationX(Rotation.X);
            Matrix Ry = Matrix.RotationY(Rotation.Y);
            Matrix Rz = Matrix.RotationZ(Rotation.Z);

            itsTransform = Matrix.Identity * Rz * Ry * Rx * T * S;

            string e;
            shader = Effect.FromFile(D3DDevice.Instance.Device, Game.Default.ShadersDirectory + "TrainingShader.fx", null, null, ShaderFlags.None, null, out e);
        }
        public override void Update(float elapsedTime)
        {

        }
        public override void Render(TgcFrustum frustum)
        {
            ClearScreen();
            SetMatrices();

            //shader.Begin(FX.None);
            //shader.BeginPass(0);

            //D3DDevice.Instance.Device.VertexFormat = CustomVertex.PositionColored.Format;
            //D3DDevice.Instance.Device.DrawUserPrimitives(
            //    PrimitiveType.TriangleList,
            //    vertexBuffer.Length / 3,
            //    vertexBuffer
            //    );

            //shader.EndPass();
            //shader.End();

            orientationArrow.Render();
        }
        private void SetMatrices()
        {
            //D3DDevice.Instance.Device.Transform.World = Matrix.RotationZ((float)Math.PI);
            //D3DDevice.Instance.Device.Transform.View = Matrix.Translation(-1, 0, 0);
            //D3DDevice.Instance.Device.Transform.Projection = Matrix.Scaling(0.5f, 0.5f, 0.5f);

            D3DDevice.Instance.Device.Transform.World = Matrix.Identity;
            D3DDevice.Instance.Device.Transform.View = Matrix.Identity;
            D3DDevice.Instance.Device.Transform.Projection = Matrix.Identity;

            //D3DDevice.Instance.Device.Transform.World = itsTransform;
            //D3DDevice.Instance.Device.Transform.View = Matrix.LookAtLH(new Vector3(0, 0, -10), new Vector3(0, 0, 0), new Vector3(0, -1, 0));
            //D3DDevice.Instance.Device.Transform.Projection = Matrix.PerspectiveFovLH(
            //        (float)Math.PI / 4,
            //        D3DDevice.Instance.Device.Viewport.Width / D3DDevice.Instance.Device.Viewport.Height,
            //        1f, 500f);
        }
    }
}
