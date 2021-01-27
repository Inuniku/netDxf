using netDxf;
using netDxf.Blocks;
using netDxf.Blocks.Dynamic;
using netDxf.Blocks.Dynamic.Property;
using netDxf.Entities;
using netDxf.Objects;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace netDxf.Blocks.Dynamic.Property
{
    public class DynamicBlockReferencePropertyCollection : IReadOnlyCollection<DynamicBlockReferenceProperty>
    {
        readonly EvalGraph _evalGraph;

        private DocumentDictionary _enhancedBlockData;
        private List<DynamicBlockReferenceProperty> _properties;


        internal DynamicBlockReferencePropertyCollection(Insert blockReference)
        {
            var representationDict = blockReference.ExtensionDictionary["AcDbBlockRepresentation"] as DocumentDictionary;
            var repData = representationDict["AcDbRepData"] as BlockRepresentationData;
            var appDataCache = representationDict["AppDataCache"] as DocumentDictionary;

            BlockRecord referencedDynamicBlock = repData.BlockRecord;

            _evalGraph = referencedDynamicBlock.ExtensionDictionary["ACAD_ENHANCEDBLOCK"] as EvalGraph;
            _enhancedBlockData = appDataCache["ACAD_ENHANCEDBLOCKDATA"] as DocumentDictionary;


            _properties = new List<DynamicBlockReferenceProperty>();
            foreach (var node in _evalGraph.Nodes.Select(n => n.Expression))
            {
                if(node is BlockParameter blockParam)
                {
                    var properties = PropertyFactory.CreatePropertyWrappers(blockParam);
                    _properties.AddRange(properties);
                }
            }
        }

        public int Count => _properties.Count;

        public IEnumerator<DynamicBlockReferenceProperty> GetEnumerator()
        {
            return _properties.GetEnumerator();
        }

        public void UpdateFromApplication()
        {
            foreach (var record in _enhancedBlockData.Names)
            {
                XRecord xrecord = _enhancedBlockData[record] as XRecord;
                {
                    int nodeId = int.Parse(record);
                    NodeEntry entry = _evalGraph.Nodes.FirstOrDefault(n => n.Id == nodeId);
                    if (entry != null)
                    {
                        XRecordValueReader reader = new XRecordValueReader(xrecord);
                        (entry.Expression as EvalExpr)?.RuntimeDataIn(reader);
                    }
                }
            }
        }

        public void CommitChanges(DynamicBlockReferenceProperty property)
        {
            List<int> nodesToActive = new List<int>();
            List<int> activatedNodes = new List<int>();


            nodesToActive.Add(property.Parameter.NodeId);

            /*
            _evalGraph.Activate(nodesToActive, activatedNodes);

            var expressions = activatedNodes.Select(n => _evalGraph.Nodes.First(m => m.Id == n).Expression).ToArray();
            var Context = new BlockEvaluationContext(_evalGraph);*/
            var Context = new BlockEvaluationContext(_evalGraph);
            List<int> gripNodes = new List<int>();
            _evalGraph.GetSuperNodes(nodesToActive, gripNodes);

            _evalGraph.Eval(nodesToActive, Context);
            _evalGraph.Eval(gripNodes, Context, EvalStep.Update);


            foreach (var record in _enhancedBlockData.Names)
            {
                XRecord xrecord = _enhancedBlockData[record] as XRecord;
                {
                    int nodeId = int.Parse(record);
                    NodeEntry entry = _evalGraph.Nodes.FirstOrDefault(n => n.Id == nodeId);
                    if (entry != null)
                    {
                        xrecord.Entries.Clear();
                        XRecordValueWriter writer = new XRecordValueWriter(xrecord);
                        (entry.Expression)?.RuntimeDataOut(writer);
                    }
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _properties.GetEnumerator();
        }
    }
}
