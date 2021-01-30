using netDxf.Blocks.Dynamic.Attributes;
using netDxf.Blocks.Dynamic.Util;
using netDxf.IO;
using netDxf.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace netDxf.Blocks.Dynamic
{
    [AcadClassName("AcDbBlockPointParameter", 11324608, 15407465)]
    public class BlockPointParameter : Block1PtParameter
    {
        public string Label { get; set; }
        public string LabelDesc { get; set; }
        public Vector3 LabelPosition { get; set; }
        public BlockPointParameter(string codename) : base(codename)
        {}




        internal override void DXFOutLocal(ICodeValueWriter writer)
        {
            base.DXFOutLocal(writer);
            WriteClassBegin(writer, "AcDbBlockPointParameter");


            writer.Write(303, Label);
            writer.Write(304, LabelDesc);
            writer.WriteVector3(1011, LabelPosition);
        }
        internal override void DXFInLocal(ICodeValueReader reader)
        {
            base.DXFInLocal(reader);
            ReadClassBegin(reader, "AcDbBlockPointParameter");

            ReaderAdapter reader2 = new ReaderAdapter(reader);

            reader2.Read<string>(303, v => Label = v);
            reader2.Read<string>(304, v => LabelDesc = v);
            reader2.ReadVector3(1011, v => LabelPosition = v);
            reader2.ExecReadUntil(0, 100, 1001);
        }
    }
}
