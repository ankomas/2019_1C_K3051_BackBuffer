using System.Collections.Generic;
using System.Linq;
using Microsoft.DirectX.DirectInput;
using TGC.Core.BoundingVolumes;
using TGC.Core.Collision;
using TGC.Core.Direct3D;
using TGC.Core.Geometry;
using TGC.Core.Mathematica;

namespace TGC.Group.Model.Utils
{
    public class Cube
    {
        public TGCVector3 PMin { get; }
        public TGCVector3 PMax { get; }

        public Cube(TGCVector3 PMin, TGCVector3 PMax)
        {
            this.PMin = PMin;
            this.PMax = PMax;
        }

        public Cube(TGCVector3 origin, int radius)
        {
            this.PMin = new TGCVector3(origin.X - radius, origin.Y - radius, origin.Z - radius);
            this.PMax = new TGCVector3(origin.X + radius, origin.Y + radius, origin.Z + radius);
        }

        public bool contains(TGCVector3 point)
        {
            return betweenBounds(point.X, this.PMin.X, this.PMax.X)
                && betweenBounds(point.Y, this.PMin.Y, this.PMax.Y)
                && betweenBounds(point.Z, this.PMin.Z, this.PMax.Z);
        }

        private bool betweenBounds(float point, float min, float max)
        {
            return point >= min && point <= max;
        }

        public bool isIntersectedBy(TgcRay r)
        {
            var tMin = (this.PMin.X - r.Origin.X) / r.Direction.X;
            var tMax = (this.PMax.X - r.Origin.X) / r.Direction.X;
            float aux;
            
            if (tMin > tMax)
            {
                aux = tMin;
                tMin = tMax;
                tMax = aux;
            };
            
            var tyMin = (this.PMin.Y - r.Origin.Y) / r.Direction.Y;
            var tyMax = (this.PMax.Y - r.Origin.Y) / r.Direction.Y;

            if (tyMin > tyMax)
            {
                aux = tyMin;
                tyMin = tyMax;
                tyMax = aux;
            };

            if ((tMin > tyMax) || (tyMin > tMax))
                return false;

            if (tyMin > tMin)
                tMin = tyMin;

            if (tyMax < tMax)
                tMax = tyMax;

            var tzMin = (this.PMin.Z - r.Origin.Z) / r.Direction.Z;
            var tzMax = (this.PMax.Z - r.Origin.Z) / r.Direction.Z;

            if (tzMin > tzMax)
            {
                aux = tzMin;
                tzMin = tzMax;
                tzMax = aux;
            };

            return !(tMin > tzMax) && !(tzMin > tMax);
        }

        //Returns the eight points of the cube anti - clock wise starting bottom left and from down to up
        //8----7
        //5----6
        //|    |
        //4----3
        //1----2
        private TGCVector3[] eightPoins()
        {
            TGCVector3[] points = new TGCVector3[8];
            
            points[0] = PMin;
            points[1] = new TGCVector3(PMax.X, PMin.Y, PMin.Z);
            points[2] = new TGCVector3(PMax.X, PMin.Y, PMax.Z);
            points[3] = new TGCVector3(PMin.X, PMin.Y, PMax.Z);
            points[4] = new TGCVector3(PMin.X, PMax.Y, PMin.Z);
            points[5] = new TGCVector3(PMax.X, PMax.Y, PMin.Z);
            points[6] = PMax;
            points[7] = new TGCVector3(PMin.X, PMax.Y, PMax.Z);

            return points;
        }
        
        // check if any point of the cube is the frustum
        public bool isIn(TgcFrustum frustum)
        {
            var points = eightPoins();

            return frustum == null || frustum.FrustumPlanes.All(plane => points.Any(point => plane.Dot(point) >= 0));
        } 
    }
}