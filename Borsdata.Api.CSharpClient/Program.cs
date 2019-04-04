// All sample code is provided for illustrative purposes only.
// These examples have not been thoroughly tested under all conditions. 
// Börsdata cannot guarantee or imply reliability, serviceability, or function of these programs.
// All programs contained herein are provided to you “AS IS” without any warranties of any kind. 

using Borsdata.Api.Dal;
using Borsdata.Api.Dal.Infrastructure;
using Borsdata.Api.Dal.Model;
using System;
using System.Linq;

namespace Borsdata.Api.SimpleClient
{
    class Program 
    {

        static string _apiKey = "xxxx"; // Add your Api Key. 
      
        static void Main(string[] args)
        {
            Console.WriteLine("Start Test Client!!");

            LastStockPricesForAllInstruments();

            // StockPricesForAllInstruments();
            // InstrumentsWithMetadata();
            // StockPricesTimeRange();

            // Reports();
            // LatestPE();

            // Kpis();
            // InstrumentsUpdated();
            // KpisUpdated();
            // GetStockSplits();

            Console.ReadKey();
        }





        // Get all Meta data about Instruments and connect data to Instrument Object.
        static void InstrumentsWithMetadata()
        {
            ApiClient api = new ApiClient(_apiKey);

            // get all Meta data
            CountriesRespV1 cr = api.GetCountries();
            BranchesRespV1 br = api.GetBranches();
            SectorsRespV1 sr = api.GetSectors();
            MarketsRespV1 mr = api.GetMarkets();

            // Get all Instruments
            InstrumentRespV1 inst = api.GetInstruments();

            // Connect Meta data to Instruments
            foreach(InstrumentV1 c in inst.Instruments)
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

            // Print Data to see all is ok
            foreach (InstrumentV1 c in inst.Instruments)
            {
                if (c.Instrument == Instrument.Index) // Index don't have any Branch or Sector
                {
                    Console.WriteLine(c.Name + " : " + c.CountryModel.Name + " : " + c.MarketModel.Name  );
                }
                else
                {
                    Console.WriteLine(c.Name + " : " + c.CountryModel.Name + " : " + c.MarketModel.Name + " : " + c.BranchModel.Name + " : " + c.SectorModel.Name);
                }
                
            }
        }


        static void StockPricesForAllInstruments()
        {
            ApiClient api = new ApiClient(_apiKey);
            InstrumentRespV1 inst = api.GetInstruments();

            // Get all stockprices for each Instrument
            foreach (var i in inst.Instruments)
            {
                //StockPricesRespV1 sp = api.GetStockPrices(i.InsId.Value, Convert.ToDateTime("2018-12-01"), DateTime.Today);

                StockPricesRespV1 sp = api.GetStockPrices(i.InsId.Value);
                Console.WriteLine(DateTime.Now.ToLongTimeString() +  "  GetStockPrices() " + i.InsId.Value + " " + sp.StockPricesList.Count());
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



        // Stockprices in Time period for Company ABB => InsId=3
        static void StockPricesTimeRange()
        {
            ApiClient api = new ApiClient(_apiKey);
            StockPricesRespV1 sp2 = api.GetStockPrices(3, Convert.ToDateTime("2018-06-03"), Convert.ToDateTime("2018-10-10"));
            Console.WriteLine("sp2 count: " + sp2.StockPricesList.Count());

            StockPricesRespV1 sp3 = api.GetStockPrices(3, Convert.ToDateTime("2018-01-10"), DateTime.Today);
            Console.WriteLine("sp3 count: " + sp3.StockPricesList.Count());
        }




        //Get Reports for one company (ABB)
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

            ReportsQuarterRespV1 rQ = api.GetReportsQuarter(3);
            Console.WriteLine("Quarter count: " + rQ.Reports.Count());

        }

        // Calc latest P/E for HM
        static void LatestPE()
        {
            ApiClient api = new ApiClient(_apiKey);
            int insId = 97; // HM

            StockPricesRespV1 spResp = api.GetStockPrices(insId, DateTime.Today.AddDays(-7), DateTime.Today); // We need lastest price. Ask for one week back.
            StockPriceV1 lastSp = spResp.StockPricesList.Last(); // First in list in latest stockprice. (Order Asc)

            ReportsR12RespV1 r12 = api.GetReportsR12(insId);
            ReportR12V1 lastReport = r12.Reports.First(); // Get last R12 report (Desc)

            var pe = lastSp.C / lastReport.EarningsPerShare;
            Console.WriteLine("Lastest R12 P/E is : " + pe);
        }





        // Get list of last 100 updated Instrument where Instrument data or Instrument Reports is changed.
        static void InstrumentsUpdated()
        {
            ApiClient api = new ApiClient(_apiKey);
            InstrumentsUpdatedRespV1 resp = api.GetInstrumentsUpdated();
            Console.WriteLine("Updated count: " + resp.Instruments.Count());
        }


        /// <summary>
        /// Test get some Kpis
        /// </summary>
        static void Kpis()
        {
            ApiClient api = new ApiClient(_apiKey);

            // Get Historical Kpi Value for one Instrument and one Kpi (ABB=3, PE=2)
            var kpis = api.GetKpiHistory(3, 2, ReportType.year, PriceType.mean);

            // Get One kpi screener value for one Instrument. (HM=97, PE=2)
            var kpis2 = api.GetKpiScreenerSingle(97, 2, TimeType._15year, CalcType.mean);

            // Get list of kpi Screener value for all Instruments. PE=2
            var kpis3 = api.GetKpiScreener(2, TimeType._15year, CalcType.mean);

        }



        // Get datetime of last Kpis calculation.
        static void KpisUpdated()
        {
            ApiClient api = new ApiClient(_apiKey);
   
            KpisCalcUpdatedRespV1 kpisUpdated = api.GetKpisCalcUpdated();
            Console.WriteLine("Updated time: " + kpisUpdated.kpisCalcUpdated.ToLongDateString());
        }

        // Get list of Instruments with StockSplit. 
        static void GetStockSplits()
        {
            ApiClient api = new ApiClient(_apiKey);

            StockSplitRespV1 splits = api.GetStockSplits();
            Console.WriteLine("StockSplits count: " + splits.stockSplitList.Count());
        }

        

    }

}
