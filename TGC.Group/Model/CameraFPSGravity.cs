using Microsoft.DirectX.DirectInput;
using System.Drawing;
using System.Windows.Forms;
using TGC.Core.Camara;
using TGC.Core.Direct3D;
using TGC.Core.Input;
using TGC.Core.Mathematica;
using TGC.Group.Model.Input;

namespace TGC.Group.Model
{
    public class CameraFPSGravity : TgcCamera
    {
        private bool manual = false;
        private bool ConsideringInput = true;

        private readonly Point mouseCenter;
        private TGCMatrix cameraRotation;
        private TGCVector3 initialDirectionView;
        private float leftrightRot;
        private float updownRot;

        private TgcD3dInput Input { get; }
        public float MovementSpeed { get; set; }
        public float RotationSpeed { get; set; }

        public CameraFPSGravity(TGCVector3 position, TgcD3dInput input)
        {
            Input = input;
            Position = position;
            mouseCenter = GetMouseCenter();
            RotationSpeed = 0.1f;
            MovementSpeed = 500f;
            initialDirectionView = new TGCVector3(0, 0, -1);
            leftrightRot = 0;
            updownRot = 0;
            Cursor.Hide();
        }

        private static Point GetMouseCenter()
        {

            return new Point(D3DDevice.Instance.Device.Viewport.Width / 2, D3DDevice.Instance.Device.Viewport.Height / 2);
        }

        public override void UpdateCamera(float elapsedTime)
        {
            if (manual) return;

            cameraRotation = CalculateCameraRotation();

            var newPos = Position + TGCVector3.TransformNormal(CalculateInputTranslation() * elapsedTime, CalculateCameraRotationY());
            if (InsideBoundsX(newPos)) Position = new TGCVector3(newPos.X, Position.Y, Position.Z);
            if (InsideBoundsZ(newPos)) Position = new TGCVector3(Position.X, Position.Y, newPos.Z);

            LookAt = Position + TGCVector3.TransformNormal(initialDirectionView, cameraRotation);

            UpVector = TGCVector3.TransformNormal(DEFAULT_UP_VECTOR, cameraRotation);

            if (ConsideringInput) Cursor.Position = mouseCenter;

            base.SetCamera(Position, LookAt, UpVector);
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
        public TGCMatrix CalculateCameraRotationY()
        {
            if (ConsideringInput)
                leftrightRot += Input.XposRelative * RotationSpeed;

            return TGCMatrix.RotationY(leftrightRot);
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

        private TGCVector3 CalculateInputTranslation()
        {
            var moveVector = TGCVector3.Empty;

            if (ConsideringInput)
                moveVector = GetInputTranslation(moveVector);

            return moveVector;
        }
        public void UseManually()
        {
            manual = true;
        }
        public void StopUsingManually()
        {
            manual = false;
        }
        private bool InsideBoundsX(TGCVector3 pos)
        {
            return true;
        }
        private bool InsideBoundsZ(TGCVector3 pos)
        {
            return true;
        }
        public void IgnoreInput()
        {
            ConsideringInput = false;
        }
        public void ConsiderInput()
        {
            ConsideringInput = true;
        }
    }
}
