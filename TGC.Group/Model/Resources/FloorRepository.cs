using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Microsoft.DirectX.Direct3D;
using TGC.Core.Direct3D;
using TGC.Core.Mathematica;
using TGC.Core.Terrain;
using TGC.Core.Textures;
using TGC.Group.Model.Chunks;
using TGC.Group.Model.UI;

namespace TGC.Group.Model.Resources
{
    public static class FloorRepository
    {
        private static readonly string FloorPath = Game.Default.MediaDirectory + Game.Default.TexturaTierra;
        private static Texture FloorTexture;
        private static readonly string FloorHeightmap = Game.Default.MediaDirectory + "\\Heightmap";
        public static Dictionary<TGCVector3, MySimpleTerrain> Floors = new Dictionary<TGCVector3, MySimpleTerrain>();
        
        private static bool preLoading = false;
        public static int generating;

        private static Dictionary<String, Bitmap> heightmaps = new Dictionary<string, Bitmap>();

        private static Image img;
        
        private static Texture loadTexture(string path)
        {
            if (FloorTexture != null)
                return FloorTexture;
            
            Bitmap image = (Bitmap) Image.FromFile(path);
            image.RotateFlip(RotateFlipType.Rotate90FlipX);
            
            FloorTexture = Texture.FromBitmap(D3DDevice.Instance.Device, image, Usage.AutoGenerateMipMap, Pool.Managed);
            
            image.Dispose();

            return FloorTexture;
        }

        public static MySimpleTerrain getFloor(TGCVector3 position)
        {
            return Floors.ContainsKey(position) ? Floors[position] : Add(position);
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
            
            var toGenerate = xz.FindAll(xxzz => !Floors.ContainsKey(xxzz));
            generating += toGenerate.Count;
            toGenerate.ForEach(xxzz => getFloor(xxzz));
            preLoading = false;
        }

        private static MySimpleTerrain Add(TGCVector3 position)
        {
            var terrain = CreateFloor(position);
            Floors.Add(position, terrain);
            return terrain;
        }
        
        private static MySimpleTerrain CreateFloor(TGCVector3 origin)
        {
            var bitmap = getHeightmap(origin);

            var imgSize = bitmap.Height;
            var floor = new MySimpleTerrain();

            var epsilon = 0.27f;
            var scaleXz = Chunk.DefaultSize.X / imgSize + epsilon * Chunk.DefaultSize.X/1000 * imgSize/64;
            var xCenter = origin.X + Chunk.DefaultSize.X / 2 + imgSize * 2.0f / scaleXz;
            var zCenter = origin.Z + Chunk.DefaultSize.Z / 2 + imgSize * 2.0f / scaleXz;            
            
            origin = new TGCVector3(origin.X, origin.Y, origin.Z);
            
            floor.loadHeightmap(getHeightmap(origin), scaleXz, 5.5f, origin);
            floor.loadTexture(loadTexture(FloorPath));

            return floor;
        }

        private static string getPath(TGCVector3 origin)
        {
            var x = Math.Abs(origin.X) % (Chunk.DefaultSize.X * 2) < Chunk.DefaultSize.X ? "1" : "2";
            
            var z = Math.Abs(origin.Z) % (Chunk.DefaultSize.Z * 2) < Chunk.DefaultSize.Z ? "1" : "2";

            return FloorHeightmap + x + z + ".jpg";
        }
        
        private static Bitmap getHeightmap(TGCVector3 origin)
        {
            var x = Math.Abs(origin.X) % (Chunk.DefaultSize.X * 2) < Chunk.DefaultSize.X ? "1" : "2";
            
            var z = Math.Abs(origin.Z) % (Chunk.DefaultSize.Z * 2) < Chunk.DefaultSize.Z ? "1" : "2";

            return loadHeightmap(FloorHeightmap + x + z + ".jpg", x+z);
        }

        private static Bitmap loadHeightmap(string path, string number)
        {
            var res = heightmaps.ContainsKey(number) ? heightmaps[number] : (Bitmap) Image.FromFile(path);            

            return res;
        }

        public static void Dispose(MySimpleTerrain floor)
        {
            //TODO something?
        }
    }
}