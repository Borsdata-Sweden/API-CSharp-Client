// All sample code is provided for illustrative purposes only.
// These examples have not been thoroughly tested under all conditions.
// Börsdata cannot guarantee or imply reliability, serviceability, or function of these programs.
// All programs contained herein are provided to you “AS IS” without any warranties of any kind.

// Not all API functions is added.


// -------------------
// Update 2020-10-21
// Added maxcount for 20 years data
// Added new Report values
//
// Update 2024-02-19
// Added new flags. Original
// -------------------




using Borsdata.Api.Dal;
using Borsdata.Api.Dal.Infrastructure;
using Borsdata.Api.Dal.Model;
using System;
using System.Linq;

namespace Borsdata.Api.SimpleClient
{
    class Program
    {
        static string _apiKey = "YOUR APIKEY"; // Add your API Key.

        static void Main(string[] args)
        {
            Console.WriteLine("Start Test Client!!");

            StockPricesForOneInstruments(3); // Get stockprices for ABB (InsId=3)

            //LastStockPricesForAllInstruments();

            //StockPricesForAllInstruments();
            InstrumentsWithMetadata();
            StockPricesTimeRange();

            Reports();
            LatestPE();

            HistoryKpi();
            ScreenerKpi();

            InstrumentsUpdated();
            KpisUpdated();
            GetStockSplits();

            Console.ReadKey();
        }

        /// <summary> Get all meta data about instruments and connect data to instrument object.</summary>
        static void InstrumentsWithMetadata()
        {
            ApiClient api = new ApiClient(_apiKey);

            // Get all meta data
            CountriesRespV1 cr = api.GetCountries();
            BranchesRespV1 br = api.GetBranches();
            SectorsRespV1 sr = api.GetSectors();
            MarketsRespV1 mr = api.GetMarkets();

            // Get all instruments
            InstrumentRespV1 inst = api.GetInstruments();

            // Connect meta data to instruments
            foreach (InstrumentV1 c in inst.Instruments)
            {
                CountryV1 country = cr.Countries.FirstOrDefault(o => o.Id == c.CountryId);
                c.CountryModel = country;

                MarketV1 Market = mr.Markets.FirstOrDefault(o => o.Id == c.MarketId);
                c.MarketModel = Market;


                BranchV1 Branch = br.Branches.FirstOrDefault(o => o.Id == c.BranchId);
                c.BranchModel = Branch;

                SectorV1 Sector = sr.Sectors.FirstOrDefault(o => o.Id == c.SectorId);
                c.SectorModel = Sector;
            }

            // Print data to see all is OK
            foreach (InstrumentV1 c in inst.Instruments)
            {
                if (c.Instrument != Instrument.Stocks) // Index don't have any Branch or Sector
                {
                    Console.WriteLine(c.Name + " : " + c.CountryModel.Name + " : " + c.MarketModel.Name);
                }
                else
                {
                    Console.WriteLine(c.Name + " : " + c.CountryModel.Name + " : " + c.MarketModel.Name + " : " + c.BranchModel.Name + " : " + c.SectorModel.Name);
                }
            }
        }

        /// <summary>
        /// Get stock prices for one instrument
        /// 20 year history
        /// </summary>
        static void StockPricesForOneInstruments(long InsId)
        {
            ApiClient api = new ApiClient(_apiKey);
            StockPricesRespV1 spList = api.GetStockPrices(InsId);

            foreach (var sp in spList.StockPricesList)
            {
                Console.WriteLine(sp.D + " : " + sp.C);
            }
        }

        /// <summary>
        /// 1. Call GetInstruments to get a list of all instruments
        /// 2. Call GetStockPrices for each InsId
        /// </summary>
        static void StockPricesForAllInstruments()
        {
            ApiClient api = new ApiClient(_apiKey);
            InstrumentRespV1 inst = api.GetInstruments();

            // Get all stock prices for each instrument
            foreach (var i in inst.Instruments)
            {
                //--Get for a time range
                //StockPricesRespV1 sp = api.GetStockPrices(i.InsId.Value, Convert.ToDateTime("2018-12-01"), DateTime.Today);

                StockPricesRespV1 sp = api.GetStockPrices(i.InsId.Value);
                Console.WriteLine(DateTime.Now.ToLongTimeString() + "  GetStockPrices() " + i.InsId.Value + " " + sp.StockPricesList.Count());
            }
        }

        static void LastStockPricesForAllInstruments()
        {
            ApiClient api = new ApiClient(_apiKey);

            StockPricesLastRespV1 spList = api.GetStockpricesLast();

            foreach (var sp in spList.StockPricesList)
            {
                //print InsID : closeprice
                Console.WriteLine(sp.I + " : " + sp.C);
            }
        }

        /// <summary> Stock prices in time period for company ABB => InsId=3 </summary>
        static void StockPricesTimeRange()
        {
            ApiClient api = new ApiClient(_apiKey);
            StockPricesRespV1 sp2 = api.GetStockPrices(3, Convert.ToDateTime("2018-06-03"), Convert.ToDateTime("2018-10-10"));
            Console.WriteLine("sp2 count: " + sp2.StockPricesList.Count());

            StockPricesRespV1 sp3 = api.GetStockPrices(3, Convert.ToDateTime("2018-01-10"), DateTime.Today);
            Console.WriteLine("sp3 count: " + sp3.StockPricesList.Count());
        }

