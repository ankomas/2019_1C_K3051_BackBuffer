using BulletSharp;
using Microsoft.DirectX.DirectInput;
using System;
using System.Drawing;
using System.Windows.Forms;
using TGC.Core.Camara;
using TGC.Core.Direct3D;
using TGC.Core.Input;
using TGC.Core.Mathematica;
using TGC.Core.Text;
using TGC.Group.Model.Input;
using TGC.Group.TGCUtils;

namespace TGC.Group.Model
{
    public class Camera : TgcCamera
    {
        public RigidBody RigidBody { get; }

        private readonly Point mouseCenter;
        public TGCMatrix cameraRotation;
        private TGCVector3 initialDirectionView;
        public float leftrightRot;
        public float updownRot;

        delegate void CameraUpdateLogic(float elapsedTime);
        CameraUpdateLogic currentUpdateLogic;

        private TgcD3dInput Input { get; }
        public float MovementSpeed { get; set; }
        public float RotationSpeed { get; set; }

        private bool ConsideringInput = true;
        private bool manual = false;
        
        public Camera(TGCVector3 position, TgcD3dInput input, RigidBody rigidBody)
        {
            Input = input;
            Position = position;
            RigidBody = rigidBody;
            mouseCenter = GetMouseCenter();
            RotationSpeed = 0.1f;
            MovementSpeed = 2000f * 30f;
            initialDirectionView = new TGCVector3(0, 0, -1);
            leftrightRot = 0;
            updownRot = 0;
            Cursor.Hide();
            currentUpdateLogic = MoveNormally;
        }

        private static Point GetMouseCenter()
        {

            return new Point(D3DDevice.Instance.Device.Viewport.Width / 2, D3DDevice.Instance.Device.Viewport.Height / 2);
        }

        public void Update(float elapsedTime)
        {
            currentUpdateLogic(elapsedTime);
        }

        private TGCVector3 CalculateTranslation(float elapsedTime, TGCMatrix cameraRotation)
        {
            var normalizedTranslation =  TGCVector3.TransformNormal(CalculateInputTranslation(), cameraRotation);
            RigidBody.LinearVelocity = normalizedTranslation.ToBulletVector3() * elapsedTime;
            return new TGCVector3(RigidBody.CenterOfMassPosition);
        }

        public TGCMatrix CalculateCameraRotation()
        {
            if(ConsideringInput)
            {
                leftrightRot += Input.XposRelative * RotationSpeed;
                updownRot = FastMath.Clamp(updownRot - Input.YposRelative * RotationSpeed, -FastMath.PI_HALF, FastMath.PI_HALF);
            }
                
            return TGCMatrix.RotationX(updownRot) * TGCMatrix.RotationY(leftrightRot);
        }       
        

        private TGCVector3 CalculateInputTranslation()
        {
            var moveVector = TGCVector3.Empty;

            if(ConsideringInput)
                moveVector = GetInputTranslation(moveVector);

            return moveVector;
        }
        void MoveNormally(float elapsedTime)
        {
            
            if (manual) return;

            cameraRotation = CalculateCameraRotation();
            
            var translation = CalculateTranslation(elapsedTime, cameraRotation);

            if (translation.Y < 100)
            {
                Position = translation;   
            }
            else
            {
                Position = new TGCVector3(translation.X, Position.Y, translation.Z);
            }
            
            LookAt = Position + TGCVector3.TransformNormal(initialDirectionView, cameraRotation);

            UpVector = TGCVector3.TransformNormal(DEFAULT_UP_VECTOR, cameraRotation);

            if(ConsideringInput) Cursor.Position = mouseCenter;

            base.SetCamera(Position, LookAt, UpVector);
        }
        public void Freeze()
        {
            currentUpdateLogic = (elapsedTime) => {};
        }
        public void Unfreeze()
        {
            currentUpdateLogic = MoveNormally;
        }
        private TGCVector3 GetInputTranslation(TGCVector3 moveVector)
        {
            if (GameInput._Up.IsDown(Input))
            {
                moveVector += new TGCVector3(0, 0, -1) * MovementSpeed;
            }

            if (GameInput._Down.IsDown(Input))
            {
                moveVector += new TGCVector3(0, 0, 1) * MovementSpeed;
            }

            if (GameInput._Right.IsDown(Input))
            {
                moveVector += new TGCVector3(-1, 0, 0) * MovementSpeed;
            }

            if (GameInput._Left.IsDown(Input))
            {
                moveVector += new TGCVector3(1, 0, 0) * MovementSpeed;
            }

            if (GameInput._Float.IsDown(Input))
            {
                moveVector += new TGCVector3(0, 1, 0) * MovementSpeed;
            }

            return moveVector;
        }
        public void IgnoreInput()
        {
            ConsideringInput = false;
        }
        public void ConsiderInput()
        {
            ConsideringInput = true;
        }
        public void UseManually()
        {
            manual = true;
        }
        public void StopUsingManually()
        {
            manual = false;
        }
    }
}
