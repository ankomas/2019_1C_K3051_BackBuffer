using BulletSharp;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;
using TGC.Group.Model.Movements;
using TGC.Group.Model.Player;

namespace TGC.Group.Model
{
    public abstract class Entity : Collisionable
    {
        public TgcMesh Mesh { get; }
        public RigidBody RigidBody { get; }
        
        public Movement Movement { get; set; }

        public Entity(TgcMesh mesh, RigidBody rigid, Movement movement)
        {
            Mesh = mesh;
            RigidBody = rigid;
            Movement = movement;
        }


        public virtual void Render()
        {
            Mesh.Render();

        }
        public virtual void Update(Camera camera, Character character)
        {
            var movementToApply = Movement.Move(Mesh.Position, camera.Position);
            RigidBody.CenterOfMassTransform *= movementToApply.Translation.ToBsMatrix;
            Mesh.Position = new TGCVector3(RigidBody.CenterOfMassPosition);
            Mesh.Rotation += movementToApply.AnglesToRotate;
            Mesh.Transform = CalculateTransform();
        }

        private TGCMatrix CalculateTransform()
        {
            return TGCMatrix.Scaling(Mesh.Scale) * new TGCMatrix(RigidBody.CenterOfMassTransform);
        }

        public abstract void Dispose();
        public override abstract IRenderObject getCollisionVolume();
        

    }
}