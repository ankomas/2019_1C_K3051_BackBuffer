using System.Collections.Generic;
using System.Linq;
using TGC.Core.Mathematica;
using TGC.Group.Model.Elements;

namespace TGC.Group.Model.Chunks
{
    public class NoneChunk : Chunk

    {
        public NoneChunk() : base(TGCVector3.Empty, AquaticPhysics.Instance)
        {
        }
        
        public override IEnumerable<Element> Init()
        {
            return Enumerable.Empty<Element>();
        }

        public override void Update(Camera camera)
        {
        }

        public override void Render()
        {
        }

        public override void RenderBoundingBox()
        {
        }

        public override void Dispose()
        {
        }
    }
}