using WebapiProject.Models;

using Microsoft.EntityFrameworkCore;

using System.Data;

using Microsoft.Data.SqlClient;

namespace WebapiProject.Repository

{

    public class ReportRepository : IReportRepository

    {

        private readonly ApplicationDbContext db;

        public ReportRepository(ApplicationDbContext db)

        {

            this.db = db;

        }

        public IEnumerable<object> GenerateInventoryReport(DateTime startDate, DateTime endDate)

        {

            if (startDate > endDate)

            {

                throw new System.Exception("Start date cannot be later than end date.");

            }

            // Execute the raw SQL query and get the result set

            var stockReport = db.Stocks

                .FromSqlRaw("EXEC GenerateInventoryReport @StartDate = {0}, @EndDate = {1}", startDate, endDate)

                .AsEnumerable() // Convert to IEnumerable to perform LINQ operations in memory

                .ToList();

            // Join the result set with the Products table to get the product name

            var report = stockReport

                .Join(db.Products,

                      stock => stock.ProductId,

                      product => product.ProductId,

                      (stock, product) => new

                      {

                          stock.StockId,

                          stock.ProductId,

                          ProductName = product.ProductName,

                          stock.TotalStock,

                          stock.Date

                      })

                .ToList();

            if (report == null || !report.Any())

            {

                throw new System.Exception("No inventory data found for the given date range.");

            }

            return report;

        }

        public IEnumerable<object> GenerateOrderReport(DateTime startDate, DateTime endDate)

        {

            if (startDate > endDate)

            {

                throw new System.Exception("Start date cannot be later than end date.");

            }

            var report = db.Orders

                .FromSqlRaw("EXEC GenerateOrderReport @StartDate = {0}, @EndDate = {1}", startDate, endDate)

                .Select(o => new

                {

                    o.OrderId,

                    o.OrderDate,

                    o.ProductId,

                    ProductName = "Product Name", // Replace with actual product name lookup

                    o.OrderedQuantity

                })

                .ToList();

            if (report == null || !report.Any())

            {

                throw new System.Exception("No order data found for the given date range.");

            }

            return report;

        }

        public IEnumerable<object> GenerateLowStockItemsReport()

        {

            var report = db.Stocks

                .FromSqlRaw("EXEC GenerateLowStockItemsReport")

                .Select(s => new

                {

                    s.ProductId,

                    ProductName = "Product Name", // Replace with actual product name lookup

                    s.TotalStock

                })

                .ToList();

            if (report == null || !report.Any())

            {

                throw new System.Exception("No low stock items found.");

            }

            return report;

        }

        public object GenerateMostOrderedProductReport(DateTime startDate, DateTime endDate)

        {

            if (startDate > endDate)

            {

                throw new System.Exception("Start date cannot be later than end date.");

            }

            var report = db.Orders

                .FromSqlRaw("EXEC GenerateMostOrderedProductReport @StartDate = {0}, @EndDate = {1}", startDate, endDate)

                .Select(o => new

                {

                    o.ProductId,

                    TotalOrdered = o.OrderedQuantity // Adjust as needed based on your stored procedure's output

                })

                .FirstOrDefault();

            if (report == null)

            {

                throw new System.Exception("No order data found for the given date range.");

            }

            return report;

        }

        public IEnumerable<UserOrderReport> GetUserOrderReport()

        {

            var reports = new List<UserOrderReport>();

            using (var connection = db.Database.GetDbConnection())

            {

                using (var command = connection.CreateCommand())

                {

                    command.CommandText = "GetUserOrderReport";

                    command.CommandType = CommandType.StoredProcedure;

                    connection.Open();

                    using (var reader = command.ExecuteReader())

                    {

                        while (reader.Read())

                        {

                            var report = new UserOrderReport

                            {

                                UserId = reader.GetInt32(0),

                                UserName = reader.GetString(1),

                                ProductName = reader.GetString(2),

                                TotalOrderedQuantity = reader.GetInt32(3)

                            };

                            reports.Add(report);

                        }

                    }

                }

            }

            return reports;

        }


        public IEnumerable<UserOrderDetails> GetUserOrderDetails(int userId)

        {

            var reports = new List<UserOrderDetails>();

            using (var connection = db.Database.GetDbConnection())

            {

                using (var command = connection.CreateCommand())

                {

                    command.CommandText = "GetUserOrderDetails";

                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.Add(new SqlParameter("@UserId", userId));

                    connection.Open();

                    using (var reader = command.ExecuteReader())

                    {

                        while (reader.Read())

                        {

                            var report = new UserOrderDetails

                            {

                                UserId = reader.GetInt32(0),

                                UserName = reader.GetString(1),

                                ProductName = reader.GetString(2),

                                OrderedQuantity = reader.GetInt32(3)

                            };

                            reports.Add(report);

                        }

                    }

                }

            }

            return reports;

        }

        public IEnumerable<StockLevelReport> GetStockLevelReport()

