using System;
using System.CodeDom;
using Microsoft.DirectX.Direct3D;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using BulletSharp;
using Microsoft.DirectX.Direct3D;
using TGC.Core.BoundingVolumes;
using TGC.Core.Camara;
using TGC.Core.Collision;
using TGC.Core.Direct3D;
using TGC.Core.Example;
using TGC.Core.Mathematica;
using TGC.Core.Terrain;
using TGC.Group.Model.Chunks;
using TGC.Group.Model.Elements;
using TGC.Group.Model.Elements.ElementFactories;
using TGC.Group.Model.Input;
using TGC.Group.Model.Player;
using TGC.Group.Model.Utils;
using Chunk = TGC.Group.Model.Chunks.Chunk;
using Element = TGC.Group.Model.Elements.Element;

namespace TGC.Group.Model
{
    public class World
    {
        public static readonly int RenderRadius = 
            Math.Max(
                Math.Min(
                    (int)Math.Floor(D3DDevice.Instance.ZFarPlaneDistance/Chunk.DefaultSize.X), 
                5), // maximum radius
                1); // minimum radius
        public static readonly int UpdateRadius = RenderRadius;
        private const int InteractionRadius = 1000000; // Math.pow(1000, 2)

        public readonly Dictionary<TGCVector3, Chunk> chunks;
        private List<Chunk> chunksToUpdate = new List<Chunk>();
        public List<Element> elementsToUpdate = new List<Element>();
        public Element SelectableElement { get; private set; }

        private readonly List<Element> entities;
        private Entity shark;

        public int generating;
        public int elementsUpdated;
        public int elementsRendered;
        

        public World(TGCVector3 initialPoint)
        {
            chunks = new Dictionary<TGCVector3, Chunk>();
            
            entities = new List<Element>();

            var initialChunk = new InitialChunk(initialPoint);
            
            chunks.Add(new TGCVector3(initialPoint), initialChunk);
            entities.AddRange(initialChunk.Init());
            shark = SharkFactory.Create(new TGCVector3(30, -1000, -2000));
            
        }

        private Chunk AddChunk(TGCVector3 origin)
        {
            var chunk = Chunk.ByYAxis(origin);
            
            chunks.Add(origin, chunk);

            entities.AddRange(chunk.Init());

            return chunk;
        }

        private List<Chunk> GetChunksByRadius(TGCVector3 origin, int radius)
        {
            var toUpdate = new ConcurrentBag<Chunk>();

            var yFloor = Chunk.underSeaLimit;
            var yTop = Chunk.surface;

            var vectors = new HashSet<TGCVector3>();

            var trueOrigin = new TGCVector3(
                Chunk.DefaultSize.X * (int) (Math.Round(origin.X / Chunk.DefaultSize.X)+radius),
                Chunk.DefaultSize.Y * (int) (Math.Round(origin.Y / Chunk.DefaultSize.Y)),
                Chunk.DefaultSize.Z * (int) (Math.Round(origin.Z / Chunk.DefaultSize.Z)+radius));

            var zaCube = new Cube(trueOrigin, (int)Math.Floor(radius*Chunk.DefaultSize.X), (int)Math.Floor(radius*Chunk.DefaultSize.Y));
            
            var ySegments = Segment.GenerateSegments(
                new TGCVector3(zaCube.PMin.X, (int)Math.Floor(Chunk.DefaultSize.Y * yFloor), zaCube.PMin.Z),
                new TGCVector3(zaCube.PMax.X, (int)Math.Floor(Chunk.DefaultSize.Y * yTop), zaCube.PMax.Z), 
                Math.Abs(yFloor) + Math.Abs(yTop));
            
            var xzCubes = ySegments.ConvertAll(segment => segment.Cube)
                .SelectMany(cube => Segment.GenerateXzCubes(cube.PMin, cube.PMax, -radius*2, radius*2));

            foreach (var xzCube in xzCubes)
            {
                vectors.Add(xzCube.PMin);
            }

            this.generating = vectors.Count;

            TGCVector3 trunkedPosition;
            //var limit = 10 * Chunk.DefaultSize.X;
            foreach (var position in vectors)
            {
                trunkedPosition = position;
                //trunkedPosition = new TGCVector3(Math.Abs(position.X)%limit, position.Y, Math.Abs(position.X)%limit);
                toUpdate.Add(chunks.ContainsKey(trunkedPosition) ? chunks[trunkedPosition] : AddChunk(trunkedPosition));
            }
            
            return toUpdate.ToList();
        }

