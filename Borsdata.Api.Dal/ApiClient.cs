using Borsdata.Api.Dal.Infrastructure;
using Borsdata.Api.Dal.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

namespace Borsdata.Api.Dal
{
    public class ApiClient : IDisposable
    {
        HttpClient _client;
        string _urlParameters;
        Stopwatch _timer;

        public ApiClient()
        {
            _urlParameters = "?authKey=<API KEY>";
        
            _timer = Stopwatch.StartNew();
        }


        public InstrumentRespV1 GetInstruments()
        {
            string url = string.Format("http://bdapidev.azurewebsites.net/api/v1/instruments/");
            HttpResponseMessage response = WebbCall(url, _urlParameters);

            if (response.IsSuccessStatusCode)
            {
                string json = response.Content.ReadAsStringAsync().Result;
                InstrumentRespV1 res = JsonConvert.DeserializeObject<InstrumentRespV1>(json);
                return res;
            }
            else
            {
                Console.WriteLine("GetInstruments {0} ({1})", (int)response.StatusCode, response.ReasonPhrase);
            }

            return null;
        }

        public ReportsYearRespV1 GetReportsYear(long borsdataId)
        {
         
            string url = string.Format("http://bdapidev.azurewebsites.net/api/v1/instruments/{0}/reports/year", borsdataId);
            HttpResponseMessage response = WebbCall(url, _urlParameters);

            if (response.IsSuccessStatusCode)
            {
                string json = response.Content.ReadAsStringAsync().Result;
                ReportsYearRespV1 res = JsonConvert.DeserializeObject<ReportsYearRespV1>(json);
                return res;
            }
            else
            {
                Console.WriteLine("GetReportsYear {0} ({1})", (int)response.StatusCode, response.ReasonPhrase);
            }

            return null;
        }

        public ReportsR12RespV1 GetReportsR12(long borsdataId)
        {

            string url = string.Format("http://bdapidev.azurewebsites.net/api/v1/instruments/{0}/reports/r12", borsdataId);
            HttpResponseMessage response = WebbCall(url, _urlParameters);

            if (response.IsSuccessStatusCode)
            {
                string json = response.Content.ReadAsStringAsync().Result;
                ReportsR12RespV1 res = JsonConvert.DeserializeObject<ReportsR12RespV1>(json);
                return res;
            }
            else
            {
                Console.WriteLine("GetReportsR12 {0} ({1})", (int)response.StatusCode, response.ReasonPhrase);
            }

            return null;
        }

        public ReportsQuarterRespV1 GetReportsQuarter(long borsdataId)
        {

            string url = string.Format("http://bdapidev.azurewebsites.net/api/v1/instruments/{0}/reports/quarter", borsdataId);
            HttpResponseMessage response = WebbCall(url, _urlParameters);

            if (response.IsSuccessStatusCode)
            {
                string json = response.Content.ReadAsStringAsync().Result;
                ReportsQuarterRespV1 res = JsonConvert.DeserializeObject<ReportsQuarterRespV1>(json);
                return res;
            }
            else
            {
                Console.WriteLine("GetReportsQuarter {0} ({1})", (int)response.StatusCode, response.ReasonPhrase);
            }

            return null;
        }


        public StockPricesRespV1 GetStockPrices(long borsdataId)
        {
            string url = string.Format("http://bdapidev.azurewebsites.net/api/v1/instruments/{0}/stockprices", borsdataId);
            HttpResponseMessage response = WebbCall(url, _urlParameters);

            if (response.IsSuccessStatusCode)
            {
                string json = response.Content.ReadAsStringAsync().Result;
                StockPricesRespV1 res = JsonConvert.DeserializeObject<StockPricesRespV1>(json);
                return res;
            }
            else
            {
                Console.WriteLine("GetStockPrices {0} ({1})", (int)response.StatusCode, response.ReasonPhrase);
            }

            return null;
        }


        public StockPricesRespV1 GetStockPrices(long borsdataId, DateTime from, DateTime to)
        {
            string url = string.Format("http://bdapidev.azurewebsites.net/api/v1/instruments/{0}/stockprices", borsdataId);
            string urlPar = string.Format(_urlParameters + "&from={0}&to={1}", from.ToShortDateString(), to.ToShortDateString());
            HttpResponseMessage response = WebbCall(url, urlPar);



            if (response.IsSuccessStatusCode)
            {
                string json = response.Content.ReadAsStringAsync().Result;
                StockPricesRespV1 res = JsonConvert.DeserializeObject<StockPricesRespV1>(json);
                return res;
            }
            else
            {
                Console.WriteLine("GetStockPrices time  {0} ({1})", (int)response.StatusCode, response.ReasonPhrase);
            }

            return null;
        }


        public KpisHistoryRespV1 GetKpiHistory(long borsdataId, int KpiId, ReportType rt, PriceType pt)
        {

            string url = string.Format("http://bdapidev.azurewebsites.net/api/v1/Instruments/{0}/kpis/{1}/{2}/{3}/history", borsdataId, KpiId, rt.ToString(), pt.ToString());
            string urlPar = string.Format(_urlParameters);
            HttpResponseMessage response = WebbCall(url, urlPar);

            if (response.IsSuccessStatusCode)
            {
                string json = response.Content.ReadAsStringAsync().Result;
                KpisHistoryRespV1 res = JsonConvert.DeserializeObject<KpisHistoryRespV1>(json);
                return res;
            }
            else
            {
                Console.WriteLine("GetStockPrices time  {0} ({1})", (int)response.StatusCode, response.ReasonPhrase);
            }

            return null;
        }


