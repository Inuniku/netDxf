using netDxf.Blocks.Dynamic.Attributes;
using netDxf.Blocks.Dynamic.Util;
using netDxf.IO;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace netDxf.Blocks.Dynamic
{
    [AcadClassName("AcDbBlockLinearGrip")]
    public class BlockLinearGrip : BlockGrip
    {
        public Vector3 InitialDistance { get; set; }
        public BlockLinearGrip(string codename) : base(codename) { }
        internal override void DXFOutLocal(ICodeValueWriter writer)
        {
            base.DXFOutLocal(writer);
            WriteClassBegin(writer, "AcDbBlockLinearGrip");

            writer.WriteVector3(140, InitialDistance, 1);
        }

        internal override void DXFInLocal(ICodeValueReader reader)
        {
            base.DXFInLocal(reader);
            ReadClassBegin(reader, "AcDbBlockLinearGrip");
            ReaderAdapter reader2 = new ReaderAdapter(reader);

            reader2.ReadVector3(140, v => InitialDistance = v, 1);

            reader2.ExecReadUntil(0, 100, 1001);
        }
    }
}
