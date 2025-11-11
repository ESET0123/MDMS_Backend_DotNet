////using Microsoft.EntityFrameworkCore;
////using MDMS_Backend.Models;
////using System.Collections.Generic;
////using System.Threading.Tasks;

////namespace MDMS_Backend.Repositories
////{
////    public class MonthlyBillRepository : IMonthlyBillRepository
////    {
////        private readonly MdmsDbContext _context;

////        public MonthlyBillRepository(MdmsDbContext context)
////        {
////            _context = context;
////        }

////        public async Task<IEnumerable<MonthlyBill>> GetAllAsync()
////        {
////            return await _context.MonthlyBills
////                                 .Include(m => m.Consumer)
////                                 .Include(m => m.Meter)
////                                 .ToListAsync();
////        }

////        public async Task<MonthlyBill> GetByIdAsync(int billId)
////        {
////            return await _context.MonthlyBills
////                                 .Include(m => m.Consumer)
////                                 .Include(m => m.Meter)
////                                 .FirstOrDefaultAsync(m => m.BillId == billId);
////        }

////        public async Task AddAsync(MonthlyBill monthlyBill)
////        {
////            await _context.MonthlyBills.AddAsync(monthlyBill);
////            await _context.SaveChangesAsync();
////        }

////        public async Task UpdateAsync(MonthlyBill monthlyBill)
////        {
////            //_context.Entry(monthlyBill).State = EntityState.Modified;
////            await _context.SaveChangesAsync();
////        }

////        public async Task DeleteAsync(int billId)
////        {
////            var monthlyBill = await _context.MonthlyBills.FindAsync(billId);
////            if (monthlyBill != null)
////            {
////                _context.MonthlyBills.Remove(monthlyBill);
////                await _context.SaveChangesAsync();
////            }
////        }
////    }
////}

//// MonthlyBillRepository.cs
//using Dapper;
//using MDMS_Backend.Controllers;
//using MDMS_Backend.Models;
//using Microsoft.EntityFrameworkCore;
//using System.Collections.Generic;
//using System.Data;
//using System.Linq;
//using System.Threading.Tasks;

//namespace MDMS_Backend.Repositories
//{
//    public class MonthlyBillRepository : IMonthlyBillRepository
//    {
//        private readonly MdmsDbContext _context;
//        private readonly IDbConnection _dbConnection;

//        public MonthlyBillRepository(MdmsDbContext context, IDbConnection dbConnection)
//        {
//            _context = context;
//            _dbConnection = dbConnection;
//        }

//        public async Task<IEnumerable<MonthlyBill>> GetAllAsync()
//        {
//            return await _context.MonthlyBills
//                                 .Include(m => m.Consumer)
//                                 .Include(m => m.Meter)
//                                 .OrderByDescending(m => m.BillingDate)
//                                 .ThenBy(m => m.Consumer.Name)
//                                 .ToListAsync();
//        }

//        public async Task<MonthlyBill> GetByIdAsync(int billId)
//        {
//            return await _context.MonthlyBills
//                                 .Include(m => m.Consumer)
//                                 .Include(m => m.Meter)
//                                 .FirstOrDefaultAsync(m => m.BillId == billId);
//        }

//        public async Task AddAsync(MonthlyBill monthlyBill)
//        {
//            await _context.MonthlyBills.AddAsync(monthlyBill);
//            await _context.SaveChangesAsync();
//        }

//        public async Task UpdateAsync(MonthlyBill monthlyBill)
//        {
//            _context.MonthlyBills.Update(monthlyBill);
//            await _context.SaveChangesAsync();
//        }

//        public async Task DeleteAsync(int billId)
//        {
//            var monthlyBill = await _context.MonthlyBills.FindAsync(billId);
//            if (monthlyBill != null)
//            {
//                _context.MonthlyBills.Remove(monthlyBill);
//                await _context.SaveChangesAsync();
//            }
//        }

//        public async Task<int> GenerateMonthlyBillsAsync(int month, int year)
//        {
//            var parameters = new
//            {
//                BillingMonth = month,
//                BillingYear = year
//            };

//            var result = await _dbConnection.QuerySingleAsync<int>(
//                "GenerateMonthlyBills",
//                parameters,
//                commandType: CommandType.StoredProcedure
//            );

//            return result;
//        }

//        public async Task<IEnumerable<MonthlyBill>> GetByMeterAndMonthAsync(int meterId, int month, int year)
//        {
//            return await _context.MonthlyBills
//                                 .Include(m => m.Consumer)
//                                 .Include(m => m.Meter)
//                                 .Where(m => m.MeterId == meterId &&
//                                            m.BillingDate.Month == month &&
//                                            m.BillingDate.Year == year)
//                                 .ToListAsync();
//        }

//        public async Task<IEnumerable<MonthlyBillDetailDTO>> GetBillDetailsAsync(int billId)
//        {
//            var bill = await _context.MonthlyBills
//                                    .Include(m => m.Consumer)
//                                    .Include(m => m.Meter)
//                                    .FirstOrDefaultAsync(m => m.BillId == billId);

//            if (bill == null)
//                return Enumerable.Empty<MonthlyBillDetailDTO>();

//            var billingDate = bill.BillingDate;
//            var startDate = billingDate.AddMonths(-1);
//            var endDate = billingDate.AddDays(-1);

//            var dailyReadings = await _context.DailyMeterReadings
//                                            .Include(d => d.TodRule)
//                                            .Where(d => d.MeterId == bill.MeterId &&
//                                                       d.ReadingDate >= startDate &&
//                                                       d.ReadingDate <= endDate)
//                                            .OrderBy(d => d.ReadingDate)
//                                            .ThenBy(d => d.StartTime)
//                                            .ToListAsync();

//            var billDetail = new MonthlyBillDetailDTO
//            {
//                BillId = bill.BillId,
//                MeterId = bill.MeterId,
//                ConsumerId = bill.ConsumerId,
//                ConsumerName = bill.Consumer.Name,
//                MeterNumber = bill.Meter.MeterNumber,
//                BillingDate = bill.BillingDate,
//                TotalConsumptionKwh = bill.TotalConsumptionKwh,
//                TotalAmount = bill.TotalAmount,
//                BillStatus = bill.BillStatus,
//                DailyReadings = dailyReadings.Select(d => new DailyReadingForBillDTO
//                {
//                    ReadingDate = d.ReadingDate,
//                    TodRuleName = d.TodRule.RuleName,
//                    ConsumptionKwh = d.ConsumptionKwh,
//                    Amount = d.Amount
//                }).ToList()
//            };

//            return new List<MonthlyBillDetailDTO> { billDetail };
//        }
//    }
//    public class MonthlyBillDetailDTO
//    {
//        public int BillId { get; set; }
//        public int MeterId { get; set; }
//        public int ConsumerId { get; set; }
//        public string ConsumerName { get; set; }
//        public string MeterNumber { get; set; }
//        public DateOnly BillingDate { get; set; }
//        public decimal TotalConsumptionKwh { get; set; }
//        public decimal TotalAmount { get; set; }
//        public string BillStatus { get; set; }
//        public List<DailyReadingForBillDTO> DailyReadings { get; set; } = new();
//    }

//    public class DailyReadingForBillDTO
//    {
//        public DateOnly ReadingDate { get; set; }
//        public string TodRuleName { get; set; }
//        public decimal ConsumptionKwh { get; set; }
//        public decimal Amount { get; set; }
//    }
//}