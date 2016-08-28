using ExchangeRateAnalyser.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExchangeRateAnalyser.Models
{
    public class CurrencyProfit
    {
        public Currency Currency { get; set; }

        public ExchangeRate BestRateToBuy { get; set; }

        public ExchangeRate BestRateToSell { get; set; }

        public double MostProfitableDelta
        {
            get
            {
                if (this.BestRateToBuy != null && this.BestRateToSell != null)
                    return this.BestRateToSell.Rate - this.BestRateToBuy.Rate;
                else
                    return double.MinValue;

            }
        }

        private CurrencyProfit()
        {

        }

        public decimal GetCapitalProfit(decimal initialCapital)
        {
            return (decimal)this.MostProfitableDelta * initialCapital / (decimal)this.BestRateToBuy.Rate;
        }

        public static CurrencyProfit Load(Currency currency)
        {
            CurrencyProfit curProfit = new CurrencyProfit();
            curProfit.Currency = currency;
            curProfit.LoadBestRates();
            return curProfit;
        }

        private bool LoadBestRates()
        {
            var result = true;

            ExchangeRate erBestToBuy = null;
            ExchangeRate erBestToSell = null;

            List<ExchangeRate> exchangeRates = this.Currency.ExchangeRates;

            if (exchangeRates != null && exchangeRates.Count > 0 && exchangeRates[0] != null)
            {
                try
                {
                    double startRateToBuy = exchangeRates[0].Rate;
                    double potentialStartRateToBuy = double.MaxValue;

                    erBestToBuy = exchangeRates[0];
                    ExchangeRate erPotentialBestToBuy = null;

                    erBestToSell = exchangeRates[0];

                    double maxDelta = erBestToSell.Rate - erBestToBuy.Rate;

                    for (int i = 1; i < exchangeRates.Count; i++)
                    {
                        if (exchangeRates[i] != null)
                        {
                            if (maxDelta < exchangeRates[i].Rate - startRateToBuy)
                            {
                                maxDelta = exchangeRates[i].Rate - startRateToBuy;
                                erBestToSell = exchangeRates[i];
                            }

                            if (exchangeRates[i].Rate < startRateToBuy &&
                                exchangeRates[i].Rate < potentialStartRateToBuy)
                            {
                                potentialStartRateToBuy = exchangeRates[i].Rate;
                                erPotentialBestToBuy = exchangeRates[i];
                            }

                            if (maxDelta < exchangeRates[i].Rate - potentialStartRateToBuy)
                            {
                                maxDelta = exchangeRates[i].Rate - potentialStartRateToBuy;

                                startRateToBuy = potentialStartRateToBuy;
                                potentialStartRateToBuy = double.MaxValue;

                                erBestToBuy = erPotentialBestToBuy;
                                erPotentialBestToBuy = null;

                                erBestToSell = exchangeRates[i];
                            }

                        }
                    }

                    if (maxDelta != double.MinValue)
                    {
                        this.BestRateToBuy = erBestToBuy;
                        this.BestRateToSell = erBestToSell;
                    }
                }
                catch (Exception ex)
                {
                    Logger.Log(Strings.ErrorOccured, ex);

                    erBestToBuy = null;
                    erBestToSell = null;
                }
            }


            if (erBestToBuy == null || erBestToSell == null)
            {
                result = false;
            }

            return result;
        }
    }
}
