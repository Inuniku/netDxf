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

        internal LinearProperty(BlockLinearParameter property, DynamicBlockReferenceContext context) : base(property, context)
        {
            Parameter = property;
        }

        internal override object InternalValue {
            get => Parameter.UpdatedDistance;
            set => Parameter.UpdatedDistance = (double)value; 
        }

        public override UnitsType UnitsType => UnitsType.Distance;

        public override DataType PropertyTypeCode => DataType.DxfReal;

        public override string PropertyName => Parameter.Label;

        public override string Description => Parameter.LabelDesc;
    }
}
