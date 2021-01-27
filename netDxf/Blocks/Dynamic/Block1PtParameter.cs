using netDxf.Blocks.Dynamic.Attributes;
using netDxf.Blocks.Dynamic.Util;
using netDxf.IO;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace netDxf.Blocks.Dynamic
{
    [AcadClassName("AcDbBlock1PtParameter")]
    public abstract class Block1PtParameter : BlockParameter
    {
        public Block1PtParameter(string codename) : base(codename) { }

        [ConnectableProperty("", ConnectableVectorType.XY)]
        public Vector3 BasePoint { get; set; }

        [ConnectableProperty("Updated", ConnectableVectorType.XY)]
        public Vector3 UpdatedPoint { get; set; }

        [ConnectableProperty("", ConnectableVectorType.XY, "Delta")]
        public Vector3 PointDelta
        {
            get => UpdatedPoint - BasePoint;
            set => UpdatedPoint = BasePoint + value;
        }

        public int Messages { get; set; }
        public BlockConnection Connection { get; private set; } = new BlockConnection();
        public int Messages1 { get; set; }
        public BlockConnection Connection1 { get; private set; } = new BlockConnection();
        public int GripId { get; set; }

        public override bool Eval(EvalStep step, BlockEvaluationContext context)
        {
            if (!base.Eval(step, context))
                return false;

            if (step == EvalStep.Initialize || step == EvalStep.Abort)
            {
                UpdatedPoint = BasePoint;
                return true;
            }

            if (step == EvalStep.Update)
            {
                Vector3 updatedpoint = UpdatedPoint;

                if (Connection.IsValid)
                    updatedpoint.X += (double)Connection.Evaluate(context);
                if (Connection1.IsValid)
                    updatedpoint.Y += (double)Connection1.Evaluate(context);

                UpdatedPoint = updatedpoint;

                return true;
            }

            if (step == EvalStep.Commit)
            {
                BasePoint = UpdatedPoint;
                return true;
            }
            return true;
        }

        internal override void DXFOutLocal(ICodeValueWriter writer)
        {
            base.DXFOutLocal(writer);
            WriteClassBegin(writer, "AcDbBlock1PtParameter");

            writer.WriteVector3(1010, BasePoint);

            writer.Write(170, (short)Messages);
            writer.WriteDefault(91, Connection.Id);
            writer.WriteDefault(301, Connection.Connection);

            writer.Write(171, (short)Messages1);
            writer.WriteDefault(92, Connection1.Id);
            writer.WriteDefault(302, Connection1.Connection);

            writer.Write(93, GripId);
        }

        internal override void DXFInLocal(ICodeValueReader reader)
        {
            base.DXFInLocal(reader);
            ReadClassBegin(reader, "AcDbBlock1PtParameter");

            ReaderAdapter reader2 = new ReaderAdapter(reader);
            reader2.ReadVector3(1010, v => BasePoint = v);

            reader2.Read<short>(170, v => Messages = v);
            reader2.ReadDefault<int>(91, v => Connection.Id = v);
            reader2.ReadDefault<string>(301, v => Connection.Connection = v);

            reader2.Read<short>(171, v => Messages1 = v);
            reader2.ReadDefault<int>(92, v => Connection1.Id = v);
            reader2.ReadDefault<string>(302, v => Connection1.Connection = v);

            reader2.Read<int>(93, v => GripId = v);
            reader2.ExecReadUntil(0, 100, 1001);
        }
        internal override void RuntimeDataIn(ICodeValueReader reader)
        {
            ReaderAdapter reader2 = new ReaderAdapter(reader);
            Vector3 currentPoint = new Vector3();

            base.RuntimeDataIn(reader);

            currentPoint.X = reader2.ReadNow<double>(10);
            currentPoint.Y = reader2.ReadNow<double>(20);
            currentPoint.Z = reader2.ReadNow<double>(30);

            UpdatedPoint = currentPoint;
        }

        internal override void RuntimeDataOut(ICodeValueWriter writer)
        {
            base.RuntimeDataOut(writer);

            writer.Write(10, UpdatedPoint.X);
            writer.Write(20, UpdatedPoint.Y);
            writer.Write(30, UpdatedPoint.Z);
        }
    }
}
