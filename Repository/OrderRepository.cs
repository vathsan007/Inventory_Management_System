using Microsoft.EntityFrameworkCore;
using WebapiProject.Models;

namespace WebapiProject.Repository
{
    public class OrderRepository : IOrderRepository
    {
        private readonly ApplicationDbContext db;
        private readonly IStockRepository _stockRepository;

        public OrderRepository(IStockRepository stockRepository, ApplicationDbContext db)
        {
            this.db = db;
            _stockRepository = stockRepository;
        }

        public void PlaceOrder(Order order)
        {
            var stock = db.Stocks.FirstOrDefault(s => s.ProductId == order.ProductId);
            if (stock == null)
            {
                throw new System.Exception("Product not found.");
            }

            if (order.OrderedQuantity > stock.TotalStock)
            {
                throw new System.Exception("Ordered quantity exceeds available stock.");
            }
            order.Status = "Placed";
            db.Database.ExecuteSqlRaw("EXEC PlaceOrder @p0, @p1, @p2",
                order.ProductId, order.UserId, order.OrderedQuantity);
        }

        public void UpdateOrderStatus(int orderId, string status)
        {
            var order = db.Orders.Find(orderId);
            if (order == null)
            {
                throw new KeyNotFoundException("Order not found.");
            }

            db.Database.ExecuteSqlRaw("EXEC UpdateOrderStatus @p0, @p1", orderId, status);
        }

        public List<Order> GetAllOrders()
        {
            return db.Orders.ToList();
        }

        public Order GetOrderById(int orderId)
        {
            return db.Orders.FirstOrDefault(o => o.OrderId == orderId);
        }
        public void CancelOrder(int orderId)
        {
            var order = db.Orders.Find(orderId);
            if (order == null)
            {
                throw new KeyNotFoundException("Order not found.");
            }

            db.Database.ExecuteSqlRaw("EXEC CancelOrder @p0", orderId);
        }
    }
}
