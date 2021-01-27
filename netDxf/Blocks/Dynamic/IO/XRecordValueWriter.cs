﻿using netDxf.IO;
using netDxf.Objects;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace netDxf.Blocks.Dynamic
{
    internal class XRecordValueWriter : ICodeValueWriter
    {
        private XRecord Record;
        private short dxfCode = -1;
        private object value;

        private List<XRecordEntry> Entries => Record.Entries;

        internal XRecordValueWriter(XRecord record)
        {
            Record = record;
        }

        public short Code => dxfCode;

        public object Value => value;

        public long CurrentPosition => Entries.Count;

        public void Flush()
        {
        }

        public void Write(short code, object value)
        {
            this.dxfCode = code;
            
            if (code >= 0 && code <= 9) // string
            {
                Debug.Assert(value is string, "Incorrect value type.");
                this.WriteString((string)value);
            }
            else if (code >= 10 && code <= 39) // double precision 3D point value
            {
                Debug.Assert(value is double, "Incorrect value type.");
                this.WriteDouble((double)value);
            }
            else if (code >= 40 && code <= 59) // double precision floating point value
            {
                Debug.Assert(value is double, "Incorrect value type.");
                this.WriteDouble((double)value);
            }
            else if (code >= 60 && code <= 79) // 16-bit integer value
            {
                Debug.Assert(value is short, "Incorrect value type.");
                this.WriteShort((short)value);
            }
            else if (code >= 90 && code <= 99) // 32-bit integer value
            {
                Debug.Assert(value is int, "Incorrect value type.");
                this.WriteInt((int)value);
            }
            else if (code == 100) // string (255-character maximum; less for Unicode strings)
            {
                Debug.Assert(value is string, "Incorrect value type.");
                this.WriteString((string)value);
            }
            else if (code == 101) // string (255-character maximum; less for Unicode strings). This code is undocumented and seems to affect only the AcdsData in dxf version 2013
            {
                Debug.Assert(value is string, "Incorrect value type.");
                this.WriteString((string)value);
            }
            else if (code == 102) // string (255-character maximum; less for Unicode strings)
            {
                Debug.Assert(value is string, "Incorrect value type.");
                this.WriteString((string)value);
            }
            else if (code == 105) // string representing hexadecimal (hex) handle value
            {
                Debug.Assert(value is string, "Incorrect value type.");
                this.WriteString((string)value);
            }
            else if (code >= 110 && code <= 119) // double precision floating point value
            {
                Debug.Assert(value is double, "Incorrect value type.");
                this.WriteDouble((double)value);
            }
            else if (code >= 120 && code <= 129) // double precision floating point value
            {
                Debug.Assert(value is double, "Incorrect value type.");
                this.WriteDouble((double)value);
            }
            else if (code >= 130 && code <= 139) // double precision floating point value
            {
                Debug.Assert(value is double, "Incorrect value type.");
                this.WriteDouble((double)value);
            }
            else if (code >= 140 && code <= 149) // double precision scalar floating-point value
            {
                Debug.Assert(value is double, "Incorrect value type.");
                this.WriteDouble((double)value);
            }
            else if (code >= 160 && code <= 169) // 64-bit integer value
            {
                Debug.Assert(value is long, "Incorrect value type.");
                this.WriteLong((long)value);
            }
            else if (code >= 170 && code <= 179) // 16-bit integer value
            {
                Debug.Assert(value is short, "Incorrect value type.");
                this.WriteShort((short)value);
            }
            else if (code >= 210 && code <= 239) // double precision scalar floating-point value
            {
                Debug.Assert(value is double, "Incorrect value type.");
                this.WriteDouble((double)value);
            }
            else if (code >= 270 && code <= 279) // 16-bit integer value
            {
                Debug.Assert(value is short, "Incorrect value type.");
                this.WriteShort((short)value);
            }
            else if (code >= 280 && code <= 289) // 16-bit integer value
            {
                Debug.Assert(value is short, "Incorrect value type.");
                this.WriteShort((short)value);
            }
            else if (code >= 290 && code <= 299) // byte (boolean flag value)
            {
                Debug.Assert(value is bool, "Incorrect value type.");
                this.WriteBool((bool)value);
            }
            else if (code >= 300 && code <= 309) // arbitrary text string
            {
                Debug.Assert(value is string, "Incorrect value type.");
                this.WriteString((string)value);
            }
            else if (code >= 310 && code <= 319) // string representing hex value of binary chunk
            {
                Debug.Assert(value is byte[], "Incorrect value type.");
                this.WriteBytes((byte[])value);
            }
            else if (code >= 320 && code <= 329) // string representing hex handle value
            {
                Debug.Assert(value is string, "Incorrect value type.");
                this.WriteString((string)value);
            }
            else if (code >= 330 && code <= 369) // string representing hex object IDs
            {
                Debug.Assert(value is string, "Incorrect value type.");
                this.WriteString((string)value);
            }
            else if (code >= 370 && code <= 379) // 16-bit integer value
            {
                Debug.Assert(value is short, "Incorrect value type.");
                this.WriteShort((short)value);
            }
            else if (code >= 380 && code <= 389) // 16-bit integer value
            {
                Debug.Assert(value is short, "Incorrect value type.");
                this.WriteShort((short)value);
            }
            else if (code >= 390 && code <= 399) // string representing hex handle value
            {
                Debug.Assert(value is string, "Incorrect value type.");
                this.WriteString((string)value);
            }
            else if (code >= 400 && code <= 409) // 16-bit integer value
            {
                Debug.Assert(value is short, "Incorrect value type.");
                this.WriteShort((short)value);
            }
            else if (code >= 410 && code <= 419) // string
            {
                Debug.Assert(value is string, "Incorrect value type.");
                this.WriteString((string)value);
            }
            else if (code >= 420 && code <= 429) // 32-bit integer value
            {
                Debug.Assert(value is int, "Incorrect value type.");
                this.WriteInt((int)value);
            }
            else if (code >= 430 && code <= 439) // string
            {
                Debug.Assert(value is string, "Incorrect value type.");
                this.WriteString((string)value);
            }
            else if (code >= 440 && code <= 449) // 32-bit integer value
            {
                Debug.Assert(value is int, "Incorrect value type.");
                this.WriteInt((int)value);
            }
            else if (code >= 450 && code <= 459) // 32-bit integer value
            {
                Debug.Assert(value is int, "Incorrect value type.");
                this.WriteInt((int)value);
            }
            else if (code >= 460 && code <= 469) // double-precision floating-point value
            {
                Debug.Assert(value is double, "Incorrect value type.");
                this.WriteDouble((double)value);
            }
            else if (code >= 470 && code <= 479) // string
            {
                Debug.Assert(value is string, "Incorrect value type.");
                this.WriteString((string)value);
            }
            else if (code >= 480 && code <= 481) // string representing hex handle value
            {
                Debug.Assert(value is string, "Incorrect value type.");
                this.WriteString((string)value);
            }
            else if (code == 999) // comment (string)
            {
                Debug.Assert(value is string, "Incorrect value type.");
                this.WriteString((string)value);
            }
            else if (code >= 1010 && code <= 1059) // double-precision floating-point value
            {
                Debug.Assert(value is double, "Incorrect value type.");
                this.WriteDouble((double)value);
            }
            else if (code >= 1000 && code <= 1003) // string (same limits as indicated with 0-9 code range)
            {
                Debug.Assert(value is string, "Incorrect value type.");
                this.WriteString((string)value);
            }
            else if (code == 1004) // string representing hex value of binary chunk
            {
                Debug.Assert(value is byte[], "Incorrect value type.");
                this.WriteBytes((byte[])value);
            }
            else if (code >= 1005 && code <= 1009) // string (same limits as indicated with 0-9 code range)
            {
                Debug.Assert(value is string, "Incorrect value type.");
                this.WriteString((string)value);
            }
            else if (code >= 1060 && code <= 1070) // 16-bit integer value
            {
                Debug.Assert(value is short, "Incorrect value type.");
                this.WriteShort((short)value);
            }
            else if (code == 1071) // 32-bit integer value
            {
                Debug.Assert(value is int, "Incorrect value type.");
                this.WriteInt((int)value);
            }
            else
            {
                throw new Exception(string.Format("Code {0} not valid at line {1}", Code, CurrentPosition));
            }

            this.value = value;
        }


        public void WriteBool(bool value)
        {

            Entries.Add(new XRecordEntry(Code, value));
        }

        public void WriteByte(byte value)
        {
            Entries.Add(new XRecordEntry(Code, value));
        }

        public void WriteBytes(byte[] value)
        {
            Entries.Add(new XRecordEntry(Code, value));
        }

        public void WriteDouble(double value)
        {
            Entries.Add(new XRecordEntry(Code, value));
        }

        public void WriteInt(int value)
        {
            Entries.Add(new XRecordEntry(Code, value));
        }

        public void WriteLong(long value)
        {
            throw new NotImplementedException();
        }

        public void WriteShort(short value)
        {
            Entries.Add(new XRecordEntry(Code, value));
        }

        public void WriteString(string value)
        {
            Entries.Add(new XRecordEntry(Code, value));
        }
    }
}
