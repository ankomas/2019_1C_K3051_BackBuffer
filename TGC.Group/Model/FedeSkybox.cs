using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.Direct3D;

namespace TGC.Group.Model
{
    class FedeSkybox
    {
        CustomVertex.PositionNormalTextured[] vertexBuffer;
        Texture texture;
        public Vector3 position = new Vector3(), size = new Vector3();
        public static readonly float DefaultEpsilon = 0.00075f;

        public FedeSkybox(string path, float epsilon)
        {
            var corner0 = new Vector3( 1, -1, -1);
            var corner1 = new Vector3(-1, -1, -1);
            var corner2 = new Vector3( 1,  1, -1);
            var corner3 = new Vector3(-1,  1, -1);
            var corner4 = new Vector3( 1, -1,  1);
            var corner5 = new Vector3(-1, -1,  1);
            var corner6 = new Vector3( 1,  1,  1);
            var corner7 = new Vector3(-1,  1,  1);

            var normal0 = Vector3.Normalize(-corner0);
            var normal1 = Vector3.Normalize(-corner1);
            var normal2 = Vector3.Normalize(-corner2);
            var normal3 = Vector3.Normalize(-corner3);
            var normal4 = Vector3.Normalize(-corner4);
            var normal5 = Vector3.Normalize(-corner5);
            var normal6 = Vector3.Normalize(-corner6);
            var normal7 = Vector3.Normalize(-corner7);

            vertexBuffer = new CustomVertex.PositionNormalTextured[36];

            float x0 = 0 + epsilon, x1 = 0.25f + epsilon, x2 = 0.5f - epsilon, x3 = 0.75f, x4 = 1 - epsilon;
            float y0 = 0 + epsilon, y1 = 1 / 3f + epsilon, y2 = 2 / 3f - epsilon, y3 = 1 - epsilon;

            // Front
            vertexBuffer[0] = new CustomVertex.PositionNormalTextured(corner0, normal0, x1, y2);
            vertexBuffer[1] = new CustomVertex.PositionNormalTextured(corner1, normal1, x2, y2);
            vertexBuffer[2] = new CustomVertex.PositionNormalTextured(corner2, normal2, x1, y1);

            vertexBuffer[3] = new CustomVertex.PositionNormalTextured(corner1, normal1, x2, y2);
            vertexBuffer[4] = new CustomVertex.PositionNormalTextured(corner2, normal2, x1, y1);
            vertexBuffer[5] = new CustomVertex.PositionNormalTextured(corner3, normal3, x2, y1);

            // Top
            vertexBuffer[6] = new CustomVertex.PositionNormalTextured(corner2, normal2, x1, y1);
            vertexBuffer[7] = new CustomVertex.PositionNormalTextured(corner3, normal3, x2, y1);
            vertexBuffer[8] = new CustomVertex.PositionNormalTextured(corner7, normal7, x2, y0);

            vertexBuffer[9]  = new CustomVertex.PositionNormalTextured(corner2, normal2, x1, y1);
            vertexBuffer[10] = new CustomVertex.PositionNormalTextured(corner6, normal6, x1, y0);
            vertexBuffer[11] = new CustomVertex.PositionNormalTextured(corner7, normal7, x2, y0);

            // Right
            vertexBuffer[12] = new CustomVertex.PositionNormalTextured(corner1, normal1, x2, y2);
            vertexBuffer[13] = new CustomVertex.PositionNormalTextured(corner3, normal3, x2, y1);
            vertexBuffer[14] = new CustomVertex.PositionNormalTextured(corner7, normal7, x3, y1);

            vertexBuffer[15] = new CustomVertex.PositionNormalTextured(corner1, normal1, x2, y2);
            vertexBuffer[16] = new CustomVertex.PositionNormalTextured(corner5, normal5, x3, y2);
            vertexBuffer[17] = new CustomVertex.PositionNormalTextured(corner7, normal7, x3, y1);

            //Left
            vertexBuffer[18] = new CustomVertex.PositionNormalTextured(corner4, normal4, x0, y2);
            vertexBuffer[19] = new CustomVertex.PositionNormalTextured(corner6, normal6, x0, y1);
            vertexBuffer[20] = new CustomVertex.PositionNormalTextured(corner2, normal2, x1, y1);

            vertexBuffer[21] = new CustomVertex.PositionNormalTextured(corner4, normal4, x0, y2);
            vertexBuffer[22] = new CustomVertex.PositionNormalTextured(corner0, normal0, x1, y2);
            vertexBuffer[23] = new CustomVertex.PositionNormalTextured(corner2, normal2, x1, y1);

            //Down
            vertexBuffer[24] = new CustomVertex.PositionNormalTextured(corner1, normal1, x2, y2);
            vertexBuffer[25] = new CustomVertex.PositionNormalTextured(corner4, normal4, x1, y3);
            vertexBuffer[26] = new CustomVertex.PositionNormalTextured(corner5, normal5, x2, y3);

            vertexBuffer[27] = new CustomVertex.PositionNormalTextured(corner0, normal0, x1, y2);
            vertexBuffer[28] = new CustomVertex.PositionNormalTextured(corner1, normal1, x2, y2);
            vertexBuffer[29] = new CustomVertex.PositionNormalTextured(corner4, normal4, x1, y3);

            //Back
            vertexBuffer[30] = new CustomVertex.PositionNormalTextured(corner4, normal4, x4, y2);
            vertexBuffer[31] = new CustomVertex.PositionNormalTextured(corner5, normal5, x3, y2);
            vertexBuffer[32] = new CustomVertex.PositionNormalTextured(corner7, normal7, x3, y1);

            vertexBuffer[33] = new CustomVertex.PositionNormalTextured(corner4, normal4, x4, y2);
            vertexBuffer[34] = new CustomVertex.PositionNormalTextured(corner6, normal6, x4, y1);
            vertexBuffer[35] = new CustomVertex.PositionNormalTextured(corner7, normal7, x3, y1);

            //texture = TextureLoader.FromFile(D3DDevice.Instance.Device, Game.Default.MediaDirectory + "Daylight-Box-UV.png");
            //texture = TextureRepository.SkyboxOutside;

            texture = TextureLoader.FromFile(D3DDevice.Instance.Device, Game.Default.MediaDirectory + path);
        }

