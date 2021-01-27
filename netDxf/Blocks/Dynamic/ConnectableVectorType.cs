using System;

namespace netDxf.Blocks.Dynamic
{

    public enum ConnectableVectorType
    {
        Normal = 1,
        X = 2,
        Y = 4,
        Z = 8,
        XY = 6,
        XYZ = 14,
        Normal_And_XY = 7,
        Normal_And_XYZ = 15
    };
}