using System;
using TGC.Group.Model.Player;
using TGC.Group.Model.Utils;

namespace TGC.Group.Model.UI
{
    public class StatsIndicators
    {
        private OxygenIndicator oxygenIndicator;
        private LifeIndicator lifeIndicator;

        public static int OxygenMeterSize = (int)Math.Floor(Screen.Height * 0.25f);
        public static int LifeMeterSize = (int)Math.Floor(Screen.Height * 0.1f);

        public StatsIndicators(int baseX0, int baseY0)
        {

            this.oxygenIndicator = new OxygenIndicator(OxygenMeterSize, baseX0, baseY0);
            this.lifeIndicator = new LifeIndicator(LifeMeterSize, baseX0 - LifeMeterSize, baseY0 - LifeMeterSize);
        }

        public void init()
        {
            this.oxygenIndicator.init();
            this.lifeIndicator.init();
        }

        public void Render(Character character)
        {
            this.oxygenIndicator.render(character);
            this.lifeIndicator.render(character);
        }
    }
}