using netDxf.Blocks.Dynamic.Attributes;
using netDxf.Blocks.Dynamic.IO;
using netDxf.IO;
using netDxf.Objects;
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
    public abstract class DbObject : DxfObject, ICloneable
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



        public virtual void SetSoftHandles(Queue<string> referencedHandles, bool includeSelf = false)
        {
            if (includeSelf)
                OwnerHandle = referencedHandles.Dequeue();
        }


        public virtual void GetSoftHandles(Queue<string> result, bool includeSelf = false)
        {
            if (includeSelf)
                result.Enqueue(OwnerHandle);
        }


        public virtual void GetHardHandles(Queue<string> result, bool includeSelf = false)
        {
            if (includeSelf)
                result.Enqueue(Handle);
        }

        public virtual void SetHardObjects(Queue<DxfObject> ownedObjects, bool includeSelf = false)
        {
            if (includeSelf)
            {
                Handle = ownedObjects.Dequeue().Handle;
            }
        }

        public virtual void GetHardReferences(Queue<DxfObject> result, bool includeSelf = false)
        {
            if (includeSelf)
                result.Enqueue(this);
        }

        public object Clone()
        {
            return InternalClone(null);
        }
        
        internal object InternalClone(CloneEntry[] clonemap)
        {
            // Clone also all owned items
            Queue<DxfObject> ownedObjects = new Queue<DxfObject>();
            Queue<DxfObject> clonedObjects = new Queue<DxfObject>();
            GetHardReferences(ownedObjects);

            foreach (var ownedObject in ownedObjects)
            {
                DxfObject clonedObject;
                if (ownedObject is XRecord record)
                {
                    XRecord clonedXRecord = new XRecord()
                    {
                        Flags = record.Flags,
                        OwnerHandle = record.OwnerHandle
                    };

                    foreach (XRecordEntry entry in record.Entries)
                    {
                        clonedXRecord.Entries.Add(new XRecordEntry(entry.Code, entry.Value));
                    }
                    clonedObject = clonedXRecord;
                }
                else
                {
                    DbObject clonedDbObject = (DbObject)((DbObject)ownedObject).__Clone();
                    clonedDbObject.Owner = null;
                    clonedDbObject.Handle = null;
                    clonedObject = clonedDbObject;
                }
                clonedObjects.Enqueue(clonedObject);
            }

            DbObject copy = (DbObject)__Clone();
   
            // Clone this object an all data (still points to old references)
            /*if (Document != null)
            {
                for (int i = 0; i < clonedObjects.Count; i++)
                {
                    clonedObjects.ElementAt(i).Handle = Document.NumHandles.ToString("X");
                    Document.NumHandles++;
                }

                copy.Handle = Document.NumHandles.ToString("X");
                Document.NumHandles++;
            }*/

            copy.SetHardObjects(clonedObjects);
            copy.Owner = null;
            copy.OwnerHandle = null;
            copy.Handle = null;

            /*
            Document.NumHandles = copy.AssignHandle(Document.NumHandles);
           
            Dictionary<string, string> clonedMap = new Dictionary<string, string>();
            for(int i = 0; i < clonedObjects.Count; i++)
            {
                clonedMap[ownedObjects.ElementAt(i).Handle] = clonedObjects.ElementAt(i).Handle;
            }
            clonedMap.Add(Handle, copy.Handle);*/

            // Update all potentially cloned pointers
            /*Queue<string> pointer = new Queue<string>();
            Queue<string> updatedPointer = new Queue<string>();
            copy.GetSoftHandles(pointer, false);
            

            for (int i = 0; i < pointer.Count; i++)
            { 
                foreach(var entry in clonemap)
                {

                }
            }

            copy.SetSoftHandles(pointer, false);*/
            return copy;
        }

        internal object __Clone()
        {
            var constInfo = GetType().GetConstructor(new Type[] { typeof(string) });
            DbObject copy = constInfo.Invoke(new object[] { CodeName }) as DbObject;
            Debug.Assert(copy != null);

            CopyReaderWriter copyBuffer = new CopyReaderWriter(h => Document.GetObjectByHandle(h));
            DXFOutLocal(copyBuffer);
            copyBuffer.Rewind();
            copy.DXFInLocal(copyBuffer);
            return copy;
        }
    }
}
