using netDxf.Blocks.Dynamic.Attributes;
using netDxf.Blocks.Dynamic.Util;
using netDxf.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace netDxf.Blocks.Dynamic
{
    public static class DxfClassRegistry
    {

        private static readonly List<Tuple<string, Type>> Classes = new List<Tuple<string, Type>>();
        private static readonly Dictionary<string, Func<DbObject>> FactoryDictionary = new Dictionary<string, Func<DbObject>>();

        private static readonly Dictionary<Type, Tuple<int, int>> XRecordMap = new Dictionary<Type, Tuple<int, int>>();

        static DxfClassRegistry()
        {

            RegisterDXFInstance(DxfObjectCode.EvaluationGraph, typeof(EvalGraph));
            RegisterDXFInstance(DxfObjectCode.BlockGripLocationComponent, typeof(BlockGripExpr));
            RegisterDXFInstance(DxfObjectCode.DynamicBlockProxyNode, typeof(DynamicBlockProxyNode));

            RegisterDXFInstance(DxfObjectCode.BlockVisibilityParameter, typeof(BlockVisibilityParameter));
            RegisterDXFInstance(DxfObjectCode.BlockFlipParameter, typeof(BlockFlipParameter));
            RegisterDXFInstance(DxfObjectCode.BlockLinearParameter, typeof(BlockLinearParameter));
            RegisterDXFInstance(DxfObjectCode.BlockLookupParameter, typeof(BlockLookUpParameter));
            RegisterDXFInstance(DxfObjectCode.BlockXYParameter, typeof(BlockXYParameter));
            RegisterDXFInstance(DxfObjectCode.BlockBasePointParameter, typeof(BlockBasepointParameter));
            RegisterDXFInstance(DxfObjectCode.BlockPointParameter, typeof(BlockPointParameter));

            RegisterDXFInstance(DxfObjectCode.BlockVisibilityGrip, typeof(BlockVisibilityGrip));
            RegisterDXFInstance(DxfObjectCode.BlockFlipGrip, typeof(BlockFlipGrip));
            RegisterDXFInstance(DxfObjectCode.BlockLinearGrip, typeof(BlockLinearGrip));
            RegisterDXFInstance(DxfObjectCode.BlockLookupGrip, typeof(BlockLookUpGrip));
            RegisterDXFInstance(DxfObjectCode.BlockXYGrip, typeof(BlockXYGrip));

            RegisterDXFInstance(DxfObjectCode.BlockFlipAction, typeof(BlockFlipAction));
            RegisterDXFInstance(DxfObjectCode.BlockStretchAction, typeof(BlockStretchAction));
            RegisterDXFInstance(DxfObjectCode.BlockMoveAction, typeof(BlockMoveAction));

            RegisterDXFInstance(DxfObjectCode.BlockRepresentationData, typeof(BlockRepresentationData));
            RegisterDXFInstance(DxfObjectCode.DynamicBlockPurgePreventer, typeof(DynamicBlockPurgePreventer));


            RegisterDXFInstance(DxfObjectCode.SortentsTable, typeof(SortentsTable));


            XRecordMap = Assembly.GetExecutingAssembly().GetTypes()
                                 .Select(t => new { Type = t, Att = t.GetCustomAttribute<AcadClassNameAttribute>() })
                                 .Where(e => e.Att != null)
                                 .ToDictionary(e => e.Type, e => new Tuple<int, int>(e.Att.Id1, e.Att.Id2)); ;
            // RegisterDXFInstance(DxfObjectCode.SortentsTable, typeof(SortentsTable));

        }
        public static bool HasClassInstance(string dxfName)
        {
            return FactoryDictionary.ContainsKey(dxfName);
        }

        public static DbObject CreateClassInstance(string dxfName)
        {
            if (FactoryDictionary.TryGetValue(dxfName, out var constructor))
            {
                return constructor();
            }

            return null;
        }
        private static void RegisterDXFInstance(string dxfName, Type type)
        {
            ConstructorInfo constructorInfo = type.GetConstructor(new Type[] { typeof(string) });
            FactoryDictionary.Add(dxfName, () => constructorInfo.Invoke(new string[] { dxfName }) as DbObject);
            Classes.Add(new Tuple<string, Type>(dxfName, type));
        }

        internal static void WriteClasses(ICodeValueWriter chunk, DxfDocument doc)
        {
            foreach (var classEntry in Classes)
            {
                chunk.Write(0, "CLASS");
                chunk.Write(1, classEntry.Item1);
                chunk.Write(2, "AcDb" + classEntry.Item2.Name);
                chunk.Write(3, "Classes defined in " + classEntry.Item2.Namespace);
                chunk.Write(90, 1153);
                chunk.Write(91, 100);
                chunk.WriteBool(280, false);
                chunk.WriteBool(281, false);
            }
        }

        internal static void GetXRecordClassIdentifier(Type type, out int id1, out int id2)
        {
            var classId = XRecordMap[type];
            id1 = classId.Item1;
            id2 = classId.Item2;
        }
    }
}
