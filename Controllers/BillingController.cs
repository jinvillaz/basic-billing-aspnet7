using BasicBilling.API.Data;
using BasicBilling.API.Models;
using BasicBilling.API.Utils;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;

namespace BasicBilling.API.Controllers
{
    [Route("billing")]
    [ApiController]
    public class BillingController : ControllerBase
    {
        private readonly AppDbContext appDbContext;

        public BillingController(AppDbContext appDbContext)
        {
            this.appDbContext = appDbContext;
        }

        [HttpPost("bills")]
        [ServiceFilter(typeof(BillRequestFilterAttribute))]
        [ProducesResponseType(typeof(IEnumerable<Bill>), 200)]
        [Produces("application/json")]
        public IActionResult CreateBill([FromBody] BillRequest billRequest)
        {
            var clients = appDbContext.Clients.ToList();

            bool billsExist = clients.All(client =>
                appDbContext.Bills.Any(bill =>
                    bill.ClientId == client.Id &&
                    bill.Period == billRequest.Period &&
                    bill.Category == Enum.Parse<BillCategory>(billRequest.Category)
                )
            );

            if (billsExist)
            {
                var errorMessage = $"Bills already exist for period {billRequest.Period} and category {billRequest.Category}.";
                return new BadRequestObjectResult(new ErrorResponse { Errors = new List<string> { errorMessage } });
            }

            foreach (var client in clients)
            {
                decimal randomAmount = Math.Round((decimal)new Random().NextDouble() * (200.0m - 100.0m) + 100.0m, 2);
                var newBill = new Bill
                {
                    ClientId = client.Id,
                    Period = billRequest.Period,
                    Category = Enum.Parse<BillCategory>(billRequest.Category),
                    Amount = randomAmount,
                    State = BillState.Pending,

                };
                appDbContext.Bills.Add(newBill);
            }

            appDbContext.SaveChanges();

            var createdBills = appDbContext.Bills
                .Where(bill =>
                    bill.Period == billRequest.Period &&
                    bill.Category == Enum.Parse<BillCategory>(billRequest.Category)
                )
                .ToList();

            return new OkObjectResult(createdBills);
        }

        [HttpPost("pay")]
        [ServiceFilter(typeof(PaymentRequestFilterAttribute))]
        [ProducesResponseType(typeof(Bill), 200)]
        [Produces("application/json")]
        public IActionResult ProcessPayment([FromBody] PaymentRequest paymentRequest)
        {

            var bill = appDbContext.Bills.Where(bill => 
                bill.ClientId == paymentRequest.ClientId &&
                bill.Period == paymentRequest.Period && 
                bill.State == BillState.Pending &&
                bill.Category == (BillCategory)Enum.Parse(typeof(BillCategory), paymentRequest.Category)).FirstOrDefault();
            if (bill == null)
            {
                var errorMessage = $"There is no bill with clientId={paymentRequest.ClientId} in period={paymentRequest.Period} with category={paymentRequest.Category}";
                return new BadRequestObjectResult(new ErrorResponse
                {
                    Errors = new List<string> { errorMessage }
                });
            }
            
            var payment = new Payment {
                BillId = bill.Id,
                ClientId = bill.ClientId,
            };
            appDbContext.Payments.Add(payment);
            bill.State = BillState.Paid;
            appDbContext.Bills.Update(bill);

            appDbContext.SaveChanges();

            return new OkObjectResult(payment);
        }

        [HttpGet("pending/{clientId}")]
        [ProducesResponseType(typeof(IEnumerable<Bill>), 200)]
        [Produces("application/json")]
        public IActionResult GetPendingBillByClient([FromRoute] int clientId)
        {
            var pendingBillsForClient = appDbContext.Bills
                .Where(bill => bill.ClientId == clientId && bill.State == BillState.Pending)
                .ToList();

            return Ok(pendingBillsForClient);
        }

        [HttpGet("payment-history/{clientId}")]
        [ProducesResponseType(typeof(IEnumerable<Payment>), 200)]
        [Produces("application/json")]
        public IActionResult GetPaymentHistoryByClient([FromRoute] int clientId)
        {
            var paymentHistory = appDbContext.Payments
                .Where(payment => payment.ClientId == clientId).ToList()
                .ToList();
            return new OkObjectResult(paymentHistory);
        }

        [HttpGet("search")]
        [ProducesResponseType(typeof(IEnumerable<Bill>), 200)]
        [Produces("application/json")]
        public IActionResult SearchBills([FromQuery] string? category)
        {
            var query = appDbContext.Bills.AsQueryable();

            if (category != null)
            {
                if (!Enum.IsDefined(typeof(BillCategory), category))
                {
                    var validCategories = string.Join(", ", Enum.GetNames(typeof(BillCategory)));
                    var errorMessage = $"The 'category' field must be a valid Category. Valid values are: {validCategories}";
                    return new BadRequestObjectResult(new ErrorResponse
                    {
                        Errors = new List<string> { errorMessage }
                    });
                }

                query = query.Where(bill => bill.Category == (BillCategory)Enum.Parse(typeof(BillCategory), category));
            }

            var billsByCategory = query.ToList();

            return Ok(billsByCategory);
        }        
    }
}
