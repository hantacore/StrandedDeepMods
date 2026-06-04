using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrandedDeepAnimatedFoliageMod
{
    class SeaweedBender : GrassBender
    {
        protected override bool IsUnderwaterObject()
        {
            return true;
        }
    }
}
