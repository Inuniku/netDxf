﻿using netDxf.Blocks.Dynamic.Attributes;
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
    [AcadClassName("AcDbBlockMoveAction", 1553528, 2094574)]
    public class BlockMoveAction : BlockAction
    {
        public BlockMoveAction(string codename) : base(codename)
        {}
        BlockConnection MoveXConnection { get; } = new BlockConnection();
        BlockConnection MoveYConnection { get; } = new BlockConnection();

        public double DistanceMultiplier { get; set; }
        public double AngleOffset { get; set; }
        public double CurrentAngleOffset { get; set; }
        public int XYScaleType { get; set; }

        public override bool Eval(EvalStep step, BlockEvaluationContext context)
        {
            if (!base.Eval(step, context))
                return false;


            if (step == EvalStep.Update)
            {
                double deltaX = (double)MoveXConnection.Evaluate(context);
                double deltaY = (double)MoveYConnection.Evaluate(context);

                Debug.Write("Moving");
            }

            return true;
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
            WriteClassBegin(writer, "AcDbBlockMoveAction");

            writer.Write(92,    MoveXConnection.Id);
            writer.Write(301,   MoveXConnection.Connection);
            writer.Write(93,    MoveYConnection.Id);
            writer.Write(302,   MoveYConnection.Connection);

            writer.Write(140, DistanceMultiplier);
            writer.Write(141, AngleOffset);
            writer.Write(280, (short)XYScaleType);
        }

        internal override void DXFInLocal(ICodeValueReader reader)
        {
            base.DXFInLocal(reader);
            ReadClassBegin(reader, "AcDbBlockMoveAction");

            ReaderAdapter reader2 = new ReaderAdapter(reader);

            reader2.Read<int>(92, v => MoveXConnection.Id = v);
            reader2.Read<int>(93, v => MoveYConnection.Id = v);

            reader2.Read<string>(301, v => MoveXConnection.Connection = v);
            reader2.Read<string>(302, v => MoveYConnection.Connection = v);

            reader2.Read<double>(140, v => DistanceMultiplier = v);
            reader2.Read<double>(141, v => AngleOffset = v);
            reader2.Read<short>(280, v => XYScaleType = v);

            reader2.ExecReadUntil(0, 100, 1001);
        }
    }
}
