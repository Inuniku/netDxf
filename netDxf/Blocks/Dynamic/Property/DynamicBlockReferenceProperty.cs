using netDxf;
using netDxf.Blocks;
using netDxf.Blocks.Dynamic;
using netDxf.Blocks.Dynamic.Property;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace netDxf.Blocks.Dynamic.Property
{
    public abstract class DynamicBlockReferenceProperty
    {
        internal BlockParameter Parameter;

        internal DynamicBlockReferenceProperty(BlockParameter property)
        {
            Parameter = property;
        }

        public abstract object Value { get; set; }

        public abstract UnitsType UnitsType {get;}

        public abstract string Description {get;}

        public virtual bool VisibleInCurrentVisibilityState => throw new NotImplementedException();

        public virtual bool Show => Parameter.ShowProperies;

        public virtual bool ReadOnly => false;

        public abstract DataType PropertyTypeCode {get;}

        public abstract string PropertyName {get;}

        public virtual string BlockId => Parameter.Owner.Owner.Owner.Handle;

    }
}
