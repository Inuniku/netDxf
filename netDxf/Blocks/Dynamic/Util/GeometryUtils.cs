using System;
using System.Collections.Generic;
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
    }
}
