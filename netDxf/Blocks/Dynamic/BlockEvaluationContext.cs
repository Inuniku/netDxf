using netDxf.Entities;
using netDxf.Objects;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace netDxf.Blocks.Dynamic
{
    public class BlockEvaluationContext
    {
        public BlockEvaluationContext(DynamicBlockReferenceContext context)
        {
            EvalGraph = context.EvalGraph;
            DynamicBlock = context.DynamicBlock;
            RepresentationBlock = context.RepresentationBlock;
            BlockReference = context.BlockReference;
            Visibility = context.Visibility;
        }

        internal Block DynamicBlock { get; }
        internal Block RepresentationBlock { get; }
        internal Insert BlockReference { get; }
        internal DocumentDictionary EnhancedBlockData { get; }
        internal EvalGraph EvalGraph { get; }
        internal BlockVisibilityParameter Visibility { get; }

        internal List<int> AdditionalNodes = new List<int>();

        // TODO: very slow way to do for now, Build orig => representation dict in advance
        internal void TransformRepresentation(string[] selection, Matrix4 transform)
        {
            // Move Actions
            var graphObjects = EvalGraph.Expressions.Where(e => selection.Contains(e.Handle));
            foreach (var graphObject in graphObjects)
            {
                BlockElement blockParam = (BlockElement)graphObject;
                if (blockParam is Block1PtParameter block1PtParam)
                {
                    block1PtParam.UpdatedPoint = TransformPoint(transform, block1PtParam.UpdatedPoint);
                    block1PtParam.Point = TransformPoint(transform, block1PtParam.Point);
                }
                else
                if (blockParam is Block2PtParameter block2PtParam)
                {
                    block2PtParam.Point1 = TransformPoint(transform, block2PtParam.Point1);
                    block2PtParam.UpdatedPoint1 = TransformPoint(transform, block2PtParam.UpdatedPoint1);
                    block2PtParam.Point2 = TransformPoint(transform, block2PtParam.Point2);
                    block2PtParam.UpdatedPoint2 = TransformPoint(transform, block2PtParam.UpdatedPoint2);
                }
                else
                if (blockParam is BlockGrip grip)
                {
                    grip.UpdatedLocation = TransformPoint(transform, grip.UpdatedLocation);
                    grip.Location = TransformPoint(transform, grip.Location);
                }
                else
                    throw new NotImplementedException();

                if (!AdditionalNodes.Contains(blockParam.NodeId))
                    AdditionalNodes.Add(blockParam.NodeId);
            }

            // Move Entities
            Debug.Assert(DynamicBlock.Entities.Count == RepresentationBlock.Entities.Count);

            for (int i = 0; i < selection.Length; i++)
            {
                string handle = selection[i];

                EntityObject originalEntity = DynamicBlock.Document.GetObjectByHandle(handle) as EntityObject;
                if(originalEntity == null)
                {
                    //Object might be an attribdef
                    var attDef = DynamicBlock.AttributeDefinitions.FirstOrDefault(a => a.Value.Handle == handle);
                    if(attDef.Value != null)
                    {
                        AttributeDefinition def = RepresentationBlock.AttributeDefinitions[attDef.Value.Tag];
                        def.Position = TransformPoint(transform, def.Position);

                        Entities.Attribute att = BlockReference.Attributes.Single(a => a.Tag == def.Tag);
                        att.Position = TransformPoint(transform, att.Position);
                    }
                    continue;
                }

                int index = DynamicBlock.Entities.IndexOf(originalEntity);
                EntityObject entity = RepresentationBlock.Entities[index];
                RepresentationBlock.Entities[index].TransformBy(transform);
                if(entity is Dimension dim)
                {
                    if(dim.Block !=  null)
                    {
                        //TODO, How to transform anonymous styles?
                        foreach(var element in dim.Block.Entities)
                        {
                            element.TransformBy(transform);
                        }
                    }
                    // TODO!
                    //throw new NotImplementedException();
                    //entity.TransformBy(Matrix4.RotationZ(180));
                }


            }
        }

        private Vector3 TransformPoint(Matrix4 transformation, Vector3 position)
        {
            Matrix3 m = new Matrix3(transformation.M11, transformation.M12, transformation.M13,
                        transformation.M21, transformation.M22, transformation.M23,
                        transformation.M31, transformation.M32, transformation.M33);
            Vector3 v = new Vector3(transformation.M14, transformation.M24, transformation.M34);

            return m * position + v;
        }

        internal void TransformRepresentationBy(string[] handles, Matrix4 transform)
        {
            TransformRepresentation(handles, transform);
        }

        internal void SetRepresentationVisibility(string entityHandle, bool isVisible)
        {
            EntityObject originalEntity = DynamicBlock.Document.GetObjectByHandle(entityHandle) as EntityObject;

            if (originalEntity != null)
            {
                int index = DynamicBlock.Entities.IndexOf(originalEntity);
                EntityObject entity = RepresentationBlock.Entities[index];
                entity.IsVisible = isVisible;
            }
            else
            {
                // TODO set visibility
                var attdef = DynamicBlock.AttributeDefinitions.Select(e => e.Value).SingleOrDefault(a => a.Handle == entityHandle);
                if (attdef != null)
                {
                    var repAttdef = RepresentationBlock.AttributeDefinitions.Select(e => e.Value).SingleOrDefault(a => a.Tag == attdef.Tag);

                    repAttdef.IsVisible = isVisible;
                    return;
                }

                
                //EvalExpr expr = EvalGraph.Expressions.Single(e => e.Handle == entityHandle);
                Debug.WriteLine("Unable to set visibility of " + entityHandle);
            }
                
        }
    }
}