        public KpisRespV1 GetKpiScreenerSingle(long borsdataId, int KpiId, string rt, PriceType pt)
        {
            string url = string.Format("http://bdapidev.azurewebsites.net/api/v1/Instruments/{0}/kpis/{1}/{2}/{3}", borsdataId, KpiId, rt.ToString(), pt.ToString());
            string urlPar = string.Format(_urlParameters);
            HttpResponseMessage response = WebbCall(url, urlPar);

            if (response.IsSuccessStatusCode)
            {
                string json = response.Content.ReadAsStringAsync().Result;
                KpisRespV1 res = JsonConvert.DeserializeObject<KpisRespV1>(json);
                return res;
            }
            else
            {
                Console.WriteLine("GetStockPrices time  {0} ({1})", (int)response.StatusCode, response.ReasonPhrase);
            }

            return null;
        }



        public KpisAllCompRespV1 GetKpiScreener(int KpiId, string rt, PriceType pt)
        {

            string url = string.Format("http://bdapidev.azurewebsites.net/api/v1/instruments/kpis/{0}/{1}/{2}", KpiId, rt.ToString(), pt.ToString());
            string urlPar = string.Format(_urlParameters);
            HttpResponseMessage response = WebbCall(url, urlPar);

            if (response.IsSuccessStatusCode)
            {
                string json = response.Content.ReadAsStringAsync().Result;
                KpisAllCompRespV1 res = JsonConvert.DeserializeObject<KpisAllCompRespV1>(json);
                return res;
            }
            else
            {
                Console.WriteLine("GetStockPrices time  {0} ({1})", (int)response.StatusCode, response.ReasonPhrase);
            }

            return null;
        }


        public MarketsRespV1 GetMarkets()
        {
            string url = string.Format("http://bdapidev.azurewebsites.net/api/v1/markets/");
            HttpResponseMessage response = WebbCall(url, _urlParameters);

            if (response.IsSuccessStatusCode)
            {
                string json = response.Content.ReadAsStringAsync().Result;
                MarketsRespV1 res = JsonConvert.DeserializeObject<MarketsRespV1>(json);
                return res;
            }
            else
            {
                Console.WriteLine("GetMarkets {0} ({1})", (int)response.StatusCode, response.ReasonPhrase);
            }

            return null;
        }


        public SectorsRespV1 GetSectors()
        {
            string url = string.Format("http://bdapidev.azurewebsites.net/api/v1/sectors/");
            HttpResponseMessage response = WebbCall(url, _urlParameters);

            if (response.IsSuccessStatusCode)
            {
                string json = response.Content.ReadAsStringAsync().Result;
                SectorsRespV1 res = JsonConvert.DeserializeObject<SectorsRespV1>(json);
                return res;
            }
            else
            {
                Console.WriteLine("GetSectors {0} ({1})", (int)response.StatusCode, response.ReasonPhrase);
            }

            return null;
        }


        public CountriesRespV1 GetCountries()
        {
            string url = string.Format("http://bdapidev.azurewebsites.net/api/v1/countries/");
            HttpResponseMessage response = WebbCall(url, _urlParameters);

            if (response.IsSuccessStatusCode)
            {
                string json = response.Content.ReadAsStringAsync().Result;
                CountriesRespV1 res = JsonConvert.DeserializeObject<CountriesRespV1>(json);
                return res;
            }
            else
            {
                Console.WriteLine("GetCountries {0} ({1})", (int)response.StatusCode, response.ReasonPhrase);
            }

            return null;
        }


        public BranchesRespV1 GetBranches()
        {
            string url = string.Format("http://bdapidev.azurewebsites.net/api/v1/branches/");
            HttpResponseMessage response = WebbCall(url, _urlParameters);

            if (response.IsSuccessStatusCode)
            {
                string json = response.Content.ReadAsStringAsync().Result;
                BranchesRespV1 res = JsonConvert.DeserializeObject<BranchesRespV1>(json);
                return res;
            }
            else
            {
                Console.WriteLine("GetBranches {0} ({1})", (int)response.StatusCode, response.ReasonPhrase);
            }

            return null;
        }


        HttpResponseMessage WebbCall(string url, string urlParameters)
        {
            _client = new HttpClient();
            _client.BaseAddress = new Uri(url);
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            SleepBeforeNewApiCall(); // Sleep if needed to avoid RateLimit
            HttpResponseMessage response = _client.GetAsync(urlParameters).Result;

            Console.WriteLine(url + " " + urlParameters);

            if ((int)response.StatusCode == 429) // We still get RateLimit error. Sleep more
            {
                //Console.WriteLine("StatusCode == 429.. Sleep!!");
                System.Threading.Thread.Sleep(500);
                response = _client.GetAsync(urlParameters).Result;
            }

            return response;
        }


        /// <summary>
        /// Check if the time sice last API call is less than 500ms. Then sleep to avoid RateLimit.
        /// </summary>
        void SleepBeforeNewApiCall()
        {
            _timer.Stop();
            if (_timer.ElapsedMilliseconds < 500)
            {
                int sleepms = 550 - (int)_timer.ElapsedMilliseconds; //Add 50 extra ms.
                Console.WriteLine("Sleep Before New Api Call ms:" + sleepms);
                System.Threading.Thread.Sleep(sleepms);
            }
            _timer.Restart();
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _client.Dispose();
                }

                disposedValue = true;
            }
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion

    }
}
