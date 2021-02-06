# IgTradingApp
This is an IgTrading app built in .Net 3.1. 

# To get started please sponsor the app.
That will allow me to add new functionality, keep this repo updated  and generally pay for the odd beer as motivation to keep adding functionality. 

     
               ~~~~~~~~
              |~~~~~~~~|
              |        |- .
             o|        |- .\
             o|        |  \ \
              |        |   | |
             o|        |  / /
              |        |." "
              |        |- '
              .========.   


The app drives the spreadbetting account and was designed to quickly allow me to buy a screen of stocks with a total budget (e.g. total risk / cost of the position)
but it will prove a decent example of logging into the IgTrading API and opening positions or orders using API calls. 

To get started edit the app.settings.json and include insert your API key, username and password. I've found that the calls only work on the live API, the sandbox/test
calls don't appear to work. To test I opened a second spreadbet account so I could place test orders etc in isolation of my main trading account. 

Build and run the application in the console, and run the app with --help and it will guide you through a few options. 

Examples 

# Will list all your accounts 
Providing the account Id's which are used elsewhere. 

dotnet run accounts 

# List Your Postions
List positions including the total position size, and profits for a given account. 

dotnet run positions --account 1234

# Place an order
Will order the Ig market API epic code KA.D.LLOY.SEP.IP. This is chartcode LLOY, which is Lloyds PLC on an expiry of Sep-21
The account is the account Id from the 'accounts' call, level is the buy point, stopdistance and limitdistance are in points,
cash is the cash value to invest which is calculated back to the bid size. e.g. 350 cash at level 35 will open with a 10 position size = 350 
expiry is required and is the expiry on the EPIC

dotnet run order --direction BUY --account 1234 --level 35 --stopdistance 5 --limitdistance 10 --cash 350 --epic KA.D.LLOY.SEP.IP --expiry SEP-21

# Increase Stops to Perserve Profits
If the profit is greater than 10% set the top to 10% less than the current market value.
dotnet run updatestop --account 1234 --value 10

# Buy a number of positions
Buys positions based on a total value to purchase, will ask you to confirm positions prior to executing the trade. Stops and limits are set as a percentage of bids. This example buys Simon Thompson of Investors Chronicles Bargain Shares 2021 portfolio. 


dotnet run buy --names "Anexo,Arix Bioscience,Canadian General Investments,Downing Strategic Micro-cap Investment Trust,Duke Royalty,Ramsdens Holdings,San Leon Energy,Springfield Properties,Vietnam Holding,Wynnstay Group"  --e SEP-21 --stopdistance 25 --limitdistance 50 --value 10000 --account 12345

Have fun.

Neil