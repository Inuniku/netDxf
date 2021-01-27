using netDxf.Blocks.Dynamic.Attributes;
using netDxf.Blocks.Dynamic.Property;
using netDxf.Blocks.Dynamic.Util;
using netDxf.IO;
using netDxf.Objects;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace netDxf.Blocks.Dynamic
{
    [AcadClassName("AcDbBlockXYParameter", 2550941,   3467982 )]
    public class BlockXYParameter : Block2PtParameter
    {
        public BlockXYParameter(string codename) : base(codename) { }
        public string YLabel { get; set; }
        public string XLabel { get; set; }
        public string YLabelDesc { get; set; }
        public string XLabelDesc { get; set; }
        public string Unk1 { get; set; }
        public string Unk2 { get; set; }
        public double XLabelOffset { get; set; }
        public double YLabelOffset { get; set; }
        public double LabelOffset { get; set; }
        public BlockParamValueSet ParamValueSetX { get; set; }
        public BlockParamValueSet ParamValueSetY { get; set; }


        internal override void RuntimeDataIn(ICodeValueReader reader)
        {
            base.RuntimeDataIn(reader);

           // VisibilityState = reader2.ReadNow<string>(1);
        }

        internal override void RuntimeDataOut(ICodeValueWriter writer)
        {
            base.RuntimeDataOut(writer);
            //writer.Write(1, VisibilityState);
        }

        internal override void DXFOutLocal(ICodeValueWriter writer)
        {
            base.DXFOutLocal(writer);
            WriteClassBegin(writer, "AcDbBlockXYParameter");

            writer.Write(305, YLabel);
            writer.Write(306, XLabel);
            writer.Write(307, YLabelDesc);
            writer.Write(308, XLabelDesc);

            writer.Write(141, XLabelOffset);
            writer.Write(140, YLabelOffset);

            writer.Write(309, Unk1);
            writer.Write(97, (int)ParamValueSetY.Type);
            writer.Write(146, ParamValueSetY.LowerBound);
            writer.Write(147, ParamValueSetY.UpperBound);
            writer.Write(148, ParamValueSetY.Increment);
            writer.WriteList(176, 149, ParamValueSetY.ValueList);


            writer.Write(410, Unk1);
            writer.Write(96, (int)ParamValueSetY.Type);
            writer.Write(142, ParamValueSetY.LowerBound);
            writer.Write(143, ParamValueSetY.UpperBound);
            writer.Write(144, ParamValueSetY.Increment);
            writer.WriteList(175, 148, ParamValueSetY.ValueList);
        }

        internal override void DXFInLocal(ICodeValueReader reader)
        {
            base.DXFInLocal(reader);
            ReadClassBegin(reader, "AcDbBlockXYParameter");
            ReaderAdapter reader2 = new ReaderAdapter(reader);

            reader2.Read<string>(305, v => YLabel = v);
            reader2.Read<string>(306, v => XLabel = v);
            reader2.Read<string>(307, v => YLabelDesc = v);
            reader2.Read<string>(308, v => XLabelDesc = v);

            reader2.Read<double>(141, v => XLabelOffset = v);
            reader2.Read<double>(140, v => YLabelOffset = v);

            reader2.Read<string>(309, v => Unk1 = v);
            ParamValueSetY = new BlockParamValueSet();
            reader2.Read<int>(97, v => ParamValueSetY.Type = (ParamValueSetType)v);
            reader2.Read<double>(146, v => ParamValueSetY.LowerBound = v);
            reader2.Read<double>(147, v => ParamValueSetY.UpperBound = v);
            reader2.Read<double>(148, v => ParamValueSetY.Increment = v);
            reader2.ReadList<double>(176, 149, v => ParamValueSetY.ValueList = v.ToArray());

            reader2.Read<string>(410, v => Unk2 = v);
            ParamValueSetX = new BlockParamValueSet();
            reader2.Read<int>(96, v => ParamValueSetX.Type = (ParamValueSetType)v);
            reader2.Read<double>(142, v => ParamValueSetX.LowerBound = v);
            reader2.Read<double>(143, v => ParamValueSetX.UpperBound = v);
            reader2.Read<double>(144, v => ParamValueSetX.Increment = v);
            reader2.ReadList<double>(175, 148, v => ParamValueSetX.ValueList = v.ToArray());

            reader2.ExecReadUntil(0, 100, 1001);
        }

    }
}
