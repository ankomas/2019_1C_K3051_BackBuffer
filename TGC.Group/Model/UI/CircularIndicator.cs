using System;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.DirectX.Direct3D;
using TGC.Core.Direct3D;
using TGC.Core.Mathematica;
using TGC.Core.Text;
using TGC.Group.Model.Player;
using TGC.Group.Model.Resources.Sprites;
using TGC.Group.TGCUtils;
using Screen = TGC.Group.Model.Utils.Screen;

namespace TGC.Group.Model.UI
{
    public abstract class CircularIndicator
    {
        private static readonly Drawer2D Drawer = new Drawer2D();
        private const int DefaultMeterSize = 145;

        private Effect effect;

        protected readonly int MeterSize;
        protected readonly int MeterX0;
        protected readonly int MeterY0;

        private CustomVertex.TransformedColored[] vertices;

        protected readonly TgcText2D TextBig = new TgcText2D();
        protected readonly TgcText2D TextSmall = new TgcText2D();

        private static float ScalingFactor(float defaultSize)
        {
            return defaultSize / DefaultMeterSize;
        }

        protected static float Scale(float toScale, float defaultSize)
        {
            return toScale*ScalingFactor(defaultSize);
        }

        protected static int ToInt(float number)
        {
            return (int)Math.Floor(number);
        }

        public CircularIndicator(int meterSize, int meterX0, int meterY0)
        {
            this.MeterSize = meterSize;
            this.MeterX0 = meterX0;
            this.MeterY0 = meterY0;
            
            this.TextBig.changeFont(new System.Drawing.Font("Arial Narrow Bold", Scale(this.MeterSize, 25)));
            this.TextSmall.changeFont(new System.Drawing.Font("Arial Narrow Bold", Scale(this.MeterSize, 15)));
        }

        public void Init()
        {
            string compilationErrors;
            
            try
            {
                this.effect = Effect.FromFile(D3DDevice.Instance.Device, "../../../Shaders/Oxygen.fx", null, null, ShaderFlags.None, null, out compilationErrors);
            }
            catch(Exception e)
            {
                throw new Exception("No pudo cargar el archivo csm");
            }
            if(this.effect == null)
            {
                throw new Exception("Errores de compilación oxigen.fx: " + compilationErrors);
            }

            this.effect.Technique = "OxygenTechnique";

            var black = 0x000000;
            var green = 0x00FF00;
            var red = 0xFF0000;
            var yellow = 0xFFFF00;

            vertices = new CustomVertex.TransformedColored[6];
            vertices[0] = new CustomVertex.TransformedColored(this.MeterX0, this.MeterY0, 0, 1, black);
            vertices[1] = new CustomVertex.TransformedColored(this.MeterX0 + this.MeterSize, this.MeterY0, 0, 1, red);
            vertices[2] = new CustomVertex.TransformedColored(this.MeterX0, this.MeterY0 + this.MeterSize, 0, 1, green);
            vertices[3] = new CustomVertex.TransformedColored(this.MeterX0, this.MeterY0 + this.MeterSize, 0, 1, green);
            vertices[4] = new CustomVertex.TransformedColored(this.MeterX0 + this.MeterSize, this.MeterY0  , 0, 1, red);
            vertices[5] = new CustomVertex.TransformedColored(this.MeterX0 + this.MeterSize, this.MeterY0 + this.MeterSize, 0, 1, yellow);
        }

        public void Render(Character character)
        {
            RenderEffect(character);
            RenderText(character);
        }

        protected void RenderEffect(float actualStat, float maxStat)
        {
            /**********OXYGEN METER SHADER***********/
            this.effect.Begin(FX.None);
            this.effect.BeginPass(0);
            this.effect.SetValue("oxygen", actualStat / maxStat);
            D3DDevice.Instance.Device.RenderState.AlphaBlendEnable = true;
            D3DDevice.Instance.Device.VertexFormat = CustomVertex.TransformedColored.Format;
            D3DDevice.Instance.Device.DrawUserPrimitives(PrimitiveType.TriangleList, this.vertices.Length / 3, this.vertices);
            this.effect.EndPass();
            this.effect.BeginPass(1);
            this.effect.SetValue("oxygen", actualStat / maxStat);
            D3DDevice.Instance.Device.RenderState.AlphaBlendEnable = true;
            D3DDevice.Instance.Device.VertexFormat = CustomVertex.TransformedColored.Format;
            D3DDevice.Instance.Device.DrawUserPrimitives(PrimitiveType.TriangleList, this.vertices.Length / 3, this.vertices);
            this.effect.EndPass();
            this.effect.BeginPass(2);
            this.effect.SetValue("oxygen", actualStat / maxStat);
            D3DDevice.Instance.Device.RenderState.AlphaBlendEnable = true;
            D3DDevice.Instance.Device.VertexFormat = CustomVertex.TransformedColored.Format;
            D3DDevice.Instance.Device.DrawUserPrimitives(PrimitiveType.TriangleList, this.vertices.Length / 3, this.vertices);
            this.effect.EndPass();
            this.effect.End();
            /****************************************/
        }

        protected abstract void RenderEffect(Character character);
        protected abstract void RenderText(Character character);
    }
}