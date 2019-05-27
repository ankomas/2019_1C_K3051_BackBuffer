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
        private TgcText2D DrawText = new TgcText2D();
        private List<string> Lines = new List<string>();
        private static int LineHeight = 21;
        private static int LetterWidth = 9;
        private static int MinHeight = 52;
        private TGCVector2 textOffset = new TGCVector2(4, 2);
        private int lineOffset = LineHeight;
        public DialogBox()
        {
            InitSprite();
        }
        private void InitSprite()
        {
            darknessCover = BitmapRepository.CreateSpriteFromBitmap(BitmapRepository.BlackRectangle);
            darknessCover.Scaling = new TGCVector2(.35f, .02f);
            darknessCover.Color = Color.FromArgb(188, 0, 0, 0);
            Screen.CenterSprite(darknessCover);
            darknessCover.Position = new TGCVector2(darknessCover.Position.X + 120, darknessCover.Position.Y + 80);
        }
        private int findMaxLineLength()
        {
            int max = 0;
            foreach (string line in Lines)
            {
                if (line.Length > max) max = line.Length;
            }
            return max;
        }
        public void AddLine(string line)
        {
            Lines.Add(line);

            float bitmapY = darknessCover.Bitmap.Height;
            float targetY = Lines.Count * LineHeight;
            float scaleY = targetY / bitmapY;

            float bitmapX = darknessCover.Bitmap.Width;
            float targetX = findMaxLineLength() * LetterWidth;
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
            drawer2D.BeginDrawSprite();
            drawer2D.DrawSprite(darknessCover);
            drawer2D.EndDrawSprite();
            int lineNumber = 0;
            foreach (string line in Lines)
            {
                DrawText.drawText(line, (int)(darknessCover.Position.X + textOffset.X), (int)(darknessCover.Position.Y + textOffset.Y) + lineNumber++ * lineOffset, Color.White);
            }
        }
    }
}
