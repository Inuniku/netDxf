using netDxf.Entities;
using netDxf.Objects;
using System;

namespace netDxf.Blocks.Dynamic
{
    public class BlockEvaluationContext
    {
        public BlockEvaluationContext(EvalGraph evalGraph)
        {
            EvalGraph = evalGraph;
        }

        private BlockRecord DynamicBlock { get; set; }
        private BlockRecord Representation { get; set; }
        private Insert BlockReference { get; set; }
        private DocumentDictionary EnhancedBlockData { get; set; }
        public EvalGraph EvalGraph { get; }

        internal void Begin()
        {
            throw new NotImplementedException();
        }
    }
}