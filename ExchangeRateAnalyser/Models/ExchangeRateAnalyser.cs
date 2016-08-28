using ExchangeRateAnalyser.DataAccess;
using ExchangeRateAnalyser.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExchangeRateAnalyser.Models
{
    /// <summary>
    /// Represents an Exchange rates analyser
    /// </summary>
    class ExchangeRateAnalyser
    {
        /// <summary>
        /// Gets of sets start date of analysis
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// Gets of sets end date of analysis
        /// </summary>
        public DateTime EndDate { get; set; }

        /// <summary>
        /// Gets or sets initial amount of money
        /// </summary>
        public decimal Capital { get; set; }


        /// <summary>
        /// Initializes a new instance of the ExchangeRateAnalyser class
        /// </summary>
        public ExchangeRateAnalyser()
        {

        }

        /// <summary>
        ///  Initializes a new instance of the ExchangeRateAnalyser class with specified start date, end date and initial amount of money
        /// </summary>
        /// <param name="startDate">Dtart date of analysis</param>
        /// <param name="endDate">End date of analysis</param>
        /// <param name="capital">Initial amount of money</param>
        public ExchangeRateAnalyser(DateTime startDate, DateTime endDate, decimal capital)
        {
            this.StartDate = startDate;
            this.EndDate = endDate;
            this.Capital = capital;
        }

        /// <summary>
        /// Processes the list of currencies exchange rates fetched using current data access object and returns the data containing information about most profitable exchange deal
        /// </summary>
        /// <returns>ExchangeRateAnalysisResult object</returns>
        public ExchangeRateAnalysisResult PerformAnalysis()
        {
            var validationError = this.ValidateParameters();

            if (!string.IsNullOrEmpty(validationError))
            {
                return new ExchangeRateAnalysisResult(validationError);
            }

            var currencyDAO = DAOFactory.GetCurrencyDAO();

            var currencies = currencyDAO.GetCurrencyRates(this.StartDate, this.EndDate);

            var bestCurrency = GetMostProfitableCurrency(currencies);

            if (bestCurrency == null)
            {
                return new ExchangeRateAnalysisResult(Strings.ErrorBestCurrencyNull);
            }

            ExchangeRateAnalysisResult analysisResult = ExchangeRateAnalysisResult.Load(bestCurrency, this.Capital);

            return analysisResult;
        }

        /// <summary>
        /// Gets the most profitable currency using CurrencyProfit
        /// </summary>
        /// <returns>CurrencyProfit instance in case it was possible to find the most profitable currency, otherwise null</returns>
        private CurrencyProfit GetMostProfitableCurrency(List<Currency> currencies)
        {
            CurrencyProfit bestCurrencyProfit = null;

            decimal maxProfit = decimal.MinValue;
            decimal currentProfit = decimal.MinValue;


            try
            {
                CurrencyProfit currencyProfit;
                foreach (var currency in currencies)
                {
                    currencyProfit = CurrencyProfit.Load(currency);

                    currentProfit = currencyProfit.GetCapitalProfit(this.Capital);

                    if (currentProfit > maxProfit)
                    {
                        maxProfit = currentProfit;
                        bestCurrencyProfit = currencyProfit;
                    }
                }
            }
            catch (OverflowException ex)
            {
                Logger.Log(Strings.ErrorValueIsNotInRange, ex);

                bestCurrencyProfit = null;
            }
            catch (Exception ex)
            {
                Logger.Log(Strings.ErrorOccured, ex);

                bestCurrencyProfit = null;
            }

            return bestCurrencyProfit;
        }

        /// <summary>
        /// Checks whether initial parameters are valid.
        /// </summary>
        /// <returns>Error message in case initial parameters are invalid, otherwise null</returns>
        private string ValidateParameters()
        {
            if (this.Capital <= 0)
            {
                return Strings.ErrorInitialCapitalNegative;
            }

            if (this.StartDate > DateTime.Today)
            {
                return Strings.ErrorStartDateGreaterThanToday;
            }

            if (this.EndDate > DateTime.Today)
            {
                return Strings.ErrorEndDateGreaterThanToday;
            }

            if (this.StartDate.Date > this.EndDate.Date)
            {
                return Strings.ErrorStatDateGreaterThanEndDate;
            }

            return null;
        }
    }
}
