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
    [AcadClassName("AcDbSortentsTable")]
    public class SortentsTable : DbObject
    {
        public string OwnerBlockId { get; set; }
        public Dictionary<string, string> Dictionary { get; } = new Dictionary<string, string>();

        public SortentsTable(string codename) : base(codename)
        {}
        internal override void DXFOutLocal(ICodeValueWriter writer)
        {
            base.DXFOutLocal(writer);
            WriteClassBegin(writer, "AcDbSortentsTable");
            foreach(var entry in Dictionary)
            {
                writer.Write(331, entry.Key);
                writer.Write(5, entry.Value);
            }

            writer.Write(330, OwnerBlockId);
        }

        internal override void DXFInLocal(ICodeValueReader reader)
        {
            base.DXFInLocal(reader);
            ReadClassBegin(reader, "AcDbSortentsTable");
            ReaderAdapter reader2 = new ReaderAdapter(reader);

            reader2.Read<string>(330, v => OwnerBlockId = v);
            reader2.WhenCode(331, r =>
            {
                while(r.Code == 331)
                {
                    string first = r.ReadString();
                    r.Next(); Debug.Assert(r.Code == 5);
                    string second = r.ReadString();
                    r.Next();
                    Dictionary.Add(first, second);
                }
            });

            reader2.ExecReadUntil(0, 100, 1001);
        }
        public override void SetSoftHandles(Queue<string> referencedHandles, bool includeSelf = false)
        {
            base.SetSoftHandles(referencedHandles, includeSelf);

            int numEntries = Dictionary.Count;
            Dictionary.Clear();
            for (int i = 0; i < numEntries; i++)
            {
                var key = referencedHandles.Dequeue();
                var value = referencedHandles.Dequeue();
                Dictionary.Add(key, value);
            }
        }

        public override void GetSoftHandles(Queue<string> result, bool includeSelf = false)
        {
            base.GetSoftHandles(result, includeSelf);
            foreach(var entry in Dictionary)
            {
                result.Enqueue(entry.Key);
                result.Enqueue(entry.Value);
            }
        }

        /*public override object Clone()
        {
            SortentsTable clone = new SortentsTable(CodeName);
            clone.Dictionary = new Dictionary<string, string>(Dictionary);
            return clone;
        }*/
    }
}
