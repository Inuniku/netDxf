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
    [AcadClassName("AcDbBlockFlipParameter", 6887019, 9372877)]
    public class BlockFlipParameter : Block2PtParameter
    {
        public BlockFlipParameter(string codename) : base(codename) { }
        public string Label { get; set; }
        public string LabelDesc { get; set; }
        public string NotFlippedStateName { get; set; }
        public string FlippedStateName { get; set; }
        public Vector3 LabelPosition { get; set; }
        public BlockConnection FlipConnection { get; } = new BlockConnection();
        public double UpperBound { get; set; }
        public double Increment { get; set; }
        public double ValueListSize { get; set; }

        [ConnectableProperty("UpdatedFlip")]
        internal FlipState UpdatedState { get; set; }
        [ConnectableProperty("Flip")]
        internal FlipState State { get; set; }

        public override bool Eval(EvalStep step, BlockEvaluationContext context)
        {
            if (!base.Eval(step, context))
                return false;

            if (step == EvalStep.Execute)
            {
                UpdatedState = (FlipState)FlipConnection.Evaluate(context);
                return true;
            }

            if (step == EvalStep.Commit)
            {
                State = UpdatedState;
                return true;
            }
            return true;
        }

        internal override void DXFOutLocal(ICodeValueWriter writer)
        {
            base.DXFOutLocal(writer);
            WriteClassBegin(writer, "AcDbBlockFlipParameter");

            writer.Write(305, Label);
            writer.Write(306, LabelDesc);
            writer.Write(307, NotFlippedStateName);
            writer.Write(308, FlippedStateName);

            writer.WriteVector3(1012, LabelPosition);

            writer.Write(96, FlipConnection.Id);
            writer.Write(309, FlipConnection.Connection);
        }

        internal override void DXFInLocal(ICodeValueReader reader)
        {
            ReaderAdapter reader2 = new ReaderAdapter(reader);

            base.DXFInLocal(reader);
            ReadClassBegin(reader, "AcDbBlockFlipParameter");

            reader2.Read<string>(305, v => Label = v);
            reader2.Read<string>(306, v => LabelDesc = v);
            reader2.Read<string>(307, v => NotFlippedStateName = v);
            reader2.Read<string>(308, v => FlippedStateName = v);

            reader2.ReadVector3(1012, v => LabelPosition = v);

            reader2.Read<string>(309, v => FlipConnection.Connection = v);
            reader2.Read<int>(96, v => FlipConnection.Id = v);

            reader2.ExecReadUntil(0, 100, 1001);
        }
        internal override void InitializeRuntimeData()
        {
            base.InitializeRuntimeData();
            State = UpdatedState = FlipState.NotFlipped;
        }

        internal override void RuntimeDataIn(ICodeValueReader reader)
        {
            ReaderAdapter reader2 = new ReaderAdapter(reader);

            base.RuntimeDataIn(reader);
            State = UpdatedState = (FlipState)reader2.ReadNow<short>(70);
        }

        internal override void RuntimeDataOut(ICodeValueWriter writer)
        {
            base.RuntimeDataOut(writer);
            writer.Write(70, (short)State);
        }
    }
}
