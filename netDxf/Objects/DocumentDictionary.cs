using netDxf.Blocks.Dynamic;
using netDxf.Blocks.Dynamic.Attributes;
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
    [AcadClassName("AcDbDictionary")]
    public class DocumentDictionary : DbObject
    {
        DictionaryCloningFlags cloning;
        bool isHardOwner;

        Dictionary<string, DxfObject> _dictionary = new Dictionary<string, DxfObject>();
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
            foreach (var entry in Values)
            {
                Debug.Assert(string.IsNullOrEmpty(entry.Handle));
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

        public override IEnumerable<DxfObject> GetHardReferences()
        {
            List<DxfObject> _result = new List<DxfObject>();
            foreach (var entry in _dictionary)
            {
                DbObject dbValue = entry.Value as DbObject;

                _result.Add(entry.Value);
                if (dbValue != null)
                {
                    _result.AddRange(dbValue.GetHardReferences());
                }

            }
            return _result;
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
