using netDxf.IO;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace netDxf.Blocks.Dynamic.IO
{
    public class CopyValue
    {
        public CopyValue(short code, object value)
        {
            Code = code;
            Value = value;

            if (code == 5)
            {
                IsHard = true;
            }

            if (code >= 330 && code <= 339)
            {
                IsSoft = true;
                IsReference = true;
            }

            if (code >= 340 && code <= 349)
            {
                IsHard = true;
                IsReference = true;
            }

            if (code >= 350 && code <= 359)
            {
                IsSoft = true;
                IsOwner = true;
            }

            if (code >= 360 && code <= 369)
            {
                IsHard = true;
                IsReference = true;
            }
        }
        public short Code;
        public object Value;

        public bool IsHard;
        public bool IsSoft;
        public bool IsReference;
        public bool IsOwner;
    }

    public class CopyReaderWriter : IResolveReader, ICodeValueWriter
    {
        private List<CopyValue> buffer = new List<CopyValue>();
        private Func<string, DxfObject> _resolveReference;

        Dictionary<string, DxfObject> clonedObjects = new Dictionary<string, DxfObject>();

        public CopyReaderWriter(Func<string, DxfObject> resolveReference)
        {
            _resolveReference = resolveReference;
        }

        public short Code { get; set; }
        public object Value { get; set; }
        public int Index => (int)CurrentPosition;

        public long CurrentPosition { get; private set; } = -1;

        public void Flush()
        {}
        public void Rewind()
        {
            CurrentPosition = 0;
        }

        public void Next()
        {
            CurrentPosition++;
            if(CurrentPosition < buffer.Count)
            {
                Code = buffer[Index].Code;
                Value = buffer[Index].Value;
            }
        }

        public bool ReadBool()
        {
            return (bool)buffer[Index].Value;
        }

        public byte ReadByte()
        {
            return (byte)buffer[Index].Value;
        }

        public byte[] ReadBytes()
        {
            return (byte[])buffer[Index].Value;
        }

        public double ReadDouble()
        {
            return (double)buffer[Index].Value;
        }

        public string ReadHex()
        {
            return (string)buffer[Index].Value;
        }

        public int ReadInt()
        {
            return (int)buffer[Index].Value;
        }

        public long ReadLong()
        {
            return (long)buffer[Index].Value;
        }

        public short ReadShort()
        {
            return (short)buffer[Index].Value;
        }

        public string ReadString()
        {
            return (string)buffer[Index].Value;
        }

        public void Write(short code, object value)
        {
            Code = code;

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

            Value = value;
        }

        public void WriteBool(bool value)
        {
            buffer.Add(new CopyValue(Code, value));
            CurrentPosition++;
        }

        public void WriteByte(byte value)
        {
            buffer.Add(new CopyValue(Code, value));
            CurrentPosition++;
        }

        public void WriteBytes(byte[] value)
        {
            buffer.Add(new CopyValue(Code, value));
            CurrentPosition++;
        }

        public void WriteDouble(double value)
        {
            buffer.Add(new CopyValue(Code, value));
            CurrentPosition++;
        }

        public void WriteInt(int value)
        {
            buffer.Add(new CopyValue(Code, value));
            CurrentPosition++;
        }

        public void WriteLong(long value)
        {
            buffer.Add(new CopyValue(Code, value));
            CurrentPosition++;
        }

        public void WriteShort(short value)
        {
            buffer.Add(new CopyValue(Code, value));
            CurrentPosition++;
        }

        public void WriteString(string value)
        {
            buffer.Add(new CopyValue(Code, value));
            CurrentPosition++;
        }

        public DxfObject ResolveHandle(string handle)
        {
            return _resolveReference(handle);
        }
    }
}
