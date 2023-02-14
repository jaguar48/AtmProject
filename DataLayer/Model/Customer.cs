namespace DataBoard.Model
{
    public class Customer
    {
        public int Id {get; set; }  
        public string? Name { get; set; }
        public decimal Balance { get; set; }
        public int AccountNumber { get; set; }
        public int Pin { get; set; }

    }
}
