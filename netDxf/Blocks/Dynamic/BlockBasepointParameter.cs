using netDxf.Blocks.Dynamic.Attributes;
using netDxf.Blocks.Dynamic.Util;
using netDxf.IO;
using netDxf.Objects;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace netDxf.Blocks.Dynamic
{
    [AcadClassName("AcDbBlockBasepointParameter")]
    internal class BlockBasepointParameter : Block1PtParameter
    {
        public Vector3 BasePoint1 { get; set; }
        public Vector3 BasePoint2 { get; set; }

        public BlockBasepointParameter(string codename) : base(codename)
        {}

        internal override void DXFOutLocal(ICodeValueWriter writer)
        {
            base.DXFOutLocal(writer);
            WriteClassBegin(writer, "AcDbBlockBasepointParameter");

            writer.WriteVector3(1011, BasePoint1);
            writer.WriteVector3(1012, BasePoint2);
        }

        internal override void DXFInLocal(ICodeValueReader reader)
        {
            base.DXFInLocal(reader);
            ReadClassBegin(reader, "AcDbBlockBasepointParameter");

            ReaderAdapter reader2 = new ReaderAdapter(reader); 
            
            reader2.ReadVector3(1011, v => BasePoint2 = v);
            reader2.ReadVector3(1012, v => BasePoint2 = v);

            reader2.ExecReadUntil(0, 100, 1001);
        }
    }
}
