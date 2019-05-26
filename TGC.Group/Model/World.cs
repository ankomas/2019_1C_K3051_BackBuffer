using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using BulletSharp;
using Microsoft.DirectX.Direct3D;
using TGC.Core.BoundingVolumes;
using TGC.Core.Camara;
using TGC.Core.Collision;
using TGC.Core.Direct3D;
using TGC.Core.Mathematica;
using TGC.Core.Terrain;
using TGC.Group.Model.Chunks;
using TGC.Group.Model.Elements;
using TGC.Group.Model.Elements.ElementFactories;
using TGC.Group.Model.Player;
using TGC.Group.Model.Utils;
using Chunk = TGC.Group.Model.Chunks.Chunk;
using Element = TGC.Group.Model.Elements.Element;

namespace TGC.Group.Model
{
    internal class World
    {
        public static readonly int RenderRadius = (int)Math.Floor(D3DDevice.Instance.ZFarPlaneDistance/Chunk.DefaultSize.X)+1;
        public static readonly int UpdateRadius = RenderRadius;
        private const int InteractionRadius = 490000; // Math.pow(700, 2)
        
        private readonly Dictionary<TGCVector3, Chunk> chunks;
        private List<Chunk> chunksToUpdate = new List<Chunk>();
        private List<Element> elementsToUpdate = new List<Element>();
        public Element SelectableElement { get; private set; }

        private readonly List<Element> entities;
        private Entity shark;
        public TgcSimpleTerrain Floor { get; set; }

        private Effect effect;

        public int elementsUpdated;
        public int elementsRendered;
        
        public World(TGCVector3 initialPoint)
        {
            chunks = new Dictionary<TGCVector3, Chunk>();
            
            entities = new List<Element>();

            var initialChunk = new InitialChunk(initialPoint);
            
            chunks.Add(new TGCVector3(initialPoint), initialChunk);
            entities.AddRange(initialChunk.Init());
            shark = SharkFactory.Create(new TGCVector3(30, 0, -2000));

            /*
            string path = "../../../Shaders/Fede.fx", compilationErrors;
            try
            {
                effect = Effect.FromFile(D3DDevice.Instance.Device, path, null, null, ShaderFlags.None, null, out compilationErrors);
            }
            catch (Exception e)
            {
                throw new Exception("Error al cargar shader: " + path + ". Errores: Ni lo cargó xd");
            }

            if (effect == null)
            {
                throw new Exception("Error al cargar shader: " + path + ". Errores: " + compilationErrors);
            }

            foreach (var e in this.entities)
            {
                e.Mesh.Effect = effect;
                e.Mesh.Technique = "FedeTechnique";
            }
            */
        }

        //todo: need refactor
        private void AddHeightMap()
        {
            Floor = new TgcSimpleTerrain();
            Floor.loadHeightmap(Game.Default.MediaDirectory + "Heightmap3.jpg", 1000, 65, 
                new TGCVector3(0,-100,0));
            Floor.loadTexture(Game.Default.MediaDirectory + Game.Default.TexturaTierra);
            AquaticPhysics.Instance.Add(CreateSurfaceFromHeighMap(Floor.getData()));

        }

