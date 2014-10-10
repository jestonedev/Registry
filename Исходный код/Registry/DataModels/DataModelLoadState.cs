using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Registry.DataModels
{
    public enum DataModelLoadState
    {
        BeforeLoad,
        Loading,
        ErrorLoad,
        SuccessLoad
    }
}
