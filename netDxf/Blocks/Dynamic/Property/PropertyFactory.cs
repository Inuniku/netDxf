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
        internal static IEnumerable<DynamicBlockReferenceProperty> CreatePropertyWrappers(BlockParameter blockParam, DynamicBlockReferenceContext context)
        {
            if (blockParam is BlockLinearParameter linearParameter)
            {
                return new DynamicBlockReferenceProperty[] { new LinearProperty(linearParameter, context) };
            }

            if (blockParam is BlockPointParameter pointParameter)
            {
                return new DynamicBlockReferenceProperty[] { new PointProperty(pointParameter, 0, context), new PointProperty(pointParameter, 1, context) };
            }

            if (blockParam is BlockFlipParameter flipParameter)
            {
                return new DynamicBlockReferenceProperty[] { new FlipProperty(flipParameter, context) };
            }

            if (blockParam is BlockVisibilityParameter visibilityParameter)
            {
                return new DynamicBlockReferenceProperty[] { new VisibilityProperty(visibilityParameter, context) };
            }

            if (blockParam is BlockXYParameter xyParameter)
            {
                return new DynamicBlockReferenceProperty[] { new XYProperty(xyParameter, 0, context) , new XYProperty(xyParameter, 1, context) };
            }

            if (blockParam is BlockBasepointParameter)
            {
                return Array.Empty<DynamicBlockReferenceProperty>();
            }
            throw new NotImplementedException();
        }
    }
}
