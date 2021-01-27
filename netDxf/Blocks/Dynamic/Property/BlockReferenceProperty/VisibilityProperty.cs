using netDxf.Blocks.Dynamic.Util;
using netDxf.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace netDxf.Blocks.Dynamic.Property
{
    public class VisibilityProperty : DynamicBlockReferenceProperty
    {
        new private BlockVisibilityParameter Parameter;

        internal VisibilityProperty(BlockVisibilityParameter property) : base(property)
        {
            Parameter = property;
        }

        public override object Value { get => Parameter.VisibilityState; set => Parameter.VisibilityState = (string)value; }

        public override UnitsType UnitsType => UnitsType.Distance;

        public override DataType PropertyTypeCode => DataType.DxfReal;

        public override string PropertyName => Parameter.Label;

        public override string Description => Parameter.LabelDesc;
    }
}