        {

            var reports = new List<StockLevelReport>();

            using (var connection = db.Database.GetDbConnection())

            {

                using (var command = connection.CreateCommand())

                {

                    command.CommandText = "GetStockLevelReport";

                    command.CommandType = CommandType.StoredProcedure;

                    connection.Open();

                    using (var reader = command.ExecuteReader())

                    {

                        while (reader.Read())

                        {

                            var report = new StockLevelReport

                            {

                                ProductId = reader.GetString(0),

                                ProductName = reader.GetString(1),

                                AvailableQuantity = reader.GetInt64(2),

                                SupplierName = reader.GetString(3)

                            };

                            reports.Add(report);

                        }

                    }

                }

            }

            return reports;

        }

        public IEnumerable<LowStockReport> GetLowStockReport()

        {

            var reports = new List<LowStockReport>();

            using (var connection = db.Database.GetDbConnection())

            {

                using (var command = connection.CreateCommand())

                {

                    command.CommandText = "GetLowStockReport";

                    command.CommandType = CommandType.StoredProcedure;

                    connection.Open();

                    using (var reader = command.ExecuteReader())

                    {

                        while (reader.Read())

                        {

                            var report = new LowStockReport

                            {

                                ProductId = reader.GetString(0),

                                ProductName = reader.GetString(1),

                                AvailableQuantity = reader.GetInt64(2),

                                SupplierName = reader.GetString(3)

                            };

                            reports.Add(report);

                        }

                    }

                }

            }

            return reports;

        }

        public IEnumerable<SalesReport> GetSalesReport(DateTime startDate, DateTime endDate)

        {

            var reports = new List<SalesReport>();

            using (var connection = db.Database.GetDbConnection())

            {

                using (var command = connection.CreateCommand())

                {

                    command.CommandText = "GetSalesReport";

                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.Add(new SqlParameter("@StartDate", startDate));

                    command.Parameters.Add(new SqlParameter("@EndDate", endDate));

                    connection.Open();

                    using (var reader = command.ExecuteReader())

                    {

                        while (reader.Read())

                        {

                            var report = new SalesReport

                            {

                                ProductId = reader.GetString(0),

                                ProductName = reader.GetString(1),

                                QuantitySold = reader.GetInt32(2),

                                TotalSalesAmount = reader.GetDecimal(3),

                                SalesDate = reader.GetDateTime(4)

                            };

                            reports.Add(report);

                        }

                    }

                }

            }

            return reports;

        }

        public IEnumerable<SupplierPerformanceReport> GetSupplierPerformanceReport()

        {

            var reports = new List<SupplierPerformanceReport>();

            using (var connection = db.Database.GetDbConnection())

            {

                using (var command = connection.CreateCommand())

                {

                    command.CommandText = "GetSupplierPerformanceReport";

                    command.CommandType = CommandType.StoredProcedure;

                    connection.Open();

                    using (var reader = command.ExecuteReader())

                    {

                        while (reader.Read())

                        {

                            var report = new SupplierPerformanceReport

                            {

                                SupplierId = reader.GetInt32(0),

                                SupplierName = reader.GetString(1),

                                NumberOfDeliveries = reader.GetInt32(2)

                            };

                            reports.Add(report);

                        }

                    }

                }

            }

            return reports;

        }

        public IEnumerable<OrderHistoryReport> GetOrderHistoryReport()

        {

            var reports = new List<OrderHistoryReport>();

            using (var connection = db.Database.GetDbConnection())

            {

                using (var command = connection.CreateCommand())

                {

                    command.CommandText = "GetOrderHistoryReport";

                    command.CommandType = CommandType.StoredProcedure;

                    connection.Open();

                    using (var reader = command.ExecuteReader())

                    {

                        while (reader.Read())

                        {

                            var report = new OrderHistoryReport

                            {

                                OrderId = reader.GetInt32(0),

                                UserId = reader.GetInt32(1),

                                ProductId = reader.GetString(2),

                                OrderedQuantity = reader.GetInt32(3),

                                OrderDate = reader.GetDateTime(4),

                                Status = reader.GetString(5)

                            };

                            reports.Add(report);

                        }

                    }

                }

            }

            return reports;

        }

        public IEnumerable<ProductDemandReport> GetProductDemandReport(DateTime startDate, DateTime endDate)

        {

            var reports = new List<ProductDemandReport>();

            using (var connection = db.Database.GetDbConnection())

            {

                using (var command = connection.CreateCommand())

                {

                    command.CommandText = "GetProductDemandReport";

                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.Add(new SqlParameter("@StartDate", startDate));

                    command.Parameters.Add(new SqlParameter("@EndDate", endDate));

                    connection.Open();

                    using (var reader = command.ExecuteReader())

                    {

                        while (reader.Read())

                        {

                            var report = new ProductDemandReport

                            {

                                ProductId = reader.GetString(0),

                                ProductName = reader.GetString(1),

                                QuantityOrdered = reader.GetInt32(2),

                                OrderDate = reader.GetDateTime(3),

                                UserId = reader.GetInt32(4)

                            };

                            reports.Add(report);

                        }

                    }

                }

            }

            return reports;

        }

    }

}
