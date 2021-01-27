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
    [AcadClassName("AcDbBlockFlipGrip", 575085, 763560)]
    public class BlockFlipGrip : BlockGrip
    {
        public int FlipGripExpressionId { get; set; }
        public Vector3 Orientation { get; set; }

        [ConnectableProperty("UpdatedFlip")]
        internal FlipState UpdatedFlipState { get; set; }
        [ConnectableProperty("Flip")]
        internal FlipState FlipState { get; set; }

        public BlockFlipGrip(string codename) : base(codename) { }
        internal override void RuntimeDataIn(ICodeValueReader reader)
        {
            ReaderAdapter reader2 = new ReaderAdapter(reader);

            base.RuntimeDataIn(reader);
            UpdatedFlipState = (FlipState)reader2.ReadNow<short>(70);
            FlipState = (FlipState)reader2.ReadNow<short>(70);
        }

        internal override void RuntimeDataOut(ICodeValueWriter writer)
        {
            base.RuntimeDataOut(writer);
            writer.Write(70, (short)UpdatedFlipState);
            writer.Write(70, (short)FlipState);
        }

        internal override void DXFOutLocal(ICodeValueWriter writer)
        {
            base.DXFOutLocal(writer);
            WriteClassBegin(writer, "AcDbBlockFlipGrip");

            writer.WriteVector3(140, Orientation, 1);
            writer.Write(93, FlipGripExpressionId);
        }

        internal override void DXFInLocal(ICodeValueReader reader)
        {
            base.DXFInLocal(reader);
            ReadClassBegin(reader, "AcDbBlockFlipGrip");

            ReaderAdapter reader2 = new ReaderAdapter(reader);

            reader2.ReadVector3(140, v => Orientation = v, 1);
            reader2.Read<int>(93, v => FlipGripExpressionId = v);

            reader2.ExecReadUntil(0, 100, 1001);
        }
    }
}
