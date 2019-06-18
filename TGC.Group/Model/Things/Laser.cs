using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.Direct3D;

namespace TGC.Group.Model.Things
{
    class Laser
    {
        CustomVertex.PositionOnly[] vertexBuffer;
        public Vector3 position;
        public Vector2 delta = new Vector2(30, 130);
        public Vector3 scale = new Vector3(50, 130, 0);
        public float rotation = 0;
        public Laser()
        {
            vertexBuffer = new CustomVertex.PositionOnly[6];

            vertexBuffer[0] = new CustomVertex.PositionOnly(-0.1f,  0, 0);
            vertexBuffer[1] = new CustomVertex.PositionOnly( 0.1f,  0, 0);
            vertexBuffer[2] = new CustomVertex.PositionOnly(-0.1f, -1, 0);
            vertexBuffer[3] = new CustomVertex.PositionOnly( 0.1f, -1, 0);
            vertexBuffer[4] = new CustomVertex.PositionOnly(-0.1f, -1, 0);
            vertexBuffer[5] = new CustomVertex.PositionOnly( 0.1f,  0, 0);
        }
        public void Render()
        {
            scale.Y = Vector2.Length(delta);

            Matrix world = Matrix.Identity
                * Matrix.Scaling(scale)
                * Matrix.RotationZ(-(float)Math.Atan(delta.X / delta.Y))
                * Matrix.Translation(position)
                ;

            ShaderRepository.LaserShader.SetValue("world", world);
            ShaderRepository.LaserShader.SetValue("view", D3DDevice.Instance.Device.Transform.View);
            ShaderRepository.LaserShader.SetValue("projection", D3DDevice.Instance.Device.Transform.Projection);

            D3DDevice.Instance.Device.RenderState.AlphaBlendEnable = true;
            D3DDevice.Instance.Device.VertexFormat = CustomVertex.PositionOnly.Format;
            ShaderRepository.LaserShader.Begin(FX.None);
            ShaderRepository.LaserShader.BeginPass(0);
            D3DDevice.Instance.Device.DrawUserPrimitives(PrimitiveType.TriangleList, 2, vertexBuffer);
            ShaderRepository.LaserShader.EndPass();
            ShaderRepository.LaserShader.End();
        }
    }
}
