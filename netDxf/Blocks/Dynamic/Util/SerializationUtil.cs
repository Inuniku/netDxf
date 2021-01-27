using netDxf.IO;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace netDxf.Blocks.Dynamic.Util
{
    public static class SerializationUtil
    {
        internal static Vector2[] ReadPoint2List(this ICodeValueReader reader, int sizeCode, int elementCodeX, int elementCodeY)
        {
            Debug.Assert(reader.Code == sizeCode);
            int numElements = reader.ReadShort();

            reader.Next();

            List<Vector2> result = new List<Vector2>(numElements);
            for (int i = 0; i < numElements; i++)
            {
                Vector2 point = new Vector2();
                Debug.Assert(reader.Code == elementCodeX);
                point.X = reader.ReadDouble();
                reader.Next();

                Debug.Assert(reader.Code == elementCodeY);
                point.Y = reader.ReadDouble();
                reader.Next();

                result.Add(point);
            }
            return result.ToArray();
        }

        internal static Vector3[] ReadPoint3List(this ICodeValueReader reader, int sizeCode, int elementCodeX, int elementCodeY, int elementCodeZ)
        {
            Debug.Assert(reader.Code == sizeCode);
            int numElements = reader.ReadInt();
            reader.Next();

            List<Vector3> result = new List<Vector3>(numElements);
            for (int i = 0; i < numElements; i++)
            { 
                Vector3 point = new Vector3();

                Debug.Assert(reader.Code == elementCodeX);
                point.X = reader.ReadDouble();
                reader.Next();

                Debug.Assert(reader.Code == elementCodeY);
                point.Y = reader.ReadDouble();
                reader.Next();

                Debug.Assert(reader.Code == elementCodeZ);
                point.Z = reader.ReadDouble();
                reader.Next();
                result.Add(point);
            }
            return result.ToArray();
        }

        internal static string[] ReadSoftPointerList(this ICodeValueReader reader, int sizeCode, int elementCode)
        {
            Debug.Assert(reader.Code == sizeCode);
            int numElements = reader.ReadShort();
            reader.Next();

            List<string> result = new List<string>(numElements);
            for (int i = 0; i < numElements; i++)
            {
                Debug.Assert(reader.Code == elementCode);
                result.Add(reader.ReadHex());
                reader.Next();
            }
            return result.ToArray();
        }


        internal static double[] ReadDoubleList (this ICodeValueReader reader, int sizeCode, int elementCode)
        {
            Debug.Assert(reader.Code == sizeCode);
            int numElements = reader.ReadShort();
            reader.Next();

            List<double> result = new List<double>(numElements);
            for (int i = 0; i < numElements; i++)
            {
                Debug.Assert(reader.Code == elementCode);
                result.Add(reader.ReadDouble());
                reader.Next();
            }
            return result.ToArray();
        }

        internal static int[] ReadIntList(this ICodeValueReader reader, int sizeCode, int elementCode)
        {
            Debug.Assert(reader.Code == sizeCode);
            int numElements = reader.ReadShort();
            reader.Next();

            List<int> result = new List<int>(numElements);
            for (int i = 0; i < numElements; i++)
            {
                Debug.Assert(reader.Code == elementCode);
                result.Add(reader.ReadInt());
                reader.Next();
            }
            return result.ToArray();
        }
        internal static void WriteDefault<T>(this ICodeValueWriter writer, short code, T value) where T : IEquatable<T>
        {
            if (!EqualityComparer<T>.Default.Equals(value, default(T)))
            {
                writer.Write(code, value);
            }
        }

        internal static void WriteVector3(this ICodeValueWriter writer, short elementXCode, Vector3 value, short stepSize = 10)
        {
            writer.Write(elementXCode, value.X);
            writer.Write((short)(elementXCode + stepSize), value.Y);
            writer.Write((short)(elementXCode + 2 * stepSize), value.Z);
        }

        internal static void WriteIntList(this ICodeValueWriter writer, short sizeCode, short elementCode, IEnumerable<int> list)
        {
            object sizeValue = ConvertByDXFCode(list.Count(), sizeCode);
            writer.Write(sizeCode, sizeValue);
            for(int i = 0; i < list.Count(); i++)
                writer.Write(elementCode, list.ElementAt(i));
        }

        internal static void WriteList<T>(this ICodeValueWriter writer, short sizeCode, short elementCode, IEnumerable<T> list)
        {
            object sizeValue = ConvertByDXFCode(list.Count(), sizeCode);
            writer.Write(sizeCode, sizeValue);
            for (int i = 0; i < list.Count(); i++)
            {
                object value = ConvertByDXFCode(list.ElementAt(i), elementCode);
                writer.Write(elementCode, value);
            }
        }
        internal static void WriteVector2List(this ICodeValueWriter writer, short sizeCode, short elementCodeX, IEnumerable<Vector2> list, short step = 10)
        {
            object sizeValue = ConvertByDXFCode(list.Count(), sizeCode);
            writer.Write(sizeCode, sizeValue);
            for (int i = 0; i < list.Count(); i++)
            {
                writer.Write(elementCodeX, list.ElementAt(i).X);
                writer.Write((short)(elementCodeX + step), list.ElementAt(i).Y);
            }
        }

        internal static void WriteBool(this ICodeValueWriter writer, short code, bool value)
        {
            writer.Write(code, value ? (short)1 : (short)0);
        } 

        public static IEnumerable<T> TakeLast<T>(this IEnumerable<T> source, int N)
        {
            return source.Skip(Math.Max(0, source.Count() - N));
        }

        public static object ConvertByDXFCode(object obj, short code)
        {
            Type targetType;

            if (code >= 0 && code <= 9) // string
            {
                targetType = typeof(string);
            }
            else if (code >= 10 && code <= 39) // double precision 3D point value
            {
                targetType = typeof(double);
            }
            else if (code >= 40 && code <= 59) // double precision floating point value
            {
                targetType = typeof(double);
            }
            else if (code >= 60 && code <= 79) // 16-bit integer value
            {

                targetType = typeof(short);
            }
            else if (code >= 90 && code <= 99) // 32-bit integer value
            {
                targetType = typeof(int);
            }
            else if (code == 100) // string (255-character maximum; less for Unicode strings)
            {

                targetType = typeof(string);
            }
            else if (code == 101) // string (255-character maximum; less for Unicode strings). This code is undocumented and seems to affect only the AcdsData in dxf version 2013
            {

                targetType = typeof(string);
            }
            else if (code == 102) // string (255-character maximum; less for Unicode strings)
            {

                targetType = typeof(string);
            }
            else if (code == 105) // string representing hexadecimal (hex) handle value
            {

                targetType = typeof(string);
            }
            else if (code >= 110 && code <= 119) // double precision floating point value
            {
                targetType = typeof(double);
            }
            else if (code >= 120 && code <= 129) // double precision floating point value
            {
                targetType = typeof(double);
            }
            else if (code >= 130 && code <= 139) // double precision floating point value
            {
                targetType = typeof(double);
            }
            else if (code >= 140 && code <= 149) // double precision scalar floating-point value
            {
                targetType = typeof(double);
            }
            else if (code >= 160 && code <= 169) // 64-bit integer value
            {
                targetType = typeof(long);
            }
            else if (code >= 170 && code <= 179) // 16-bit integer value
            {

                targetType = typeof(short);
            }
            else if (code >= 210 && code <= 239) // double precision scalar floating-point value
            {
                targetType = typeof(double);
            }
            else if (code >= 270 && code <= 279) // 16-bit integer value
            {

                targetType = typeof(short);
            }
            else if (code >= 280 && code <= 289) // 16-bit integer value
            {

                targetType = typeof(short);
            }
            else if (code >= 290 && code <= 299) // byte (boolean flag value)
            {
                targetType = typeof(bool);
            }
            else if (code >= 300 && code <= 309) // arbitrary text string
            {

                targetType = typeof(string);
            }
            else if (code >= 310 && code <= 319) // string representing hex value of binary chunk
            {
                targetType = typeof(byte[]);
            }
            else if (code >= 320 && code <= 329) // string representing hex handle value
            {

                targetType = typeof(string);
            }
            else if (code >= 330 && code <= 369) // string representing hex object IDs
            {

                targetType = typeof(string);
            }
            else if (code >= 370 && code <= 379) // 16-bit integer value
            {

                targetType = typeof(short);
            }
            else if (code >= 380 && code <= 389) // 16-bit integer value
            {

                targetType = typeof(short);
            }
            else if (code >= 390 && code <= 399) // string representing hex handle value
            {

                targetType = typeof(string);
            }
            else if (code >= 400 && code <= 409) // 16-bit integer value
            {

                targetType = typeof(short);
            }
            else if (code >= 410 && code <= 419) // string
            {

                targetType = typeof(string);
            }
            else if (code >= 420 && code <= 429) // 32-bit integer value
            {
                targetType = typeof(int);
            }
            else if (code >= 430 && code <= 439) // string
            {

                targetType = typeof(string);
            }
            else if (code >= 440 && code <= 449) // 32-bit integer value
            {
                targetType = typeof(int);
            }
            else if (code >= 450 && code <= 459) // 32-bit integer value
            {
                targetType = typeof(int);
            }
            else if (code >= 460 && code <= 469) // double-precision floating-point value
            {
                targetType = typeof(double);
            }
            else if (code >= 470 && code <= 479) // string
            {

                targetType = typeof(string);
            }
            else if (code >= 480 && code <= 481) // string representing hex handle value
            {

                targetType = typeof(string);
            }
            else if (code == 999) // comment (string)
            {

                targetType = typeof(string);
            }
            else if (code >= 1010 && code <= 1059) // double-precision floating-point value
            {
                targetType = typeof(double);
            }
            else if (code >= 1000 && code <= 1003) // string (same limits as indicated with 0-9 code range)
            {

                targetType = typeof(string);
            }
            else if (code == 1004) // string representing hex value of binary chunk
            {
                targetType = typeof(byte[]);
            }
            else if (code >= 1005 && code <= 1009) // string (same limits as indicated with 0-9 code range)
            {

                targetType = typeof(string);
            }
            else if (code >= 1060 && code <= 1070) // 16-bit integer value
            {

                targetType = typeof(short);
            }
            else if (code == 1071) // 32-bit integer value
            {
                targetType = typeof(int);
            }
            else
            {
                throw new Exception(string.Format("Code {0} not valid.", code));
            }

            return Convert.ChangeType(obj, targetType);
        }
    }
}
