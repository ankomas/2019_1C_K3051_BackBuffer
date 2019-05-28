using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        private CustomSprite darknessCover;
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
        public DialogBox()
        {
            InitSprite();
            textBig.changeFont(new System.Drawing.Font("Arial Black", 12f));
        }
        private void InitSprite()
        {
            darknessCover = BitmapRepository.CreateSpriteFromBitmap(BitmapRepository.BlackRectangle);
            darknessCover.Scaling = new TGCVector2(.35f, .02f);
            darknessCover.Color = Color.FromArgb(220, 0, 0, 0);
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
        private void AddLine(string line, Color color, TgcText2D text)
        {
            Lines.Add(new Line(line, color, text));

            float bitmapY = darknessCover.Bitmap.Height;
            float targetY = 0;
            foreach(Line lineElement in Lines)
            {
                targetY += lineElement.textDrawer == textSmall ? SmallLineHeight : BigLineHeight;
            }
            float scaleY = targetY / bitmapY;

            float bitmapX = darknessCover.Bitmap.Width;
            float targetX = findMaxLineLength() * (text == textSmall ? SmallLetterWidth : BigLetterWidth);
            float scaleX = targetX / bitmapX;

            TGCVector2 s = new TGCVector2(scaleX, scaleY);

            darknessCover.Scaling = s;
        }
        public void Clear()
        {
            Lines.Clear();
            darknessCover.Scaling = new TGCVector2(0, 0);
        }
        public void Render()
        {
            var cursorPosition = System.Windows.Forms.Cursor.Position;
            darknessCover.Position = new TGCVector2(cursorPosition.X + 20, cursorPosition.Y + 40);
            drawer2D.BeginDrawSprite();
            drawer2D.DrawSprite(darknessCover);
            drawer2D.EndDrawSprite();
            int lineNumber = 0;
            int lineOffset = 0;
            foreach (Line line in Lines)
            {
                line.textDrawer.drawText(line.text, (int)(darknessCover.Position.X + textOffset.X), (int)(darknessCover.Position.Y + textOffset.Y) + lineOffset, line.color);
                lineOffset += line.textDrawer == textSmall ? smallLineOffset : bigLineOffset;
            }
        }
    }
    class Line
    {
        public string text;
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
