using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.Models
{
    public class TransferRequestDetail
    {
        public int TransferRequestDetailId { get; set; }

        public int TransferRequestId { get; set; }
        public int IngredientId { get; set; }

        public decimal Quantity { get; set; }

        public TransferRequest TransferRequest { get; set; } = null!;
        public Ingredient Ingredient { get; set; } = null!;
    }
}
