using netDxf.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace netDxf.Blocks.Dynamic.Property
{
    public class PointProperty : DynamicBlockReferenceProperty
    {
        new private BlockPointParameter Parameter;

        private Vector3 Point1 => Parameter.UpdatedPoint;
        internal PointProperty(BlockPointParameter property, int axis, DynamicBlockReferenceContext context) : base(property, context)
        {
            Axis = axis;
            Parameter = property;
        }

        new public double Value { get => (double)base.Value; set => base.Value = value; }

        internal override object InternalValue
        {
            get => Axis == 0 ? Point1.X : Point1.Y; 
            set
            {
                Vector3 currentPoint = Parameter.UpdatedPoint;
                if (Axis == 0)
                    currentPoint.X = Convert.ToDouble(value);
                else
                    currentPoint.Y = Convert.ToDouble(value);

                Parameter.UpdatedPoint = currentPoint;
            }
        }

        public override UnitsType UnitsType => UnitsType.Distance;

        public override DataType PropertyTypeCode => DataType.DxfReal;

        public override string PropertyName => Parameter.Label + (Axis == 0 ? " X" : " Y");

        public override string Description => Parameter.LabelDesc;
        public int Axis { get; private set; }
    }
}
