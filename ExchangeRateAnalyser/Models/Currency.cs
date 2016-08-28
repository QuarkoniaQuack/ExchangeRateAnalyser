using ExchangeRateAnalyser.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExchangeRateAnalyser.Models
{
    /// <summary>
    /// Represents Currency entity
    /// </summary>
    public class Currency
    {
       
        /// <summary>
        /// Gets or sets currency unique code string
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// Gets or sets currency name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets currency num code
        /// </summary>
        public int NumCode { get; set; }

        /// <summary>
        /// Gets or sets currency char code
        /// </summary>
        public string CharCode { get; set; }


        /// <summary>
        /// Gets or sets currency exchange rates sorted by dates asc
        /// </summary>
        public List<ExchangeRate> ExchangeRates { get; set; }

        /// <summary>
        /// Initializes a new instance of the Currency class
        /// </summary>
        public Currency()
        {

        }

    }
}
