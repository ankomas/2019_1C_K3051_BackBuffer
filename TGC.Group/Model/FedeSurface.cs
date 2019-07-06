using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.Direct3D;
using TGC.Core.Mathematica;

namespace TGC.Group.Model
{
    class FedeSurface
    {
        int tessLevelX = 500;
        int tessLevelZ = 500;
        CustomVertex.PositionColored[] vertexBuffer;
        float sizeX;
        float sizeZ;
        int quadsNumber;
        public Vector3 position = new Vector3(0, 0, 0);
        float time = 0;
        public float velocity = 1;
        public FedeSurface(float sizeX, float sizeZ)
        {
            this.sizeX = sizeX;
            this.sizeZ = sizeZ;

            this.quadsNumber = tessLevelX * tessLevelZ;

            vertexBuffer = new CustomVertex.PositionColored[6 * quadsNumber];

            //var x0 = new Vector3(1, 0, -1) * size;
            //var x1 = new Vector3(-1, 0, -1) * size;
            //var x2 = new Vector3(1, 0, 1) * size;
            //var x3 = new Vector3(-1, 0, 1) * size;

            //vertexBuffer[0] = new CustomVertex.PositionOnly(x0);
            //vertexBuffer[1] = new CustomVertex.PositionOnly(x1);
            //vertexBuffer[2] = new CustomVertex.PositionOnly(x2);
            //vertexBuffer[3] = new CustomVertex.PositionOnly(x1);
            //vertexBuffer[4] = new CustomVertex.PositionOnly(x2);
            //vertexBuffer[5] = new CustomVertex.PositionOnly(x3);

            for (int i = 0; i < quadsNumber; ++i)
            {
                SetQuad(i % tessLevelX, i / tessLevelZ);
            }
        }
        public void Update(float elapsedTime)
        {
            time += elapsedTime;
        }
        public void Render(Vector4 camPos)
        {
            Matrix world = Matrix.Translation(position);
            Matrix view = D3DDevice.Instance.Device.Transform.View;
            Matrix projection = D3DDevice.Instance.Device.Transform.Projection;

            ShaderRepository.WaterSurface.Begin(FX.None);
            ShaderRepository.WaterSurface.BeginPass(0);

            D3DDevice.Instance.Device.RenderState.AlphaBlendEnable = true;
            ShaderRepository.WaterSurface.SetValue("transform", world * view * projection);
            ShaderRepository.WaterSurface.SetValue("time", time * velocity);
            ShaderRepository.WaterSurface.SetValue("camPos", camPos);

            D3DDevice.Instance.Device.VertexFormat = CustomVertex.PositionColored.Format;
            D3DDevice.Instance.Device.DrawUserPrimitives(PrimitiveType.TriangleList, quadsNumber * 2, vertexBuffer);

            ShaderRepository.WaterSurface.EndPass();
            ShaderRepository.WaterSurface.End();
        }
        private void SetQuad(int x, int z)
        {
            float fx = sizeX / tessLevelX;
            float fz = sizeZ / tessLevelZ;

            Vector3 positionOffset = new Vector3(sizeX / 2f, 0, sizeZ / 2f);
            Vector3 quadOffset = new Vector3(x * fx, 0, z * fz);

            var p0 = quadOffset + new Vector3(0, 0, 0) - positionOffset;
            var p1 = quadOffset + new Vector3(fx, 0, 0) - positionOffset;
            var p2 = quadOffset + new Vector3(0, 0, fz) - positionOffset;
            var p3 = quadOffset + new Vector3(fx, 0, fz) - positionOffset;

            int quads = z * tessLevelX + x;
            int prevTriangles = 2 * quads;
            int prevVertices = prevTriangles * 3;

            vertexBuffer[prevVertices + 0] = new CustomVertex.PositionColored(p0, 0x000000);
            vertexBuffer[prevVertices + 1] = new CustomVertex.PositionColored(p1, 0xFF0000);
            vertexBuffer[prevVertices + 2] = new CustomVertex.PositionColored(p2, 0x00FF00);
            vertexBuffer[prevVertices + 3] = new CustomVertex.PositionColored(p1, 0xFF0000);
            vertexBuffer[prevVertices + 4] = new CustomVertex.PositionColored(p2, 0x00FF00);
            vertexBuffer[prevVertices + 5] = new CustomVertex.PositionColored(p3, 0xFFFF00);
        }
    }
}
