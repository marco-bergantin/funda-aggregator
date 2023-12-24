# funda-aggregator

## Assignment

The funda API returns information about the objects that are listed on funda.nl which are for sale.

An example of one of the URLs in our REST API is:
http://partnerapi.funda.nl/feeds/Aanbod.svc/[key]/?type=koop&zo=/amsterdam/tuin/&page=1&pagesize=25

Most of the parameters that you can pass are self explanatory. The parameter 'zo' (zoekopdracht or
search query) is the same as the key used in funda.nl URLs. For example the URL shown above
searches for houses in Amsterdam with a garden: http://www.funda.nl/koop/amsterdam/tuin/.
The API returns data about the object in XML. If you add the key 'json' between 'Aanbod.svc' and
[key], a JSON representation of the data will be returned instead of XML.


The assignment
Determine which makelaar's in Amsterdam have the most object listed for sale. Make a table of the
top 10. Then do the same thing but only for objects with a tuin which are listed for sale.  
For the
assignment you may write a program in any object oriented language of your choice and you may
use any libraries that you find useful.

## How to run

```
git clone https://github.com/marco-bergantin/funda-aggregator.git
cd .\funda-aggregator\FundaAggregator\FundaAggregator\
dotnet user-secrets set 'FundaPartnerApiKey' '<your-key-here>'
dotnet run "koop" "/amsterdam/"
dotnet run "koop" "/amsterdam/tuin/"
```