        /// <summary> Get reports for one company (ABB) </summary>
        static void Reports()
        {
            ApiClient api = new ApiClient(_apiKey);

            // One call to get all report for one Instrument
            ReportsRespV1 rAll = api.GetReports(3);
            Console.WriteLine("Year count: " + rAll.ReportsYear.Count());
            Console.WriteLine("R12 count: " + rAll.ReportsR12.Count());
            Console.WriteLine("Quarter count: " + rAll.ReportsQuarter.Count());

            // You can also get list of reports for each Year, R12, Quarter
            ReportsYearRespV1 rY = api.GetReportsYear(3);
            Console.WriteLine("Year count: " + rY.Reports.Count());

            ReportsR12RespV1 r12 = api.GetReportsR12(3);
            Console.WriteLine("r12 count: " + r12.Reports.Count());

            // Select number count and if original reportdata
            ReportsQuarterRespV1 rQ = api.GetReportsQuarter(750,3,0);
            Console.WriteLine("Quarter count: " + rQ.Reports.Count());

            ReportsQuarterRespV1 rQ2 = api.GetReportsQuarter(750, 3, 1);
            Console.WriteLine("Quarter count: " + rQ2.Reports.Count());

        }


        /// <summary>
        /// Calculate latest P/E 
        /// Sample to calculate my own KPI
        /// Currency_Ratio convert Report values to Stockprice Currency
        /// </summary>
        static void LatestPE()
        {
            ApiClient api = new ApiClient(_apiKey);
            int insId = 750; // Evolution
            bool originalReportdata = false; // Return data in original report currency ?

            StockPricesRespV1 spResp = api.GetStockPrices(insId, DateTime.Today.AddDays(-7), DateTime.Today); // We need lastest price. Ask for one week back.
            StockPriceV1 lastSp = spResp.StockPricesList.Last(); // First in list in latest stock price. (Order asc)

            if (originalReportdata==false)
            {
                ReportsR12RespV1 r12 = api.GetReportsR12(insId, 5, original:0);
                ReportR12V1 lastReport = r12.Reports.First(); // Get last R12 report (Desc)

                //  closeprice / Epa 
                var pe = lastSp.C / (lastReport.EarningsPerShare);
                Console.WriteLine("Latest R12 P/E is : " + pe);
            }
            else 
            {
                // If we get original report currency we must convert report data to Stockprice currency before Calc Kpis. [Currency_Ratio]

                ReportsR12RespV1 r12 = api.GetReportsR12(insId, 5, original: 1);
                ReportR12V1 lastReport = r12.Reports.First(); // Get last R12 report (Desc)

                //  closeprice / EPS 
                var pe = lastSp.C / (lastReport.EarningsPerShare * lastReport.Currency_Ratio);
                Console.WriteLine("Latest R12 P/E is : " + pe);
            }


        }

        /// <summary> Get list of last 100 updated instruments where instrument data or instrument reports is changed.</summary>
        static void InstrumentsUpdated()
        {
            ApiClient api = new ApiClient(_apiKey);
            InstrumentsUpdatedRespV1 resp = api.GetInstrumentsUpdated();
            Console.WriteLine("Updated count: " + resp.Instruments.Count());
        }

        /// <summary>
        /// Test get some historical KPIs
        /// This gives history values for one KPI
        /// </summary>
        static void HistoryKpi()
        {
            ApiClient api = new ApiClient(_apiKey);

            // Get historical KPI Value for one Instrument and one KPI (ABB=3, PE=2)
            var kpis = api.GetKpiHistory(3, 2, ReportType.year, PriceType.mean);
        }

        static void ScreenerKpi()
        {
            ApiClient api = new ApiClient(_apiKey);

            // Get One KPI screener value for one Instrument. (HM=97, PE=2)
            var kpis2 = api.GetKpiScreenerSingle(97, 2, TimeType._15year, CalcType.mean);
            Console.WriteLine("Single value from Screener :" + kpis2.Value.N + " / " + kpis2.Value.S);

            // Get list of KPI screener value for all instruments. PE=2
            var kpis3 = api.GetKpiScreener(2, TimeType.last, CalcType.latest);
            foreach (var kpi in kpis3.Values)
            {
                Console.WriteLine(kpi.I + " : " + kpi.N + " / " + kpi.S);
            }
        }

        /// <summary> Get datetime of last KPIs calculation.</summary>
        static void KpisUpdated()
        {
            ApiClient api = new ApiClient(_apiKey);

            KpisCalcUpdatedRespV1 kpisUpdated = api.GetKpisCalcUpdated();
            Console.WriteLine("Updated time: " + kpisUpdated.kpisCalcUpdated.ToLongDateString());
        }

        /// <summary> Get list of instruments with stock split.</summary>
        static void GetStockSplits()
        {
            ApiClient api = new ApiClient(_apiKey);

            StockSplitRespV1 splits = api.GetStockSplits();
            Console.WriteLine("StockSplits count: " + splits.stockSplitList.Count());
        }
    }
}
