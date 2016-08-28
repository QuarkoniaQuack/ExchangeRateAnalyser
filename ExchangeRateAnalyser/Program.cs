using ExchangeRateAnalyser.Models;
using ExchangeRateAnalyser.Utils;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExchangeRateAnalyser
{
    class Program
    {
        const string startDateArgCode = "-startDate";
        const string endDateArgCode = "-endDate";
        const string capitalArgCode = "-capital";

        static void Main(string[] args)
        {
            DateTime startDate = DateTime.MinValue;
            DateTime endDate = DateTime.MinValue;
            decimal capital = 0;

            string startDateString = "";
            string endDateString = "";
            string capitalString = "";

            //Read user defined parameters
            if (args.Length > 0)
            {
                for (int i = 0; i < args.Length; i++)
                {
                    if (args[i] == startDateArgCode
                        && args.Length > i)
                    {
                        startDateString = args[i + 1];
                    }
                    else if (args[i] == endDateArgCode
                        && args.Length > i)
                    {
                        endDateString = args[i + 1];
                    }
                    else if (args[i] == capitalArgCode
                        && args.Length > i)
                    {
                        capitalString = args[i + 1];
                    }
                }
            }

            /// а если прогу запускают батничком??
            //check if start date parameter is valid and ask to re-enter it in case it is not
            do
            {
                if (string.IsNullOrEmpty(startDateString))
                {
                    Console.WriteLine(Strings.InfoEnterValidStartDate);

                    startDateString = Console.ReadLine();
                }

                if (DateTime.TryParse(startDateString, CultureInfo.InvariantCulture, DateTimeStyles.None, out startDate))
                {
                    if (startDate.Date > DateTime.Today.Date)
                    {
                        Console.WriteLine(Strings.ErrorStartDateGreaterThanToday);
                        startDate = DateTime.MinValue;
                        startDateString = "";
                    }
                }
            } while (startDate == DateTime.MinValue);


            /// а если прогу запускают батничком??
            //check if end date parameter is valid and ask to re-enter it in case it is not
            do
            {
                if (string.IsNullOrEmpty(endDateString))
                {
                    Console.WriteLine(Strings.InfoEnterValidEndDate);

                    endDateString = Console.ReadLine();
                }

                if (DateTime.TryParse(endDateString, CultureInfo.InvariantCulture, DateTimeStyles.None, out endDate))
                {
                    if (endDate.Date > DateTime.Today.Date)
                    {
                        Console.WriteLine(Strings.ErrorEndDateGreaterThanToday);
                        endDate = DateTime.MinValue;
                        endDateString = "";
                    }
                    else if (startDate.Date > endDate.Date)
                    {
                        Console.WriteLine(Strings.ErrorStatDateGreaterThanEndDate);
                        endDate = DateTime.MinValue;
                        endDateString = "";
                    }
                }
            } while (endDate == DateTime.MinValue);

            /// а если прогу запускают батничком??
            //check if capital parameter is valid and ask to re-enter it in case it is not
            do
            {
                if (string.IsNullOrEmpty(capitalString))
                {
                    Console.WriteLine(Strings.InfoEnterValidInitialCapital);

                    capitalString = Console.ReadLine();
                }

                if (!decimal.TryParse(capitalString, NumberStyles.Number, CultureInfo.InvariantCulture, out capital)
                    || capital <= 0)
                {
                    capitalString = "";
                }

            } while (string.IsNullOrEmpty(capitalString));

            Analyze(startDate, endDate, capital);

            Console.ReadKey();
        }

        /// <summary>
        /// Runs ExchangeRateAnalyzer
        /// </summary>
        /// <param name="startDate">Start date for analysis</param>
        /// <param name="endDate">End date for analysis</param>
        /// <param name="сapital">Initial amount of money</param>
        static void Analyze(DateTime startDate, DateTime endDate, decimal сapital)
        {
            try
            {
                Console.WriteLine(Strings.InfoStartedWithParameters, startDate.ToString("yyyy-MM-dd"), endDate.ToString("yyyy-MM-dd"), сapital.ToString("F2"));
                //Initialise ExchangeRateAnalyzer with user-defined parameters
                var analyzer = new ExchangeRateAnalyser.Models.ExchangeRateAnalyser(startDate, endDate, сapital);

                var analysisResult = analyzer.PerformAnalysis();

                if (string.IsNullOrEmpty(analysisResult.Error)) //if there was no errors
                {
                    Console.WriteLine(Strings.InfoAnalysisCompleted);

                    if (analysisResult.Profit < 0) //in case there was no possible profitable deals show warning message to the user
                    {
                        Console.WriteLine(Strings.WarningNoProfit);
                    }
                    else
                    {
                        Console.WriteLine(Strings.InfoSeeResultSet);
                    }

                    Console.WriteLine(analysisResult.ToString());

                }
                else //if there was any error
                {
                    Console.WriteLine(analysisResult.Error);
                }
            }
            catch (Exception ex)
            {
                Logger.Log(Strings.ErrorOccured, ex);
                Console.WriteLine(Strings.ErrorOccuredTryLater);
            }

        }
    }
}
