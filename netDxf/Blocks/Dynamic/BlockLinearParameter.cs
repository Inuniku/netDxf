using netDxf.Blocks.Dynamic.Attributes;
using netDxf.Blocks.Dynamic.Property;
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
    [AcadClassName("AcDbBlockLinearParameter", 18597260, 25303744)]
    public class BlockLinearParameter : Block2PtParameter
    {
        public BlockLinearParameter(string codename) : base(codename) { }
        public string Label { get; set; }
        public string LabelDesc { get; set; }
        public string Unk { get; set; }
        public double LabelOffset { get; set; }
        public BlockParamValueSet ParamValueSet { get; set; }
        internal override void DXFOutLocal(ICodeValueWriter writer)
        {
            base.DXFOutLocal(writer);
            WriteClassBegin(writer, "AcDbBlockLinearParameter");

            writer.Write(305, Label);
            writer.Write(306, LabelDesc);
            writer.Write(140, LabelOffset);

            writer.Write(307, Unk);
            writer.Write(96, (int)ParamValueSet.Type);
            writer.Write(141, ParamValueSet.LowerBound);
            writer.Write(142, ParamValueSet.UpperBound);
            writer.Write(143, ParamValueSet.Increment);

            writer.WriteList(175, 144, ParamValueSet.ValueList);
        }

        internal override void DXFInLocal(ICodeValueReader reader)
        {
            base.DXFInLocal(reader);
            ReadClassBegin(reader, "AcDbBlockLinearParameter");
            ReaderAdapter reader2 = new ReaderAdapter(reader);

            reader2.Read<string>(305, v => Label = v);
            reader2.Read<string>(306, v => LabelDesc = v);
            reader2.Read<double>(140, v => LabelOffset = v);

            reader2.Read<string>(307, v => Unk = v);
            ParamValueSet = new BlockParamValueSet();
            reader2.Read<int>(96, v =>ParamValueSet.Type = (ParamValueSetType)v);
            reader2.Read<double>(141, v => ParamValueSet.LowerBound = v);
            reader2.Read<double>(142, v => ParamValueSet.UpperBound = v);
            reader2.Read<double>(143, v => ParamValueSet.Increment = v);
            reader2.ReadList<double>(175, 144, v => ParamValueSet.ValueList = v.ToArray());

            reader2.ExecReadUntil(0, 100, 1001);

        }
    }
}
