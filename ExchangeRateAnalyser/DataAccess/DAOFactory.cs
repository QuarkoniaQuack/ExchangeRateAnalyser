using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ExchangeRateAnalyser.DataAccess
{
    public static class DAOFactory
    {
        private static string ExchangeRateDAO = System.Configuration.ConfigurationManager.AppSettings["ExchangeRateDAO"];

        /// <summary>
        /// Gets data access object for fetching currency data
        /// </summary>
        /// <returns>Current data access object for fetching currency data.</returns>
        public static ICurrencyDAO GetCurrencyDAO()
        {
            Type daoType = Assembly.GetExecutingAssembly().GetType(string.Format("ExchangeRateAnalyser.DataAccess.{0}", ExchangeRateDAO));

            return Activator.CreateInstance(daoType) as ICurrencyDAO;
        }
    }
}
