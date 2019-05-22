using System;
using System.Collections.Generic;
using System.Linq;
using TGC.Core.Mathematica;
using TGC.Group.Model.Elements;
using TGC.Group.Model.Elements.ElementFactories;
using TGC.Group.Model.Elements.RigidBodyFactories;
using TGC.Group.Model.Resources.Meshes;
using TGC.Group.Model.Utils;

namespace TGC.Group.Model.Chunks
{
    public class AquaticChunk : Chunk
    {
        private List<Segment> segments;
        private int divisions;

        public AquaticChunk(TGCVector3 origin) : base(origin, AquaticPhysics.Instance)
        {
            var max = origin + DefaultSize;

            this.segments = Segment.GenerateSegments(origin, max, (int)DefaultSize.Y / 1000);

            this.divisions = (int)(DefaultSize.X / 100);
        }

        public override IEnumerable<Element> Init()
        {
            var elements = GenerateElements(segments, divisions);
            AddElementsToPhysicsWorld(elements);

            return elements;
        }

        private List<Element> GenerateElements(List<Segment> segments, int divisions)
        {
            return segments.SelectMany(segment => GenerateElementsBySegment(segment, divisions)).ToList();
        }

        private static IEnumerable<Element> GenerateElementsBySegment(Segment segment, int divisions)
        {
            return segment.GenerateElements(divisions / 2, SpawnRate.Of(1, 1000), FishFactory.Instance);
        }

    }
}