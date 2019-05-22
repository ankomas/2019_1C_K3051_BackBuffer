using System;
using System.Drawing;
using TGC.Group.Model.Player;

namespace TGC.Group.Model.UI
{
    public class OxygenIndicator : CircularIndicator
    {
        public OxygenIndicator(int meterSize, int meterX0, int meterY0) : base(meterSize, meterX0, meterY0)
        {
        }

        protected override void renderEffect(Character character)
        {
            renderEffect(character.ActualStats.Oxygen, character.MaxStats.Oxygen);
        }

        protected override void renderText(Character character)
        {
            double o2Level = Math.Floor(character.ActualStats.Oxygen);

            var oXPosition = this.MeterX0 + ToInt(Scale(this.MeterSize, 54));
            var oYPosition = this.MeterY0 + ToInt(Scale(this.MeterSize, 32));

            var twoXPosition = this.MeterX0 + ToInt(Scale(this.MeterSize, 79));
            var twoYPosition = this.MeterY0 + ToInt(Scale(this.MeterSize, 45));

            var o2LevelXPosition = o2Level >= 10
                ? this.MeterX0 + ToInt(Scale(this.MeterSize, 55))
                : this.MeterX0 + ToInt(Scale(this.MeterSize, 65));
            var o2LevelYPosition = this.MeterY0 + ToInt(Scale(this.MeterSize, 74));

            this.TextBig.drawText("O", oXPosition, oYPosition, Color.Bisque);
            this.TextSmall.drawText("2", twoXPosition, twoYPosition, Color.Bisque);
            this.TextBig.drawText("" + o2Level, o2LevelXPosition, o2LevelYPosition, Color.Bisque);
        }
    }
}