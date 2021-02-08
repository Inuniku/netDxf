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

            if (step == EvalStep.Execute)
            {
                FlipState flip = (FlipState)FlipConnection.Evaluate(context);
                FlipState updateFlip = (FlipState)UpdateFlipConnection.Evaluate(context);

                if(flip != updateFlip)
                {
                    //TODO Vector2 Only
                    Vector3 basePos = (Vector3)UpdateBaseConnection.Evaluate(context);
                    Vector3 endPos = (Vector3)UpdateEndConnection.Evaluate(context);

                    //Vector2 basePos2d = new Vector2(basePos.X, basePos.Y);
                    //Vector2 endPos2d = new Vector2(endPos.X, endPos.Y);

                    //Step 01. Translate to Unit space
                    Vector3 dirX = (endPos - basePos).Normalized();
                    Vector3 dirY = Vector3.CrossProduct(Vector3.UnitZ, dirX);
                    Vector3 dirZ = Vector3.UnitZ;
                    Matrix4 toObjectSpace = GeometryUtils.ToObjectSpaceTransform(basePos, dirX, dirY, dirZ);
                   // Matrix4 toUnitSpace = GeometryUtils.ToUnitSpaceTransform(basePos, dirX, dirY);
                    Matrix4 reflect = new Matrix4(1, 0, 0, 0, 0, -1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1);

                    Matrix4 flipTransform = toObjectSpace * reflect * toObjectSpace.Inverse();
                    //Matrix3 flipMatrix = GeometryUtils.FlipMatrix2D(basePos2d, endPos2d, out Vector3 translation);
                    context.TransformRepresentationBy(Selection, flipTransform);
                }
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