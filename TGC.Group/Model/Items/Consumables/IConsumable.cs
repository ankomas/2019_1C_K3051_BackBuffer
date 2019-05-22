using TGC.Core.Mathematica;
using TGC.Group.Model.Items.Type;
using TGC.Group.Model.Player;
using TGC.Group.TGCUtils;

namespace TGC.Group.Model.Items.Consumables
{
    public abstract class IConsumable : IItem
    {
        public abstract Stats stats { get; }

        public override void Use(Character character)
        {
            character.Consume(this);
        }
    }
}