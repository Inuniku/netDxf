using netDxf.Blocks.Dynamic.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace netDxf.Blocks.Dynamic
{
    [AcadClassName("AcDbDynamicBlockProxyNode")]
    public class DynamicBlockProxyNode : EvalExpr
    {
        public DynamicBlockProxyNode(string codename) : base(codename)
        {
        }
    }
}
