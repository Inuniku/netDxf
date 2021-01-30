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
    [AcadClassName("AcDbBlockGripExpr")]
    public class BlockGripExpr : BlockEvalConnectable
    {
        public BlockGripExpr(string codename) : base(codename) { }
        public BlockConnection GripConnection { get; } = new BlockConnection();

        [ConnectableProperty("Value")]
        public object Value { get; set; }

        public override bool Eval(EvalStep step, BlockEvaluationContext context)
        {
            if (!base.Eval(step, context))
                return false;


            if (step == EvalStep.Execute)
            {
                //Value = GripConnection.Evaluate(context);
                return true;
            }

            return true;
        }

        internal override void DXFOutLocal(ICodeValueWriter writer)
        {
            base.DXFOutLocal(writer);
            WriteClassBegin(writer, "AcDbBlockGripExpr");

            writer.Write(91, GripConnection.Id);
            writer.Write(300, GripConnection.Connection);
        }

        internal override void DXFInLocal(ICodeValueReader reader)
        {
            base.DXFInLocal(reader);
            ReadClassBegin(reader, "AcDbBlockGripExpr");
            ReaderAdapter reader2 = new ReaderAdapter(reader);

            reader2.Read<int>(91, v => GripConnection.Id = v);
            reader2.Read<string>(300, v => GripConnection.Connection = v);

            reader2.ExecReadUntil(0, 100, 1001);
        }

    }
}
