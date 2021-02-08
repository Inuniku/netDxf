using netDxf.Blocks.Dynamic;
using netDxf.Blocks.Dynamic.Attributes;
using netDxf.Blocks.Dynamic.IO;
using netDxf.Blocks.Dynamic.Util;
using netDxf.IO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace netDxf.Objects
{
    //TODO make a IDictionary or something
    [AcadClassName("AcDbDictionary")]
    public class DocumentDictionary : DbObject
    {
        DictionaryCloningFlags cloning;
        bool isHardOwner;

        Dictionary<string, DxfObject> _dictionary = new Dictionary<string, DxfObject>();

        public DocumentDictionary(string codename) : base(codename)
        {
            IsHardOwner = true;
            Cloning = DictionaryCloningFlags.KeepExisting;
        }


        internal DocumentDictionary(bool issHardOwner = true, DictionaryCloningFlags cloning = DictionaryCloningFlags.KeepExisting) : base(DxfObjectCode.Dictionary)
        {
            IsHardOwner = issHardOwner;
            Cloning = cloning;
        }

        /// <summary>
        /// Gets or sets if the dictionary object is hard owner.
        /// </summary>
        internal bool IsHardOwner
        {
            get { return this.isHardOwner; }
            set { this.isHardOwner = value; }
        }

        /// <summary>
        /// Gets or sets the dictionary object cloning flags.
        /// </summary>
        internal DictionaryCloningFlags Cloning
        {
            get { return this.cloning; }
            set { this.cloning = value; }
        }

        public ICollection<string> Names
        {
            get { return _dictionary.Keys; }
        }

        public ICollection<DxfObject> Values
        {
            get { return _dictionary.Values; }
        }
        public bool HasKey(string key)
        {
            return _dictionary.ContainsKey(key);
        }
        
        public DxfObject this[string key]
        {
            get
            {
                return _dictionary[key];
            }
        }

        public void Clear()
        {
            _dictionary.Clear();
        }
        internal override long AssignHandle(long entityNumber)
        {
            base.AssignHandle(entityNumber);

            foreach (var entry in Values)
            {
                //Debug.Assert(string.IsNullOrEmpty(entry.Handle));
                entityNumber = entry.AssignHandle(entityNumber);
            }

            return base.AssignHandle(entityNumber);
        }

        public void AddHardReference(string key, DxfObject dxfObj)
        {
            if (dxfObj.Owner != null)
                throw new InvalidOperationException("Obj has already an owner");
            if (Document != null && dxfObj.Document != null)
            {
                if (dxfObj.Document != Document)
                    throw new InvalidOperationException("Different Document, please use closing.");
            }

            _dictionary.Add(key, dxfObj);
            dxfObj.Owner = this;
            if (Document != null)
            {
                Document.NumHandles = dxfObj.AssignHandle(Document.NumHandles);
                Document.AddedObjects.Add(dxfObj.Handle, dxfObj);
            }
        }

        public void AddSoftReference(string key, DxfObject dxfObj)
        {
            _dictionary.Add(key, dxfObj);
        }

        public void RemoveItem(string key)
        {
            _dictionary.Remove(key);
        }


        public override void GetHardReferences(Queue<DxfObject> result, bool includeSelf = false)
        {
            base.GetHardReferences(result, includeSelf);
            foreach (var entry in _dictionary)
            {
                result.Enqueue(entry.Value);
                if (entry.Value is DbObject dbValue)
                {
                    dbValue.GetHardReferences(result, includeSelf);
                }
            }
        }

        public override void SetHardObjects(Queue<DxfObject> ownedObjects, bool includeSelf = false)
        {

            base.SetHardObjects(ownedObjects, includeSelf);
            for (int i = 0; i < _dictionary.Count(); i++)
            {
                var entryObj = ownedObjects.Dequeue();

                string key = _dictionary.ElementAt(i).Key;
                _dictionary[key] = entryObj;
                entryObj.Owner = this;
                if (entryObj is DbObject dbValue)
                {
                    dbValue.SetHardObjects(ownedObjects, includeSelf);
                }

            }

        }
        public override void SetSoftHandles(Queue<string> referencedHandles, bool includeSelf = false)
        {
            base.SetSoftHandles(referencedHandles, includeSelf);
            for (int i = 0; i < _dictionary.Count(); i++)
            {
                var entryObj = _dictionary.ElementAt(i).Value;
                if (entryObj is DbObject dbValue)
                {
                    dbValue.SetSoftHandles(referencedHandles, includeSelf);
                }
            }
        }


        public override void GetSoftHandles(Queue<string> result, bool includeSelf = false)
        {
            base.GetSoftHandles(result, includeSelf);
            for (int i = 0; i < _dictionary.Count(); i++)
            {
                var entryObj = _dictionary.ElementAt(i).Value;
                if (entryObj is DbObject dbValue)
                {
                    dbValue.GetSoftHandles(result, includeSelf);
                }
            }
        }

        internal override void DXFInLocal(ICodeValueReader reader)
        {
            base.DXFInLocal(reader);
            ReadClassBegin(reader, "AcDbDictionary");

            ReaderAdapter reader2 = new ReaderAdapter(reader);

            reader2.Read<short>(280, v => IsHardOwner = v != 0);
            reader2.Read<short>(281, v => Cloning = (DictionaryCloningFlags) v);

            reader2.WhenCode(3, r =>
            {
                while (r.Code == 3)
                {
                    string first = r.ReadString();
                    r.Next(); Debug.Assert(r.Code == 350);
                    string second = r.ReadString();
                    r.Next();
                    _dictionary.Add(first, null);
                }
            });

            reader2.ExecReadUntil(0, 100, 1001);
        }
        internal override void DXFOutLocal(ICodeValueWriter writer)
        {
            base.DXFOutLocal(writer);
            WriteClassBegin(writer, "AcDbDictionary");
            writer.WriteBool(280, IsHardOwner);
            writer.Write(281, (short)Cloning);

            foreach (var entry in _dictionary)
            {
                writer.Write(3, entry.Key);
                writer.Write(350, entry.Value.Handle);
            }
        }

    }
}
