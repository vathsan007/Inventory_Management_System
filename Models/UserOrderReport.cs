namespace WebapiProject.Models
{
    public class UserOrderReport
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string ProductName { get; set; }
        public int TotalOrderedQuantity { get; set; }
    }
}
