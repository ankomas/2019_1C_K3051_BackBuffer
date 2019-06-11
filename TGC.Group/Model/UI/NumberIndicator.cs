using System;
using System.Drawing;
using System.Security.Cryptography;
using System.Windows.Forms.VisualStyles;
using TGC.Group.Model.Player;

namespace TGC.Group.Model.UI
{
    public class NumberIndicator : CircularIndicator
    {
        public NumberIndicator(int meterSize, int meterX0, int meterY0) : base(meterSize, meterX0, meterY0)
        {
        }

        protected override void RenderEffect(Character character)
        {
            throw new System.NotImplementedException();
        }

        public void Render(int min, int max)
        {
            RenderEffect(min, max);
        }

        protected override void RenderText(Character character)
        {
            throw new System.NotImplementedException();
        }

        public void RenderText(int actual)
        {
            double o2Level = actual;

            var oXPosition = this.MeterX0 + ToInt(Scale(this.MeterSize, 54));
            var oYPosition = this.MeterY0 + ToInt(Scale(this.MeterSize, 32));

            var o2LevelXPosition = o2Level >= 10
                ? this.MeterX0 + ToInt(Scale(this.MeterSize, 55))
                : this.MeterX0 + ToInt(Scale(this.MeterSize, 65));
            var o2LevelYPosition = this.MeterY0 + ToInt(Scale(this.MeterSize, 74));

            this.TextBig.drawText("%", oXPosition, oYPosition, Color.Bisque);
            this.TextBig.drawText("" + o2Level, o2LevelXPosition, o2LevelYPosition, Color.Bisque);
        }
    }
}