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
    [AcadClassName("AcDbDynamicBlockPurgePreventer")]
    public class DynamicBlockPurgePreventer : DbObject
    {
        public DynamicBlockPurgePreventer(string codename) : base(codename) { }

        public int Version { get; set; }
        internal override void DXFOutLocal(ICodeValueWriter writer)
        {
            base.DXFOutLocal(writer);
            WriteClassBegin(writer, "AcDbDynamicBlockPurgePreventer");

            writer.Write(70, (short)Version);
        }

        internal override void DXFInLocal(ICodeValueReader reader)
        {
            base.DXFInLocal(reader);
            ReadClassBegin(reader, "AcDbDynamicBlockPurgePreventer");

            ReaderAdapter reader2 = new ReaderAdapter(reader);
            reader2.Read<short>(70, v => Version = v);
            reader2.ExecReadUntil(0, 100, 1001);
        }
    }
}
