
using BasicBilling.API.Controllers;
using BasicBilling.API.Data;
using BasicBilling.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Linq;
using Xunit;

namespace BasicBilling.API.Tests
{
    public class BillControllerTest
    {

        [Fact]
        public void CreateBill_ReturnsOkResult()
        {
            int period = 202301;
            var categoryWater = BillCategory.WATER;
            var billRequest = new BillRequest
            {
                Period = period,
                Category = categoryWater.ToString(),
            };

            var billExpected = new Bill
            {
                Id = 1,
                ClientId = 100,
                Period = period,
                Amount = 100.2m,
                State = BillState.Pending,
                Category = categoryWater,
            };

            var bills = new List<Bill>
            {
            };

            var clients = new List<Client>
            {
                new Client
                {
                    Id = 100,
                    Name = "Jhon",
                },
            };

            var mockBillDbSet = new Mock<DbSet<Bill>>();
            mockBillDbSet.As<IQueryable<Bill>>().Setup(m => m.Provider).Returns(bills.AsQueryable().Provider);
            mockBillDbSet.As<IQueryable<Bill>>().Setup(m => m.Expression).Returns(bills.AsQueryable().Expression);
            mockBillDbSet.As<IQueryable<Bill>>().Setup(m => m.ElementType).Returns(bills.AsQueryable().ElementType);
            mockBillDbSet.As<IQueryable<Bill>>().Setup(m => m.GetEnumerator()).Returns(bills.AsQueryable().GetEnumerator());
            mockBillDbSet.Setup(m => m.Add(It.IsAny<Bill>())).Callback((Bill bill) => bills.Add(bill));

            var mockClientDbSet = new Mock<DbSet<Client>>();
            mockClientDbSet.As<IQueryable<Client>>().Setup(m => m.Provider).Returns(clients.AsQueryable().Provider);
            mockClientDbSet.As<IQueryable<Client>>().Setup(m => m.Expression).Returns(clients.AsQueryable().Expression);
            mockClientDbSet.As<IQueryable<Client>>().Setup(m => m.ElementType).Returns(clients.AsQueryable().ElementType);
            mockClientDbSet.As<IQueryable<Client>>().Setup(m => m.GetEnumerator()).Returns(clients.AsQueryable().GetEnumerator());


            var mockDbContext = new Mock<AppDbContext>();

            mockDbContext.Setup(db => db.Bills).Returns(mockBillDbSet.Object);
            mockDbContext.Setup(db => db.Clients).Returns(mockClientDbSet.Object);


            var billingController = new BillingController(mockDbContext.Object);


            var result = billingController.CreateBill(billRequest);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var createdBills = Assert.IsAssignableFrom<IEnumerable<Bill>>(okResult.Value);

            Assert.True(createdBills.Any());

            var firstCreatedBill = createdBills.First();
            Assert.Equal(billExpected.ClientId, firstCreatedBill.ClientId);
            Assert.Equal(billExpected.Period, firstCreatedBill.Period);
            Assert.Equal(billExpected.State, firstCreatedBill.State);
            Assert.Equal(billExpected.Category, firstCreatedBill.Category);
        }

