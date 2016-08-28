using ExchangeRateAnalyser.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExchangeRateAnalyser.DataAccess
{
    /// <summary>
    /// Defines methods for fetching currency exchange rates
    /// </summary>
    public interface ICurrencyDAO
    {
        /// <summary>
        /// Fetches exchange rates using certain data source
        /// </summary>
        /// <param name="startDate">Start date</param>
        /// <param name="endDate">End date</param>
        /// <returns>List of obtained Currencies including their exchange rates for the period [StartDate;EndDate].</returns>
        List<Currency> GetCurrencyRates(DateTime startDate, DateTime endDate);
    }
}
