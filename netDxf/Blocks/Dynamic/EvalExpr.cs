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
    [AcadClassName("AcDbEvalExpr")]
    public abstract class EvalExpr : DbObject
    {
        public EvalExpr(string codename) : base(codename) { }
        public int NodeId { get; internal set; }
        public int VersionMajor { get; internal set; }
        public int VersionMinor { get; internal set; }
        public object DefaultValue { get; internal set; }
        public Int16 Rt { get; set; }

        public virtual bool Eval(EvalStep step, BlockEvaluationContext context) => true;

        internal override void DXFOutLocal(ICodeValueWriter writer)
        {
            base.DXFOutLocal(writer);
            WriteClassBegin(writer, "AcDbEvalExpr");

            writer.Write(90, NodeId);
            writer.Write(98, VersionMinor);
            writer.Write(99, VersionMajor);
            if(Rt != -9999)
            {
                writer.Write(1, "");
                writer.Write(70, Rt);

                if (DefaultValue is short)
                    writer.Write(70, DefaultValue);
                else if (DefaultValue is double)
                    writer.Write(40, DefaultValue);
                else
                    throw new NotImplementedException();
            }
        }

        internal override void DXFInLocal(ICodeValueReader reader)
        {
            base.DXFInLocal(reader);
            ReadClassBegin(reader, "AcDbEvalExpr");
            Rt = -9999;

            ReaderAdapter reader2 = new ReaderAdapter(reader);
            reader2.Read<int>(90, v => NodeId = v);
            reader2.Read<int>(98, v => VersionMinor = v);
            reader2.Read<int>(99, v => VersionMajor = v);

            reader2.WhenCode(1, r =>
            {
                r.Next();
                
                Rt = r.ReadShort();
                r.Next();
                if (Rt == 40)
                    DefaultValue = r.ReadDouble();
                else if (Rt == 70)
                    DefaultValue = r.ReadShort();
                else
                    throw new NotImplementedException();
                r.Next();
            });
            reader2.ExecReadUntil(0, 100, 1001);
        }

        internal virtual void InitializeRuntimeData()
        {}

        internal virtual void RuntimeDataIn(ICodeValueReader reader)
        {
            DxfClassRegistry.GetXRecordClassIdentifier(GetType(), out int id1, out int id2);
            ReaderAdapter reader2 = new ReaderAdapter(reader);
            Debug.Assert(reader2.ReadNow<int>(1071) == id1);
            Debug.Assert(reader2.ReadNow<int>(1071) == id2);
            reader2.ReadNow<short>(70);
            reader2.ReadNow<short>(70);

        }

        internal virtual void RuntimeDataOut(ICodeValueWriter writer)
        {
            DxfClassRegistry.GetXRecordClassIdentifier(GetType(), out int id1, out int id2);
            writer.Write(1071, id1);
            writer.Write(1071, id2);
            writer.Write(70, (short)25);
            writer.Write(70, (short)104);
        }
    }
}
