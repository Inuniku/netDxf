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

        internal VisibilityProperty(BlockVisibilityParameter property, DynamicBlockReferenceContext context) : base(property, context)
        {
            Parameter = property;
        }
        new public string Value { get => (string)base.Value; set => base.Value = value; }

        internal override object InternalValue { get => Parameter.VisibilityState; set => Parameter.VisibilityState = (string)value; }

        public override UnitsType UnitsType => UnitsType.Distance;

        public override DataType PropertyTypeCode => DataType.DxfReal;

        public override string PropertyName => Parameter.Label;

        public override string Description => Parameter.LabelDesc;

        public override IEnumerable<object> AllowedValues => Parameter.States.Select(s => s.Name);
    }
}
