using System.Collections.Generic;
using TGC.Core.BoundingVolumes;
using TGC.Core.Collision;
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
        
        // false if fully outside, true if inside or intersects
        bool boxInFrustum( TgcFrustum fru)
        {
            // check box outside/inside of frustum
            for( int i=0; i<6; i++ )
            {
                int out_ = 0;
                out_ += ((dot( fru.FrustumPlanes[i], vec4(this.PMin.X, this.PMin.X, this.PMin.Z, 1.0f) ) < 0.0 )?1:0);
                out_+= ((dot( fru.FrustumPlanes[i], vec4(this.PMax.X, this.PMin.X, this.PMin.Z, 1.0f) ) < 0.0 )?1:0);
                out_+= ((dot( fru.FrustumPlanes[i], vec4(this.PMin.X, this.PMax.Y, this.PMin.Z, 1.0f) ) < 0.0 )?1:0);
                out_+= ((dot( fru.FrustumPlanes[i], vec4(this.PMax.X, this.PMax.Y, this.PMin.Z, 1.0f) ) < 0.0 )?1:0);
                out_+= ((dot( fru.FrustumPlanes[i], vec4(this.PMin.X, this.PMin.X, this.PMax.Z, 1.0f) ) < 0.0 )?1:0);
                out_+= ((dot( fru.FrustumPlanes[i], vec4(this.PMax.X, this.PMin.X, this.PMax.Z, 1.0f) ) < 0.0 )?1:0);
                out_+= ((dot( fru.FrustumPlanes[i], vec4(this.PMin.X, this.PMax.Y, this.PMax.Z, 1.0f) ) < 0.0 )?1:0);
                out_+= ((dot( fru.FrustumPlanes[i], vec4(this.PMax.X, this.PMax.Y, this.PMax.Z, 1.0f) ) < 0.0 )?1:0);
                if( out_==8 ) return false;
            }

            var points = new HashSet<TGCVector3>();
            foreach (var fruFrustumPlane in fru.FrustumPlanes)
            {
                points.Add(fruFrustumPlane.);
            }
            // check frustum outside/inside box
            int out2_;
                out2_=0; for( int i=0; i<8; i++ ) out2_ += ((fru.mPoints[i].x > this.PMax.X)?1:0); if( out2_==8 ) return false;
                out2_=0; for( int i=0; i<8; i++ ) out2_ += ((fru.mPoints[i].x < this.PMin.X)?1:0); if( out2_==8 ) return false;
                out2_=0; for( int i=0; i<8; i++ ) out2_ += ((fru.mPoints[i].y > this.PMax.Y)?1:0); if( out2_==8 ) return false;
                out2_=0; for( int i=0; i<8; i++ ) out2_ += ((fru.mPoints[i].y < this.PMin.X)?1:0); if( out2_==8 ) return false;
                out2_=0; for( int i=0; i<8; i++ ) out2_ += ((fru.mPoints[i].z > this.PMax.Z)?1:0); if( out2_==8 ) return false;
                out2_=0; for( int i=0; i<8; i++ ) out2_ += ((fru.mPoints[i].z < this.PMin.Z)?1:0); if( out2_==8 ) return false;

            return true;
        } 
    }
}