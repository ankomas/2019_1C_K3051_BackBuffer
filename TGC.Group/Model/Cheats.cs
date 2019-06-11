using TGC.Group.Model.Items;
using TGC.Group.Model.Items.Equipment;
using TGC.Group.Model.Items.Materials;
using TGC.Group.Model.Player;

namespace TGC.Group.Model
{
    public static class Cheats
    {
        public static bool GodMode;

        public static bool StartingItems;

        public static void ActivateNext()
        {
            if (!GodMode)
            {
                GodMode = true;
            } else if (!StartingItems)
            {
                StartingItems = true;
            }
        }
        
        public static void DesactivateNext()
        {
            if (GodMode)
            {
                GodMode = false;
            } else if (StartingItems)
            {
                StartingItems = false;
            }
        }

        public static void ApplyCheats(Character character)
        {
            if (StartingItems)
            {
                character.GiveItem(new InfinityGauntlet());
                character.GiveItem(new GreenPotion());
                character.GiveItem(new OxygenTank());

                StartingItems = false;
            }
        }
    }
}