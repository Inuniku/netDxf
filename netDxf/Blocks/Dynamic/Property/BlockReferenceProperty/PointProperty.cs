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
        internal PointProperty(BlockPointParameter property, int axis) : base(property)
        {
            Axis = axis;
            Parameter = property;
        }
        public override object Value
        {
            get => Axis == 0 ? Point1.X : Point1.Y; 
            set
            {
                Vector3 currentPoint = Parameter.UpdatedPoint;
                if (Axis == 0)
                    currentPoint.X = (double)value;
                else
                    currentPoint.Y = (double)Value;

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
