using System.Collections.Generic;
using BulletSharp;
using TGC.Core.Direct3D;
using TGC.Core.Geometry;
using System.Linq;
using TGC.Core.Mathematica;
using TGC.Core.Textures;
using TGC.Group.Model.Elements;
using TGC.Group.Model.Elements.ElementFactories;
using TGC.Group.Model.Elements.RigidBodyFactories;
using TGC.Group.Model.Utils;
using Element = TGC.Group.Model.Elements.Element;

namespace TGC.Group.Model.Chunks
{
    public class FloorChunk : Chunk
    {
        private static readonly TgcTexture FloorTexture =TgcTexture.createTexture(D3DDevice.Instance.Device,Game.Default.MediaDirectory + Game.Default.TexturaTierra);
        public RigidBody FloorRigidBody { get; set; }

        public TgcPlane Floor { get; set; }
        
        private List<Segment> segments;
        private int divisions;
        public FloorChunk(TGCVector3 origin) : base(origin, AquaticPhysics.Instance)
        {
            var max = origin + DefaultSize;

            this.segments = Segment.GenerateSegments(origin, max, 10);

            this.divisions = (int)(DefaultSize.X / 100);

            var corals = CreateCorals(segments[0], divisions);
            AddElementsToPhysicsWorld(corals);
            this.Elements.AddRange(corals);
            
            segments.Remove(segments[0]);
            CreateFloor(origin);
        }
        
        private static List<Element> CreateFishes(List<Segment> segments, int divisions)
        {
            return segments.SelectMany(segment => segment.GenerateElements(divisions / 2, SpawnRate.Of(1, 1200), FishFactory.Instance)).ToList();
        }

        private static List<Element> CreateCorals(Segment segment, int divisions)
        {
            return segment.GenerateElements(divisions / 2, SpawnRate.Of(1, 100), CoralFactory.Instance).ToList();
        }

        private void CreateFloor(TGCVector3 origin)
        {
            
            Floor = new TgcPlane(origin, DefaultSize, TgcPlane.Orientations.XZplane, FloorTexture);
            FloorRigidBody = new BoxFactory().CreatePlane(this.Floor);
            
        }

        public override IEnumerable<Element> Init()
        {
            var fishes = CreateFishes(this.segments, this.divisions);
            AddElementsToPhysicsWorld(fishes);
            return fishes;
        }

        public override void Render()
        {
            Floor.updateValues();
            Floor.Render();
            base.Render();
        }


        public override void Dispose()
        {
            Floor.Dispose();
            base.Dispose();
        }
    }
}