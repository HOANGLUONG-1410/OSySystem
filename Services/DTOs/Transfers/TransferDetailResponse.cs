using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.DTOs.Transfers
{
    public class TransferDetailResponse
    {
        public int TransferRequestDetailId { get; set; }

        public int IngredientId { get; set; }

        public string IngredientName { get; set; } = null!;

        public string Unit { get; set; } = null!;

        public decimal Quantity { get; set; }
    }
}