        //todo: need refactor
        public RigidBody CreateSurfaceFromHeighMap(CustomVertex.PositionTextured[] triangleDataVB)
        {
            
            var triangleMesh = new TriangleMesh();
            int i = 0;
            TGCVector3 vector0;
            TGCVector3 vector1;
            TGCVector3 vector2;

            while (i < triangleDataVB.Length)
            {
                vector0 = new TGCVector3(triangleDataVB[i].X, triangleDataVB[i].Y, triangleDataVB[i].Z);
                vector1 = new TGCVector3(triangleDataVB[i + 1].X, triangleDataVB[i + 1].Y, triangleDataVB[i + 1].Z);
                vector2 = new TGCVector3(triangleDataVB[i + 2].X, triangleDataVB[i + 2].Y, triangleDataVB[i + 2].Z);

                i = i + 3;

                triangleMesh.AddTriangle(vector0.ToBulletVector3(), vector1.ToBulletVector3(),
                    vector2.ToBulletVector3());
            }

            CollisionShape meshCollisionShape = new BvhTriangleMeshShape(triangleMesh, true);
            var meshMotionState = new DefaultMotionState();
            var meshRigidBodyInfo = new RigidBodyConstructionInfo(0, meshMotionState, meshCollisionShape);
            RigidBody meshRigidBody = new RigidBody(meshRigidBodyInfo);
            return meshRigidBody;
            
            
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
            var intOrigin = new TGCVector3(
                (int) Math.Floor(origin.X / Chunk.DefaultSize.X),
                (int) Math.Floor(origin.Y/Chunk.DefaultSize.Y), 
                (int) Math.Floor(origin.Z/Chunk.DefaultSize.Z));

            var yFloor = Chunk.underSeaLimit;
            var yTop = Chunk.surface;

            var xRange = Enumerable.Range(-radius, radius * 2 + 1);
            var yRange = Enumerable.Range(yFloor, Math.Abs(yFloor) + yTop + 1);
            var zRange = Enumerable.Range(-radius, radius * 2 + 1);

            var vectors = new HashSet<TGCVector3>();

            var trueOrigin = new TGCVector3(
                Chunk.DefaultSize.X * (int) Math.Round(origin.X / Chunk.DefaultSize.X),
                Chunk.DefaultSize.Y * (int) Math.Round(origin.Y / Chunk.DefaultSize.Y),
                Chunk.DefaultSize.Z * (int) Math.Round(origin.Z / Chunk.DefaultSize.Z));

            var zaCube = new Cube(trueOrigin, (int)Math.Floor(radius*Chunk.DefaultSize.X));
            
            var ySegments = Segment.GenerateSegments(new TGCVector3(zaCube.PMin.X, (int)Math.Floor(Chunk.DefaultSize.Y * yFloor), zaCube.PMin.Z),
                new TGCVector3(zaCube.PMax.X, (int)Math.Floor(Chunk.DefaultSize.Y * yTop), zaCube.PMax.Z), 
                Math.Abs(yFloor) + Math.Abs(yTop));
            
            var xzCubes = ySegments.ConvertAll(segment => segment.Cube)
                .SelectMany(cube => Segment.GenerateXzCubes(cube.PMin, cube.PMax, radius * 2    ));

            foreach (var xzCube in xzCubes)
            {
                vectors.Add(xzCube.PMin);
                vectors.Add(new TGCVector3(xzCube.PMin.X, xzCube.PMin.Y, xzCube.PMax.Z));
                vectors.Add(new TGCVector3(xzCube.PMax.X, xzCube.PMin.Y, xzCube.PMin.Z));
                vectors.Add(new TGCVector3(xzCube.PMax.X, xzCube.PMin.Y, xzCube.PMax.Z));
            }
            /*
            foreach (var x in xRange)
            {
                foreach (var y in yRange)
                {
                    foreach (var z in zRange)
                    {
                        vectors.Add(
                                new TGCVector3(
                                    Chunk.DefaultSize.X * (intOrigin.X + x),
                                    Chunk.DefaultSize.Y * (intOrigin.Y + y),
                                    Chunk.DefaultSize.Z * (intOrigin.Z + z))
                            );
                    }
                }
            }
            */

            foreach (var position in vectors)
            {
                toUpdate.Add(chunks.ContainsKey(position) ? chunks[position] : AddChunk(position));
            }            
            /*for (var j = yFloor; j <= yTop; j++)
            {
                for (var k = -radius; k <= radius; k++)
                {
                    var position = new TGCVector3(
                        Chunk.DefaultSize.X * (intOrigin.X + i),
                        Chunk.DefaultSize.Y * (intOrigin.Y + j),
                        Chunk.DefaultSize.Z * (intOrigin.Z + k));
                    
                    toUpdate.Add(chunks.ContainsKey(position) ? chunks[position] : AddChunk(position));
                }
            }*/

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

        public void Update(Camera camera, Character character)
        {
            var toUpdate = ToUpdate(camera.Position);
            renderedOrigins = toUpdate.ConvertAll(chunk => chunk.Origin).FindAll(v3 => v3.Y/(Chunk.DefaultSize.Y) == Chunk.seaFloor);
            
            var elements = new List<Element>();
            var updateCube = new Cube(camera.Position, (int)Math.Floor((UpdateRadius+1)*Chunk.DefaultSize.X));
                
            elements.AddRange(elementsInCube(entities, updateCube));
            elements.AddRange(elementsInCube(toUpdate.SelectMany(chunk => chunk.Elements).ToList(), updateCube));

            elements.ForEach(element => element.Update(camera));
            toUpdate.ForEach(chunk => chunk.Update(camera));
            
            elementsToUpdate = elements;
            
            shark.Update(camera, character);

            elementsUpdated = elements.Count;

            SelectableElement = GetSelectableElement(camera, elements);
        }

        public void Render(TgcCamera camera, TgcFrustum frustum)
        {
            var toRender = ToRender(camera.Position, frustum);
            var elements = new List<Element>();
            
            elements.AddRange(elementsToUpdate.FindAll(entity => entity.asCube().isIn(frustum)));
            
            elements.ForEach(element => element.Render());
            
            elementsRendered = elements.Count;
            
            toRender.ForEach(chunk => chunk.Render());
            
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
            chunks.Values.ToList().ForEach(chunk => chunk.Dispose());
            entities.ForEach(entity => entity.Dispose());
            shark.Dispose();
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
    }
}
