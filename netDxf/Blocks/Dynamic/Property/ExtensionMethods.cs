using netDxf.Entities;
using netDxf.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace netDxf.Blocks.Dynamic.Property
{
    public static class ExtensionMethods
    {
        public static bool IsDynamicBlock(this Insert blockReference)
        {
            if (blockReference.ExtensionDictionary == null)
                return false;

            var representationDict = blockReference.ExtensionDictionary["AcDbBlockRepresentation"] as DocumentDictionary;
            if (representationDict == null)
                return false;

            var repData = representationDict["AcDbRepData"] as BlockRepresentationData;
            if (repData == null)
                return false;


            return true;
        }


        public static DynamicBlockReferencePropertyCollection GetDynamicBlockReferencePropertyCollection(this Insert blockReference)
        {
            if(!blockReference.IsDynamicBlock())
            {
                return null;
            }


            return new DynamicBlockReferencePropertyCollection(blockReference);
        }


    }
}