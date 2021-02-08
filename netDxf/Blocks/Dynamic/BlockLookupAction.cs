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
    [AcadClassName("AcDbBlockLookupAction")]
    public class BlockLookupAction : BlockAction
    {
        public int NumRows { get; set; }
        public int NumCols { get; set; }
        public string[] Table { get; set; }

        public string  Unk1 { get; set; }
        public string Unk2 { get; set; }
        public bool Unk3 { get; set; }
        public int ParameterNodeId { get; set; }
        public int ResType { get; set; }
        public int LookupType { get; set; }
        public bool IsLookupProperty { get; set; }
        public string UnmatchedName { get; set; }
        public bool IsNotReadonly { get; set; }
        public string Connection { get; set; }

        public BlockLookupAction(string codename) : base(codename) { }


        public override bool Eval(EvalStep step, BlockEvaluationContext context)
        {
            return true;
        }

        internal override void DXFOutLocal(ICodeValueWriter writer)
        {
            base.DXFOutLocal(writer);
            WriteClassBegin(writer, "AcDbBlockLookupAction");
            writer.Write(92, NumRows);
            writer.Write(93, NumCols);

            writer.Write(301, Unk1);
            for (int i = 0; i < NumRows * NumCols; i++)
            {
                writer.Write(302, Table[i]);
                
            }
            writer.Write(303, Unk2);

            writer.Write(94, ParameterNodeId);
            writer.Write(95, ResType);
            writer.Write(96, LookupType);

            writer.Write(282, (short)(IsLookupProperty ? 1: 0));
            writer.Write(305, UnmatchedName);
            writer.Write(281, (short)(IsNotReadonly ? 1 : 0));
            writer.Write(304, Connection);
            writer.Write(280, (short)(Unk3 ? 1 : 0));
        }

        internal override void DXFInLocal(ICodeValueReader reader)
        {
            base.DXFInLocal(reader);
            ReadClassBegin(reader, "AcDbBlockLookupAction");

            ReaderAdapter reader2 = new ReaderAdapter(reader);

            reader2.Read<int>(92, v => NumRows = v);
            reader2.Read<int>(93, v => NumCols = v);

            reader2.Read<string>(301, v => Unk1 = v);
            reader2.WhenCode(302, r => {
                Table = new string[NumRows * NumCols];
                for (int i = 0; i < NumRows * NumCols; i++)
                {
                    Table[i] = r.ReadString();
                    r.Next();
                }});
            reader2.Read<string>(303, v => Unk2 = v);
            reader2.Read<int>(94, v => ParameterNodeId = v);
            reader2.Read<int>(95, v => ResType = v);
            reader2.Read<int>(96, v => LookupType = v);

            reader2.Read<short>(282, v => IsLookupProperty = v != 0);
            reader2.Read<string>(305, v => UnmatchedName = v);
            reader2.Read<short>(281, v => IsNotReadonly = v != 0);
            reader2.Read<string>(304, v => Connection = v);
            reader2.Read<short>(280, v => Unk3 = v != 0);

            reader2.ExecReadUntil(0, 100, 1001); 
        }
    }
}