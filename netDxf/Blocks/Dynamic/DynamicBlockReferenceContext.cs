using netDxf.Blocks.Dynamic.Property;
using netDxf.Entities;
using netDxf.Objects;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace netDxf.Blocks.Dynamic
{
    public class DynamicBlockReferenceContext
    {
        private List<DynamicBlockReferenceProperty> _properties = new List<DynamicBlockReferenceProperty>();

        internal Insert BlockReference { get; }
        internal Block DynamicBlock { get; set; }
        internal Block RepresentationBlock { get; set; }
        internal DocumentDictionary EnhancedBlockData { get; set; }
        internal XRecord EnhancedBlockHistory { get; set; }
        internal EvalGraph EvalGraph { get; set; }
        internal BlockVisibilityParameter Visibility { get; set; }
        public IReadOnlyCollection<DynamicBlockReferenceProperty> Properties => new ReadOnlyCollection<DynamicBlockReferenceProperty>(_properties);

        public DynamicBlockReferenceContext(Insert dynamicBlockReference)
        {
            if (dynamicBlockReference == null || !dynamicBlockReference.IsDynamicBlock())
            {
                throw new InvalidOperationException($"Can't create {nameof(DynamicBlockReferenceContext)} for a non dynamic block.");
            }

            if (dynamicBlockReference.Document == null)
                throw new InvalidOperationException($"Insert must live in a valid document.");

            BlockReference = dynamicBlockReference;

            bool isRepresentationBlock = false;
            if (BlockReference.ExtensionDictionary != null)
            {
                if (BlockReference.ExtensionDictionary.HasKey("AcDbBlockRepresentation"))
                {
                    var representationDict = BlockReference?.ExtensionDictionary["AcDbBlockRepresentation"] as DocumentDictionary;

                    var repData = representationDict["AcDbRepData"] as BlockRepresentationData;
                    BlockRecord dynamicBlockRecord = repData.BlockRecord;
                    DynamicBlock = BlockReference.Document.Blocks.Single(b => b.Record == dynamicBlockRecord);

                    var appDataCache = representationDict["AppDataCache"] as DocumentDictionary;
                    EnhancedBlockData = appDataCache["ACAD_ENHANCEDBLOCKDATA"] as DocumentDictionary;
                    EnhancedBlockHistory = appDataCache["ACAD_ENHANCEDBLOCKHISTORY"] as XRecord;
                    isRepresentationBlock = true;
                }
            }

            if (!isRepresentationBlock)
            {
                DynamicBlock = dynamicBlockReference.Block;
            }
            else
            {
                RepresentationBlock = dynamicBlockReference.Block;
            }

            EvalGraph = DynamicBlock.Record.ExtensionDictionary["ACAD_ENHANCEDBLOCK"] as EvalGraph;
            Visibility = EvalGraph.Nodes.Select(n => n.Expression).OfType<BlockVisibilityParameter>().SingleOrDefault();

            CreateProperties();
        }


        public bool HasRepresentation => RepresentationBlock != null;

        public void CreateRepresentation()
        {
            if (HasRepresentation)
                return;

            DxfDocument document = BlockReference.Document;

            // Todo: Create anonymous block
            List<int> usedIDs = new List<int>();
            foreach(Block block in document.Blocks)
            {
                if(block.Name.StartsWith("*U"))
                {
                    string id = block.Name.Remove(0, 2);
                    if (int.TryParse(id,  out int numId))
                        usedIDs.Add(numId);
                }
            }
            int newId = usedIDs.Max() + 1;

            RepresentationBlock = new Block($"*U" + newId.ToString(CultureInfo.InvariantCulture), null, null, false);
            BlockReference.Block = RepresentationBlock;

            // clone entities
            foreach (var entity in DynamicBlock.Entities)
            {
                EntityObject newEntity = entity.Clone() as EntityObject;
                RepresentationBlock.Entities.Add(newEntity);
            }

            // Add extended information
            DocumentDictionary appDataCache = new DocumentDictionary();
            EnhancedBlockData = new DocumentDictionary();
            EnhancedBlockHistory = new XRecord();

            BlockRepresentationData repData = new BlockRepresentationData("ACDB_BLOCKREPRESENTATION_DATA");
            repData.BlockRecord = DynamicBlock.Record;

            document.Blocks.Add(RepresentationBlock);

            var representationBlockDict = BlockReference.CreateExtensionDictionary();
            var representationDict = new DocumentDictionary();

            representationBlockDict.AddHardReference("AcDbBlockRepresentation", representationDict);
            representationDict.AddHardReference("AcDbRepData", repData);
            representationDict.AddHardReference("AppDataCache", appDataCache);

            appDataCache.AddHardReference("ACAD_ENHANCEDBLOCKDATA", EnhancedBlockData);
            appDataCache.AddHardReference("ACAD_ENHANCEDBLOCKHISTORY", EnhancedBlockHistory);

            // Init History Header
            EnhancedBlockHistory.Entries.Add(new XRecordEntry(1070, (short)3));

            // Collect Properties
            CreateProperties();
        }
        private void CreateProperties()
        {
            _properties.Clear();
            foreach (var node in EvalGraph.Nodes.Select(n => n.Expression))
            {
                if (node is BlockParameter blockParam)
                {
                    var properties = PropertyFactory.CreatePropertyWrappers(blockParam, this);
                    _properties.AddRange(properties);
                }
            }
        }

        public bool HasHistory => EnhancedBlockHistory != null && EnhancedBlockHistory.Entries.Count > 2;

        public List<BlockParameter> ChangedNodes { get; internal set; } = new List<BlockParameter>();

        internal void AddHistoryChange(int nodeId, string parameter, object newValue)
        {
            EnhancedBlockHistory.Entries.Add(new XRecordEntry(1071, nodeId));
            EnhancedBlockHistory.Entries.Add(new XRecordEntry(300, parameter));


            if (newValue is short shortValue)
                EnhancedBlockHistory.Entries.Add(new XRecordEntry(70, shortValue));
            else if (newValue is string stringValue)
                EnhancedBlockHistory.Entries.Add(new XRecordEntry(1, stringValue));
            else if (newValue is double doubleValue)
                EnhancedBlockHistory.Entries.Add(new XRecordEntry(40, doubleValue));
            else if (newValue is Vector3 vectorValue)
            {
                EnhancedBlockHistory.Entries.Add(new XRecordEntry(11, vectorValue.X));
                EnhancedBlockHistory.Entries.Add(new XRecordEntry(21, vectorValue.Y));
                EnhancedBlockHistory.Entries.Add(new XRecordEntry(31, vectorValue.Z));
            }
            else
                throw new NotImplementedException();
        }

        internal void InitializeEvaluation()
        {
            ChangedNodes.Clear();

            foreach (var record in EnhancedBlockData.Names)
            {
                XRecord xrecord = EnhancedBlockData[record] as XRecord;
                {
                    int nodeId = int.Parse(record);
                    EvalExpr node = EvalGraph.GetNode(nodeId);
                    if (node != null)
                    {
                        XRecordValueReader reader = new XRecordValueReader(xrecord);
                        node?.RuntimeDataIn(reader);
                    }
                }
            }
        }

        public void CommitEvaluation()
        {
            List<int> nodesToActive = ChangedNodes.Select(n => n.NodeId).ToList();
            //List<int> activatedNodes = new List<int>();

            var context = new BlockEvaluationContext(this);

            List<int> gripNodes = new List<int>();
            EvalGraph.GetSuperNodes(nodesToActive, gripNodes);

            EvalGraph.Eval(nodesToActive, context);
            EvalGraph.Eval(nodesToActive, context, EvalStep.Commit);

            // save Runtimedata
            EnhancedBlockData.Clear();

            foreach (var node in EvalGraph.Nodes)
            {
                XRecord xrecord = new XRecord();
                XRecordValueWriter writer = new XRecordValueWriter(xrecord);
                node.Expression.RuntimeDataOut(writer);

                // Only header information (4 entries) so skip serialzation of runtime data
                if (xrecord.Entries.Count > 4)
                {
                    EnhancedBlockData.AddHardReference(
                   node.Id.ToString(CultureInfo.InvariantCulture),
                   xrecord
                   );
                }
            }
        }

    }
}
