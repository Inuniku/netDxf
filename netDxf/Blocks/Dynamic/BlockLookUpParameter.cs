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
    [AcadClassName("AcDbBlockLookUpParameter")]
    public class BlockLookUpParameter : Block1PtParameter
    {
        public BlockLookUpParameter(string codename) : base(codename) { }
        public string Label { get; set; }
        public string Description { get; set; }
        public int ActionId { get; set; }

        public override bool Eval(EvalStep step, BlockEvaluationContext context)
        {
            throw new NotImplementedException();
        }

        internal override void DXFOutLocal(ICodeValueWriter writer)
        {
            base.DXFOutLocal(writer);
            WriteClassBegin(writer, "AcDbBlockLookUpParameter");

            writer.Write(303, Label);
            writer.Write(304, Description);

            writer.Write(94, ActionId);
        }

        internal override void DXFInLocal(ICodeValueReader reader)
        {
            base.DXFInLocal(reader);
            ReadClassBegin(reader, "AcDbBlockLookUpParameter");

            ReaderAdapter reader2 = new ReaderAdapter(reader);

            reader2.Read<string>(303, v => Label = v);
            reader2.Read<string>(304, v => Description = v);
            reader2.Read<int>(94, v => ActionId = v);

            reader2.ExecReadUntil(0, 100, 1001);
        }
    }
}
