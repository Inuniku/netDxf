using netDxf.Entities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace netDxf.Blocks.Dynamic.Util
{
    public static class GeometryUtils
    {
        public static Vector3 Normalized(this Vector3 vector)
        {
            vector.Normalize();
            return vector;
        }
        public static Vector2 Normalized(this Vector2 vector)
        {
            vector.Normalize();
            return vector;
        }

        public static void StretchObject(EntityObject entity, int[] indices, Vector3 delta)
        {
            Vector2 delta2d = new Vector2(delta.X, delta.Y);

            if (entity is Line line)
            {
                for (int i = 0; i < indices.Length; i++)
                {
                    if (indices.Contains(0))
                        line.StartPoint += delta;
                    if (indices.Contains(1))
                        line.EndPoint += delta;
                }
            }
            else
            if (entity is Polyline pline)
            {
                for (int i = 0; i < indices.Length; i++)
                    pline.Vertexes[indices[i]].Position += delta;
            }
            else
            if (entity is LwPolyline lwpline)
            {
                int prevLoopId = -1;
                double prevLoopSag = 0;
                Vector2 prevLoopPos = new Vector2();
                // TODO check Integrity
                for (int i = 0; i < indices.Length; i++)
                {
                    int thisId = indices[i];
                    int prevId = indices[i] - 1;
                    int nextId = indices[i] + 1;

                    Vector2 thisPos = lwpline.Vertexes[thisId].Position;

                    // Check if just begun, or if we jumped continous segments
                    if (prevId != prevLoopId && prevId > 0)
                    {
                        if (prevLoopId != -1 && prevLoopId + 1 < lwpline.Vertexes.Count)
                        {
                            Vector2 newPos = lwpline.Vertexes[prevLoopId + 1].Position;
                            double newDist = Vector2.Distance(prevLoopPos, newPos);
                            lwpline.Vertexes[prevLoopId].Bulge = prevLoopSag / newDist;
                        }

                        prevLoopPos = lwpline.Vertexes[prevId].Position;
                        double prevDist = Vector2.Distance(prevLoopPos, thisPos);
                        prevLoopSag = lwpline.Vertexes[prevId].Bulge * prevDist;
                        prevLoopId = prevId;
                    }

                    // Get current sag
                    Vector2 nextPos = lwpline.Vertexes[nextId].Position;
                    double dist = Vector2.Distance(thisPos, nextPos);
                    double sag = lwpline.Vertexes[thisId].Bulge * dist * 0.5;
                    
                    // Update position (and sag)
                    Vector2 updatedPosition = thisPos + delta2d;
                    lwpline.Vertexes[thisId].Position = updatedPosition;

                    // Update prev segment sag
                    double updatedDistance = Vector2.Distance(prevLoopPos, updatedPosition);
                    lwpline.Vertexes[prevId].Bulge = prevLoopSag / updatedDistance;

                    prevLoopId = thisId;
                    prevLoopSag = sag;
                    prevLoopPos = thisPos;
                }

                if(prevLoopId + 1 < lwpline.Vertexes.Count)
                {
                    Vector2 nextPos = lwpline.Vertexes[prevLoopId + 1].Position;
                    double dist = Vector2.Distance(prevLoopPos, nextPos);
                    lwpline.Vertexes[prevLoopId].Bulge = prevLoopSag / dist;
                }
            }
            else
            if (entity is Wipeout wipeout)
            {
                wipeout.ClippingBoundary = new ClippingBoundary(wipeout.ClippingBoundary.Vertexes.Select(v => delta2d));
            }
            else
                throw new NotImplementedException("Stretching object is not supported");
        }

        public static Matrix3 FlipMatrix2D(Vector2 p1, Vector2 p2, out Vector3 translate)
        {
            Vector2 d = (p2 - p1).Normalized();
            double dx = d.X;
            double dy = d.Y;
            Vector2 t = 2.0f * Vector2.DotProduct(d, p1) * d;
            double tx = t.X;
            double ty = t.Y;
            double f = Vector2.DotProduct(d, d);


            double a = (dy * dy - (dx * dx));
            double b = (-2.0 * dx * dy) ;
            double c = (-2.0 * dx * dy);
            double _d = ((dx - dy) * (dx + dy));
            double inv_f = (1.0 / f);

            Matrix3 mat = new Matrix3(a, b, 0, c, _d, 0, 0, 0, 0);
            translate = new Vector3(tx * inv_f, ty * inv_f, 0);

            return mat * inv_f;
        }

        internal static Matrix4 ToObjectSpaceTransform(Vector3 basePos, Vector3 dirX, Vector3 dirY)
        {
            return new Matrix4(dirX.X, dirY.X, 0, basePos.X, dirX.Y, dirY.Y, 0, basePos.Y, dirX.Z, dirY.Z, 0, basePos.Z, 0, 0, 0, 1);
        }
    }
}
