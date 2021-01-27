using netDxf;
using netDxf.Blocks;
using netDxf.Blocks.Dynamic;
using netDxf.Blocks.Dynamic.Property;
using netDxf.Blocks.Dynamic.Util;
using netDxf.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace netDxf.Blocks.Dynamic.Property
{
    public class BlockParamValueSet
    {
        public ParamValueSetType Type { get; set; }
        public double LowerBound { get; set; }
        public double UpperBound { get; set; }
        public double Increment { get; set; }
        public double[] ValueList { get; set; }
    }
}
