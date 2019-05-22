using System.Collections.Generic;
using BulletSharp;
using TGC.Core.Direct3D;
using TGC.Core.Geometry;
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
        
        
        public FloorChunk(TGCVector3 origin) : base(origin, AquaticPhysics.Instance)
        {
            var max = origin + DefaultSize;

            var segments = Segment.GenerateSegments(origin, max, 10);

            var divisions = (int)(DefaultSize.X / 100);

            CreateElements(segments, divisions);
            CreateFloor(origin);
            AddElementsToPhysicsWorld();
        }

        private void CreateElements(List<Segment> segments, int divisions)
        {
            Elements.AddRange(CreateCorals(segments, divisions));
            segments.ForEach(segment => Elements.AddRange(CreateFishes(segment, divisions)));
        }

        private void CreateFloor(TGCVector3 origin)
        {
            
            Floor = new TgcPlane(origin, DefaultSize, TgcPlane.Orientations.XZplane, FloorTexture);
            FloorRigidBody = new BoxFactory().CreatePlane(this.Floor);
            
        }

        private static IEnumerable<Element> CreateFishes(Segment segment, int divisions)
        {
            return segment.GenerateElements(divisions / 2, SpawnRate.Of(1, 750), FishFactory.Instance);
        }

        private static IEnumerable<Element> CreateCorals(List<Segment> segments, int divisions)
        {
            var corals = segments[0].GenerateElements(divisions / 2, SpawnRate.Of(1, 25), CoralFactory.Instance);
            segments.Remove(segments[0]);
            return corals;
        }

        private new void AddElementsToPhysicsWorld()
        {
            AquaticPhysics.Instance.Add(FloorRigidBody);
            base.AddElementsToPhysicsWorld();
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