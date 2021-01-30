using netDxf.Blocks.Dynamic.Util;
using netDxf.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace netDxf.Blocks.Dynamic.Property
{
    public class XYProperty : DynamicBlockReferenceProperty
    {
        new private BlockXYParameter Parameter;

        public int Axis { get; }
        new public double Value { get => (double)base.Value; set => base.Value = value; }

        internal XYProperty(BlockXYParameter property, int axis, DynamicBlockReferenceContext context) : base(property, context)
        {
            Parameter = property;
            Axis = axis;
        }

        internal override object InternalValue
        {
            get => Axis == 0 ? Parameter.UpdatedDifference.X : Parameter.UpdatedDifference.Y;
            set
            {
                Vector3 currentPoint = Parameter.UpdatedDifference;
                if (Axis == 0)
                    currentPoint.X = Convert.ToDouble(value);
                else
                    currentPoint.Y = Convert.ToDouble(value);

                Parameter.UpdatedDifference = currentPoint;
            }
        }

        public override UnitsType UnitsType => UnitsType.Distance;

        public override DataType PropertyTypeCode => DataType.DxfReal;

        public override string PropertyName => Axis == 0 ? Parameter.XLabel : Parameter.YLabel;

        public override string Description => Axis == 0 ? Parameter.XLabelDesc : Parameter.YLabelDesc;
    }
}
