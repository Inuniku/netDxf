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
    [AcadClassName("AcDbBlockAction")]
    public abstract class BlockAction : BlockElement
    {
        public Vector3 Label { get; set; }
        public BlockAction(string codename) : base(codename) { }
        public string[] Selection { get; protected set; }
        public int[] SelectionSet { get; protected set; }

        internal override void DXFOutLocal(ICodeValueWriter writer)
        {
            base.DXFOutLocal(writer);
            WriteClassBegin(writer, "AcDbBlockAction");

            writer.WriteList(70, 91, SelectionSet);
            writer.WriteList(71, 330, Selection);

            writer.WriteVector3(1010, Label);
        }

        internal override void DXFInLocal(ICodeValueReader reader)
        {
            base.DXFInLocal(reader);
            ReadClassBegin(reader, "AcDbBlockAction");
            ReaderAdapter reader2 = new ReaderAdapter(reader);

            reader2.ReadList<int>(70, 91, v => SelectionSet = v.ToArray());
            reader2.ReadList<string>(71, 330, v=> Selection = v.ToArray());

            reader2.ReadVector3(1010, v => Label = v);

            reader2.ExecReadUntil(0, 100, 1001);
        }
    }
}