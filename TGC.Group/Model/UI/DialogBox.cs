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
using TGC.Group.Model.Resources.Sprites;
using TGC.Group.Model.Utils;
using TGC.Group.TGCUtils;

namespace TGC.Group.Model.UI
{
    class DialogBox
    {
        private Drawer2D drawer2D = new Drawer2D();
        public static TgcText2D textSmall = new TgcText2D();
        public static TgcText2D textBig = new TgcText2D();
        private List<Line> Lines = new List<Line>();
        private static int SmallLineHeight = 21;
        private static int BigLineHeight = 25;
        private static int SmallLetterWidth = 9;
        private static int BigLetterWidth = 12;
        private static int MinHeight = 52;
        private TGCVector2 textOffset = new TGCVector2(4, 2);
        private int smallLineOffset = SmallLineHeight;
        private int bigLineOffset = BigLineHeight;

        private CustomVertex.TransformedColored[] vertexBuffer;
        private float width = 0, height = 0;
        private Vector2 position = new Vector2();
        private float borderThickness = 2;
        public Color color = Color.FromArgb(180, 0, 0, 0), borderColor = Color.Transparent;
        private float interiorOpacity = 1, borderOpacity = 1;

        private delegate void Renderer();
        Renderer renderer = () => {};
        private delegate void Updater(float elapsedTime);
        Updater updater = (e) => {};

        float openingLevel = 1;
        public DialogBox()
        {
            textBig.changeFont(new System.Drawing.Font("Arial Black", 10f));

            vertexBuffer    = new CustomVertex.TransformedColored[6];
            vertexBuffer[0] = new CustomVertex.TransformedColored(  0,   0, 0, 1, 0x000000); // Top-Left
            vertexBuffer[1] = new CustomVertex.TransformedColored(100,   0, 0, 1, 0xFF0000); // Top-Right
            vertexBuffer[2] = new CustomVertex.TransformedColored(100, 100, 0, 1, 0xFFFF00); // Bottom-Right
            vertexBuffer[3] = new CustomVertex.TransformedColored(  0,   0, 0, 1, 0x000000); // Top-Left
            vertexBuffer[4] = new CustomVertex.TransformedColored(  0, 100, 0, 1, 0x00FF00); // Bottom-Left
            vertexBuffer[5] = new CustomVertex.TransformedColored(100, 100, 0, 1, 0xFFFF00); // Bottom-Right

            updater = MainUpdate;
            renderer = MainRender;
        }
        private int findMaxLineLength()
        {
            int max = 0;
            foreach (Line line in Lines)
            {
                if (line.text.Length > max) max = line.text.Length;
            }
            return max;
        }
        public void AddLineSmall(string line)
        {
            AddLineSmall(line, Color.White);
        }
        public void AddLineBig(string line)
        {
            AddLineBig(line, Color.White);
        }
        public void AddLineBig(string line, Color color)
        {
            AddLine(line, color, textBig);
        }
        public void AddLineSmall(string line, Color color)
        {
            AddLine(line, color, textSmall);
        }
        private void SetPositionWidthHeight(Vector2 p, float w, float h)
        {
            // Top-Left
            vertexBuffer[0].X = vertexBuffer[3].X = p.X;
            vertexBuffer[0].Y = vertexBuffer[3].Y = p.Y;

            // Top-Right
            vertexBuffer[1].X = p.X + w;
            vertexBuffer[1].Y = p.Y;

            // Bottom-Left
            vertexBuffer[4].X = p.X;
            vertexBuffer[4].Y = p.Y + h;

            // Bottom-Right
            vertexBuffer[2].X = vertexBuffer[5].X = p.X + w;
            vertexBuffer[2].Y = vertexBuffer[5].Y = p.Y + h;
        }
        private void AddLine(string line, Color color, TgcText2D text)
        {
            Lines.Add(new Line(line, color, text));

            float targetHeight = 0;
            foreach(Line lineElement in Lines)
            {
                targetHeight += lineElement.textDrawer == textSmall ? SmallLineHeight : BigLineHeight;
            }
            height = targetHeight;
            width = findMaxLineLength() * (text == textSmall ? SmallLetterWidth : BigLetterWidth);
        }
        public void Clear()
        {
            Lines.Clear();
            width = height = 0;
        }
        public void Open()
        {
            openingLevel = 0;
            updater = UpdateOpen;
            renderer = RenderOpen;
        }
        private void UpdateOpen(float elapsedTime)
        {
            openingLevel += 5f * elapsedTime;
            interiorOpacity = color.A * openingLevel;
            borderOpacity = borderColor.A * openingLevel;
            SetPositionWidthHeight(position, width * openingLevel, height * openingLevel);

            if (openingLevel >= 1)
            {
                updater = MainUpdate;
                renderer = MainRender;
            }
        }
        private void MainUpdate(float elapsedTime)
        {
            interiorOpacity = color.A;
            borderOpacity = borderColor.A;
            SetPositionWidthHeight(position, width, height);
        }
        public void Update(float elapsedTime)
        {
            var cursorPosition = System.Windows.Forms.Cursor.Position;
            position.X = cursorPosition.X + 20;
            position.Y = cursorPosition.Y + 40;
            updater(elapsedTime);
        }
        public void Render()
        {
            renderer();
        }
        public void RenderOpen()
        {
            RenderRectangle();
        }
        private void RenderRectangle()
        {
            var shader = ShaderRepository.DialogBoxShader;
            shader.Begin(FX.None);
            shader.BeginPass(0);

            D3DDevice.Instance.Device.VertexFormat = CustomVertex.TransformedColored.Format;
            D3DDevice.Instance.Device.RenderState.AlphaBlendEnable = true;
            shader.SetValue("screenPos", new float[2] { position.X, position.Y });
            shader.SetValue("size", new float[2] { width * openingLevel, height * openingLevel });
            shader.SetValue("borderThickness", borderThickness);
            shader.SetValue("color", new float[4] { color.R / 255f, color.G / 255f, color.B / 255f, interiorOpacity / 255f });
            D3DDevice.Instance.Device.DrawUserPrimitives(PrimitiveType.TriangleList, 2, vertexBuffer);

            shader.EndPass();

            if (borderColor != Color.Transparent)
            {
                shader.BeginPass(1);

                D3DDevice.Instance.Device.VertexFormat = CustomVertex.TransformedColored.Format;
                D3DDevice.Instance.Device.RenderState.AlphaBlendEnable = true;
                shader.SetValue("borderColor", new float[4] { borderColor.R / 255f, borderColor.G / 255f, borderColor.B / 255f, borderOpacity / 255f });
                D3DDevice.Instance.Device.DrawUserPrimitives(PrimitiveType.TriangleList, 2, vertexBuffer);

                shader.EndPass();
            }

            shader.End();
        }
        public void MainRender()
        {
            RenderRectangle();
            int lineNumber = 0;
            int lineOffset = 0;
            foreach (Line line in Lines)
            {
                line.textDrawer.drawText(line.text, (int)(position.X + textOffset.X), (int)(position.Y + textOffset.Y) + lineOffset, line.color);
                lineOffset += line.textDrawer == textSmall ? smallLineOffset : bigLineOffset;
            }
        }
    }
    class Line
    {
        public string text = "";
        public Color color;
        public TgcText2D textDrawer;
        public Line(string text) : this(text, Color.White, DialogBox.textSmall) {}
        public Line(string text, Color color, TgcText2D textDrawer)
        {
            this.text = text;
            this.color = color;
            this.textDrawer = textDrawer;
        }
    }
}
