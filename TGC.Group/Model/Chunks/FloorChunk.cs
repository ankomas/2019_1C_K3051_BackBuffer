using System.Collections.Generic;
using System.Linq;
using TGC.Core.Mathematica;
using TGC.Group.Model.Elements;
using TGC.Group.Model.Elements.ElementFactories;
using TGC.Group.Model.Utils;
using Element = TGC.Group.Model.Elements.Element;

namespace TGC.Group.Model.Chunks
{
    public class FloorChunk : Chunk
    {
        private static readonly string FloorTexture = Game.Default.MediaDirectory + Game.Default.TexturaTierra;
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
            return segments.SelectMany(segment => segment.GenerateElements(divisions / 2, SpawnRate.Of(1, 750), FishFactory.Instance)).ToList();
        }

        private static List<Element> CreateCorals(Segment segment, int divisions)
        {
            return segment.GenerateElements(divisions / 2, SpawnRate.Of(1, 25), CoralFactory.Instance).ToList();
        }

        private void CreateFloor(TGCVector3 origin)
        {

        }

        public override IEnumerable<Element> Init()
        {
            var fishes = CreateFishes(this.segments, this.divisions);
            AddElementsToPhysicsWorld(fishes);
            return fishes;
        }

        public override void Render()
        {
            base.Render();
        }

        public override void Dispose()
        {
            base.Dispose();
        }
    }
}