# API-CSharp-Client
Our simple C# console test client to get a quick start with Börsdata API.  
[More details about API is found here](https://github.com/Borsdata-Sweden/API)  

# Api Key
If you dont have an API Key, you need to Apply for an API key on [Börsdata webbpage](https://borsdata.se/).  
You need to be a Pro member to get Access to API.

# How to get started with Client
1. Download project and open in latest  [VisualStudio or Community](https://visualstudio.microsoft.com/downloads/#DownloadFamilies_2).    
2. In Program.cs you replace "YOUR APIKEY" with your uniqe API Key.
3. Compile



# Updates
2019-04-04  
- Add new API call to get list of Last Stockprice for all Instruments.  
(No need to call all Instruments to get last stockprices on daily basis. One call instead of +1600)
  
- Add new API call to get all reports for one Instrument.  
(To get Year, R12, Quarter Reports you only need one call)

2020-10-21 
- Add new report values
- Add 20 year history querystring

2024-02-19 
- Added new flags. Original

Happy coding!  




