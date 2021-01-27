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
    [AcadClassName("AcDbBlockElement")]
    public abstract class BlockElement : BlockEvalConnectable
    {
        public BlockElement(string codename) : base(codename) { }

        public int UK1 { get; protected set; }
        public string Name { get; protected set; }

        internal override void DXFOutLocal(ICodeValueWriter writer)
        {
            base.DXFOutLocal(writer);
            WriteClassBegin(writer, "AcDbBlockElement");

            writer.Write(300, Name);
            writer.Write(98, VersionMinor);
            writer.Write(99, VersionMajor);
            writer.Write(1071, UK1);
        }

        internal override void DXFInLocal(ICodeValueReader reader)
        {
            base.DXFInLocal(reader);
            ReadClassBegin(reader, "AcDbBlockElement");

            ReaderAdapter reader2 = new ReaderAdapter(reader);

            reader2.Read<string>(300, v => Name = v);
            reader2.Read<int>(98, v => VersionMinor = v);
            reader2.Read<int>(99, v => VersionMajor = v);
            reader2.Read<int>(1071, v => UK1 = v);

            reader2.ExecReadUntil(0, 100, 1001);
        }
    }
}
