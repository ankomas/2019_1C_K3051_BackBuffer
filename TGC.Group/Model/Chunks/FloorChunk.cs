using System;
using System.Collections.Generic;
using System.Drawing;
using BulletSharp;
using TGC.Core.Direct3D;
using TGC.Core.Geometry;
using System.Linq;
using System.Runtime.CompilerServices;
using TGC.Core.Mathematica;
using TGC.Core.Terrain;
using TGC.Core.Textures;
using TGC.Group.Model.Elements;
using TGC.Group.Model.Elements.ElementFactories;
using TGC.Group.Model.Elements.RigidBodyFactories;
using TGC.Group.Model.Resources;
using TGC.Group.Model.UI;
using TGC.Group.Model.Utils;
using Element = TGC.Group.Model.Elements.Element;

namespace TGC.Group.Model.Chunks
{
    public class FloorChunk : Chunk
    {
        public RigidBody FloorRigidBody { get; set; }

        public MySimpleTerrain Floor { get; set; }
        
        private List<Segment> segments;
        private int divisions;
        public FloorChunk(TGCVector3 origin) : base(origin, AquaticPhysics.Instance)
        {
            var max = origin + DefaultSize;

            this.segments = Segment.GenerateSegments(origin, max, 10);

            this.divisions = (int)(DefaultSize.X / 100);

            this.Floor = FloorRepository.getFloor(origin);
            FloorRigidBody = TriangleShapeFactory.CreateFromHeighMap(Floor.getData());
            
            var corals = CreateCorals(segments[0], divisions, Floor);
            AddElementsToPhysicsWorld(corals);
            this.Elements.AddRange(corals);

            segments.Remove(segments[0]);
        }
        
        private static List<Element> CreateFishes(List<Segment> segments, int divisions)
        {
            return segments.SelectMany(segment => segment.GenerateElements(divisions / 2, SpawnRate.Of(1, 1200), FishFactory.Instance)).ToList();
        }

        private static List<Element> CreateCorals(Segment segment, int divisions, MySimpleTerrain floor)
        {
            var corals = segment.GenerateElements(divisions / 2, SpawnRate.Of(1, 100), CoralFactory.Instance)
                .ToList();
            corals.ForEach(coral => coral.yPosition(floor));
            return corals;
        }
        
        public override IEnumerable<Element> Init()
        {
            var fishes = CreateFishes(this.segments, this.divisions);
            AquaticPhysics.Instance.Add(FloorRigidBody);
            AddElementsToPhysicsWorld(fishes);
            return fishes;
        }

        public override void Render()
        {
            //Floor.updateValues();
            Floor.Effect = ShaderRepository.WorldWaterFog;
            Floor.Technique = "WorldWaterFog";
            Floor.Render();
            base.Render();
        }


        public override void Dispose()
        {
            FloorRepository.Dispose(Floor);
            base.Dispose();
        }
    }
}