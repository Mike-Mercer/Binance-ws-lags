# Test Binance websoket lags

## There are unacceptable huge delays between an Event and receiving this event by websocket client. 

The delays are even worse on Futures since binance turned off `permessage-deflate` support (which was reported many times but still completely ignored.)

###### To measure delays, we subscribe to `@trade` streams for all USDT pairs on Spot or `@aggTrade` on Futures and measure the difference between Event.T and the time the event was received.
Subscription is done either in 2 threads (BTC is always in a separete thread, other altcoins on 2nd thread) or in N threads.
The N and whether to work with Spot or Futures are specified in the `config.txt`

### Important question: how much time one should run this test? 
### The asnwer is obvious:  If you are a serious trader, you should run it forever. Be sure. the lag will happen exactly when you max your position and don't expect any bad. 
> If there is a possibility of several things going wrong, the one that will cause the most damage will be the one to go wrong  
-- Murphy's laws

# Some results. Binance futures, 50 markets at the moment of this writing

## Tokyo AWS (recomended by binance)
### 09.13.2020 12:52:42 Latency (ms) = 6201 ms
`  data: {"stream":"sxpusdt@trade","data":{"e":"trade","E":1600001556423,"T":1600001556418,"s":"SXPUSDT","t":16207863,"p":"1.7560","q":"0.4","X":"MARKET","m":true}} `

### 09.13.2020 15:47:00 Latency (ms) = 9828 ms
`  data: {"stream":"bnbusdt@trade","data":{"e":"trade","E":1600012010422,"T":1600012010416,"s":"BNBUSDT","t":24304526,"p":"30.126","q":"5.02","X":"MARKET","m":true}}`


## For those who has the misfortune of living not in Asia and who wants to use the API for manual trading, Frankfurt AWS:

### 9/13/2020 13:45:35 Latency (ms) = 52532 ms (yes, 52 seconds)
`  data: {"stream":"ontusdt@trade","data":{"e":"trade","E":1600004682694,"T":1600004682688,"s":"ONTUSDT","t":13294672,"p":"0.7548","q":"2.5","X":"MARKET","m":false}}`
### 9/13/2020 13:45:38 PM: Latency (ms) = 52785 ms
`  data: {"stream":"xtzusdt@trade","data":{"e":"trade","E":1600004685713,"T":1600004685705,"s":"XTZUSDT","t":32607370,"p":"2.610","q":"44.6","X":"MARKET","m":true}}`
### 9/13/2020 13:45:40 PM: Latency (ms) = 52876 ms
`  data: {"stream":"bandusdt@trade","data":{"e":"trade","E":1600004687377,"T":1600004687372,"s":"BANDUSDT","t":11119309,"p":"8.6255","q":"34.0","X":"MARKET","m":true}}`


## Tokyo vultr VPS, 2 cores 

### 9/14/2020 13:13:16: Latency (ms) = 14223
  data: {"stream":"btcusdt@trade","data":{"e":"trade","E":1600089182477,"T":1600089182471,"s":"BTCUSDT","t":208017356,"p":"10571.15","q":"0.101","X":"MARKET","m":true}}
### 9/14/2020 13:13:18: Latency (ms) = 15915
  data: {"stream":"btcusdt@trade","data":{"e":"trade","E":1600089182599,"T":1600089182593,"s":"BTCUSDT","t":208017441,"p":"10565.19","q":"0.709","X":"MARKET","m":true}}


