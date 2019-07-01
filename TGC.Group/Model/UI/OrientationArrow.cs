using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.Direct3D;
using TGC.Core.Mathematica;
using TGC.Core.Text;

namespace TGC.Group.Model.UI
{
    class OrientationArrow
    {
        Effect shader;
        const int numOfTriangles = 6;
        CustomVertex.PositionColored[] vertexBuffer = new CustomVertex.PositionColored[numOfTriangles * 3];
        Matrix transform, viewProjection;
        Vector2 backwardsUnitVector = new Vector2(0, -1);
        Vector2 forwardsUnitVector = new Vector2(0, 1);
        Vector2 shipPointingUnitVector;

        float triangleHeight = (float)Math.Sin((float)Math.PI / 3);
        public OrientationArrow()
        {
            InitShader();
            InitVertexBuffer();
            InitViewProjectionMatrix();
        }
        private void InitVertexBuffer()
        {
            vertexBuffer[0]  = new CustomVertex.PositionColored(-0.25f, -triangleHeight / 2f, 0f, 0xFF0000);
            vertexBuffer[1]  = new CustomVertex.PositionColored( 0.25f, -triangleHeight / 2f, 0f, 0x00FF00);
            vertexBuffer[2]  = new CustomVertex.PositionColored(    0f,  triangleHeight / 2f, 0f, 0x0000FF);
                             
            vertexBuffer[3]  = new CustomVertex.PositionColored(-0.25f, -triangleHeight / 2f, 0.1f, 0xFF0000);
            vertexBuffer[4]  = new CustomVertex.PositionColored( 0.25f, -triangleHeight / 2f, 0.1f, 0x00FF00);
            vertexBuffer[5]  = new CustomVertex.PositionColored(    0f,  triangleHeight / 2f,   0f, 0x0000FF);

            vertexBuffer[6]  = new CustomVertex.PositionColored(-0.25f, -triangleHeight / 2f,   0f, 0xFF0000);
            vertexBuffer[7]  = new CustomVertex.PositionColored(-0.25f, -triangleHeight / 2f, 0.1f, 0x00FF00);
            vertexBuffer[8]  = new CustomVertex.PositionColored(    0f,  triangleHeight / 2f,   0f, 0x0000FF);

            vertexBuffer[9]  = new CustomVertex.PositionColored(0.25f, -triangleHeight / 2f,   0f, 0xFF0000);
            vertexBuffer[10] = new CustomVertex.PositionColored(0.25f, -triangleHeight / 2f, 0.1f, 0x00FF00);
            vertexBuffer[11] = new CustomVertex.PositionColored(   0f,  triangleHeight / 2f,   0f, 0x0000FF);

            vertexBuffer[12] = new CustomVertex.PositionColored(-0.25f, -triangleHeight / 2f,   0f, 0xFF0000);
            vertexBuffer[13] = new CustomVertex.PositionColored(-0.25f, -triangleHeight / 2f, 0.1f, 0xFFFF00);
            vertexBuffer[14] = new CustomVertex.PositionColored( 0.25f, -triangleHeight / 2f,   0f, 0xFF00FF);

            vertexBuffer[15] = new CustomVertex.PositionColored( 0.25f, -triangleHeight / 2f,   0f, 0xFFFF00);
            vertexBuffer[16] = new CustomVertex.PositionColored( 0.25f, -triangleHeight / 2f, 0.1f, 0x00FF00);
            vertexBuffer[17] = new CustomVertex.PositionColored(-0.25f, -triangleHeight / 2f, 0.1f, 0x00FFFF);
        }
        private void InitViewProjectionMatrix()
        {
            Viewport viewport = D3DDevice.Instance.Device.Viewport;
            float aspectRatio = (float)viewport.Width / viewport.Height;

            viewProjection = Matrix.Identity
                * Matrix.RotationX(5f)
                * Matrix.Scaling(0.5f, 0.5f, 0.5f)
                * Matrix.Translation(0, 0.75f, 0)
                * Matrix.Translation(0, 0, triangleHeight / 2)
                * Matrix.Scaling(1 / aspectRatio, 1, 1);
        }
        private void InitShader()
        {
            shader = Effect.FromFile(D3DDevice.Instance.Device,
                Game.Default.ShadersDirectory + "OrientationArrow.fx", null, null, ShaderFlags.None, null);
            shader.Technique = "OrientationArrow";
        }
        public void Update(TGCVector3 cameraPosition, TGCVector3 targetPosition, TGCVector3 lookAt)
        {
            Vector2 camera = new Vector2(cameraPosition.X, cameraPosition.Z);
            Vector2 target = new Vector2(targetPosition.X, targetPosition.Z);
            TGCVector3 lookAtFromPos = lookAt - cameraPosition;
            Vector2 lookDirection = Vector2.Normalize(new Vector2(lookAtFromPos.X, lookAtFromPos.Z));

            shipPointingUnitVector = Vector2.Normalize(target - camera);

            float dot = Vector2.Dot(forwardsUnitVector, shipPointingUnitVector);

            float angle = (float)Math.Acos(dot);

            if (shipPointingUnitVector.X < 0) angle -= 2 * angle;

            float angleLook = (float)Math.Acos(Vector2.Dot(lookDirection, backwardsUnitVector));
            if (lookDirection.X < 0) angleLook -= 2 * angleLook;

            angle += angleLook;

            transform = Matrix.RotationZ(angle) * viewProjection;
        }
        public void Render()
        {
            shader.Begin(FX.None);
            shader.BeginPass(0);
            shader.SetValue("transform", transform);

            D3DDevice.Instance.Device.RenderState.AlphaBlendEnable = true;
            D3DDevice.Instance.Device.VertexFormat = CustomVertex.PositionColored.Format;
            D3DDevice.Instance.Device.DrawUserPrimitives(PrimitiveType.TriangleList, vertexBuffer.Length / 3, vertexBuffer);

            shader.EndPass();
            shader.End();
        }
    }
}
