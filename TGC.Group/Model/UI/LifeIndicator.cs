using System;
using System.Drawing;
using TGC.Group.Model.Player;

namespace TGC.Group.Model.UI
{
    public class LifeIndicator : CircularIndicator
    {
        public LifeIndicator(int meterSize, int meterX0, int meterY0) : base(meterSize, meterX0, meterY0)
        {
        }

        protected override void RenderEffect(Character character)
        {
            RenderEffect(character.ActualStats.Life, character.MaxStats.Life);
        }

        protected override void RenderText(Character character)
        {
            double lifeLevel = Math.Floor(character.ActualStats.Life);

            var oXPosition = this.MeterX0 + ToInt(Scale(this.MeterSize, 60));
            var oYPosition = this.MeterY0 + ToInt(Scale(this.MeterSize, 32));

            var o2LevelXPosition = lifeLevel >= 10
                ? this.MeterX0 + ToInt(Scale(this.MeterSize, 48))
                : this.MeterX0 + ToInt(Scale(this.MeterSize, 65));
            var o2LevelYPosition = this.MeterY0 + ToInt(Scale(this.MeterSize, 74));

            this.TextBig.drawText("❤", oXPosition, oYPosition, Color.Bisque);
            this.TextBig.drawText("" + lifeLevel, o2LevelXPosition, o2LevelYPosition, Color.Bisque);
        }
    }
}