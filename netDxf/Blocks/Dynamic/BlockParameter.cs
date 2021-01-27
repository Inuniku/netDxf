using netDxf.Blocks.Dynamic.Attributes;
using netDxf.Blocks.Dynamic.Util;
using netDxf.IO;
using netDxf.Objects;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace netDxf.Blocks.Dynamic
{
    [AcadClassName("AcDbBlockParameter")]
    public abstract class BlockParameter : BlockElement
    {
        public BlockParameter(string codename) : base(codename) { }
        public bool ShowProperies { get; set; }
        public bool ChainActions { get; set; }

        internal override void DXFOutLocal(ICodeValueWriter writer)
        {
            base.DXFOutLocal(writer);
            WriteClassBegin(writer, "AcDbBlockParameter");

            writer.WriteBool(280, ShowProperies);
            writer.WriteBool(281, ChainActions);
        }


        internal override void DXFInLocal(ICodeValueReader reader)
        {
            base.DXFInLocal(reader);
            ReadClassBegin(reader, "AcDbBlockParameter");
            ReaderAdapter reader2 = new ReaderAdapter(reader);

            reader2.Read<short>(280, v => ShowProperies = v != 0);
            reader2.Read<short>(281, v => ChainActions = v != 0);

            reader2.ExecReadUntil(0, 100, 1001);

        }
    }
}
