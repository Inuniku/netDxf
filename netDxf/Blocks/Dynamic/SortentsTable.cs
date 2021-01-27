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
    [AcadClassName("AcDbSortentsTable")]
    public class SortentsTable : DbObject
    {
        public Dictionary<string, string> Dictionary = new Dictionary<string, string>();
        public string OwnerBlockId { get; set;}
        public SortentsTable(string codename) : base(codename)
        {}
        internal override void DXFOutLocal(ICodeValueWriter writer)
        {
            base.DXFOutLocal(writer);
            WriteClassBegin(writer, "AcDbSortentsTable");
            writer.Write(330, OwnerBlockId);
        }

        internal override void DXFInLocal(ICodeValueReader reader)
        {
            base.DXFInLocal(reader);
            ReadClassBegin(reader, "AcDbSortentsTable");
            ReaderAdapter reader2 = new ReaderAdapter(reader);

            reader2.Read<string>(330, v => OwnerBlockId = v);




            reader2.ExecReadUntil(0, 100, 1001);
        }

    }
}
