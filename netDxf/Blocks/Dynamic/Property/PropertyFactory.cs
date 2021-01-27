using netDxf.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace netDxf.Blocks.Dynamic.Property
{
    internal class PropertyFactory
    {
        internal static IEnumerable<DynamicBlockReferenceProperty> CreatePropertyWrappers(BlockParameter blockParam)
        {
            if(blockParam is BlockLinearParameter linearParameter)
            {
                return new DynamicBlockReferenceProperty[] { new LinearProperty(linearParameter) };
            }

            if (blockParam is BlockPointParameter pointParameter)
            {
                return new DynamicBlockReferenceProperty[] { new PointProperty(pointParameter, 0), new PointProperty(pointParameter, 1) };
            }

            if (blockParam is BlockFlipParameter flipParameter)
            {
                return new DynamicBlockReferenceProperty[] { new FlipProperty(flipParameter) };
            }

            if (blockParam is BlockVisibilityParameter visibilityParameter)
            {
                return new DynamicBlockReferenceProperty[] { new VisibilityProperty(visibilityParameter) };
            }
            return Enumerable.Empty<DynamicBlockReferenceProperty>();
            //throw new NotImplementedException();
        }
    }
}
