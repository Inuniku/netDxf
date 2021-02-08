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
    public class Block1PtParameter : BlockParameter
    {
        public Block1PtParameter(string codename) : base(codename) { }

        public Vector3 DefinitionPoint { get; set; }


        [ConnectableProperty("", ConnectableVectorType.XY)]
        public Vector3 Point { get; set; }


        [ConnectableProperty("Updated", ConnectableVectorType.XY)]
        public Vector3 UpdatedPoint { get; set; }


        [ConnectableProperty("", ConnectableVectorType.XY, "Delta")]
        public Vector3 PointDelta
        {
            get => UpdatedPoint - Point;
            set => UpdatedPoint = Point + value;
        }

        public int Messages { get; set; }
        public BlockConnection UpdatedPointMessageX { get; private set; } = new BlockConnection();
        public int Messages1 { get; set; }
        public BlockConnection UpdatedPointMessageY { get; private set; } = new BlockConnection();
        public int GripId { get; set; }

        public override bool Eval(EvalStep step, BlockEvaluationContext context)
        {
            if (!base.Eval(step, context))
                return false;

            if (step == EvalStep.Execute)
            {
                if(GripId != 0)
                {
                    BlockGrip grip = context.EvalGraph.GetNode(GripId) as BlockGrip;
                    grip.Eval(EvalStep.Execute, context);
                }

                Vector3 updatedpoint = UpdatedPoint;
                if (UpdatedPointMessageX.IsValid)
                    updatedpoint.X += (double)UpdatedPointMessageX.Evaluate(context);
                if (UpdatedPointMessageY.IsValid)
                    updatedpoint.Y += (double)UpdatedPointMessageY.Evaluate(context);
                UpdatedPoint = updatedpoint;

                return true;
            }

            if (step == EvalStep.Commit)
            {
                Point = UpdatedPoint;
                return true;
            }
            return true;
        }

        internal override void DXFOutLocal(ICodeValueWriter writer)
        {
            base.DXFOutLocal(writer);
            WriteClassBegin(writer, "AcDbBlock1PtParameter");

            writer.WriteVector3(1010, DefinitionPoint);

            writer.Write(170, (short)Messages);
            writer.WriteDefault(91, UpdatedPointMessageX.Id);
            writer.WriteDefault(301, UpdatedPointMessageX.Connection);

            writer.Write(171, (short)Messages1);
            writer.WriteDefault(92, UpdatedPointMessageY.Id);
            writer.WriteDefault(302, UpdatedPointMessageY.Connection);

            writer.Write(93, GripId);
        }
        internal override void DXFInLocal(ICodeValueReader reader)
        {
            base.DXFInLocal(reader);
            ReadClassBegin(reader, "AcDbBlock1PtParameter");

            ReaderAdapter reader2 = new ReaderAdapter(reader);
            reader2.ReadVector3(1010, v => DefinitionPoint = v);

            reader2.Read<short>(170, v => Messages = v);
            reader2.ReadDefault<int>(91, v => UpdatedPointMessageX.Id = v);
            reader2.ReadDefault<string>(301, v => UpdatedPointMessageX.Connection = v);

            reader2.Read<short>(171, v => Messages1 = v);
            reader2.ReadDefault<int>(92, v => UpdatedPointMessageY.Id = v);
            reader2.ReadDefault<string>(302, v => UpdatedPointMessageY.Connection = v);

            reader2.Read<int>(93, v => GripId = v);
            reader2.ExecReadUntil(0, 100, 1001);

            UpdatedPoint = Point = DefinitionPoint;
        }

        internal override void InitializeRuntimeData()
        {
            base.InitializeRuntimeData();
            UpdatedPoint = Point = DefinitionPoint;
        }

        internal override void RuntimeDataIn(ICodeValueReader reader)
        {
            ReaderAdapter reader2 = new ReaderAdapter(reader);
            Vector3 currentPoint = new Vector3();

            base.RuntimeDataIn(reader);

            currentPoint.X = reader2.ReadNow<double>(10);
            currentPoint.Y = reader2.ReadNow<double>(20);
            currentPoint.Z = reader2.ReadNow<double>(30);

            UpdatedPoint = Point = currentPoint;
        }

        internal override void RuntimeDataOut(ICodeValueWriter writer)
        {
            base.RuntimeDataOut(writer);

            writer.Write(10, Point.X);
            writer.Write(20, Point.Y);
            writer.Write(30, Point.Z);
        }
    }
}
