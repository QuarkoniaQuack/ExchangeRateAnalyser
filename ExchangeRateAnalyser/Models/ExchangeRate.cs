using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExchangeRateAnalyser.Models
{
    /// <summary>
    /// Represents currency exchange rate
    /// </summary>
    public class ExchangeRate
    {
        /// <summary>
        /// Gets or sets exchange rate date
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Gets or sets exchange rate nominal
        /// </summary>
        private int Nominal { get; set; }

        /// <summary>
        /// Gets or sets exchange rate value
        /// </summary>
        private double Value { get; set; }

        /// <summary>
        ///  Gets exchange rate. Depends on Nominal and Value
        /// </summary>
        public double Rate
        {
            get
            {
                if (Value > 0 && Nominal > 0)
                    return Value / Nominal;
                else
                    return double.NaN;
            }
        }

        /// <summary>
        /// Initializes a new instance of the ExchangeRate class
        /// </summary>
        public ExchangeRate()
        {

        }

        /// <summary>
        /// Initializes a new instance of the ExchangeRate class for the specified exchange rate date, value and nominal
        /// </summary>
        /// <param name="date">Exchange rate date</param>
        /// <param name="value">Exchange rate value</param>
        /// <param name="nominal">Exchange rate nominal</param>
        public ExchangeRate(DateTime date, double value, int nominal)
        {
            Date = date;
            Value = value;
            Nominal = nominal;
        }
    }
}
