using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.Enums
{
    public enum InventoryLogType
    {
        Import = 0,
        SaleDeduction = 1,
        TransferOut = 2,
        TransferIn = 3,
        Adjustment = 4,
        Warning = 5
    }
}
