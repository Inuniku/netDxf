﻿using netDxf.Blocks.Dynamic.Attributes;
using netDxf.IO;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace netDxf.Blocks.Dynamic
{
    [AcadClassName("AcDbBlockVisibilityGrip")]
    public class BlockVisibilityGrip : BlockGrip
    {
        public BlockVisibilityGrip(string codename) : base(codename) { }
        internal override void DXFOutLocal(ICodeValueWriter writer)
        {
            base.DXFOutLocal(writer);
            WriteClassBegin(writer, "AcDbBlockVisibilityGrip");
        }

        internal override void DXFInLocal(ICodeValueReader reader)
        {
            base.DXFInLocal(reader);
            ReadClassBegin(reader, "AcDbBlockVisibilityGrip");

            while (reader.Code != 0 && reader.Code != 100 && reader.Code != 1001)
            {
                switch (reader.Code)
                {
                    default:
                        Debug.Assert(false);
                        reader.Next();
                        break;
                }
            }
        }
    }
}
