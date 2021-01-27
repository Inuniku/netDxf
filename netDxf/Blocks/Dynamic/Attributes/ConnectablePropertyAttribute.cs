using System;

namespace netDxf.Blocks.Dynamic
{

    [AttributeUsage(AttributeTargets.Property)]
    internal class ConnectablePropertyAttribute : Attribute
    {
        public string PropertyName { get; }
        public ConnectableVectorType VectorType { get; }
        public string Postfix { get; }

        public ConnectablePropertyAttribute(string propertyName, ConnectableVectorType vectorType = ConnectableVectorType.Normal, string postfix = "")
        {
            PropertyName = propertyName;
            VectorType = vectorType;
            Postfix = postfix;
        }

    }
}