using System;
using TGC.Core.Direct3D;
using TGC.Core.Geometry;
using TGC.Core.Mathematica;
using TGC.Core.Textures;
using TGC.Group.Model.Chunks;

namespace TGC.Group.Model.Elements
{
    public class WaterSurface
    {
        private static readonly TgcTexture FloorTexture = 
            TgcTexture.createTexture(D3DDevice.Instance.Device, Game.Default.ResDirectory + Game.Default.TexturaAgua);
        
        private TgcPlane surface;

        public WaterSurface(TGCVector3 initialPoint)
        {
            this.surface = new TgcPlane(initialPoint, Chunk.DefaultSize*World.RenderRadius, TgcPlane.Orientations.XZplane, FloorTexture);
        }

        public void Update(TGCVector3 position)
        {
            if (Math.Abs((int) (position.Y / Chunk.DefaultSize.Y)) > World.UpdateRadius) return;
            
            var size = Chunk.DefaultSize * World.RenderRadius * 2;
            var surfacePosition = new TGCVector3(position.X - size.X/2, 0, position.Z - size.Z/2);
            
            this.surface = new TgcPlane(surfacePosition, size, TgcPlane.Orientations.XZplane, FloorTexture);
        }

        public void Render(TGCVector3 position)
        {
            if (Math.Abs((int) (position.Y / Chunk.DefaultSize.Y)) > World.RenderRadius) return;
            
            this.surface.Render();
        }
    }
}