        [Fact]
        public void ProcessPayment_ReturnsOkResult()
        {
            var paymentRequest = new PaymentRequest
            {
                ClientId = 100,
                Period = 202301,
                Category = BillCategory.WATER.ToString(),
            };

            var bill = new Bill
            {
                Id = 1,
                ClientId = paymentRequest.ClientId,
                Period = paymentRequest.Period,
                State = BillState.Pending,
                Category = BillCategory.WATER,
            };

            var bills = new List<Bill> { bill };

            var payments = new List<Payment>();

            var mockDbContext = new Mock<AppDbContext>();
            var mockBillDbSet = new Mock<DbSet<Bill>>();
            var mockPaymentDbSet = new Mock<DbSet<Payment>>();

            mockBillDbSet.As<IQueryable<Bill>>().Setup(m => m.Provider).Returns(bills.AsQueryable().Provider);
            mockBillDbSet.As<IQueryable<Bill>>().Setup(m => m.Expression).Returns(bills.AsQueryable().Expression);
            mockBillDbSet.As<IQueryable<Bill>>().Setup(m => m.ElementType).Returns(bills.AsQueryable().ElementType);
            mockBillDbSet.As<IQueryable<Bill>>().Setup(m => m.GetEnumerator()).Returns(bills.AsQueryable().GetEnumerator());
            mockBillDbSet.Setup(m => m.Find(It.IsAny<object[]>())).Returns<object[]>(ids => bills.FirstOrDefault(b => b.Id == (int)ids[0]));

            mockPaymentDbSet.As<IQueryable<Payment>>().Setup(m => m.Provider).Returns(payments.AsQueryable().Provider);
            mockPaymentDbSet.As<IQueryable<Payment>>().Setup(m => m.Expression).Returns(payments.AsQueryable().Expression);
            mockPaymentDbSet.As<IQueryable<Payment>>().Setup(m => m.ElementType).Returns(payments.AsQueryable().ElementType);
            mockPaymentDbSet.As<IQueryable<Payment>>().Setup(m => m.GetEnumerator()).Returns(payments.AsQueryable().GetEnumerator());

            mockDbContext.Setup(db => db.Bills).Returns(mockBillDbSet.Object);
            mockDbContext.Setup(db => db.Payments).Returns(mockPaymentDbSet.Object);

            var billingController = new BillingController(mockDbContext.Object);

            var result = billingController.ProcessPayment(paymentRequest);

            var okResult = Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public void GetPendingBillByClient_ReturnsOkResult()
        {
            int clientId = 100;

            var pendingBills = new List<Bill>
            {
                new Bill
                {
                    Id = 1,
                    ClientId = clientId,
                    State = BillState.Pending,
                },
                new Bill
                {
                    Id = 2,
                    ClientId = clientId,
                    State = BillState.Pending,
                },
            };

            var mockDbContext = new Mock<AppDbContext>();
            var mockBillDbSet = new Mock<DbSet<Bill>>();

            mockBillDbSet.As<IQueryable<Bill>>().Setup(m => m.Provider).Returns(pendingBills.AsQueryable().Provider);
            mockBillDbSet.As<IQueryable<Bill>>().Setup(m => m.Expression).Returns(pendingBills.AsQueryable().Expression);
            mockBillDbSet.As<IQueryable<Bill>>().Setup(m => m.ElementType).Returns(pendingBills.AsQueryable().ElementType);
            mockBillDbSet.As<IQueryable<Bill>>().Setup(m => m.GetEnumerator()).Returns(pendingBills.AsQueryable().GetEnumerator());

            mockDbContext.Setup(db => db.Bills).Returns(mockBillDbSet.Object);

            var billingController = new BillingController(mockDbContext.Object);

            var result = billingController.GetPendingBillByClient(clientId);

            var okResult = Assert.IsType<OkObjectResult>(result);

            var pendingBillsResponse = Assert.IsAssignableFrom<IEnumerable<Bill>>(okResult.Value);

            Assert.Equal(pendingBills.Count, pendingBillsResponse.Count());
        }

        [Fact]
        public void GetPaymentHistoryByClient_ReturnsOkResult()
        {
            int clientId = 100;

            var paymentHistory = new List<Payment>
            {
                new Payment
                {
                    Id = 1,
                    ClientId = clientId,
                    BillId = 1,
                },
                new Payment
                {
                    Id = 2,
                    ClientId = clientId,
                    BillId = 2,
                },
            };

            var mockDbContext = new Mock<AppDbContext>();
            var mockPaymentDbSet = new Mock<DbSet<Payment>>();

            mockPaymentDbSet.As<IQueryable<Payment>>().Setup(m => m.Provider).Returns(paymentHistory.AsQueryable().Provider);
            mockPaymentDbSet.As<IQueryable<Payment>>().Setup(m => m.Expression).Returns(paymentHistory.AsQueryable().Expression);
            mockPaymentDbSet.As<IQueryable<Payment>>().Setup(m => m.ElementType).Returns(paymentHistory.AsQueryable().ElementType);
            mockPaymentDbSet.As<IQueryable<Payment>>().Setup(m => m.GetEnumerator()).Returns(paymentHistory.AsQueryable().GetEnumerator());

            mockDbContext.Setup(db => db.Payments).Returns(mockPaymentDbSet.Object);

            var billingController = new BillingController(mockDbContext.Object);

            var result = billingController.GetPaymentHistoryByClient(clientId);

            var okResult = Assert.IsType<OkObjectResult>(result);

            var paymentHistoryResponse = Assert.IsAssignableFrom<IEnumerable<Payment>>(okResult.Value);

            Assert.Equal(paymentHistory.Count, paymentHistoryResponse.Count());
        }

        [Fact]
        public void SearchBills_WithValidCategory_ReturnsOkResult()
        {
            string validCategory = BillCategory.WATER.ToString();
            var appDbContext = new Mock<AppDbContext>();
            var billingController = new BillingController(appDbContext.Object);
            int period = 202309;

            var bills = new List<Bill>
            {
                new Bill { 
                    Id = 1, 
                    Category = BillCategory.WATER, 
                    Period = period,
                    Amount = 100.2m,
                    State = BillState.Pending, 
                },
                new Bill { 
                    Id = 2, 
                    Category = BillCategory.WATER, 
                    Period = period,
                    Amount = 100.2m,
                    State = BillState.Pending, 
                },
            };

            var mockDbSet = new Mock<DbSet<Bill>>();
            mockDbSet.As<IQueryable<Bill>>().Setup(m => m.Provider).Returns(bills.AsQueryable().Provider);
            mockDbSet.As<IQueryable<Bill>>().Setup(m => m.Expression).Returns(bills.AsQueryable().Expression);
            mockDbSet.As<IQueryable<Bill>>().Setup(m => m.ElementType).Returns(bills.AsQueryable().ElementType);
            mockDbSet.As<IQueryable<Bill>>().Setup(m => m.GetEnumerator()).Returns(bills.AsQueryable().GetEnumerator());
            mockDbSet.Setup(m => m.AsQueryable()).Returns(() => bills.AsQueryable());

            appDbContext.Setup(db => db.Bills).Returns(mockDbSet.Object);
            

            var result = billingController.SearchBills(validCategory);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var billsResponse = Assert.IsAssignableFrom<IEnumerable<Bill>>(okResult.Value);

            Assert.Equal(bills.Count, billsResponse.Count());
        }

        [Fact]
        public void SearchBills_WithInvalidCategory_ReturnsBadRequest()
        {
            string invalidCategory = "InvalidCategory";
            var bills = new List<Bill> { };

            var appDbContext = new Mock<AppDbContext>();
            var mockDbSet = new Mock<DbSet<Bill>>();
            mockDbSet.As<IQueryable<Bill>>().Setup(m => m.Provider).Returns(bills.AsQueryable().Provider);
            mockDbSet.As<IQueryable<Bill>>().Setup(m => m.Expression).Returns(bills.AsQueryable().Expression);
            mockDbSet.As<IQueryable<Bill>>().Setup(m => m.ElementType).Returns(bills.AsQueryable().ElementType);
            mockDbSet.As<IQueryable<Bill>>().Setup(m => m.GetEnumerator()).Returns(bills.AsQueryable().GetEnumerator());
            
            appDbContext.Setup(db => db.Bills).Returns(mockDbSet.Object);

            var billingController = new BillingController(appDbContext.Object);

            var result = billingController.SearchBills(invalidCategory);

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var errorResponse = Assert.IsType<ErrorResponse>(badRequestResult.Value);

            Assert.Single(errorResponse.Errors);
            Assert.Contains("The 'category' field must be a valid Category", errorResponse.Errors[0]);
        }

    }
}
