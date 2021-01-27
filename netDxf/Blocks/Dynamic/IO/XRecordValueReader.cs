using netDxf.IO;
using netDxf.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace netDxf.Blocks.Dynamic
{
    internal class XRecordValueReader : ICodeValueReader
    {
        private XRecord Record;

        internal XRecordValueReader(XRecord record)
        {
            Record = record;
        }

        private List<XRecordEntry> Entries => Record.Entries;
        public int Index { get; private set; }

        public short Code
        {
            get
            {
               return (short)(Index < Entries.Count ? Entries[Index].Code : 0);
            }
        }
        public object Value
        {
            get
            {
                return Index < Entries.Count ? Entries[Index].Value : "EOF";
            }
        }

        public long CurrentPosition => Index;


        public void Next()
        {
            Index++;
        }

        public bool ReadBool()
        {
            return (bool)Value;
        }

        public byte ReadByte()
        {
            return (byte)Value;
        }

        public byte[] ReadBytes()
        {
            return (byte[])Value;
        }

        public double ReadDouble()
        {
            return (double)Value;
        }

        public string ReadHex()
        {
            return (string)Value;
        }

        public int ReadInt()
        {
            return (int)Value;
        }

        public long ReadLong()
        {
            return (long)Value;
        }

        public short ReadShort()
        {
            return (short)Value;
        }

        public string ReadString()
        {
            return (string)Value;
        }
    }
}
