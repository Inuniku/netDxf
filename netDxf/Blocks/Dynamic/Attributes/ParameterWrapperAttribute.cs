using System;

namespace netDxf.Blocks.Dynamic.Property
{
    internal class ParameterWrapperAttribute : Attribute
    {
        public ParameterWrapperAttribute(string v1, int v2, int v3)
        {
            ClassName = v1;
            ClassId1 = v2;
            ClassId2 = v3;
        }

        public string ClassName { get; }
        public int ClassId1 { get; }
        public int ClassId2 { get; }
    }
}