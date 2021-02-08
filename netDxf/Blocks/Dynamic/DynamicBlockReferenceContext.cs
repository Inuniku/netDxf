using netDxf.Blocks.Dynamic.Property;
using netDxf.Entities;
using netDxf.Objects;
using netDxf.Tables;
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

        public Insert BlockReference { get; }
        public Block DynamicBlock { get; set; }
        public Block RepresentationBlock { get; set; }
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
            Visibility = EvalGraph.Expressions.OfType<BlockVisibilityParameter>().SingleOrDefault();

            CreateProperties();
        }

        public bool HasRepresentation => RepresentationBlock != null;

        public void CreateRepresentation()
        {
            if (HasRepresentation)
                return;

            DxfDocument document = BlockReference.Document;
            Dictionary<string, string> repEntityMap = new Dictionary<string, string>();


            // Todo: Create anonymous block
            string anonName = GetAnonymousBlockName(document, "*U");
            RepresentationBlock = new Block(anonName, null, null, false);
            document.Blocks.Add(RepresentationBlock);

            BlockReference.Block = RepresentationBlock;
            // clone entities
            foreach (var entity in DynamicBlock.Entities)
            {
                EntityObject newEntity = entity.Clone() as EntityObject;
                RepresentationBlock.Entities.Add(newEntity);
                repEntityMap.Add(entity.Handle, newEntity.Handle);

                if (newEntity is LinearDimension dim)
                {
                  
                    // TODO dimension code
                    string anonTextName = GetAnonymousBlockName(document, "*D");
                    Block block = new Block(anonTextName, null, null, false);
                    document.Blocks.Add(block);
                    dim.Block = block;

                    MText mText = new MText(dim.TextReferencePoint, dim.Style.TextHeight);
                    
                    //mText.Position = new Vector3(dim.TextReferencePoint.X, dim.TextReferencePoint.Y, 0);
                    mText.Layer = dim.Layer;
                    mText.Color = AciColor.Red;
                    mText.Style = dim.Style.TextStyle;
                    mText.AttachmentPoint = MTextAttachmentPoint.BottomCenter;

                    block.Entities.Add(mText);

                    Point p1 = new Point();
                    block.Entities.Add(p1);

                    Point p2 = new Point();
                    block.Entities.Add(p2);

                }
            }

            // clone Attdefs
            foreach (AttributeDefinition attdef in DynamicBlock.AttributeDefinitions.Select(e => e.Value))
            {
                AttributeDefinition newAttdef = (AttributeDefinition)attdef.Clone();
                RepresentationBlock.AttributeDefinitions.Add(newAttdef);
                repEntityMap.Add(attdef.Handle, newAttdef.Handle);
            }


            // Add extended information
            DocumentDictionary appDataCache = new DocumentDictionary();
            EnhancedBlockData = new DocumentDictionary();
            EnhancedBlockHistory = new XRecord();

            BlockRepresentationData repData = new BlockRepresentationData("ACDB_BLOCKREPRESENTATION_DATA");
            repData.BlockRecord = DynamicBlock.Record;


            var referenceBlockDict = BlockReference.CreateExtensionDictionary();
            var referenceDict = new DocumentDictionary();

            referenceBlockDict.AddHardReference("AcDbBlockRepresentation", referenceDict);
            referenceDict.AddHardReference("AcDbRepData", repData);
            referenceDict.AddHardReference("AppDataCache", appDataCache);

            appDataCache.AddHardReference("ACAD_ENHANCEDBLOCKDATA", EnhancedBlockData);
            appDataCache.AddHardReference("ACAD_ENHANCEDBLOCKHISTORY", EnhancedBlockHistory);
            // Init History Header (Version)
            EnhancedBlockHistory.Entries.Add(new XRecordEntry(1070, (short)3));


            // clone ExtensionDictionary
            DocumentDictionary extensionDictionary = DynamicBlock.Record.ExtensionDictionary;

            if (extensionDictionary != null)
            {
                var additionalEntries = extensionDictionary.Names.Except(new string[] { "ACAD_ENHANCEDBLOCK", "AcDbDynamicBlockPurgePreventer" });

                if (additionalEntries.Any())
                {
                    var additional = RepresentationBlock.Record.CreateExtensionDictionary();

                    foreach (var entry in additionalEntries)
                    {
                        DbObject entryObj = extensionDictionary[entry] as DbObject;
                        DbObject copyObject = (DbObject)entryObj.Clone();

                        Queue<string> origHandles = new Queue<string>();
                        copyObject.GetSoftHandles(origHandles);

                        Queue<string> clonedHandles = new Queue<string>(
                            origHandles.Select(h => repEntityMap.ContainsKey(h) ? repEntityMap[h] : h)
                            
                            );
                        copyObject.SetSoftHandles(clonedHandles);


                        additional.AddHardReference(entry, copyObject);
                    }
                }
            }
;
            // Update BlockRepTags of Entities
            foreach (var entity in RepresentationBlock.Entities)
            {
                if (entity.XData.ContainsAppId("ACDBBLOCKREPETAG"))
                {
                    var xDict = entity.XData["ACDBBLOCKREPETAG"];
                    var record = xDict.XDataRecord[2];
                    xDict.XDataRecord[2] = new XDataRecord(record.Code, entity.Handle);
                }
            }
            foreach (AttributeDefinition attdef in DynamicBlock.AttributeDefinitions.Select(e => e.Value))
            {
                Entities.Attribute att = BlockReference.Attributes.SingleOrDefault(a => a.Tag == attdef.Tag);
                if (att != null)
                {
                    foreach (XData data in attdef.XData.Values)
                    {
                        att.XData.Add((XData)data.Clone());
                    }
                }
            }

            // Update BlockRepTag of Reference
            XData blockRepBTag = new XData(new ApplicationRegistry("AcDbBlockRepBTag"));
            blockRepBTag.XDataRecord.Add(new XDataRecord(XDataCode.Int16, (short)1));
            blockRepBTag.XDataRecord.Add(new XDataRecord(XDataCode.DatabaseHandle, DynamicBlock.Record.Handle));
            RepresentationBlock.Record.XData.Add(blockRepBTag);

            // Collect Properties
            CreateProperties();
        }

        private static string GetAnonymousBlockName(DxfDocument document, string prefix)
        {
            List<int> usedIDs = new List<int>();
            foreach (Block block in document.Blocks)
            {
                if (block.Name.StartsWith(prefix))
                {
                    string id = block.Name.Remove(0, prefix.Length);
                    if (int.TryParse(id, out int numId))
                        usedIDs.Add(numId);
                }
            }

            int usedIDMax = 0;
            if (usedIDs.Any())
            {
                usedIDMax = usedIDs.Max();
            }
            int newId = usedIDMax + 1;
            return prefix + newId.ToString(CultureInfo.InvariantCulture);
        }

        private void CreateProperties()
        {
            _properties.Clear();
            foreach (var node in EvalGraph.Expressions)
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
            //TODO: How to handle enums? Where to validate/convert data?


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
            
            foreach (var expr in EvalGraph.Expressions)
            {
                expr.InitializeRuntimeData();
            }

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

                        SaveRuntimeData();
        }

        protected void SaveRuntimeData()
        {
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
