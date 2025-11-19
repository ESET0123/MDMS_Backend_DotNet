//using Microsoft.EntityFrameworkCore;
//using MDMS_Backend.Models;
//using System.Collections.Generic;
//using System.Threading.Tasks;
//using System.Data;
//using Microsoft.Data.SqlClient;

//namespace MDMS_Backend.Repositories
//{
//    public class MonthlyBillRepository : IMonthlyBillRepository
//    {
//        private readonly MdmsDbContext _context;

//        public MonthlyBillRepository(MdmsDbContext context)
//        {
//            _context = context;
//        }

//        public async Task<IEnumerable<MonthlyBill>> GetAllAsync()
//        {
//            return await _context.MonthlyBills
//                                 .Include(m => m.Consumer)
//                                 .Include(m => m.Meter).ThenInclude(meter => meter.Tariff)
//                                 .OrderByDescending(m => m.BillingYear)
//                                 .ThenByDescending(m => m.BillingMonth)
//                                 .ToListAsync();
//        }

//        public async Task<MonthlyBill> GetByIdAsync(int billId)
//        {
//            return await _context.MonthlyBills
//                                 .Include(m => m.Consumer)
//                                 .Include(m => m.Meter).ThenInclude(meter => meter.Tariff)
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
//            var monthParam = new SqlParameter("@BillingMonth", month);
//            var yearParam = new SqlParameter("@BillingYear", year);

//            var result = await _context.Database
//                .ExecuteSqlRawAsync("EXEC GenerateMonthlyBills @BillingMonth, @BillingYear",
//                                  monthParam, yearParam);
//            return result;
//        }

//        public async Task<int> GenerateMonthlyBillsFromReadingsAsync(int month, int year)
//        {
//            var monthParam = new SqlParameter("@BillingMonth", month);
//            var yearParam = new SqlParameter("@BillingYear", year);
//            var outputParam = new SqlParameter
//            {
//                ParameterName = "@BillsAffected",
//                SqlDbType = System.Data.SqlDbType.Int,
//                Direction = System.Data.ParameterDirection.Output
//            };

//            await _context.Database.ExecuteSqlRawAsync(
//                "EXEC GenerateMonthlyBillsFromReadings @BillingMonth, @BillingYear, @BillsAffected OUTPUT",
//                monthParam, yearParam, outputParam);

//            return (int)outputParam.Value;
//        }

//        // NEW: Generate bill for a specific meter
//        public async Task<int> GenerateBillForMeterAsync(int meterId, int month, int year)
//        {
//            var meterParam = new SqlParameter("@MeterId", meterId);
//            var monthParam = new SqlParameter("@BillingMonth", month);
//            var yearParam = new SqlParameter("@BillingYear", year);
//            var outputParam = new SqlParameter
//            {
//                ParameterName = "@BillGenerated",
//                SqlDbType = System.Data.SqlDbType.Int,
//                Direction = System.Data.ParameterDirection.Output
//            };

//            try
//            {
//                await _context.Database.ExecuteSqlRawAsync(
//                    "EXEC GenerateBillForMeter @MeterId, @BillingMonth, @BillingYear, @BillGenerated OUTPUT",
//                    meterParam, monthParam, yearParam, outputParam);

//                return (int)outputParam.Value;
//            }
//            catch (SqlException ex)
//            {
//                // Handle SQL errors with meaningful messages
//                if (ex.Message.Contains("No daily readings found"))
//                {
//                    throw new InvalidOperationException($"No daily meter readings found for Meter #{meterId} in {month}/{year}");
//                }
//                else if (ex.Message.Contains("No applicable tariff slabs"))
//                {
//                    throw new InvalidOperationException($"No applicable tariff slabs found for the billing period {month}/{year}");
//                }
//                else
//                {
//                    throw new InvalidOperationException($"Failed to generate bill: {ex.Message}");
//                }
//            }
//        }

//        // NEW: Validate sequential payment
//        public async Task<BillValidationResult> ValidateSequentialPaymentAsync(int meterId, int month, int year)
//        {
//            // Get all bills for this meter, ordered by date
//            var allBills = await _context.MonthlyBills
//                .Where(m => m.MeterId == meterId)
//                .OrderBy(m => m.BillingYear)
//                .ThenBy(m => m.BillingMonth)
//                .ToListAsync();

//            if (!allBills.Any())
//            {
//                return new BillValidationResult { IsValid = true };
//            }

//            // Find the current bill
//            var currentBillIndex = allBills.FindIndex(b =>
//                b.BillingMonth == month && b.BillingYear == year);

//            if (currentBillIndex == -1)
//            {
//                return new BillValidationResult
//                {
//                    IsValid = false,
//                    ErrorMessage = "Bill not found for the specified month and year"
//                };
//            }

//            // Check all previous bills
//            for (int i = 0; i < currentBillIndex; i++)
//            {
//                var previousBill = allBills[i];

