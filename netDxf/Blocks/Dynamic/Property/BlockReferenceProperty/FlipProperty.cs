using netDxf.Blocks.Dynamic.Util;
using netDxf.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace netDxf.Blocks.Dynamic.Property
{
    public class FlipProperty : DynamicBlockReferenceProperty
    {
        new internal BlockFlipParameter Parameter;

        public static readonly FlipState[] AllStates = new FlipState[] { FlipState.NotFlipped, FlipState.Flipped};
        new public FlipState Value { get => (FlipState)base.Value; set => base.Value = value; }

        internal FlipProperty(BlockFlipParameter property, DynamicBlockReferenceContext context) : base(property, context)
        {
            Parameter = property;
        }
        internal override object InternalValue { get => Parameter.UpdatedState; set => Parameter.UpdatedState = (FlipState)value; }

        public override UnitsType UnitsType => UnitsType.Distance;

        public override DataType PropertyTypeCode => DataType.DxfReal;

        public override string PropertyName => Parameter.Label;

        public override string Description => Parameter.LabelDesc;
        public override IEnumerable<object> AllowedValues => Enum.GetValues(typeof(FlipState)).OfType<object>();
    }
}
