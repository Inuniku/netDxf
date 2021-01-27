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
    [AcadClassName("AcDbBlockFlipAction")]
    public class BlockFlipAction : BlockAction
    {
        public BlockFlipAction(string codename) : base(codename) { }
        public BlockConnection FlipConnection { get; } = new BlockConnection();
        public BlockConnection UpdateFlipConnection { get; } = new BlockConnection();
        public BlockConnection UpdateBaseConnection { get; } = new BlockConnection();
        public BlockConnection UpdateEndConnection { get; } = new BlockConnection();

        public override bool Eval(EvalStep step, BlockEvaluationContext context)
        {
            if (!base.Eval(step, context))
                return false;

            if (step == EvalStep.Initialize || step == EvalStep.Abort)
            {
                return true;
            }

            if (step == EvalStep.Update)
            {
                FlipState flip = (FlipState)FlipConnection.Evaluate(context);
                FlipState updateFlip = (FlipState)FlipConnection.Evaluate(context);

                Vector3 basePos = (Vector3)UpdateBaseConnection.Evaluate(context);
                Vector3 EndPos = (Vector3)UpdateBaseConnection.Evaluate(context);

                Debug.WriteLine("I FlIP AUS!");

                return true;
            }

            if (step == EvalStep.Commit)
            {
                return true;
            }
            return true;
        }

        internal override void DXFOutLocal(ICodeValueWriter writer)
        {
            base.DXFOutLocal(writer);
            WriteClassBegin(writer, "AcDbBlockFlipAction");

            writer.Write(92, FlipConnection.Id);
            writer.Write(93, UpdateFlipConnection.Id);
            writer.Write(94, UpdateBaseConnection.Id);
            writer.Write(95, UpdateEndConnection.Id);

            writer.Write(301, FlipConnection.Connection);
            writer.Write(302, UpdateFlipConnection.Connection);
            writer.Write(303, UpdateBaseConnection.Connection);
            writer.Write(304, UpdateEndConnection.Connection);
        }

        internal override void DXFInLocal(ICodeValueReader reader)
        {
            base.DXFInLocal(reader);
            ReadClassBegin(reader, "AcDbBlockFlipAction");

            ReaderAdapter reader2 = new ReaderAdapter(reader);

            reader2.Read<int>(92, v => FlipConnection.Id = v);
            reader2.Read<int>(93, v => UpdateFlipConnection.Id = v);
            reader2.Read<int>(94, v => UpdateBaseConnection.Id = v);
            reader2.Read<int>(95, v => UpdateEndConnection.Id = v);

            reader2.Read<string>(301, v => FlipConnection.Connection = v);
            reader2.Read<string>(302, v => UpdateFlipConnection.Connection = v);
            reader2.Read<string>(303, v => UpdateBaseConnection.Connection = v);
            reader2.Read<string>(304, v => UpdateEndConnection.Connection = v);

            reader2.ExecReadUntil(0, 100, 1001);
        }
    }
}