//                if (previousBill.BillStatus != "Paid")
//                {
//                    var monthName = new DateTime(previousBill.BillingYear, previousBill.BillingMonth, 1)
//                        .ToString("MMMM yyyy");

//                    return new BillValidationResult
//                    {
//                        IsValid = false,
//                        ErrorMessage = $"Cannot pay bill for {new DateTime(year, month, 1):MMMM yyyy}. " +
//                                     $"Previous bill for {monthName} (Bill ID: {previousBill.BillId}) must be paid first. " +
//                                     $"Current status: {previousBill.BillStatus}",
//                        UnpaidBillId = previousBill.BillId,
//                        UnpaidMonth = previousBill.BillingMonth,
//                        UnpaidYear = previousBill.BillingYear
//                    };
//                }
//            }

//            return new BillValidationResult { IsValid = true };
//        }

//        public async Task<IEnumerable<MonthlyBill>> GetByMeterAndMonthAsync(int meterId, int month, int year)
//        {
//            return await _context.MonthlyBills
//                                 .Include(m => m.Consumer)
//                                 .Include(m => m.Meter).ThenInclude(meter => meter.Tariff)
//                                 .Where(m => m.MeterId == meterId &&
//                                            m.BillingMonth == month &&
//                                            m.BillingYear == year)
//                                 .ToListAsync();
//        }

//        public async Task<IEnumerable<MonthlyBill>> GetByConsumerAsync(int consumerId)
//        {
//            return await _context.MonthlyBills
//                                 .Include(m => m.Consumer)
//                                 .Include(m => m.Meter).ThenInclude(meter => meter.Tariff)
//                                 .Where(m => m.ConsumerId == consumerId)
//                                 .OrderByDescending(m => m.BillingYear)
//                                 .ThenByDescending(m => m.BillingMonth)
//                                 .ToListAsync();
//        }

//        public async Task<IEnumerable<MonthlyBill>> GetByConsumerIdAsync(int consumerId)
//        {
//            return await _context.MonthlyBills
//                                 .Include(m => m.Consumer)
//                                 .Include(m => m.Meter).ThenInclude(meter => meter.Tariff)
//                                 .Where(m => m.ConsumerId == consumerId)
//                                 .OrderByDescending(m => m.BillingYear)
//                                 .ThenByDescending(m => m.BillingMonth)
//                                 .ToListAsync();
//        }

//        public async Task<bool> CheckBillsExistForMonthAsync(int month, int year)
//        {
//            return await _context.MonthlyBills
//                                 .AnyAsync(m => m.BillingMonth == month &&
//                                               m.BillingYear == year);
//        }
//    }
//}

using Microsoft.EntityFrameworkCore;
using MDMS_Backend.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Data;
using Microsoft.Data.SqlClient;

namespace MDMS_Backend.Repositories
{
    public class MonthlyBillRepository : IMonthlyBillRepository
    {
        private readonly MdmsDbContext _context;

