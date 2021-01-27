using netDxf.Blocks.Dynamic.Attributes;
using netDxf.Blocks.Dynamic.Util;
using netDxf.IO;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace netDxf.Blocks.Dynamic
{
    public class NodeEntry
    {
        private EvalGraph _parentGraph;
        private EvalExpr _expression = null;
        public NodeEntry(EvalGraph parentGraph)
        {
            _parentGraph = parentGraph;
        }

        public int Index { get; set; }
        public int Id { get; set; }
        public int I32 { get; set; }
        internal string ExprId { get; set; }
        public EvalExpr Expression
        {
            get { return _expression; }
            internal set
            {
                _expression = value;
            }
        }
        public int FirstIn { get; set; }
        public int LastIn { get; set; }
        public int FirstOut { get; set; }
        public int LastOut { get; set; }
    }

    public class EdgeEntry
    {
        private EvalGraph _parentGraph;
        public EdgeEntry(EvalGraph parentGraph)
        {
            _parentGraph = parentGraph;
        }
        public int EdgeId { get; set; }
        public int Flags { get; set; }
        public int RefCount { get; set; }
        public int IdFrom { get; set; }
        public int IdTo { get; set; }
        public int PrevIn { get; set; }
        public int NextIn { get; set; }
        public int PrevOut { get; set; }
        public int NextOut { get; set; }
        public int Reverse { get; set; }
    }

    public class IntListComparer : IEqualityComparer<List<int>>
    {
        public bool Equals(List<int> x, List<int> y)
        {
            if (x.Count != y.Count)
            {
                return false;
            }
            for (int i = 0; i < x.Count; i++)
            {
                if (x[i] != y[i])
                {
                    return false;
                }
            }
            return true;
        }

        public int GetHashCode(List<int> obj)
        {
            int result = 17;
            for (int i = 0; i < obj.Count; i++)
            {
                unchecked
                {
                    result = result * 23 + obj[i];
                }
            }
            return result;
        }
    }


    [AcadClassName("AcDbEvalGraph")]
    public class EvalGraph : DbObject
    {
        public EvalGraph(string codename) : base(codename) { }
        public EdgeEntry[] Edges { get; set; }
        public NodeEntry[] Nodes { get; set; }
        public int LastNode { get; set; }
        private NodeEntry ReadEvalNode(ICodeValueReader reader)
        {
            NodeEntry nodeEntry = new NodeEntry(this);
            List<int> entries = new List<int>(4);

            while (entries.Count != 4)
            {
                switch (reader.Code)
                {
                    case 91:
                        nodeEntry.Index = reader.ReadInt();
                        reader.Next();
                        break;
                    case 92:
                        entries.Add(reader.ReadInt());
                        if (entries.Count == 4)
                            break;
                        reader.Next();
                        break;
                    case 93:
                        nodeEntry.I32 = reader.ReadInt();
                        reader.Next();
                        break;
                    case 95:
                        nodeEntry.Id = reader.ReadInt();
                        reader.Next();
                        break;
                    case 360:
                        nodeEntry.ExprId = reader.ReadHex();
                        reader.Next();
                        break;
                    default:
                        Debug.Assert(!(reader.Code >= 1000 && reader.Code <= 1071), "The extended data of an entity must start with the application registry code.");
                        Debug.Assert(false);
                        reader.Next();
                        break;
                }
            }

            Debug.Assert(entries.Count == 4);

            nodeEntry.FirstIn = entries[0];
            nodeEntry.LastIn = entries[1];
            nodeEntry.FirstOut = entries[2];
            nodeEntry.LastOut = entries[3];

            return nodeEntry;
        }

        internal void Eval(List<int> evalNodes, BlockEvaluationContext context, EvalStep step = EvalStep.Update)
        {
            // Get all Nodes that could be active (Union of all SubGraphs)
            HashSet<int> toProcessNodes = new HashSet<int>();

            for (int i = 0; i < evalNodes.Count; i++)
            {
                List<int> subNodes = new List<int>();
                GetSubNodes(new List<int> { evalNodes[i] }, subNodes);

                toProcessNodes.UnionWith(subNodes);
            }

            // Kind of naiive TopoSort
            // TODO: Optimize this code!
            List<int> nodes = new List<int>(toProcessNodes);
            List<int> preCount = nodes.Select(i => GetPredecessorCount(i, toProcessNodes)).ToList();

            bool isDone = false;
            bool isLoop = true;
            do
            {
                isDone = true;
                isLoop = true;

                for (int i = 0; i < nodes.Count; i++)
                {
                    if (preCount[i] > 0)
                        isDone = false;


                    if (preCount[i] == 0)
                    {
                        NodeEntry entry = GetNodeEntry(nodes[i]);
                        bool result = entry.Expression.Eval(step, context);
                        isLoop = false;


                        // Toposort: Decrease count of successors
                        List<int> successors = new List<int>();
                        GetSuccessors(nodes[i], successors);
                        foreach (int successor in successors)
                        {
                            int nodeIndex = nodes.IndexOf(successor);
                            preCount[nodeIndex]--;
                        }

                        preCount[i] = -1;
                    }
                }
            } while (!isDone && !isLoop);
        }
        private int GetPredecessorCount(int nodeId, HashSet<int> processingNodes)
        {
            NodeEntry entry = GetNodeEntry(nodeId);
            int predecessorCount = 0;
            for (int i = entry.FirstIn; i <= entry.LastIn && i != -1; i = Edges[i].NextIn)
            {
                int idFrom = Nodes[Edges[i].IdFrom].Id;
                if (processingNodes.Contains(idFrom))
                {
                    predecessorCount++;
                    break;
                }
            }
            return predecessorCount;
        }

        private int GetSuccessors(int nodeId, List<int> successors)
        {
            int count = 0;
            NodeEntry entry = GetNodeEntry(nodeId);
            for (int i = entry.FirstOut; i <= entry.LastOut && i != -1; i = Edges[i].NextOut)
            {
                successors?.Add(Nodes[Edges[i].IdTo].Id);
                count++;
            }
            return count;
        }

        internal void Activate(List<int> nodesToActive, List<int> activatedNodes)
        {
            Stack<int> nodesToProcess = new Stack<int>(nodesToActive);

            while (nodesToProcess.Any())
            {
                int nodeId = nodesToProcess.Pop();
                activatedNodes.Add(nodeId);

                NodeEntry entry = GetNodeEntry(nodeId);

                for (int i = entry.FirstOut; i <= entry.LastOut && i != -1;)
                {
                    EdgeEntry edge = Edges[i];
                    i = edge.NextOut;
                    nodesToProcess.Push(Nodes[edge.IdTo].Id);

                }
            }
        }

        /// <summary>
        /// All nodes down the EvalTree, in dependency order
        /// </summary>
        /// <param name="inputNodes"></param>
        /// <param name="descendentNodes"></param>
        internal void GetSubNodes(List<int> inputNodes, List<int> descendentNodes)
        {
            Stack<int> nodesToProcess = new Stack<int>(inputNodes);

            while (nodesToProcess.Any())
            {
                int nodeId = nodesToProcess.Pop();
                descendentNodes.Add(nodeId);

                NodeEntry entry = GetNodeEntry(nodeId);

                for (int i = entry.FirstOut; i <= entry.LastOut && i != -1;)
                {
                    EdgeEntry edge = Edges[i];
                    i = edge.NextOut;
                    nodesToProcess.Push(Nodes[edge.IdTo].Id);

                }
            }
        }

        /// <summary>
        /// All nodes up the EvalTree, in dependency order
        /// </summary>
        /// <param name="inputNodes"></param>
        /// <param name="precedentNodes"></param>
        internal void GetSuperNodes(List<int> inputNodes, List<int> precedentNodes)
        {
            Stack<int> nodesToProcess = new Stack<int>(inputNodes);

            while (nodesToProcess.Any())
            {
                int nodeId = nodesToProcess.Pop();
                precedentNodes.Add(nodeId);

                NodeEntry entry = GetNodeEntry(nodeId);

                for (int i = entry.FirstIn; i <= entry.LastIn && i != -1;)
                {
                    EdgeEntry edge = Edges[i];
                    i = edge.NextOut;
                    nodesToProcess.Push(Nodes[edge.IdFrom].Id);

                }
            }
        }

        private NodeEntry GetNodeEntry(int nodeId)
        {
            return Nodes.FirstOrDefault(n => n.Id == nodeId);
        }

        private EdgeEntry ReadEvalEdge(ICodeValueReader reader)
        {
            EdgeEntry edgeEntry = new EdgeEntry(this);
            List<int> entries = new List<int>(4);
            List<int> fromTo = new List<int>(2);
            while (entries.Count != 6)
            {
                switch (reader.Code)
                {
                    case 91:
                        fromTo.Add(reader.ReadInt());
                        reader.Next();
                        break;
                    case 92:
                        entries.Add(reader.ReadInt());
                        if (entries.Count == 6)
                            break;
                        reader.Next();
                        break;
                    case 93:
                        edgeEntry.Flags = reader.ReadInt();
                        reader.Next();
                        break;
                    case 94:
                        edgeEntry.RefCount = reader.ReadInt();
                        reader.Next();
                        break;
                    default:
                        Debug.Assert(!(reader.Code >= 1000 && reader.Code <= 1071), "The extended data of an entity must start with the application registry code.");
                        reader.Next();
                        break;
                }
            }

            Debug.Assert(entries.Count == 6);
            Debug.Assert(fromTo.Count == 2);

            edgeEntry.IdFrom = fromTo[0];
            edgeEntry.IdTo = fromTo[1];

            edgeEntry.EdgeId = entries[0];
            edgeEntry.PrevIn = entries[1];
            edgeEntry.NextIn = entries[2];
            edgeEntry.PrevOut = entries[3];
            edgeEntry.NextOut = entries[4];
            edgeEntry.Reverse = entries[5];
            return edgeEntry;
        }
        internal override void DXFOutLocal(ICodeValueWriter writer)
        {
            base.DXFOutLocal(writer);
            WriteClassBegin(writer, "AcDbEvalGraph");

            writer.Write(96, LastNode);
            writer.Write(97, LastNode);

            for (int i = 0; i < Nodes.Length; i++)
            {
                NodeEntry nodeEntry = Nodes[i];
                writer.Write(91, nodeEntry.Index);
                writer.Write(93, nodeEntry.I32);
                writer.Write(95, nodeEntry.Id);
                writer.Write(360, nodeEntry.ExprId);

                writer.Write(92, nodeEntry.FirstIn);
                writer.Write(92, nodeEntry.LastIn);
                writer.Write(92, nodeEntry.FirstOut);
                writer.Write(92, nodeEntry.LastOut);
            }

            for (int i = 0; i < Edges.Length; i++)
            {
                EdgeEntry edge = Edges[i];
                writer.Write(92, edge.EdgeId);
                writer.Write(93, edge.Flags);
                writer.Write(94, edge.RefCount);

                writer.Write(91, edge.IdFrom);
                writer.Write(91, edge.IdTo);

                writer.Write(92, edge.PrevIn);
                writer.Write(92, edge.NextIn);
                writer.Write(92, edge.PrevOut);
                writer.Write(92, edge.NextOut);
                writer.Write(92, edge.Reverse);
            }
        }

        internal override void DXFInLocal(ICodeValueReader reader)
        {
            base.DXFInLocal(reader);
            ReadClassBegin(reader, "AcDbEvalGraph");

            List<NodeEntry> nodes = new List<NodeEntry>();
            List<EdgeEntry> edges = new List<EdgeEntry>();

            while (reader.Code != 0)
            {
                switch (reader.Code)
                {
                    case 91:
                        NodeEntry evalnode = ReadEvalNode(reader);
                        nodes.Add(evalnode);
                        reader.Next();
                        break;
                    case 92:
                        EdgeEntry evalEdge = ReadEvalEdge(reader);
                        edges.Add(evalEdge);
                        reader.Next();
                        break;
                    case 96:
                        LastNode = reader.ReadInt();
                        reader.Next();
                        break;
                    case 97:
                        Debug.Assert(reader.ReadInt() == LastNode);
                        reader.Next();
                        break;
                    default:
                        Debug.Assert(!(reader.Code >= 1000 && reader.Code <= 1071), "The extended data of an entity must start with the application registry code.");
                        reader.Next();
                        break;
                }
            }

            Edges = edges.ToArray();
            Nodes = nodes.ToArray();
        }

        public override IEnumerable<string> GetHardHandles()
        {
            return base.GetHardHandles().Concat(Nodes.Select(n => n.ExprId)).ToArray();
        }
        public override void SetHardHandles(IEnumerable<DxfObject> objects)
        {
            var dxfObjects = objects.TakeLast(Nodes.Count());
            for (int i = 0; i < dxfObjects.Count(); i++)
            {
                Nodes[i].Expression = (EvalExpr)dxfObjects.ElementAt(i);
            }

            base.SetHardHandles(objects.Take(objects.Count() - Nodes.Count()));
        }

        public override IEnumerable<DxfObject> GetHardReferences()
        {
            return base.GetHardReferences().Concat(Nodes.Select(n => n.Expression));
        }

        internal EvalExpr GetNode(int id)
        {
            return Nodes.FirstOrDefault(n => n.Id == id)?.Expression as EvalExpr;
        }
    }
}
