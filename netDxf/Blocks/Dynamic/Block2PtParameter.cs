using netDxf.Blocks.Dynamic.Attributes;
using netDxf.Blocks.Dynamic.Util;
using netDxf.IO;
using netDxf.Objects;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace netDxf.Blocks.Dynamic
{

    [AcadClassName("AcDbBlock2PtParameter")]
    public abstract class Block2PtParameter : BlockParameter
    {
        public Block2PtParameter(string codename) : base(codename) { }
        
        public int BasePointIndex { get; set; }
        public Vector3 DefinitionPoint1 { get; set; }
        public Vector3 DefinitionPoint2 { get; set; }
        public Vector3 Normal { get => new Vector3(0, 0, -1); }

        [ConnectableProperty("Base", ConnectableVectorType.Normal_And_XY)]
        public Vector3 Point1 { get; set; }
        [ConnectableProperty("End", ConnectableVectorType.Normal_And_XY)]
        public Vector3 Point2 { get; set; }

        [ConnectableProperty("UpdatedBase", ConnectableVectorType.Normal_And_XY)]
        public Vector3 UpdatedPoint1 { get; set; }
        [ConnectableProperty("UpdatedEnd", ConnectableVectorType.Normal_And_XY)]
        public Vector3 UpdatedPoint2 { get; set; }

        [ConnectableProperty("Distance", ConnectableVectorType.Normal)]
        public double Distance
        {
            get => Vector3.Distance(Point1, Point2);
            set => DefinitionPoint2 = Point1 + Direction * (double)Convert.ChangeType(value, typeof(double));
        }

        [ConnectableProperty("UpdatedDistance", ConnectableVectorType.Normal)]
        public double UpdatedDistance
        {
            get => Vector3.Distance(UpdatedPoint1, UpdatedPoint2);
            set => UpdatedPoint2 = UpdatedPoint1 + UpdatedDirection * (double)Convert.ChangeType(value, typeof(double));
        }

        [ConnectableProperty("UpdatedDistance", ConnectableVectorType.XY)]
        public Vector3 UpdatedDifference
        {
            get => UpdatedPoint2 - Point1;
            set => UpdatedPoint2 = Point1 + value;
        }

        [ConnectableProperty("Base", ConnectableVectorType.XY, "Delta")]
        public Vector3 Point1Delta
        {
            get => UpdatedPoint1 - Point1;
            set => UpdatedPoint1 = Point1 + value;
        }

        [ConnectableProperty("End", ConnectableVectorType.XY, "Delta")]
        public Vector3 Point2Delta
        {
            get => UpdatedPoint2 - Point2;
            set => UpdatedPoint2 = Point2 + value;
        }

        [ConnectableProperty("Angle", ConnectableVectorType.Normal)]
        public double Angle
        {
            get => Math.Atan2(Direction.Y, Direction.X);
            set => DefinitionPoint2 = Distance * new Vector3(Math.Cos(value), Math.Sin(value), 0);
        }
        [ConnectableProperty("UpdatedAngle", ConnectableVectorType.Normal)]
        public double UpdatedAngle
        {
            get => Math.Atan2(UpdatedDirection.Y, UpdatedDirection.X);
            set => UpdatedPoint2 = UpdatedDistance * new Vector3(Math.Cos(value), Math.Sin(value), 0);
        }

        private Vector3 Direction => (DefinitionPoint2 - DefinitionPoint1).Normalized();
        private Vector3 UpdatedDirection => (UpdatedPoint2 - UpdatedPoint1).Normalized();

        public int Messages1 { get; set; }
        public BlockConnection Connection1 { get; set; } = new BlockConnection();

        public int Messages2 { get; set; }
        public BlockConnection Connection2 { get; set; } = new BlockConnection();

        public int Messages3 { get; set; }
        public BlockConnection Connection3 { get; set; } = new BlockConnection();

        public int Messages4 { get; set; }
        public BlockConnection Connection4 { get; set; } = new BlockConnection();

        public int MessagesNum { get; set; }

        public int[] GripNodeIds { get; set; }

        public override bool Eval(EvalStep step, BlockEvaluationContext context)
        {
            if (!base.Eval(step, context))
                return false;

            if (step == EvalStep.Execute)
            {
                for(int i = 0; i < GripNodeIds.Length; i++)
                {
                    if(GripNodeIds[i] != 0)
                    {
                        BlockGrip grip = context.EvalGraph.GetNode(GripNodeIds[i]) as BlockGrip;
                        grip.Eval(EvalStep.Initialize, context);
                    }
                }

                Vector3 updatedpoint1 = UpdatedPoint1;

                if(Connection1.IsValid)
                    updatedpoint1.X += (double)Connection1.Evaluate(context);
                if (Connection2.IsValid)
                    updatedpoint1.Y += (double)Connection2.Evaluate(context);

                UpdatedPoint1 = updatedpoint1;


                Vector3 updatedpoint2 = UpdatedPoint2;

                if (Connection3.IsValid)
                    updatedpoint2.X += (double)Connection3.Evaluate(context);
                if (Connection4.IsValid)
                    updatedpoint2.Y += (double)Connection4.Evaluate(context);

                UpdatedPoint2 = updatedpoint2;

                return true;
            }

            if (step == EvalStep.Commit)
            {
                Point1 = UpdatedPoint1;
                Point2 = UpdatedPoint2;
                return true;
            }
            return true;
        }

        internal override void DXFOutLocal(ICodeValueWriter writer)
        {
            base.DXFOutLocal(writer);
            WriteClassBegin(writer, "AcDbBlock2PtParameter");

            writer.WriteVector3(1010, DefinitionPoint1);
            writer.WriteVector3(1011, DefinitionPoint2);
            writer.WriteIntList(170, 91, GripNodeIds);

            writer.Write(171, (short)Messages1);
            writer.WriteDefault(92, Connection1.Id);
            writer.WriteDefault(301, Connection1.Connection);

            writer.Write(172, (short)Messages2);
            writer.WriteDefault(93, Connection2.Id);
            writer.WriteDefault(302, Connection2.Connection);

            writer.Write(173, (short)Messages3);
            writer.WriteDefault(94, Connection3.Id);
            writer.WriteDefault(303, Connection3.Connection);

            writer.Write(174, (short)Messages4);
            writer.WriteDefault(95, Connection4.Id);
            writer.WriteDefault(304, Connection4.Connection);

            writer.Write(177, (short)BasePointIndex);
        }

        internal override void DXFInLocal(ICodeValueReader reader)
        {
            ReaderAdapter reader2 = new ReaderAdapter(reader);

            base.DXFInLocal(reader);
            ReadClassBegin(reader, "AcDbBlock2PtParameter");

            reader2.ReadVector3(1010, v =>  DefinitionPoint1 = v);
            reader2.ReadVector3(1011, v => DefinitionPoint2 = v);
            reader2.ReadList<int>(170, 91, v => GripNodeIds = v.ToArray());

            reader2.Read<short>(171, v => Messages1 = v);
            reader2.ReadDefault<int>(92, v => Connection1.Id = v);
            reader2.ReadDefault<string>(301, v => Connection1.Connection = v);

            reader2.Read<short>(172, v => Messages2 = v);
            reader2.ReadDefault<int>(93, v => Connection2.Id = v);
            reader2.ReadDefault<string>(302, v => Connection2.Connection = v);

            reader2.Read<short>(173, v => Messages3 = v);
            reader2.ReadDefault<int>(94, v => Connection3.Id = v);
            reader2.ReadDefault<string>(303, v => Connection3.Connection = v);

            reader2.Read<short>(174, v => Messages4 = v);
            reader2.ReadDefault<int>(95, v => Connection4.Id = v);
            reader2.ReadDefault<string>(304, v => Connection4.Connection = v);

            reader2.Read<short>(177, v => BasePointIndex = v);

            reader2.ExecReadUntil(0, 100, 1001);

            UpdatedPoint1 = Point1 = DefinitionPoint1;
            UpdatedPoint2 = Point2 = DefinitionPoint2;
        }

        internal override void InitializeRuntimeData()
        {
            base.InitializeRuntimeData();
            UpdatedPoint1 = Point1 = DefinitionPoint1;
            UpdatedPoint2 = Point2 = DefinitionPoint2;
        }

        internal override void RuntimeDataIn(ICodeValueReader reader)
        {
            ReaderAdapter reader2 = new ReaderAdapter(reader);
            Vector3 point = new Vector3();

            base.RuntimeDataIn(reader);

            point.X = reader2.ReadNow<double>(10);
            point.Y = reader2.ReadNow<double>(20);
            point.Z = reader2.ReadNow<double>(30);
            Point1 = point;
            point.X = reader2.ReadNow<double>(10);
            point.Y = reader2.ReadNow<double>(20);
            point.Z = reader2.ReadNow<double>(30);
            Point2 = point;
            point.X = reader2.ReadNow<double>(10);
            point.Y = reader2.ReadNow<double>(20);
            point.Z = reader2.ReadNow<double>(30);

            UpdatedPoint1 = Point1;
            UpdatedPoint2 = Point2;
            // Normal
        }

        internal override void RuntimeDataOut(ICodeValueWriter writer)
        {
            base.RuntimeDataOut(writer);

            writer.Write(10, Point1.X);
            writer.Write(20, Point1.Y);
            writer.Write(30, Point1.Z);

            writer.Write(10, Point2.X);
            writer.Write(20, Point2.Y);
            writer.Write(30, Point2.Z);

            writer.Write(10, Normal.X);
            writer.Write(20, Normal.Y);
            writer.Write(30, Normal.Z);
        }
    }
}
