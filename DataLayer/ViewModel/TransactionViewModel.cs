
using System;

namespace DataBoard.ViewModel
{
    public class TransactionViewModel
    {
        public int CustomerID { get; set; }

        public string? Name { get; set; }
        public decimal Amount { get; set; }
        public DateTime TransactionDate { get; set; }
        public string? TransactionType { get; set; }
    }


}

