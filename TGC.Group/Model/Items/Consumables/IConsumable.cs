using TGC.Core.Mathematica;
using TGC.Group.Model.Items.Type;
using TGC.Group.Model.Player;
using TGC.Group.TGCUtils;

namespace TGC.Group.Model.Items.Consumables
{
    public abstract class IConsumable : IItem
    {
        public abstract string Name { get; }
        public abstract string Description { get; }
        public abstract ItemType type { get; }
        public abstract CustomSprite Icon { get; set; }
        public abstract TGCVector2 DefaultScale { get; }

        public abstract Stats stats { get; }

        public void Use(Character character)
        {
            character.Consume(this);
        }
    }
}