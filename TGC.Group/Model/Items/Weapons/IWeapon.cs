using Microsoft.DirectX.Direct3D;
using TGC.Core.Input;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;
using TGC.Group.Model.Items.Recipes;
using TGC.Group.Model.Items.Type;
using TGC.Group.Model.Player;
using TGC.Group.TGCUtils;

namespace TGC.Group.Model.Items
{
    public abstract class Weapon : ICrafteable
    {
        public override ItemType type { get; } = ItemType.WEAPON;

        protected TgcMesh Mesh;

        protected Weapon(Recipe recipe, TgcMesh mesh) : base(recipe)
        {
            Mesh = mesh;
        }

        public override void Use(Character character)
        {
            character.Weapon = this;
        }

        public abstract void Attack(World world, TgcD3dInput input);

        public abstract void Update(Camera camera);

        public void Render()
        {
            Mesh.Render();
        }

    }
    
}