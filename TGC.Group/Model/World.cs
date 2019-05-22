
﻿using System;
using BulletSharp.Math;
using Microsoft.DirectX.Direct3D;
using System;
 using System.Collections;
 using System.Collections.Generic;
using System.Linq;
 using System.Runtime.CompilerServices;
 using BulletSharp;
using BulletSharp.Math;
using Microsoft.DirectX.Direct3D;
 using TGC.Core.BoundingVolumes;
 using TGC.Core.Camara;
using TGC.Core.Collision;
 using TGC.Core.Mathematica;
using TGC.Core.Terrain;
 using System.Collections.Generic;
 using System.Linq;
 using BulletSharp;
 using Microsoft.DirectX.Direct3D;
 using TGC.Core.Camara;
 using TGC.Core.Collision;
 using TGC.Core.Direct3D;
 using TGC.Core.Mathematica;
 using TGC.Core.Terrain;
 using TGC.Group.Model.Chunks;
 using TGC.Group.Model.Elements;
 using TGC.Group.Model.Elements.RigidBodyFactories;
 using TGC.Group.Model.Player;
 using TGC.Group.Model.Resources.Meshes;
 using TGC.Group.Model.Utils;
 using static TGC.Core.Direct3D.D3DDevice;
 using Chunk = TGC.Group.Model.Chunks.Chunk;
 using Element = TGC.Group.Model.Elements.Element;

 namespace TGC.Group.Model
{
    internal class World
    {
        public static readonly int RenderRadius = 5;//(int)Math.Floor(D3DDevice.Instance.ZFarPlaneDistance/Chunk.DefaultSize.X);
        public static readonly int UpdateRadius = RenderRadius;
        private const int InteractionRadius = 490000; // Math.pow(700, 2)
        
        private readonly Dictionary<TGCVector3, Chunk> chunks;
        private List<Chunk> chunksToUpdate = new List<Chunk>();
        private List<Element> elementsToUpdate = new List<Element>();
        public Element SelectableElement { get; private set; }

        private readonly List<Element> entities;
        private Entity shark;
        public TgcSimpleTerrain Floor { get; set; }

        private readonly WaterSurface waterSurface;

        private Effect effect;

        public int elementsUpdated;
        public int elementsRendered;
        
        public World(TGCVector3 initialPoint)
        {
            chunks = new Dictionary<TGCVector3, Chunk>();
            
            this.entities = new List<Element>();

            waterSurface = new WaterSurface(initialPoint);

            var initialChunk = new InitialChunk(initialPoint);
            
            this.chunks.Add(new TGCVector3(initialPoint), initialChunk);
            this.entities.AddRange(initialChunk.Init());
            AddShark();
            AddHeightMap();
            
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

        protected void AddShark()
        {
            var mesh = SharkMesh.All()[0];
            mesh.Position = new TGCVector3(30, 0, -2000);
            mesh.UpdateMeshTransform();
            
            var rigidBody = new CapsuleFactory().CreateShark(mesh); ;
            AquaticPhysics.Instance.Add(rigidBody);
            
            this.shark = new Shark(mesh, rigidBody);
        }

        private Chunk AddChunk(TGCVector3 origin)
        {
            var chunk = Chunk.ByYAxis(origin);
            
            chunks.Add(origin, chunk);

            this.entities.AddRange(chunk.Init());

            return chunk;
        }

        private List<Chunk> GetChunksByRadius(TGCVector3 origin, int radius)
        {
            var toUpdate = new List<Chunk>();
            var intOrigin = new TGCVector3(
                (int)(origin.X/Chunk.DefaultSize.X), 
                (int)(origin.Y/Chunk.DefaultSize.Y), 
                (int)(origin.Z/Chunk.DefaultSize.Z));

            for (var i = -radius; i <= radius; i++)
            {
                for (var j = -radius; j <= radius; j++)
                {
                    for (var k = -radius; k <= radius; k++)
                    {
                        var position = new TGCVector3(
                            Chunk.DefaultSize.X * (intOrigin.X + i),
                            Chunk.DefaultSize.Y * (intOrigin.Y + j),
                            Chunk.DefaultSize.Z * (intOrigin.Z + k));
                        
                        toUpdate.Add(chunks.ContainsKey(position) ? chunks[position] : AddChunk(position));
                    }
                }
            }
            
            return toUpdate;
        }
        
        private List<Chunk> ToUpdate(TGCVector3 cameraPosition)
        {
            this.chunksToUpdate = GetChunksByRadius(cameraPosition, UpdateRadius);
            return this.chunksToUpdate;
        }

        private List<Chunk> ToRender(TGCVector3 cameraPosition, TgcFrustum frustum)
        {
            return this.chunksToUpdate.FindAll(chunk => chunk.asCube().isIn(frustum));
        }

        private List<Element> elementsInCube(List<Element> elements, Cube cube)
        {
            return elements.Where(element => cube.contains(element.getPosition())).ToList();
        }

        public void Update(Camera camera, Character character)
        {
            var toUpdate = ToUpdate(camera.Position);
            var elements = new List<Element>();
            var updateCube = new Cube(camera.Position, (int)Math.Floor(UpdateRadius*Chunk.DefaultSize.X));
                
            elements.AddRange(elementsInCube(this.entities, updateCube));
            elements.AddRange(elementsInCube(toUpdate.SelectMany(chunk => chunk.Elements).ToList(), updateCube));

            //toUpdate.ForEach(chunk => chunk.Update(camera));
            //this.entities.ForEach(entity => entity.Update(camera));
            elements.ForEach(element => element.Update(camera));

            toUpdate.ForEach(chunk => chunk.Update(camera));
            
            this.elementsToUpdate = elements;
            
            this.shark.Update(camera, character);

            this.elementsUpdated = elements.Count;

            SelectableElement = GetSelectableElement(camera, elements);
        }

        public void Render(TgcCamera camera, TgcFrustum frustum)
        {
            var toRender = ToRender(camera.Position, frustum);
            
            var elements = new List<Element>();
            
            elements.AddRange(this.elementsToUpdate.FindAll(entity => entity.asCube().isIn(frustum)));
            
            elements.ForEach(element => element.Render());
            
            this.elementsRendered = elements.Count;
            
            toRender.ForEach(chunk => chunk.Render());
            
            this.shark.Render();
            //waterSurface.Render(camera.Position);
            Floor.Render();
        }

        public void RenderBoundingBox(TgcCamera camera)
        {
            ToRender(camera.Position, null).ForEach(chunk => chunk.RenderBoundingBox());
        }

        public void Dispose()
        {
            chunks.Values.ToList().ForEach(chunk => chunk.Dispose());
            this.entities.ForEach(entity => entity.Dispose());
            this.shark.Dispose();
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
            this.entities.Remove(selectableElement);
            
            foreach (var chunk in chunks.Values)
            {
                chunk.Remove(selectableElement);
            }
        }
    }
}
