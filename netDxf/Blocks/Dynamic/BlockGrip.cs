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
    [AcadClassName("AcDbBlockGrip")]
    public class BlockGrip : BlockElement
    {
        public BlockGrip(string codename) : base(codename) {
            GripXConnection.Connection = "Value";
            GripYConnection.Connection = "Value";
        }

        public BlockConnection GripXConnection { get; } = new BlockConnection();
        public BlockConnection GripYConnection { get; } = new BlockConnection();

        public Vector3 DefinitionPoint { get; set; }
       
        [ConnectableProperty("", ConnectableVectorType.XY)]
        public Vector3 Location { get; set; }
        public Vector3 UpdatedLocation { get; set; }
        [ConnectableProperty("Displacement", ConnectableVectorType.XY)]
        public Vector3 LocationDelta { 
            get => UpdatedLocation - Location;
            set => UpdatedLocation = Location + value; 
        }

        public bool Cycling { get; set; }
        public int CyclingWeight { get; set; }

        public override bool Eval(EvalStep step, BlockEvaluationContext context)
        {
            if (!base.Eval(step, context))
                return false;

            if (step == EvalStep.Initialize || step == EvalStep.Execute)
            {
                BlockGripExpr xPression = context.EvalGraph.GetNode(GripXConnection.Id) as BlockGripExpr;
                BlockGripExpr yPression = context.EvalGraph.GetNode(GripYConnection.Id) as BlockGripExpr;
                Vector3 location = new Vector3();
                
                location.X = (double)xPression.GripConnection.Evaluate(context);
                location.Y = (double)yPression.GripConnection.Evaluate(context);
                return true;
            }

            if (step == EvalStep.Commit)
            { 
                Location = UpdatedLocation;
                return true;
            }

            return true;
        }

        internal override void DXFOutLocal(ICodeValueWriter writer)
        {
            base.DXFOutLocal(writer);
            WriteClassBegin(writer, "AcDbBlockGrip");

            writer.Write(91, GripXConnection.Id);
            writer.Write(92, GripYConnection.Id);

            writer.WriteVector3(1010, DefinitionPoint);

            writer.WriteBool(280, Cycling);
            writer.Write(93, CyclingWeight);
        }

        internal override void DXFInLocal(ICodeValueReader reader)
        {
            ReaderAdapter reader2 = new ReaderAdapter(reader);

            base.DXFInLocal(reader);
            ReadClassBegin(reader, "AcDbBlockGrip");

            reader2.Read<int>(91, v => GripXConnection.Id = v);
            reader2.Read<int>(92, v => GripYConnection.Id = v);

            reader2.ReadVector3(1010, v => DefinitionPoint = v);

            reader2.Read<short>(280, v => Cycling = v != 0);
            reader2.Read<int> (93, v => CyclingWeight = v);

            reader2.ExecReadUntil(0, 100, 1001);
        }
    }
}
