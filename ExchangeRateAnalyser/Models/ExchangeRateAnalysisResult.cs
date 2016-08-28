using ExchangeRateAnalyser.Utils;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExchangeRateAnalyser.Models
{
    /// <summary>
    /// Represents the result of ExchangeRateAnalyser analysis
    /// </summary>
    public class ExchangeRateAnalysisResult
    {
        private const string DecimalFormat = "F2";

        /// <summary>
        /// Return the error message if any error occured during analysis otherwise null
        /// </summary>
        public string Error { get; private set; }

        /// <summary>
        /// Gets the  CurrencyProfit object holding the most profitable currency data
        /// </summary>
        public CurrencyProfit CurrencyProfit { get; private set; }

        /// <summary>
        /// Gets initial amount of money
        /// </summary>
        public decimal InitialCapital { get; private set; }

        /// <summary>
        /// Gets the final amount of money
        /// </summary>
        public decimal AccumulatedCapital { get; private set; }

        /// <summary>
        /// Gets the profit
        /// </summary>
        public decimal Profit { get; private set; }

        /// <summary>
        /// Initializes instance of ExchangeRateAnalysisResult class
        /// </summary>
        public ExchangeRateAnalysisResult()
        {

        }

        /// <summary>
        /// Initializes instance of ExchangeRateAnalysisResult class with specified error
        /// </summary>
        /// <param name="error">Error message</param>
        public ExchangeRateAnalysisResult(string error)
        {

        }


        /// <summary>
        /// Loads ExchangeRateAnalysisResult from the specified CurrencyProfit 
        /// </summary>
        /// <param name="currencyProfit">CurrencyProfit object</param>
        /// <param name="initialCapital">Initial amount of money</param>
        /// <returns></returns>
        public static ExchangeRateAnalysisResult Load(CurrencyProfit currencyProfit, decimal initialCapital)
        {
            ExchangeRateAnalysisResult result = new ExchangeRateAnalysisResult();

            try
            {
                if (currencyProfit != null && initialCapital > 0)
                {
                    result.CurrencyProfit = currencyProfit;
                    result.InitialCapital = initialCapital;

                    result.Profit = currencyProfit.GetCapitalProfit(initialCapital);

                    result.AccumulatedCapital = initialCapital + result.Profit;
                }
                else
                {
                    result.Error = Strings.ErrorOccuredTryLater;
                }
            }
            catch (OverflowException ex)
            {
                Logger.Log(Strings.ErrorCapitalIsNotInRange, ex);

                result = new ExchangeRateAnalysisResult();
                result.Error = Strings.ErrorCapitalIsNotInRange;
            }
            catch (Exception ex)
            {
                //write to log
                Logger.Log(Strings.ErrorOccured, ex);

                result = new ExchangeRateAnalysisResult();
                result.Error = Strings.ErrorOccured;
            }

            return result;
        }

        /// <summary>
        /// Returns a string that represents the current object
        /// </summary>
        /// <returns>String that represents the current object</returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("Most profitable currency = {0}", CurrencyProfit.Currency.Name);
            sb.AppendLine();
            sb.AppendFormat("Initial capital = {0}", InitialCapital.ToString(DecimalFormat, CultureInfo.InvariantCulture));
            sb.AppendLine();
            sb.AppendFormat("Accumulated capital = {0}", AccumulatedCapital.ToString(DecimalFormat, CultureInfo.InvariantCulture));
            sb.AppendLine();
            sb.AppendFormat("Best date to buy = {0}", CurrencyProfit.BestRateToBuy.Date.ToString("dd/MM/yyyy"));
            sb.AppendLine();
            sb.AppendFormat("Best date to sell = {0}", CurrencyProfit.BestRateToSell.Date.ToString("dd/MM/yyyy"));
            sb.AppendLine();
            sb.AppendFormat("Profit = {0}", Profit.ToString(DecimalFormat, CultureInfo.InvariantCulture));
            sb.AppendLine();
            return sb.ToString();
        }
    }
}