        public void Render()
        {
            ShaderRepository.FedeSkybox.Technique = "Outside";
            ShaderRepository.FedeSkybox.Begin(FX.None);
            ShaderRepository.FedeSkybox.BeginPass(0);

            ShaderRepository.FedeSkybox.SetValue("tex", texture);

            Matrix world = Matrix.Identity * Matrix.Scaling(size) * Matrix.Translation(position);
            Matrix view = D3DDevice.Instance.Device.Transform.View;
            Matrix projection = D3DDevice.Instance.Device.Transform.Projection;
            Matrix pp = Matrix.PerspectiveFovLH(
                (float)Math.PI / 2,
                D3DDevice.Instance.AspectRatio,
                D3DDevice.Instance.ZNearPlaneDistance,
                D3DDevice.Instance.ZFarPlaneDistance * 2
                );
            ShaderRepository.FedeSkybox.SetValue("transform", world * view * projection);

            D3DDevice.Instance.Device.VertexFormat = CustomVertex.PositionNormalTextured.Format;
            D3DDevice.Instance.Device.DrawUserPrimitives(PrimitiveType.TriangleList, vertexBuffer.Length / 3, vertexBuffer);

            ShaderRepository.FedeSkybox.EndPass();
            ShaderRepository.FedeSkybox.End();
        }
        public void Render(Vector4 cameraPosition)
        {
            ShaderRepository.FedeSkybox.Technique = "Underwater";
            ShaderRepository.FedeSkybox.Begin(FX.None);
            ShaderRepository.FedeSkybox.BeginPass(0);
            
            ShaderRepository.FedeSkybox.SetValue("tex", texture);
            ShaderRepository.FedeSkybox.SetValue("skyboxFarness", size.X * 1.6f);

            Matrix world = Matrix.Identity * Matrix.Scaling(size) * Matrix.Translation(position);
            Matrix view = D3DDevice.Instance.Device.Transform.View;
            Matrix projection = D3DDevice.Instance.Device.Transform.Projection;
            Matrix pp = Matrix.PerspectiveFovLH(
                (float)Math.PI / 2,
                D3DDevice.Instance.AspectRatio,
                D3DDevice.Instance.ZNearPlaneDistance,
                D3DDevice.Instance.ZFarPlaneDistance * 2
                );
            ShaderRepository.FedeSkybox.SetValue("transform", world * view * projection);

            D3DDevice.Instance.Device.VertexFormat = CustomVertex.PositionNormalTextured.Format;
            D3DDevice.Instance.Device.DrawUserPrimitives(PrimitiveType.TriangleList, vertexBuffer.Length / 3, vertexBuffer);

            ShaderRepository.FedeSkybox.EndPass();
            ShaderRepository.FedeSkybox.End();
        }
    }
}
