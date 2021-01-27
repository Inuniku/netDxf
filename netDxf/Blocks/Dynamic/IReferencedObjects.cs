using System.Collections.Generic;

namespace netDxf.Blocks.Dynamic
{
    internal interface IReferencedHandles
    {
        IEnumerable<string> GetHardHandles();
    }
}