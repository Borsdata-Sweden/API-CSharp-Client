// All sample code is provided for illustrative purposes only.
// These examples have not been thoroughly tested under all conditions.Börsdata cannot guarantee or imply reliability, serviceability, or function of these programs.
// All programs contained herein are provided to you “AS IS” without any warranties of any kind. 

using Borsdata.Api.Dal;
using Borsdata.Api.Dal.Infrastructure;
using Borsdata.Api.Dal.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Borsdata.Api.SimpleClient
{
    class Program 
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Start Test Client!!");

            InstrumentsWithMetadata();
            // StockPricesTimeRange();
            // Reports();
            // Kpis();
            // StockPricesForAllInstruments();

            Console.ReadKey();
        }



        static void Kpis()
        {
            ApiClient api = new ApiClient();

            // Get Historical Kpi Value for one Instrument and one Kpi
            var kpis = api.GetKpiHistory(3, 2, ReportType.year, PriceType.mean);

            // Get One kpi screener value for one Instrument
            var kpis2 = api.GetKpiScreenerSingle(97, 2, "15year", PriceType.mean);

            // Get list of kpi Screener value for all Instruments
            var kpis3 = api.GetKpiScreener(2, "15year", PriceType.mean);

        }


        // Get all Meta data about Instruments and connect data to Instrument Object.
        static void InstrumentsWithMetadata()
        {
            ApiClient api = new ApiClient();

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
            ApiClient api = new ApiClient();
            InstrumentRespV1 inst = api.GetInstruments();

            // Get all stockprices for each Instrument
            foreach (var i in inst.Instruments)
            {
                //StockPricesRespV1 sp = api.GetStockPrices(i.InsId.Value, Convert.ToDateTime("2018-12-01"), DateTime.Today);

                StockPricesRespV1 sp = api.GetStockPrices(i.InsId.Value);
                Console.WriteLine(DateTime.Now.ToLongTimeString() +  "  GetStockPrices() " + i.InsId.Value + " " + sp.StockPricesList.Count());
            }
        }





        // Stockprices in Time period for Company ABB => InsId=3
        static void StockPricesTimeRange()
        {
            ApiClient api = new ApiClient();
            StockPricesRespV1 sp2 = api.GetStockPrices(3, Convert.ToDateTime("2018-06-03"), Convert.ToDateTime("2018-10-10"));
            Console.WriteLine("sp2 count: " + sp2.StockPricesList.Count());

            StockPricesRespV1 sp3 = api.GetStockPrices(3, Convert.ToDateTime("2018-01-10"), DateTime.Today);
            Console.WriteLine("sp3 count: " + sp3.StockPricesList.Count());
        }




        //Get Reports for one company (ABB)
        static void Reports()
        {
            ApiClient api = new ApiClient();
            ReportsYearRespV1 rY = api.GetReportsYear(3);
            Console.WriteLine("rY count: " + rY.Reports.Count());

            ReportsR12RespV1 r12 = api.GetReportsR12(3);
            Console.WriteLine("r12 count: " + r12.Reports.Count());

            ReportsQuarterRespV1 rQ = api.GetReportsQuarter(3);
            Console.WriteLine("rQ count: " + rQ.Reports.Count());

        }
    }

}
