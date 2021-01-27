using netDxf.IO;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace netDxf.Blocks.Dynamic.Util
{
    internal class ReaderAdapter
    {
        internal readonly ICodeValueReader Reader;

        internal ReaderAdapter(ICodeValueReader reader)
        {
            Reader = reader;
        }

        internal enum ReadResult
        {
            Unknown = 0,
            Continue = 1,
            Done = 2,
        }


        readonly List<Func<ReadResult>> EvalFuncs = new List<Func<ReadResult>>();
        readonly List<short> EvalCodes = new List<short>();
        readonly Dictionary<Func<ReadResult>, Action> DefaultFuncDict = new Dictionary<Func<ReadResult>, Action> ();

        private T _readNow<T>(short code)
        {

            Debug.Assert(Reader.Code == code);
            if (typeof(T) == typeof(string))
            {
                return (T)Convert.ChangeType(Reader.ReadString(), typeof(T));
            }
            if (typeof(T) == typeof(double))
            {
                return (T)Convert.ChangeType(Reader.ReadDouble(), typeof(T));
            }
            if (typeof(T) == typeof(short))
            {
                return (T)Convert.ChangeType(Reader.ReadShort(), typeof(T));
            }
            return (T)Convert.ChangeType(Reader.ReadInt(), typeof(T));
        }
        public T ReadNow<T>(short code)
        {

            Debug.Assert(Reader.Code == code);
            T result = _readNow<T>(code);
            Reader.Next();
            return result;
        }

        public void Read<T>(short code, Action<T> setVariable)
        {
           EvalFuncs.Add(() =>
           {
               if (Reader.Code == code)
               {
                   setVariable(_readNow<T>(code));
                   Reader.Next();
                   return ReadResult.Done;
               }

               return ReadResult.Unknown;
           });
            EvalCodes.Add(code);
        }
        public void ReadDefault<T>(short code, Action<T> setVariable, T defaultValue = default)
        {
            Func<ReadResult> readFunc = () =>
            {
                if (Reader.Code == code)
                {
                    setVariable(_readNow<T>(code));
                    Reader.Next();
                    return ReadResult.Done;
                }

                return ReadResult.Unknown;
            };

            EvalFuncs.Add(readFunc);
            EvalCodes.Add(code);

            DefaultFuncDict.Add(readFunc, () => setVariable(defaultValue));
        }

        public void ReadVector2(short elementCodeX, Action<Vector2> setVariable)
        {
            Vector2 vector = new Vector2();
            bool setX = false, setY = false;
            EvalFuncs.Add(() =>
            {
                bool canContinue = false;
                if (Reader.Code == elementCodeX)
                {
                    vector.X = _readNow<double>(elementCodeX);
                    setX = true;
                    canContinue = true;
                    Reader.Next();
                }
                if (Reader.Code == elementCodeX + 10)
                {
                    vector.Y = _readNow<double>((short)(elementCodeX + 10));
                    setY = true;
                    canContinue = true;
                    Reader.Next();
                }

                if (setX && setY)
                {
                    setVariable(vector);
                    Reader.Next();
                    return ReadResult.Done;
                }
                return canContinue ? ReadResult.Continue : ReadResult.Unknown;
            });
            EvalCodes.Add(elementCodeX);
        }
        public void WhenCode(short code, Action<ICodeValueReader> action)
        {
            EvalFuncs.Add(() =>
            {
                if (Reader.Code == code)
                {
                    action(Reader);
                    return ReadResult.Done;
                }
                return ReadResult.Unknown;
            });
            EvalCodes.Add(code);
        }

        public void ReadVector3(short elementCodeX, Action<Vector3> setVariable, short step = 10)
        {
            Vector3 vector = new Vector3();
            bool setX = false, setY = false, setZ = false;
            EvalFuncs.Add(() =>
            {
                bool canContinue = false;
                if (Reader.Code == elementCodeX)
                {
                    vector.X = _readNow<double>(elementCodeX);
                    setX = true;
                    canContinue = true;
                }
                if (Reader.Code == elementCodeX + step)
                {
                    vector.Y = _readNow<double>((short)(elementCodeX + step));
                    setY = true;
                    canContinue = true;
                }
                if (Reader.Code == elementCodeX +  2 * step)
                {
                    vector.Z = _readNow<double>((short)(elementCodeX + 2 * step));
                    setZ = true;
                    canContinue = true;
                }
                if (setX && setY && setZ)
                {
                    setVariable(vector);
                    Reader.Next();
                    return ReadResult.Done;
                }
                if(canContinue)
                {
                    Reader.Next();
                    return ReadResult.Continue;
                }
                return ReadResult.Unknown;
            });
            EvalCodes.Add(elementCodeX);
        }

        public void ReadList<T>(short sizeCode, short elementCode, Action<List<T>> setVariable)
        {
            EvalFuncs.Add(() =>
            {
                if (Reader.Code == sizeCode)
                {
                    List<T> list = new List<T>();
                    int num = Reader.ReadShort();
                    for (int i = 0; i < num; i++)
                    {
                        Reader.Next();
                        list.Add(_readNow<T>(elementCode));
                    }
                    setVariable(list);
                    Reader.Next();
                    return ReadResult.Done;
                }

                return ReadResult.Unknown;
            });
            EvalCodes.Add(sizeCode);
        }

        public void ExecReadUntil(params short[] stopCodes)
        {
            while (!stopCodes.Contains(Reader.Code))
            {

                bool readCode = false;
                for (int i = EvalFuncs.Count - 1; i >= 0; i--)
                {
                    var evalFunc = EvalFuncs[i];
                    ReadResult result = evalFunc();

                    if (result == ReadResult.Done)
                    {
                        EvalFuncs.RemoveAt(i);
                        EvalCodes.RemoveAt(i);
                    };

                    if(result == ReadResult.Done || result == ReadResult.Continue)
                    {
                        readCode = true;
                        goto continueLabel;
                    }

                }

                if(!readCode)
                {
                    Debug.WriteLine("Skipped code: " + Reader.Code);
                }

                Reader.Next();
                continueLabel:
                bool a = false;
            }

            AssignDefaults();
        }

        public void ExecTimes(int count)
        {
            throw new NotImplementedException();
            /*
            for (int i = 0; i < count; i++)
            {
                foreach (var evalFunc in EvalFuncs)
                {
                    if (evalFunc())
                        break;
                }
                Reader.Next();
            }

            AssignDefaults();*/
        }

        private void AssignDefaults()
        {
            for (int i = EvalFuncs.Count - 1; i >= 0; i--)
            {
                var evalFunc = EvalFuncs[i];
                if(DefaultFuncDict.TryGetValue(evalFunc, out Action assignDefault) )
                {
                    assignDefault();
                }
                else
                {
                    Debug.WriteLine("Value not present in DXF: " + EvalCodes[i]);
                }
            }
        }

    }
}
