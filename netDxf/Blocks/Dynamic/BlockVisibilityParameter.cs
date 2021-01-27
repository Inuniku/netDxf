using netDxf.Blocks.Dynamic.Attributes;
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
    public class VisibilityState
    {
        public string Name { get; set; }
        public string[] Selection1 { get; set; }
        public string[] Selection2 { get; set; }
    }

    [AcadClassName("AcDbBlockVisibilityParameter", 135625452, 184556386)]
    public class BlockVisibilityParameter : Block1PtParameter
    {
        public BlockVisibilityParameter(string codename) : base(codename) { }
        public bool Unk { get; set; }
        public int Unk1 { get; set; }
        public string Label { get; set; }
        public string LabelDesc { get; set; }
        public string[] AllObjects { get; set; }
        public VisibilityState[] States { get; set; }

        [ConnectableProperty("VisibilityState")]
        public string VisibilityState { get; set; }

        public override bool Eval(EvalStep step, BlockEvaluationContext context)
        {
            if (!base.Eval(step, context))
                return false;


            if (step == EvalStep.Update)
            {
            }

            return true;
        }

        internal override void RuntimeDataIn(ICodeValueReader reader)
        {
            base.RuntimeDataIn(reader);
            ReaderAdapter reader2 = new ReaderAdapter(reader);

            VisibilityState = reader2.ReadNow<string>(1);
        }

        internal override void RuntimeDataOut(ICodeValueWriter writer)
        {
            base.RuntimeDataOut(writer);
            writer.Write(1, VisibilityState);
        }

        internal override void DXFOutLocal(ICodeValueWriter writer)
        {
            base.DXFOutLocal(writer);
            WriteClassBegin(writer, "AcDbBlockVisibilityParameter");

            writer.WriteBool(281, Unk);
            writer.Write(301, Label);
            writer.Write(302, LabelDesc);
            writer.Write(91, Unk1);

            writer.WriteList(93, 331, AllObjects);

            writer.Write(92, States.Length);
            for(int i = 0; i < States.Length; i++)
            {
                var element = States[i];
                writer.Write(303, element.Name);
                writer.WriteList(94, 332, element.Selection1);
                writer.WriteList(95, 333, element.Selection2);
            }
        }
        internal override void DXFInLocal(ICodeValueReader reader)
        {
            base.DXFInLocal(reader);
            ReadClassBegin(reader, "AcDbBlockVisibilityParameter");
            // TODO:
            Vector3 basePoint = new Vector3();
            while (reader.Code != 0 && reader.Code != 100)
            {
                switch (reader.Code)
                {
                    case 91:
                        Unk1 = reader.ReadInt();
                        reader.Next();
                        break;
                    case 92:
                        States = ReadStates(reader);
                        break;
                    case 93:
                        AllObjects = ReadAllObjects(reader, 93);
                        break;
                    case 301:
                        Label = reader.ReadString();
                        reader.Next();
                        break;
                    case 302:
                        LabelDesc = reader.ReadString();
                        reader.Next();
                        break;
                    case 281:
                        Unk = reader.ReadShort() != 0;
                        reader.Next();
                        break;
                    default:
                        Debug.Assert(false);
                        reader.Next();
                        break;
                }
            }

            BasePoint = basePoint;
        }

        private VisibilityState ReadState(ICodeValueReader reader)
        {
            VisibilityState result = new VisibilityState();
            bool isDone = false;

            bool readName = false;
            bool readSel1 = false;
            bool readSel2 = false;

            while (!isDone)
            {
                switch (reader.Code)
                {
                    case 303:
                        if(readName)
                        {
                            isDone = true;
                            break;
                        }
                        result.Name = reader.ReadString();
                        readName = true;
                        reader.Next();
                        break;
                    case 94:
                        if (readSel1)
                        {
                            isDone = true;
                            break;
                        }
                        result.Selection1 = ReadAllObjects(reader, 94);
                        readSel1 = true;
                        break;
                    case 95:
                        if (readSel2)
                        {
                            isDone = true;
                            break;
                        }
                        readSel2 = true;
                        result.Selection2 = ReadAllObjects(reader, 95);
                        break;
                    default:
                        isDone = true;
                        break;
                }
            }
            return result;
        }

        private VisibilityState[] ReadStates(ICodeValueReader reader)
        {
            List<VisibilityState> result = new List<VisibilityState>();
            Debug.Assert(reader.Code == 92);
            int numObjects = reader.ReadInt();
            reader.Next();
            for(int i = 0; i < numObjects; i++)
            {
                VisibilityState visibilityState = ReadState(reader);
                result.Add(visibilityState);
            }    
            Debug.Assert(numObjects == result.Count);

            return result.ToArray();
        }

        private string[] ReadAllObjects(ICodeValueReader reader, int readCode)
        {
            List<string> result = new List<string>();
            Debug.Assert(reader.Code == readCode);
            int numObjects = reader.ReadInt();
            reader.Next();

            for (int i = 0; i < numObjects; i++)
            {
                result.Add(reader.ReadHex());
                reader.Next();
            }
            Debug.Assert(numObjects == result.Count);

            return result.ToArray();
        }
    }
}