        private List<Chunk> ToUpdate(TGCVector3 cameraPosition)
        {
            chunksToUpdate = GetChunksByRadius(cameraPosition, UpdateRadius);
            return chunksToUpdate;
        }

        private List<Chunk> ToRender(TGCVector3 cameraPosition, TgcFrustum frustum)
        {
            return chunksToUpdate.FindAll(chunk => chunk.asCube().isIn(frustum));
        }

        private List<Element> elementsInCube(List<Element> elements, Cube cube)
        {
            return elements.Where(element => cube.contains(element.getPosition())).ToList();
        }

        private float asd;

        public void Update(Camera camera, Character character)
        {
            AquaticPhysics.Instance.DynamicsWorld.StepSimulation(GameModel.GlobalElapsedTime);

            var toUpdate = ToUpdate(camera.Position);
            renderedOrigins = toUpdate.ConvertAll(chunk => chunk.Origin).FindAll(v3 => v3.Y/(Chunk.DefaultSize.Y) == Chunk.seaFloor);
            
            var elements = new List<Element>();
            var updateCube = new Cube(camera.Position, (int)Math.Floor((UpdateRadius+1)*Chunk.DefaultSize.X));
            
            var allElements = new List<Element>();
                
            elements.AddRange(elementsInCube(this.entities, updateCube));
            //elements.AddRange(elementsInCube(toUpdate.SelectMany(chunk => chunk.Elements).ToList(), updateCube));
            
            elements.ForEach(element => element.Update(camera));
            toUpdate.ForEach(chunk => chunk.Update(camera));
            
            elementsToUpdate = elements;
            
            shark.Update(camera, character);

            this.elementsUpdated = elements.Count;
            
            allElements.AddRange(elements);
            allElements.AddRange(elementsInCube(toUpdate.SelectMany(chunk => chunk.Elements).ToList(), updateCube));

            SelectableElement = GetSelectableElement(camera, allElements);
        }

        public void Render(TgcCamera camera, TgcFrustum frustum)
        {
            var toRender = ToRender(camera.Position, frustum);
            var elements = new List<Element>();

            elements.AddRange(elementsToUpdate.FindAll(entity => entity.asCube().isIn(frustum)));

            elements.ForEach(element => {
                if(element.Mesh != null)
                {
                    if (!element.HasDefaultShader())
                    {
                        element.Mesh.Effect = ShaderRepository.WorldWaterFog;
                        element.Mesh.Technique = "WorldWaterFog";
                    }
                }
                element.Render();
            });
            
            elementsRendered = elements.Count;
            
            toRender.ForEach(chunk => chunk.Render());

            shark.Mesh.Effect = ShaderRepository.WorldWaterFog;
            shark.Mesh.Technique = "WorldWaterFog";
            shark.Render();

            //waterSurface.Render(camera.Position);
            //Floor.Render();
        }

        public List<TGCVector3> renderedOrigins { get; set; }

        public void RenderBoundingBox(TgcCamera camera)
        {
            ToRender(camera.Position, null).ForEach(chunk => chunk.RenderBoundingBox());
        }

        public void Dispose()
        {
            SelectableElement?.Dispose(AquaticPhysics.Instance);
            shark?.Dispose();

            chunks.Values.ToList().ForEach(chunk => chunk.Dispose());

            entities.ForEach(entity => entity.Dispose(AquaticPhysics.Instance));

            chunks.Clear();
            chunksToUpdate.Clear();
            entities.Clear();
            elementsToUpdate.Clear();
        }

        private static Element GetSelectableElement(TgcCamera camera, List<Element> elements)
        {            
            var direction = camera.LookAt - camera.Position;
            direction.Normalize();

            var intersectedElements =
                elements
                    .FindAll(element => element.isIntersectedBy(new TgcRay(camera.Position, direction)));
            
            intersectedElements.Sort((element1, element2) => 
                (int) TGCVector3.LengthSq(camera.Position, element1.getPosition()) -
                (int) TGCVector3.LengthSq(camera.Position, element2.getPosition()));

            return intersectedElements.Find(element => 
                Math.Abs(TGCVector3.LengthSq(camera.Position, element.getPosition())) < InteractionRadius);
        }
        public void Remove(Element selectableElement)
        {
            entities.Remove(selectableElement);
            
            foreach (var chunk in chunks.Values)
            {
                chunk.Remove(selectableElement);
            }
        }

        public void preLoad(TGCVector3 origin, int preloadRadius)
        {
            this.GetChunksByRadius(origin, preloadRadius);
        }
    }
}
    