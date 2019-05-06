using System;
using TGC.Core.BoundingVolumes;
using TGC.Core.SceneLoader;

namespace TGC.Group.Model
{
    public class Entity : Collisionable
    {
        public void Render()
        {
            return;
        }

        public void Update()
        {
            return;
        }

        public void Dispose()
        {
            return;
        }

        public override IRenderObject getCollisionVolume()
        {
            throw new NotImplementedException();
        }
    }
}