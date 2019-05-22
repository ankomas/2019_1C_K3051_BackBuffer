using TGC.Core.Mathematica;
using TGC.Group.Model.Items.Type;
using TGC.Group.Model.Player;
using TGC.Group.TGCUtils;

namespace TGC.Group.Model.Items
{
    public abstract class IItem
    {
        public abstract string Name { get; }
        public abstract string Description { get; }

        public abstract ItemType type { get; }

        public abstract CustomSprite Icon { get; }

        public abstract TGCVector2 DefaultScale { get; }

        public abstract void Use(Character character);

        public override bool Equals(object o)
        {
            return 
                ReferenceEquals(this, o) ||
                !ReferenceEquals(this, null) &&
                !ReferenceEquals(o, null) &&
                this.GetType() == o.GetType() &&
                Equals(this.Name, ((IItem) o).Name);
        }
        
        public override int GetHashCode()
        {
            return this.Name.GetHashCode();
        }
    }
}