        public MonthlyBillRepository(MdmsDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<MonthlyBill>> GetAllAsync()
        {
            return await _context.MonthlyBills
                                 .Include(m => m.Consumer)
                                 .Include(m => m.Meter).ThenInclude(meter => meter.Tariff)
                                 .OrderByDescending(m => m.BillingYear)
                                 .ThenByDescending(m => m.BillingMonth)
                                 .ToListAsync();
        }

        public async Task<MonthlyBill> GetByIdAsync(int billId)
        {
            return await _context.MonthlyBills
                                 .Include(m => m.Consumer)
                                 .Include(m => m.Meter).ThenInclude(meter => meter.Tariff)
                                 .FirstOrDefaultAsync(m => m.BillId == billId);
        }

        public async Task AddAsync(MonthlyBill monthlyBill)
        {
            await _context.MonthlyBills.AddAsync(monthlyBill);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(MonthlyBill monthlyBill)
        {
            _context.MonthlyBills.Update(monthlyBill);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int billId)
        {
            var monthlyBill = await _context.MonthlyBills.FindAsync(billId);
            if (monthlyBill != null)
            {
                _context.MonthlyBills.Remove(monthlyBill);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<int> GenerateMonthlyBillsAsync(int month, int year)
        {
            var monthParam = new SqlParameter("@BillingMonth", month);
            var yearParam = new SqlParameter("@BillingYear", year);

            var result = await _context.Database
                .ExecuteSqlRawAsync("EXEC GenerateMonthlyBills @BillingMonth, @BillingYear",
                                  monthParam, yearParam);
            return result;
        }

        public async Task<int> GenerateMonthlyBillsFromReadingsAsync(int month, int year)
        {
            var monthParam = new SqlParameter("@BillingMonth", month);
            var yearParam = new SqlParameter("@BillingYear", year);
            var outputParam = new SqlParameter
            {
                ParameterName = "@BillsAffected",
                SqlDbType = System.Data.SqlDbType.Int,
                Direction = System.Data.ParameterDirection.Output
            };

            await _context.Database.ExecuteSqlRawAsync(
                "EXEC GenerateMonthlyBillsFromReadings @BillingMonth, @BillingYear, @BillsAffected OUTPUT",
                monthParam, yearParam, outputParam);

            return (int)outputParam.Value;
        }

        public async Task<int> GenerateBillForMeterAsync(int meterId, int month, int year)
        {
            var meterParam = new SqlParameter("@MeterId", meterId);
            var monthParam = new SqlParameter("@BillingMonth", month);
            var yearParam = new SqlParameter("@BillingYear", year);
            var outputParam = new SqlParameter
            {
                ParameterName = "@BillGenerated",
                SqlDbType = System.Data.SqlDbType.Int,
                Direction = System.Data.ParameterDirection.Output
            };

            try
            {
                await _context.Database.ExecuteSqlRawAsync(
                    "EXEC GenerateBillForMeter @MeterId, @BillingMonth, @BillingYear, @BillGenerated OUTPUT",
                    meterParam, monthParam, yearParam, outputParam);

                return (int)outputParam.Value;
            }
            catch (SqlException ex)
            {
                if (ex.Message.Contains("No daily readings found"))
                {
                    throw new InvalidOperationException($"No daily meter readings found for Meter #{meterId} in {month}/{year}");
                }
                else if (ex.Message.Contains("No applicable tariff slabs"))
                {
                    throw new InvalidOperationException($"No applicable tariff slabs found for the billing period {month}/{year}");
                }
                else
                {
                    throw new InvalidOperationException($"Failed to generate bill: {ex.Message}");
                }
            }
        }

        public async Task<BillValidationResult> ValidateSequentialPaymentAsync(int meterId, int month, int year)
        {
            // Get all bills for this meter, ordered by date
            var allBills = await _context.MonthlyBills
                .Where(m => m.MeterId == meterId)
                .OrderBy(m => m.BillingYear)
                .ThenBy(m => m.BillingMonth)
                .ToListAsync();

            if (!allBills.Any())
            {
                return new BillValidationResult { IsValid = true };
            }

            // Find the current bill
            var currentBillIndex = allBills.FindIndex(b =>
                b.BillingMonth == month && b.BillingYear == year);

            if (currentBillIndex == -1)
            {
                return new BillValidationResult
                {
                    IsValid = false,
                    ErrorMessage = "Bill not found for the specified month and year"
                };
            }

            // Check all previous bills
            for (int i = 0; i < currentBillIndex; i++)
            {
                var previousBill = allBills[i];

                if (previousBill.BillStatus != "Paid")
                {
                    var monthName = new DateTime(previousBill.BillingYear, previousBill.BillingMonth, 1)
                        .ToString("MMMM yyyy");

                    return new BillValidationResult
                    {
                        IsValid = false,
                        ErrorMessage = $"Cannot pay bill for {new DateTime(year, month, 1):MMMM yyyy}. " +
                                     $"Previous bill for {monthName} (Bill ID: {previousBill.BillId}) must be paid first. " +
                                     $"Current status: {previousBill.BillStatus}",
                        UnpaidBillId = previousBill.BillId,
                        UnpaidMonth = previousBill.BillingMonth,
                        UnpaidYear = previousBill.BillingYear
                    };
                }
            }

            return new BillValidationResult { IsValid = true };
        }

        public async Task<IEnumerable<MonthlyBill>> GetByMeterAndMonthAsync(int meterId, int month, int year)
        {
            return await _context.MonthlyBills
                                 .Include(m => m.Consumer)
                                 .Include(m => m.Meter).ThenInclude(meter => meter.Tariff)
                                 .Where(m => m.MeterId == meterId &&
                                            m.BillingMonth == month &&
                                            m.BillingYear == year)
                                 .ToListAsync();
        }

        public async Task<IEnumerable<MonthlyBill>> GetByConsumerAsync(int consumerId)
        {
            return await _context.MonthlyBills
                                 .Include(m => m.Consumer)
                                 .Include(m => m.Meter).ThenInclude(meter => meter.Tariff)
                                 .Where(m => m.ConsumerId == consumerId)
                                 .OrderByDescending(m => m.BillingYear)
                                 .ThenByDescending(m => m.BillingMonth)
                                 .ToListAsync();
        }

        public async Task<IEnumerable<MonthlyBill>> GetByConsumerIdAsync(int consumerId)
        {
            return await _context.MonthlyBills
                                 .Include(m => m.Consumer)
                                 .Include(m => m.Meter).ThenInclude(meter => meter.Tariff)
                                 .Where(m => m.ConsumerId == consumerId)
                                 .OrderByDescending(m => m.BillingYear)
                                 .ThenByDescending(m => m.BillingMonth)
                                 .ToListAsync();
        }

        public async Task<bool> CheckBillsExistForMonthAsync(int month, int year)
        {
            return await _context.MonthlyBills
                                 .AnyAsync(m => m.BillingMonth == month &&
                                               m.BillingYear == year);
        }
    }
}