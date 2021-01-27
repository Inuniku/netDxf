using netDxf.Blocks.Dynamic.Util;
using netDxf.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace netDxf.Blocks.Dynamic.Property
{
    public class LinearProperty : DynamicBlockReferenceProperty
    {
        new private BlockLinearParameter Parameter;

        private Vector3 Point1 => Parameter.UpdatedPoint1;
        private Vector3 Point2 => Parameter.UpdatedPoint2;
        private Vector3 Dir => (Parameter.Point2 - Parameter.Point1).Normalized();
       
        internal LinearProperty(BlockLinearParameter property) : base(property)
        {
            Parameter = property;
        }

        public override object Value {
            get => Vector3.Distance(Point1, Point2);
            set => Parameter.UpdatedPoint2 = Point1 + Dir * (double)Convert.ChangeType(value, typeof(double)); 
        }

        public override UnitsType UnitsType => UnitsType.Distance;

        public override DataType PropertyTypeCode => DataType.DxfReal;

        public override string PropertyName => Parameter.Label;

        public override string Description => Parameter.LabelDesc;
    }
}
