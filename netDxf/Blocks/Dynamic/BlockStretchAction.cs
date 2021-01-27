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
    public class NodeSelection
    {
        public int NodeId { get; set; }
        public int[] Indices { get; set; }
    }

    public class StretchSelection
    {
        public string StretchObject { get; set; }
        public int[] Indices { get; set; }
    }

    [AcadClassName("AcDbBlockStretchAction", 6895636, 9291323)]
    public class BlockStretchAction : BlockAction
    {
        public BlockStretchAction(string codename) : base(codename) { }
        public BlockConnection StretchX { get; } = new BlockConnection();
        public BlockConnection StretchY { get; } = new BlockConnection();

        public Vector2[] StretchFrame { get; set; }
        public StretchSelection[] StretchSelection { get; set; }
        public NodeSelection[] NodeSelection { get; set; }
        public double DistanceMultiplier { get; set; }
        public double AngleOffset { get; set; }
        public double CurrentAngleOffset { get; set; }
        public int XYScaleType { get; set; }


        public override bool Eval(EvalStep step, BlockEvaluationContext context)
        {
            if (step == EvalStep.Initialize || step == EvalStep.Abort)
            {
                return true;
            }

            if (step == EvalStep.Update)
            {
                double deltaX = (double)StretchX.Evaluate(context);
                double deltaY = (double)StretchY.Evaluate(context);

                Debug.Write("Stretching" + deltaX + "," +deltaY);
                return true;
            }
            return true;
        }

        internal StretchSelection[] ReadStretchSelection(ICodeValueReader reader)
        {
            Debug.Assert(reader.Code == 73);
            int numElements = reader.ReadShort();
            reader.Next();

            List<StretchSelection> result = new List<StretchSelection>(numElements);
            for (int i = 0; i < numElements; i++)
            {
                StretchSelection element = new StretchSelection();
                Debug.Assert(reader.Code == 331);
                element.StretchObject = reader.ReadHex();
                reader.Next();

                Debug.Assert(reader.Code == 74);
                element.Indices = reader.ReadIntList(74, 94);

                result.Add(element);
            }
            return result.ToArray();
        }
        internal NodeSelection[] ReadNodeSelection(ICodeValueReader reader)
        {
            Debug.Assert(reader.Code == 75);
            int numElements = reader.ReadShort();
            reader.Next();


            List<NodeSelection> result = new List<NodeSelection>(numElements);
            for (int i = 0; i < numElements; i++)
            {
                NodeSelection element = new NodeSelection();
                Debug.Assert(reader.Code == 95);
                element.NodeId = reader.ReadInt();
                reader.Next();

                Debug.Assert(reader.Code == 76);
                element.Indices = reader.ReadIntList(76, 94);

                result.Add(element);
            }
            return result.ToArray();
        }
        internal override void RuntimeDataIn(ICodeValueReader reader)
        {
            ReaderAdapter reader2 = new ReaderAdapter(reader);

            base.RuntimeDataIn(reader);

            CurrentAngleOffset = reader2.ReadNow<double>(40);
        }

        internal override void RuntimeDataOut(ICodeValueWriter writer)
        {
            base.RuntimeDataOut(writer);

            writer.Write(40, CurrentAngleOffset);
        }

        internal override void DXFOutLocal(ICodeValueWriter writer)
        {
            base.DXFOutLocal(writer);
            WriteClassBegin(writer, "AcDbBlockStretchAction");

            writer.Write(92, StretchX.Id);
            writer.Write(301, StretchX.Connection);
            writer.Write(93, StretchY.Id);
            writer.Write(302, StretchY.Connection);

            writer.WriteVector2List(72, 1011, StretchFrame);

            writer.Write(73, (short)StretchSelection.Length);
            for (int i = 0; i < StretchSelection.Length; i++)
            {
                var currentItem = StretchSelection[i];
                writer.Write(331, currentItem.StretchObject);
                writer.WriteIntList(74, 94, currentItem.Indices);
            }
            writer.Write(75, (short)NodeSelection.Length);
            for (int i = 0; i < NodeSelection.Length; i++)
            {
                var currentItem = NodeSelection[i];
                writer.Write(95, currentItem.NodeId);
                writer.WriteIntList(76, 94, currentItem.Indices);
            }
            writer.Write(140, DistanceMultiplier);
            writer.Write(141, AngleOffset);
            writer.Write(280, (short)XYScaleType);
        }

        internal override void DXFInLocal(ICodeValueReader reader)
        {
            base.DXFInLocal(reader);
            ReadClassBegin(reader, "AcDbBlockStretchAction");
            ReaderAdapter reader2 = new ReaderAdapter(reader);

            reader2.Read<int>(92, v => StretchX.Id = v);
            reader2.Read<string>(301, v => StretchX.Connection = v);
            reader2.Read<int>(93, v => StretchY.Id = v);
            reader2.Read<string>(302, v => StretchY.Connection = v);

            reader2.ReadVector3(1010, v => Label = v);

            reader2.WhenCode(72,
                r => StretchFrame = reader.ReadPoint2List(72, 1011, 1021));
            reader2.WhenCode(73,
                r => StretchSelection = ReadStretchSelection(r));
            reader2.WhenCode(75,
                r => NodeSelection = ReadNodeSelection(r));

            reader2.Read<double>(140, v => DistanceMultiplier = v);
            reader2.Read<double>(141, v => AngleOffset = v);
            reader2.Read<short>(280, v => XYScaleType = v);

            reader2.ExecReadUntil(0, 100, 1001);
        }
    }
}