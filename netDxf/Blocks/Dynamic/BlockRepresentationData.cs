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
    [AcadClassName("AcDbBlockRepresentationData")]
    public class BlockRepresentationData : DbObject
    {
        public BlockRepresentationData(string codename) : base(codename) { }

        public int Version { get; set; }
        internal string Id { get; set; }
        public BlockRecord BlockRecord
        {
            get => Document.GetObjectByHandle(Id) as BlockRecord;
            set => Id = value.Handle;
        }

        internal override void DXFOutLocal(ICodeValueWriter writer)
        {
            base.DXFOutLocal(writer);
            WriteClassBegin(writer, "AcDbBlockRepresentationData");

            writer.Write(70, (short)Version);
            writer.Write(340, Id);
        }

        internal override void DXFInLocal(ICodeValueReader reader)
        {
            base.DXFInLocal(reader);
            ReadClassBegin(reader, "AcDbBlockRepresentationData");
            ReaderAdapter reader2 = new ReaderAdapter(reader);

            reader2.Read<short>(70, v => Version = v);
            reader2.Read<string>(340, v => Id = v);

            reader2.ExecReadUntil(0, 100, 1001);
        }
    }
}
