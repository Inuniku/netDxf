using netDxf.Entities;
using netDxf.Objects;
using System;
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

        // TODO: very slow way to do for now, Build orig => representation dict in advance
        internal void TransformRepresentation(string[] selection, Action<EntityObject> transform)
        {
            // Move Actions
            var graphObjects = EvalGraph.Nodes.Where(n => selection.Contains(n.Expression.Handle)).Select(n => n.Expression);
            foreach (var graphObject in graphObjects)
            {
                //BlockParameter blockParam = (BlockParameter)graphObject;
            }

            // Move Entities
            Debug.Assert(DynamicBlock.Entities.Count == RepresentationBlock.Entities.Count);

            for (int i = 0; i < selection.Length; i++)
            {
                string handle = selection[i];

                EntityObject originalEntity = DynamicBlock.Document.GetObjectByHandle(handle) as EntityObject;
                int index = DynamicBlock.Entities.IndexOf(originalEntity);
                EntityObject entity = RepresentationBlock.Entities[index];
                transform(RepresentationBlock.Entities[index]);
            }
        }

        internal void TransformRepresentationBy(string[] handles, Matrix3 transform, Vector3 translate)
        {
            TransformRepresentation(handles, e => e.TransformBy(transform, translate));
        }

        internal void TransformRepresentationBy(string[] handles, Matrix4 transform)
        {
            TransformRepresentation(handles, e => e.TransformBy(transform));
        }

        internal void SetRepresentationVisibility(string entityHandle, bool isVisible)
        {
            EntityObject originalEntity = DynamicBlock.Document.GetObjectByHandle(entityHandle) as EntityObject;
            int index = DynamicBlock.Entities.IndexOf(originalEntity);
            EntityObject entity = RepresentationBlock.Entities[index];
            entity.IsVisible = isVisible;
        }
    }
}