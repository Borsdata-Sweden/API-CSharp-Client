﻿using Borsdata.Api.Dal.Infrastructure;
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

/// <summary>
/// Sample class to call Borsdata API V1.
/// </summary>
namespace Borsdata.Api.Dal
{
    public class ApiClient : IDisposable
    {
        HttpClient _client;
        string _querystring;                // Query string authKey
        Stopwatch _timer;                   // Check time from last API call to check rate limit
        string _urlRoot;

        public ApiClient(string apiKey)
        {
            _querystring = "?authKey=" + apiKey;

            _timer = Stopwatch.StartNew();
            _urlRoot = "https://apiservice.borsdata.se/";
        }

        /// <summary> Return list of all instruments</summary>
        public InstrumentRespV1 GetInstruments()
        {
            string url = $"{_urlRoot}/v1/instruments";
            HttpResponseMessage response = WebbCall(url, _querystring);

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

        /// <summary> Return list of all reports for one instrument</summary>
        public ReportsRespV1 GetReports(long instrumentId, int maxYearCount = 20, int maxR12QCount = 20, int original = 0)
        {

            string url = $"{_urlRoot}/v1/instruments/{instrumentId}/reports";
            string query = $"{_querystring}&maxYearCount={maxYearCount}&maxR12QCount={maxR12QCount}&original={original}";

            HttpResponseMessage response = WebbCall(url, query);

            if (response.IsSuccessStatusCode)
            {
                string json = response.Content.ReadAsStringAsync().Result;
                ReportsRespV1 res = JsonConvert.DeserializeObject<ReportsRespV1>(json);
                return res;
            }
            else
            {
                Console.WriteLine("GetReports {0} ({1})", (int)response.StatusCode, response.ReasonPhrase);
            }

            return null;
        }

        /// <summary> Return full year reports for one instrument (max 10 reports)</summary>
        public ReportsYearRespV1 GetReportsYear(long instrumentId, int maxcount = 20, int original = 0)
        {
    
            string url = $"{_urlRoot}/v1/instruments/{instrumentId}/reports/year";
            string query = $"{_querystring}&maxcount={maxcount}&original={original}";
            HttpResponseMessage response = WebbCall(url, query);

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

        /// <summary> Return R12 reports (Rolling 12 month => Sum of last four quarter reports) for one instrument (max 10 reports)</summary>
        public ReportsR12RespV1 GetReportsR12(long instrumentId, int maxcount = 20, int original = 0)
        {
            string url = $"{_urlRoot}/v1/instruments/{instrumentId}/reports/r12";
            string query = $"{_querystring}&maxcount={maxcount}&original={original}";

            HttpResponseMessage response = WebbCall(url, query);

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

        /// <summary> Return quarterly reports (Normally data for last 3 months) for one instrument</summary>
        public ReportsQuarterRespV1 GetReportsQuarter(long instrumentId, int maxcount=20, int original=0)
        {
            string url = $"{_urlRoot}/v1/instruments/{instrumentId}/reports/quarter";
            string query = $"{_querystring}&maxcount={maxcount}&original={original}";

            HttpResponseMessage response = WebbCall(url, query);

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

        /// <summary> Return end day stock price for one instrument</summary>
        public StockPricesRespV1 GetStockPrices(long instrumentId)
        {

            string url = $"{_urlRoot}/v1/instruments/{instrumentId}/stockprices";
            HttpResponseMessage response = WebbCall(url, _querystring);

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

        /// <summary> Return end day stock price for one instrument</summary>
        public StockPricesRespV1 GetStockPrices(long instrumentId, DateTime from, DateTime to)
        {
            string url = $"{_urlRoot}/v1/instruments/{instrumentId}/stockprices";
            string query = $"{_querystring}&from={from.ToShortDateString()}&to={to.ToShortDateString()}";

            HttpResponseMessage response = WebbCall(url, query);

            if (response.IsSuccessStatusCode)
            {
                string json = response.Content.ReadAsStringAsync().Result;
                StockPricesRespV1 res = JsonConvert.DeserializeObject<StockPricesRespV1>(json);
                return res;
            }
            else
            {
                Console.WriteLine("GetStockPrices time {0} ({1})", (int)response.StatusCode, response.ReasonPhrase);
            }

            return null;
        }

        /// <summary>
        /// Some KPIs have history.
        /// See list of KPIs and how to call on github:
        /// https://github.com/Borsdata-Sweden/API/wiki/KPI-History
        /// </summary>
        /// <param name="instrumentId">Company Ericsson has instrumentId=77</param>
        /// <param name="KpiId">KPI ID. P/E = 2</param>
        /// <param name="rt"> What report is KPI calculated with? [year, r12, quarter]</param>
        /// <param name="pt">What stock price is KPI calculated with? [mean, high, low]</param>
        /// <returns>List of historical KPI values</returns>
        public KpisHistoryRespV1 GetKpiHistory(long instrumentId, int KpiId, ReportType rt, PriceType pt)
        {
            string url = $"{_urlRoot}/v1/Instruments/{instrumentId}/kpis/{KpiId}/{rt}/{pt}/history";
            HttpResponseMessage response = WebbCall(url, _querystring);

            if (response.IsSuccessStatusCode)
            {
                string json = response.Content.ReadAsStringAsync().Result;
                KpisHistoryRespV1 res = JsonConvert.DeserializeObject<KpisHistoryRespV1>(json);
                return res;
            }
            else
            {
                Console.WriteLine("GetStockPrices time {0} ({1})", (int)response.StatusCode, response.ReasonPhrase);
            }

            return null;
        }

        /// <summary>
        /// Screener KPIs. Return one data point for one instrument.
        /// You can find exact API URL on Borsdata screener in the KPI window and [API URL] button.
        /// </summary>
        /// <param name="instrumentId">Company Ericsson has instrumentId=77</param>
        /// <param name="KpiId">KPI ID</param>
        /// <param name="time">Time period for the KPI</param>
        /// <param name="calc">Calculation format.</param>
        /// <returns></returns>
        public KpisRespV1 GetKpiScreenerSingle(long instrumentId, int KpiId, string time, string calc)
        {
            string url = $"{_urlRoot}/v1/Instruments/{instrumentId}/kpis/{KpiId}/{time}/{calc}";
            HttpResponseMessage response = WebbCall(url, _querystring);

            if (response.IsSuccessStatusCode)
            {
                string json = response.Content.ReadAsStringAsync().Result;
                KpisRespV1 res = JsonConvert.DeserializeObject<KpisRespV1>(json);
                return res;
            }
            else
            {
                Console.WriteLine("GetStockPrices time {0} ({1})", (int)response.StatusCode, response.ReasonPhrase);
            }

            return null;
        }

        /// <summary>
        /// Screener KPIs. Return List of datapoints for all instruments.
        /// You can find exact API URL on Borsdata screener in the KPI window and [API URL] button.
        /// </summary>
        /// <param name="KpiId">KPI ID</param>
        /// <param name="time">Time period for the KPI</param>
        /// <param name="calc">Calculation format</param>
        /// <returns></returns>
        public KpisAllCompRespV1 GetKpiScreener(int KpiId, string time, string calc)
        {
            string url = $"{_urlRoot}/v1/Instruments/kpis/{KpiId}/{time}/{calc}";
            HttpResponseMessage response = WebbCall(url, _querystring);

            if (response.IsSuccessStatusCode)
            {
                string json = response.Content.ReadAsStringAsync().Result;
                KpisAllCompRespV1 res = JsonConvert.DeserializeObject<KpisAllCompRespV1>(json);
                return res;
            }
            else
            {
                Console.WriteLine("GetStockPrices time {0} ({1})", (int)response.StatusCode, response.ReasonPhrase);
            }

            return null;
        }

        public MarketsRespV1 GetMarkets()
        {
            string url = $"{_urlRoot}/v1/markets";
            HttpResponseMessage response = WebbCall(url, _querystring);

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
            string url = $"{_urlRoot}/v1/sectors";
            HttpResponseMessage response = WebbCall(url, _querystring);

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
            string url = $"{_urlRoot}/v1/countries";
            HttpResponseMessage response = WebbCall(url, _querystring);

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
            string url = $"{_urlRoot}/v1/branches";
            HttpResponseMessage response = WebbCall(url, _querystring);

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

        /// <summary>
        /// Return last 100 instruments with changed data or where reports is updated.
        /// </summary>
        /// <returns></returns>
        public InstrumentsUpdatedRespV1 GetInstrumentsUpdated()
        {
           
            string url = $"{_urlRoot}/v1/instruments/updated";
            HttpResponseMessage response = WebbCall(url, _querystring);

            if (response.IsSuccessStatusCode)
            {
                string json = response.Content.ReadAsStringAsync().Result;
                InstrumentsUpdatedRespV1 res = JsonConvert.DeserializeObject<InstrumentsUpdatedRespV1>(json);
                return res;
            }
            else
            {
                Console.WriteLine("GetInstrumentsUpdated {0} ({1})", (int)response.StatusCode, response.ReasonPhrase);
            }

            return null;
        }

        /// <summary>
        /// Return last time the KPIs were recalculated.
        /// Normally this is at night after report and stock prices are updated.
        /// But can also be during day when new reports are added.
        /// </summary>
        /// <returns></returns>
        public KpisCalcUpdatedRespV1 GetKpisCalcUpdated()
        {
            string url = $"{_urlRoot}/v1/instruments/kpis/updated";
            HttpResponseMessage response = WebbCall(url, _querystring);

            if (response.IsSuccessStatusCode)
            {
                string json = response.Content.ReadAsStringAsync().Result;
                KpisCalcUpdatedRespV1 res = JsonConvert.DeserializeObject<KpisCalcUpdatedRespV1>(json);
                return res;
            }
            else
            {
                Console.WriteLine("GetKpisCalcUpdated {0} ({1})", (int)response.StatusCode, response.ReasonPhrase);
            }

            return null;
        }

        /// <summary>
        /// Return list of instruments with stock split.
        /// Stock split affects all historical stock prices, reportdata and KPIs for this instrument.
        /// </summary>
        public StockSplitRespV1 GetStockSplits()
        {
            string url = $"{_urlRoot}/v1/instruments/StockSplits";
            HttpResponseMessage response = WebbCall(url, _querystring);

            if (response.IsSuccessStatusCode)
            {
                string json = response.Content.ReadAsStringAsync().Result;
                StockSplitRespV1 res = JsonConvert.DeserializeObject<StockSplitRespV1>(json);
                return res;
            }
            else
            {
                Console.WriteLine("GetStockSplits {0} ({1})", (int)response.StatusCode, response.ReasonPhrase);
            }

            return null;
        }

        /// <summary> Return list of last Stockprice for all instruments</summary>
        public StockPricesLastRespV1 GetStockpricesLast()
        {
            string url = $"{_urlRoot}/v1/instruments/stockprices/last";
            HttpResponseMessage response = WebbCall(url, _querystring);

            if (response.IsSuccessStatusCode)
            {
                string json = response.Content.ReadAsStringAsync().Result;
                StockPricesLastRespV1 res = JsonConvert.DeserializeObject<StockPricesLastRespV1>(json);
                return res;
            }
            else
            {
                Console.WriteLine("GetStockpricesLast {0} ({1})", (int)response.StatusCode, response.ReasonPhrase);
            }

            return null;
        }

        /// <summary>
        /// Combine URL and query string. Check if need to sleep (rate limit). Then call API.
        /// It tries to call API 2 times if rate limit is hit.
        /// </summary>
        /// <param name="url">API url</param>
        /// <param name="querystring">Querystring</param>
        /// <returns></returns>
        HttpResponseMessage WebbCall(string url, string querystring)
        {
            _client = new HttpClient();
            _client.BaseAddress = new Uri(url);
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            HttpResponseMessage response = _client.GetAsync(querystring).Result; // Call API
            Console.WriteLine(url + querystring);

            if ((int)response.StatusCode == 429) // We get RateLimit error. Sleep.
            {
                System.Threading.Thread.Sleep(500);
                response = _client.GetAsync(querystring).Result; // Call API second time!
            }

            return response;
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
