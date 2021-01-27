using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace netDxf.Blocks.Dynamic.Attributes
{
    [System.AttributeUsage(System.AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    sealed class AcadClassNameAttribute : Attribute
    {
        public AcadClassNameAttribute(string acadName, int id1 = 0, int id2 = 0)
        {
            PositionalString = acadName;
            Id1 = id1;
            Id2 = id2;
        }

        public string PositionalString { get; private set; }
        public int Id1 { get; }
        public int Id2 { get; }
    }
}
