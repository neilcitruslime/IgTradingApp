# IgTradingApp
This is an IgTrading app built in .Net 3.1. 

To get started please sponsor the app, I recommend $3 a month that will allow me to add new functionality, keep this repo updated 
and generally pay for the odd beer as motivation to keep adding functionality. 

The app drives the spreadbetting account and was designed to quickly allow me to buy a screen of stocks with a total budget (e.g. total risk / cost of the position)
but it will prove a decent example of logging into the IgTrading API and opening positions or orders using API calls. 

To get started edit the app.settings.json and include insert your API key, username and password. I've found that the calls only work on the live API, the sandbox/test
calls don't appear to work. To test I opened a second spreadbet account so I could place test orders etc in isolation of my main trading account. 

Build and run the application in the console, and run the app with --help and it will guide you through a few options. 

Examples 

# Will enumerate all your accounts, and list positions including the total position size, and profits. 
dotnet run positions

# Will list all your accounts including the account Id's which are used elsewhere. 
dot run accounts 

# Will order the Ig market API epic code KA.D.LLOY.SEP.IP. This is chartcode LLOY, which is Lloyds PLC on an expiry of Sep-21
# The account is the account Id from the 'accounts' call, level is the buy point, stopdistance and limitdistance are in points,
# cash is the cash value to invest which is calculated back to the bid size. e.g. 350 cash at level 35 will open with a 10 position size = 350 
# expiry is required and is the expiry on the EPIC
dotnet run order --direction BUY --account ISQ5F --level 35 --stopdistance 5 --limitdistance 10 --cash 350 --epic KA.D.LLOY.SEP.IP --expiry SEP-21
