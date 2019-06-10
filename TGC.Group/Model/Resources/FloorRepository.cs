using System;
using System.Collections.Concurrent;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Threading.Tasks;
using TGC.Core.Direct3D;
using TGC.Core.Mathematica;
using TGC.Core.Terrain;
using TGC.Core.Textures;
using TGC.Group.Model.Chunks;

namespace TGC.Group.Model.Resources
{
    public static class FloorRepository
    {
        private static readonly string FloorTexture = Game.Default.MediaDirectory + Game.Default.TexturaTierra;
        private static readonly string FloorHeightmap = Game.Default.MediaDirectory + "\\Heightmap";
        private static ConcurrentDictionary<TGCVector3, TgcSimpleTerrain> Floors = new ConcurrentDictionary<TGCVector3, TgcSimpleTerrain>();
        private static bool preLoading = false;

        public static TgcSimpleTerrain getFloor(TGCVector3 position)
        {
            var cacheHit = Floors.ContainsKey(position);
            if (!cacheHit)
            {
                Task.Run(()=>preLoad(position, World.UpdateRadius));
            }
            return cacheHit ? Floors[position] : Add(position);
        }

        public static void preLoad(TGCVector3 origin, int radius)
        {
            if (preLoading) return;
            preLoading = true;
            var range = Enumerable.Range(-radius, radius * 2 + 1);
            var trueOrigin = new TGCVector3(
                Chunk.DefaultSize.X * (int) (Math.Round(origin.X / Chunk.DefaultSize.X)),
                Chunk.DefaultSize.Y * (int) (Math.Round(origin.Y / Chunk.DefaultSize.Y)),
                Chunk.DefaultSize.Z * (int) (Math.Round(origin.Z / Chunk.DefaultSize.Z)));

            var x = range.ToList().ConvertAll(num => num * Chunk.DefaultSize.X + trueOrigin.X);
            var z = range.ToList().ConvertAll(num => num * Chunk.DefaultSize.Z + trueOrigin.Z);
            var xz = x.SelectMany(xx => z.ConvertAll(zz => new TGCVector3(xx, origin.Y, zz))).ToList();
            
            xz.FindAll(xxzz => !Floors.ContainsKey(xxzz)).ForEach(xxzz => getFloor(xxzz));
            preLoading = false;
        }

        private static TgcSimpleTerrain Add(TGCVector3 position)
        {
            var terrain = CreateFloor(position);
            Floors.TryAdd(position, terrain);
            return terrain;
        }
        
        private static TgcSimpleTerrain CreateFloor(TGCVector3 origin)
        {
            var img = Image.FromFile(getHeightmap(origin));
            var imgSize = img.Height;
            var floor = new TgcSimpleTerrain();
            
            img.Dispose();
            
            var scaleXz = Chunk.DefaultSize.X / imgSize + 0.25f * Chunk.DefaultSize.X/1000 * imgSize/64;
            var xCenter = origin.X + Chunk.DefaultSize.X / 2 + imgSize * 2.0f / scaleXz;
            var zCenter = origin.Z + Chunk.DefaultSize.Z / 2 + imgSize * 2.0f / scaleXz;            
            
            floor.loadHeightmap(getHeightmap(origin), scaleXz, 1f, new TGCVector3(xCenter / scaleXz, origin.Y, zCenter / scaleXz));
            floor.loadTexture(FloorTexture);

            return floor;
        }
        private static string getHeightmap(TGCVector3 origin)
        {
            var x = Math.Abs(origin.X) % (Chunk.DefaultSize.X * 2) < Chunk.DefaultSize.X ? "1" : "2";
            
            var z = Math.Abs(origin.Z) % (Chunk.DefaultSize.Z * 2) < Chunk.DefaultSize.Z ? "1" : "2";

            return FloorHeightmap + x + z + ".jpg";
        }
    }
}