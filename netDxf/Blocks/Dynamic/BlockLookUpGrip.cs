using netDxf;
using netDxf.Blocks;
using netDxf.Blocks.Dynamic;
using netDxf.Blocks.Dynamic.Attributes;
using netDxf.Blocks.Dynamic.Util;
using netDxf.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace netDxf.Blocks.Dynamic
{
    [AcadClassName("AcDbBlockLookupGrip")]
    public class BlockLookupGrip : BlockGrip
    {
        public BlockLookupGrip(string codename) : base(codename) { }
        internal override void DXFOutLocal(ICodeValueWriter writer)
        {
            base.DXFOutLocal(writer);
            WriteClassBegin(writer, "AcDbBlockLookupGrip");
        }

        internal override void DXFInLocal(ICodeValueReader reader)
        {
            base.DXFInLocal(reader);
            ReadClassBegin(reader, "AcDbBlockLookupGrip");
            ReaderAdapter reader2 = new ReaderAdapter(reader);
            reader2.ExecReadUntil(0, 100, 1001);

        }
    }
}
