using netDxf.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace netDxf.Blocks.Dynamic.IO
{
    internal interface IResolveReader : ICodeValueReader
    {
        DxfObject ResolveHandle(string handle);
    }
}
