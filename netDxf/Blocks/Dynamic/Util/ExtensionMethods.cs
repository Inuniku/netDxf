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
            // Might be a dynamic block without representation
            if (blockReference.ExtensionDictionary == null)
            {

                var dynBlockDict = blockReference.Block.Record.ExtensionDictionary;
                if (dynBlockDict == null)
                    return false;
                if (dynBlockDict.HasKey("ACAD_ENHANCEDBLOCK"))
                    return true;
            };

            var representationDict = blockReference.ExtensionDictionary["AcDbBlockRepresentation"] as DocumentDictionary;
            if (representationDict == null)
                return false;

            var repData = representationDict["AcDbRepData"] as BlockRepresentationData;
            if (repData == null)
                return false;


            return true;
        }

        public static DynamicBlockReferenceContext GetDynamicBlockReferenceContext(this Insert blockReference)
        {
            if(!blockReference.IsDynamicBlock())
            {
                return null;
            }

            return new DynamicBlockReferenceContext(blockReference);
        }
    }
}