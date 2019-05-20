using System;
using System.Drawing;
using Microsoft.DirectX.Direct3D;
using TGC.Core.Direct3D;
using TGC.Core.Mathematica;
using TGC.Core.Text;
using TGC.Group.Model.Player;
using TGC.Group.Model.Resources.Sprites;
using TGC.Group.TGCUtils;

namespace TGC.Group.Model.UI
{
    public class OxygenIndicator
    {
        private Effect OxygenEffect;
        
        Drawer2D drawer = new Drawer2D();
        
        const int o2MeterSize = 145;
        const int o2MeterX0 = 110;
        const int o2MeterY0 = 475;

        private CustomSprite blackCircle;
        
        CustomVertex.TransformedColored[] vertices;
        
        TgcText2D TextO2Big = new TgcText2D(), TextO2Small = new TgcText2D();

        public OxygenIndicator()
        {
            blackCircle = BitmapRepository.CreateSpriteFromBitmap(BitmapRepository.BlackCircle);
            blackCircle.Scaling = new TGCVector2(.295f, .295f);
            blackCircle.Position = new TGCVector2(o2MeterX0 - 3, o2MeterY0 - 3);
            blackCircle.Color = Color.FromArgb(120, 0, 0, 0);
            
            TextO2Big.changeFont(new System.Drawing.Font("Arial Narrow Bold", 25f));
            TextO2Small.changeFont(new System.Drawing.Font("Arial Narrow Bold", 15f));
        }

        public void init()
        {
            string compilationErrors;
            try
            {
                OxygenEffect = Effect.FromFile(D3DDevice.Instance.Device, "../../../Shaders/Oxygen.fx", null, null, ShaderFlags.None, null, out compilationErrors);
            }
            catch(Exception e)
            {
                throw new Exception("No pudo cargar el archivo csm");
            }
            if(OxygenEffect == null)
            {
                throw new Exception("Errores de compilación oxigen.fx: " + compilationErrors);
            }

            OxygenEffect.Technique = "OxygenTechnique";

            vertices = new CustomVertex.TransformedColored[6];
            vertices[0] = new CustomVertex.TransformedColored(o2MeterX0, o2MeterY0, 0, 1, 0x000000);
            vertices[1] = new CustomVertex.TransformedColored(o2MeterX0 + o2MeterSize, o2MeterY0, 0, 1, 0xFF0000);
            vertices[2] = new CustomVertex.TransformedColored(o2MeterX0, o2MeterY0 + o2MeterSize, 0, 1, 0x00FF00);
            vertices[3] = new CustomVertex.TransformedColored(o2MeterX0, o2MeterY0 + o2MeterSize, 0, 1, 0x00FF00);
            vertices[4] = new CustomVertex.TransformedColored(o2MeterX0 + o2MeterSize, o2MeterY0  , 0, 1, 0xFF0000);
            vertices[5] = new CustomVertex.TransformedColored(o2MeterX0 + o2MeterSize, o2MeterY0 + o2MeterSize, 0, 1, 0xFFFF00);
        }

        public void render(Character character)
        {
            double o2Level = Math.Floor((float)character.ActualStats.Oxygen);
            
            this.TextO2Big.drawText("O", o2MeterX0 + 54, o2MeterY0 + 32, Color.Bisque);
            this.TextO2Small.drawText("2", o2MeterX0 + 79, o2MeterY0 + 45, Color.Bisque);
            this.TextO2Big.drawText("" + o2Level, o2Level >= 10 ? o2MeterX0 + 55 : o2MeterX0 + 65, o2MeterY0 + 74, Color.Bisque);
            
            /**********OXYGEN METER SHADER***********/
            OxygenEffect.Begin(FX.None);
            OxygenEffect.BeginPass(0);
            OxygenEffect.SetValue("oxygen", (float)(character.ActualStats.Oxygen) / character.MaxStats.Oxygen);
            D3DDevice.Instance.Device.RenderState.AlphaBlendEnable = true;
            D3DDevice.Instance.Device.VertexFormat = CustomVertex.TransformedColored.Format;
            D3DDevice.Instance.Device.DrawUserPrimitives(PrimitiveType.TriangleList, vertices.Length / 3, vertices);
            OxygenEffect.EndPass();
            OxygenEffect.BeginPass(1);
            D3DDevice.Instance.Device.RenderState.AlphaBlendEnable = true;
            D3DDevice.Instance.Device.VertexFormat = CustomVertex.TransformedColored.Format;
            D3DDevice.Instance.Device.DrawUserPrimitives(PrimitiveType.TriangleList, vertices.Length / 3, vertices);
            OxygenEffect.EndPass();
            OxygenEffect.End();
            /****************************************/
            
            drawer.BeginDrawSprite();
            drawer.DrawSprite(blackCircle);
            drawer.EndDrawSprite();
        }

    }
}