using EStore.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EStore.Utilities.DataRepository
{
    public class TotalSalesDataRepository : SqlDbConnection, ITotalSalesDataRepository
    {
        public List<TotalSales> GetAllPurchaseHistory()
        {
            List<TotalSales> totalSales = _dbContext.TotalSalesModel.ToList();
            return totalSales;
        }

        List<TotalSales> ITotalSalesDataRepository.GetPurchaseHistory(string userName)
        {
            List<TotalSales> totalSales = _dbContext.TotalSalesModel.Where(p => p.UserName.Equals(userName)).ToList();
            return totalSales;
        }

        public List<DateTime> GetWeekIntervals()
        {
            var timestamps = _dbContext.TotalSalesModel.Select(x => x.Timestamp).ToList();

            if (timestamps.Any())
            {
                var startTimestamp = timestamps.Min();
                var endTimestamp = timestamps.Max();

                return CalulateWeekIntervals(startTimestamp, endTimestamp);
            }

            return new List<DateTime>();
        }

        private List<DateTime> CalulateWeekIntervals(DateTime startDate, DateTime endDate)
        {
            List<DateTime> weekIntervals = new List<DateTime>();
            DateTime currentWeekStart = startDate.Date.AddDays(DayOfWeek.Monday - startDate.DayOfWeek);

            while (currentWeekStart <= endDate)
            {
                weekIntervals.Add(currentWeekStart);
                currentWeekStart = currentWeekStart.AddDays(7);
            }
            weekIntervals.Add(currentWeekStart);

            return weekIntervals;
        }
    }
}