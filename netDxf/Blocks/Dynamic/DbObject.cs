using netDxf.Blocks.Dynamic.Attributes;
using netDxf.IO;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace netDxf.Blocks.Dynamic
{
    [AcadClassName("AcDbObject")]
    public abstract class DbObject : DxfObject, IReferencedHandles
    {
        public DbObject(string codename) : base(codename) { }

        public string OwnerHandle { get; set; }

        public bool HasReactor { get; set; }
        internal void ReadClassBegin(ICodeValueReader reader, string thisObject)
        {
            Debug.Assert(reader.Code == 100);
            string className = reader.ReadString();
            Debug.Assert(thisObject == className);
            reader.Next();
        }

        internal void WriteClassBegin(ICodeValueWriter writer, string thisObject)
        {
            writer.Write(100, thisObject);
        }

        internal virtual void DXFOutLocal(ICodeValueWriter writer)
        {
            writer.Write(0, CodeName);
            writer.Write(5, Handle);
            /*
            if (xRecord.Reactors.Count > 0)
            {
                this.chunk.Write(102, "{ACAD_REACTORS");
                foreach (DxfObject o in entity.Reactors)
                {
                    Debug.Assert(!string.IsNullOrEmpty(o.Handle), "The handle cannot be null or empty.");
                    this.chunk.Write(330, o.Handle);
                }
                this.chunk.Write(102, "}");
            }*/
            writer.Write(330, Owner.Handle);
        }

        internal virtual void DXFInLocal(ICodeValueReader reader)
        {
            while (reader.Code != 0 && reader.Code != 100)
            {
                switch (reader.Code)
                {
                    case 5:
                        Handle = reader.ReadHex();
                        reader.Next();
                        break;
                    case 330:
                        OwnerHandle = reader.ReadHex();
                        reader.Next();
                        break;
                    case 102:
                        if (reader.ReadString().Equals("{ACAD_REACTORS", StringComparison.OrdinalIgnoreCase))
                        {
                            reader.Next();
                            HasReactor = true;
                            while (reader.Code != 102)
                            {
                                reader.Next();
                            }
                        }
                        reader.Next();
                        break;
                    default:
                        reader.Next();
                        break;
                }
            }
        }

        public virtual IEnumerable<string> GetHardHandles()
        {
            return Enumerable.Empty<string>();
        }

        public virtual void SetHardHandles(IEnumerable<DxfObject> objects)
        { }

        internal virtual IEnumerable<string> GetSoftHandles()
        {
            return Enumerable.Empty<string>();
        }

        internal virtual void SetSoftHandles(IEnumerable<DxfObject> objects)
        { }
        public virtual IEnumerable<DxfObject> GetHardReferences()
        {
            return Enumerable.Empty<DxfObject>();
        }

    }
}
