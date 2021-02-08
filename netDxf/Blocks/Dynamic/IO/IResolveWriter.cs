using netDxf.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace netDxf.Blocks.Dynamic.IO
{
    interface IResolveWriter : ICodeValueWriter
    {
        void WriteReferenceObject(short code, DxfObject refObject);
        void WriteReferenceObject(short code, string handle);
        void WriteOwnedObject(short code, DxfObject ownedObject);
    }
}
