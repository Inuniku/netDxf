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
    // TODO... Disposable!
    public abstract class DynamicBlockReferenceProperty
    {
        internal BlockParameter Parameter;
        internal DynamicBlockReferenceContext Context;

        private object _setValue = null;

        internal DynamicBlockReferenceProperty(BlockParameter property, DynamicBlockReferenceContext context)
        {
            Parameter = property;
            Context = context;
        }
        // TODO... More Like a function not a property
        internal abstract object InternalValue { get; set; }

        public object Value { get => InternalValue; set => _setValue = value; }

        public abstract UnitsType UnitsType {get;}

        public abstract string Description {get;}

        public virtual bool VisibleInCurrentVisibilityState => throw new NotImplementedException();

        public virtual bool Show => Parameter.ShowProperies;

        public virtual bool IsReadOnly => false;

        public abstract DataType PropertyTypeCode {get;}

        public abstract string PropertyName {get;}

        public virtual IEnumerable<object> AllowedValues => Enumerable.Empty<object>();

        public bool Apply()
        {
            if (_setValue == null)
                return false;
            

            if (!Context.HasRepresentation)
                Context.CreateRepresentation();

            Context.InitializeEvaluation();

            InternalValue = _setValue;
            Context.ChangedNodes.Add(Parameter);
            // TODO AutoCad stores Parameter.Label
            //Context.AddHistoryChange(Parameter.NodeId, Parameter.Name, _setValue);
            Context.AddHistoryChange(Parameter.NodeId, PropertyName, _setValue);
            Context.CommitEvaluation();


            return true;
        }

        public virtual string BlockId => Parameter.Owner.Owner.Owner.Handle;

    }
}
