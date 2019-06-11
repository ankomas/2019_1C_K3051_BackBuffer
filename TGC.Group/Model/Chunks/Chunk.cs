using BulletSharp.Math;
using Microsoft.DirectX.Direct3D;
using System;
using System.Collections.Generic;
using TGC.Core.Camara;
using TGC.Core.Direct3D;
using TGC.Core.Mathematica;
using TGC.Group.Model.Elements;
using TGC.Group.Model.Utils;

namespace TGC.Group.Model.Chunks
{
    public class Chunk
    {
        public List<Element> Elements { get; }

        public TGCVector3 Origin { get; }
        protected AquaticPhysics Physics { get; }

        public static readonly Chunk None = new NoneChunk();

        public static TGCVector3 DefaultSize { get; } = new TGCVector3(8000, 1000, 8000);
        
        private Cube cube;
        
        public static int surface = 0;
        public static int seaFloor = surface - 2;
        public static int underSeaLimit = seaFloor - 1;

        protected Chunk(TGCVector3 origin, AquaticPhysics physicsWorld)
        {
            this.Origin = origin;
            this.Physics = physicsWorld;
            this.Elements = new List<Element>();
            this.cube = new Cube(this.Origin, this.Origin + DefaultSize);
        }
        
        public static Chunk ByYAxis(TGCVector3 origin)
        {
            if (origin.Y < DefaultSize.Y * underSeaLimit || origin.Y > surface)
                return None;
            
            if (origin.Y < DefaultSize.Y * seaFloor)
                return new FloorChunk(origin);
            
            return new AquaticChunk(origin);
        }

        protected void AddElementsToPhysicsWorld(List<Element> elements)
        {
            elements.ForEach(element => Physics.Add(element.PhysicsBody));
        }
        public virtual IEnumerable<Element> Init()
        {
            return new List<Element>();
        }

        public virtual void Update(Camera camera)
        {
            this.Elements.ForEach(element => element.Update(camera));
        }

        public virtual void Render()
        {
            this.Elements.ForEach(element => element.Render());
        }

        public virtual void RenderBoundingBox()
        {
            this.Elements.ForEach(element => element.getCollisionVolume().Render());
        }

        public virtual void Dispose()
        {
            this.Elements.ForEach(element => {
                Physics.Remove(element.PhysicsBody);
                element.Dispose();
            });
        }

        public void Remove(Element selectableElement)
        {
            if (!this.Elements.Contains(selectableElement)) return;

            Physics.Remove(selectableElement.PhysicsBody);
            this.Elements.Remove(selectableElement);
            selectableElement.Dispose();
        }

        public Cube asCube()
        {
            return this.cube;
        }
    }
}