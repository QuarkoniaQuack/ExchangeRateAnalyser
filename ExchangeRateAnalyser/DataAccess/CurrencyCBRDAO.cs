using ExchangeRateAnalyser.Models;
using ExchangeRateAnalyser.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.Xml.XPath;

namespace ExchangeRateAnalyser.DataAccess
{
    /// <summary>
    /// Defines methods for fetching currency exchange rates using The Central Bank of the Russian Federation API
    /// </summary>
    public class CurrencyCBRDAO : ICurrencyDAO
    {
        /// <summary>
        /// {0} - dd/MM/yyy
        /// </summary>
        private string ExchangeRateAPIURIFormat = System.Configuration.ConfigurationManager.AppSettings["ExchangeRateAPIURIFormat"];


        private const string NameNode = "Name";
        private const string NumCodeNode = "NumCode";
        private const string CharCodeNode = "CharCode";
        private const string NominalNode = "Nominal";
        private const string ValueNode = "Value";

        /// <summary>
        /// Fetches exchange rates xml data files using ExchangeRateAPIURIFormat uri string
        /// </summary>
        /// <param name="startDate">Start date</param>
        /// <param name="endDate">End date</param>
        /// <returns>List of obtained Currencies including their exchange rates for the period [StartDate;EndDate].</returns>
        public List<Currency> GetCurrencyRates(DateTime startDate, DateTime endDate)
        {
            Dictionary<int, Currency> curDictionary = null;

            if (!string.IsNullOrEmpty(ExchangeRateAPIURIFormat))
            {
                try
                {
                    using (WebClient client = new WebClient())
                    {
                        TimeSpan diff = endDate.Date - startDate.Date;

                        curDictionary = new Dictionary<int, Currency>();
                        Currency currency;
                        ExchangeRate rate;

                        for (int i = 0; i < diff.TotalDays+1; i++)
                        {
                            DateTime date = startDate.AddDays(i);

                            var cbrURI = string.Format(ExchangeRateAPIURIFormat, date.ToString("dd\\/MM\\/yyyy"));

                            using (Stream xmlDataStream = client.OpenRead(cbrURI))
                            {
                                XPathDocument doc = new XPathDocument(xmlDataStream);
                                
                                XPathNavigator nav = ((IXPathNavigable)doc).CreateNavigator();
                                
                                XPathNodeIterator iter = nav.Select("/ValCurs/Valute");
                                
                                while (iter.MoveNext())
                                {
                                    var numCodeNode = iter.Current.SelectSingleNode("NumCode");
                                    if (numCodeNode != null)
                                    {
                                        int numCode = numCodeNode.ValueAsInt;
                                        if (curDictionary.ContainsKey(numCode))
                                        {
                                            currency = curDictionary[numCode];
                                        }
                                        else
                                        {
                                            currency = LoadCurrency(iter.Current);
                                            if (currency != null)
                                            {
                                                curDictionary[numCode] = currency;
                                            }
                                            else
                                            {
                                                continue;
                                            }
                                        }

                                        rate = LoadExchangeRate(iter.Current, date);
                                        if (rate != null)
                                        {
                                            currency.ExchangeRates.Add(rate);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                catch (WebException ex)
                {
                    Logger.Log(Strings.ErrorWebException, ex);
                    return new List<Currency>();
                }
                catch (Exception ex)
                {
                    Logger.Log(Strings.ErrorOccured, ex);
                    return new List<Currency>();
                }
            }

            return curDictionary != null ? curDictionary.Values.ToList() : new List<Currency>();
        }

        private Currency LoadCurrency(XPathNavigator node)
        {
            Currency currency = null;
            try
            {
                currency = new Currency()
                       {
                           NumCode = node.SelectSingleNode("NumCode").ValueAsInt,
                           CharCode = node.SelectSingleNode("CharCode").Value,
                           Name = node.SelectSingleNode("Name").Value,
                           ExchangeRates = new List<ExchangeRate>()
                       };
            }
            catch (InvalidCastException ex)
            {
                Logger.Log(Strings.ErrorOccured, ex);
            }
            catch (ArgumentException ex)
            {
                Logger.Log(Strings.ErrorOccured, ex);
            }

            catch (XPathException ex)
            {
                Logger.Log(Strings.ErrorOccured, ex);
            }
            return currency;
        }

        private ExchangeRate LoadExchangeRate(XPathNavigator node, DateTime date)
        {
            ExchangeRate rate = null;
            try
            {
                string valueStr = node.SelectSingleNode("Value").Value;
                double value;
                if (double.TryParse(valueStr, System.Globalization.NumberStyles.Number, System.Globalization.CultureInfo.GetCultureInfo("ru-ru"), out value))
                {
                    rate = new ExchangeRate(date, value, node.SelectSingleNode("Nominal").ValueAsInt);
                }

            }
            catch (ArgumentException ex)
            {
                Logger.Log(Strings.ErrorOccured, ex);
            }
            catch (XPathException ex)
            {
                Logger.Log(Strings.ErrorOccured, ex);
            }
            return rate;
        }

    }
}
