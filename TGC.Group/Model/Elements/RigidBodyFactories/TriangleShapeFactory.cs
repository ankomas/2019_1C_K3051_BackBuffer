using BulletSharp;
using Microsoft.DirectX.Direct3D;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;

namespace TGC.Group.Model.Elements.RigidBodyFactories
{
    public class TriangleShapeFactory
    {
        public static RigidBody CreateFromHeighMap(CustomVertex.PositionTextured[] triangleDataVB)
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

                triangleMesh.AddTriangle(vector0.ToBulletVector3(), vector1.ToBulletVector3(), vector2.ToBulletVector3());
            }
            
            return new RigidBody(
                new RigidBodyConstructionInfo(
                    0, 
                    new DefaultMotionState(),
                    new BvhTriangleMeshShape(triangleMesh, 
                    true)
                )
            );

        }
    }
}