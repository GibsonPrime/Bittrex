Imports System.IO
Imports System.Net
Imports System.Security.Cryptography
Imports System.Text
Imports Newtonsoft

Public Class BittrexAPI
    Enum TickInterval
        onemin
        fivemin
        hour
        day
    End Enum

    Private ApiKeyRead As ApiKey
    Private ApiKeyWrite As ApiKey


    Sub SetApiKeyRead(key As String, secret As String)
        Me.ApiKeyRead = New ApiKey(key, secret)
    End Sub
    Sub SetApiKeyWrite(key As String, secret As String)
        Me.ApiKeyWrite = New ApiKey(key, secret)
    End Sub

    Shared Sub AppendRequestParameter(ByRef requestUrl As String, parameter As String)
        If requestUrl.Contains("?"c) Then
            requestUrl &= "&"
        Else
            requestUrl &= "?"
        End If

        requestUrl &= parameter
    End Sub
    Shared Function GetResponseJson(requestURL As String) As Json.Linq.JObject
        Dim myWebRequest As WebRequest = WebRequest.Create(requestURL)
        Return Json.Linq.JObject.Parse(GetResponse(myWebRequest))
    End Function
    Shared Function GetResponseJson(requestURL As String, apiKey As ApiKey, addNonce As Boolean) As Json.Linq.JObject

        AppendRequestParameter(requestURL, "apikey=" & apiKey.Key)
        AppendRequestParameter(requestURL, "nonce=" & Now.Ticks.ToString)


        Dim myWebRequest As WebRequest = WebRequest.Create(requestURL)
        apiKey.SignWebRequest(myWebRequest)

        Return Json.Linq.JObject.Parse(GetResponse(myWebRequest))
    End Function
    Shared Function GetResponse(myWebRequest As WebRequest) As String

        Dim myWebResponse As WebResponse = Nothing
        Dim myStream As Stream = Nothing
        Dim myStreamReader As StreamReader = Nothing

        Try
            myWebResponse = myWebRequest.GetResponse
            myStream = myWebResponse.GetResponseStream
            myStreamReader = New StreamReader(myStream)

            Return myStreamReader.ReadToEnd
        Catch ex As Exception
            Throw New Exception("There was an error making the web request to " & myWebRequest.RequestUri.ToString, ex)
        Finally
            Try
                myStream.Close()
                myStream.Dispose()

                myStreamReader.Close()
                myStreamReader.Dispose()
            Catch
            End Try
        End Try

    End Function

#Region "Public"
    Const URL_GetCurrencies As String = "https://bittrex.com/api/v1.1/public/getcurrencies"
    Const URL_GetMarkets As String = "https://bittrex.com/api/v1.1/public/getmarkets"
    Const URL_GetMarketSummary As String = "https://bittrex.com/api/v1.1/public/getmarketsummary"
    Const URL_GetMarketSummaries As String = "https://bittrex.com/api/v1.1/public/getmarketsummaries"
    Const URL_GetMarketHistory As String = "https://bittrex.com/api/v1.1/public/getmarkethistory"
    Const URL_GetTicker As String = "https://bittrex.com/api/v1.1/public/getticker"
    Const URL_GetTicks As String = "https://bittrex.com/Api/v2.0/pub/market/GetTicks"
    Const URL_GetLatestTick As String = "https://bittrex.com/Api/v2.0/pub/market/GetLatestTick"
    Const URL_GetOrderBook As String = "https://bittrex.com/api/v1.1/public/getorderbook"

    Public Function GetCurrencies() As List(Of Currency)
        Dim currencies As New List(Of Currency)

        Dim currencyJObject As Json.Linq.JObject = GetResponseJson(URL_GetCurrencies)
        If currencyJObject("success").ToString.Equals("true", StringComparison.CurrentCultureIgnoreCase) Then
            For Each currencyToken As Json.Linq.JToken In currencyJObject("result").Children()
                currencies.Add(currencyToken.ToObject(Of Currency))
            Next
        Else
            Throw New Exception("Bittrex currency download failed - " & currencyJObject("message").ToString)
        End If

        Return currencies
    End Function

    Public Function GetMarkets() As List(Of Market)
        Dim markets As New List(Of Market)

        Dim marketJObject As Json.Linq.JObject = GetResponseJson(URL_GetMarkets)
        If marketJObject("success").ToString.Equals("true", StringComparison.CurrentCultureIgnoreCase) Then
            For Each marketToken As Json.Linq.JToken In marketJObject("result").Children()
                markets.Add(marketToken.ToObject(Of Market))
            Next
        Else
            Throw New Exception("Bittrex market download failed - " & marketJObject("message").ToString)
        End If

        Return markets
    End Function

    Public Function GetTicker(marketName As String) As Ticker
        Dim requestUrl As String = URL_GetTicker
        AppendRequestParameter(requestUrl, "market=" & marketName)

        Dim tickerJObject As Json.Linq.JObject = GetResponseJson(requestUrl)
        If tickerJObject("success").ToString.Equals("true", StringComparison.CurrentCultureIgnoreCase) Then
            Return tickerJObject("result").ToObject(Of Ticker)
        Else
            Throw New Exception("Bittrex ticker download failed - " & tickerJObject("message").ToString)
        End If
    End Function

    Public Function GetMarketSummary(marketName As String) As MarketSummary
        Dim requestUrl As String = URL_GetMarketSummary
        AppendRequestParameter(requestUrl, "market=" & marketName)

        Dim marketSummaryJObject As Json.Linq.JObject = GetResponseJson(requestUrl)
        If marketSummaryJObject("success").ToString.Equals("true", StringComparison.CurrentCultureIgnoreCase) Then

            If Not marketSummaryJObject("result").Children().Count = 1 Then
                Throw New Exception("Market summary returned <> 1 result")
            End If

            Return marketSummaryJObject("result").First.ToObject(Of MarketSummary)
        Else
            Throw New Exception("Bittrex market summary download failed - " & marketSummaryJObject("message").ToString)
        End If
    End Function

    Public Function GetMarketSummaries() As List(Of MarketSummary)
        Dim requestUrl As String = URL_GetMarketSummaries

        Dim marketSummaries As New List(Of MarketSummary)

        Dim marketSummariesJObject As Json.Linq.JObject = GetResponseJson(requestUrl)
        If marketSummariesJObject("success").ToString.Equals("true", StringComparison.CurrentCultureIgnoreCase) Then
            For Each marketSummaryToken As Json.Linq.JToken In marketSummariesJObject("result").Children
                marketSummaries.Add(marketSummaryToken.ToObject(Of MarketSummary))
            Next
        Else
            Throw New Exception("Bittrex market summaries download failed - " & marketSummariesJObject("message").ToString)
        End If

        Return marketSummaries
    End Function

    Public Function GetMarketHistory(marketName As String) As List(Of MarketHistory)
        Dim requestUrl As String = URL_GetMarketHistory
        AppendRequestParameter(requestUrl, "market=" & marketName)

        Dim marketHistories As New List(Of MarketHistory)

        Dim marketHistoryJObject As Json.Linq.JObject = GetResponseJson(requestUrl)
        If marketHistoryJObject("success").ToString.Equals("true", StringComparison.CurrentCultureIgnoreCase) Then
            For Each marketHistoryToken As Json.Linq.JToken In marketHistoryJObject("result").Children
                marketHistories.Add(marketHistoryToken.ToObject(Of MarketHistory))
            Next
        Else
            Throw New Exception("Bittrex market summaries download failed - " & marketHistoryJObject("message").ToString)
        End If

        Return marketHistories
    End Function
    Public Function GetTicks(marketName As String, tickInterval As TickInterval) As List(Of Tick)
        Dim requestUrl As String = URL_GetTicks

        AppendRequestParameter(requestUrl, "marketName=" & marketName)
        AppendRequestParameter(requestUrl, "tickInterval=" & tickInterval.ToString)

        Dim ticks As New List(Of Tick)

        Dim ticksJObject As Json.Linq.JObject = GetResponseJson(requestUrl)
        If ticksJObject("success").ToString.Equals("true", StringComparison.CurrentCultureIgnoreCase) Then
            For Each tickToken As Json.Linq.JToken In ticksJObject("result").Children
                ticks.Add(tickToken.ToObject(Of Tick))
            Next
        Else
            Throw New Exception("Bittrex ticks download failed - " & ticksJObject("message").ToString)
        End If

        Return ticks
    End Function
    Public Function GetLatestTick(marketName As String, tickInterval As String) As Tick
        Dim requestUrl As String = URL_GetLatestTick

        AppendRequestParameter(requestUrl, "marketName=" & marketName)
        AppendRequestParameter(requestUrl, "tickInterval=" & tickInterval)


        Dim ticksJObject As Json.Linq.JObject = GetResponseJson(requestUrl)
        If ticksJObject("success").ToString.Equals("true", StringComparison.CurrentCultureIgnoreCase) Then
            If ticksJObject("result").Children.Count <> 1 Then
                Throw New Exception("Latest tick download returned <> 1 results")
            Else
                Return ticksJObject("result").Children.First.ToObject(Of Tick)
            End If
        Else
            Throw New Exception("Bittrex latest tick download failed - " & ticksJObject("message").ToString)
        End If
    End Function
    Public Function GetBuyOrders(marketName As String) As List(Of BuyOrder)
        Dim requestUrl As String = URL_GetOrderBook

        AppendRequestParameter(requestUrl, "market=" & marketName)
        AppendRequestParameter(requestUrl, "type=buy")

        Dim buyOrders As New List(Of BuyOrder)

        Dim ticksJObject As Json.Linq.JObject = GetResponseJson(requestUrl)
        If ticksJObject("success").ToString.Equals("true", StringComparison.CurrentCultureIgnoreCase) Then
            For Each buyOrderToken As Json.Linq.JToken In ticksJObject("result").Children
                buyOrders.Add(buyOrderToken.ToObject(Of BuyOrder))
            Next
        Else
            Throw New Exception("Bittrex buy orders download failed - " & ticksJObject("message").ToString)
        End If

        Return buyOrders
    End Function
    Public Function GetSellOrders(marketName As String) As List(Of SellOrder)
        Dim requestUrl As String = URL_GetOrderBook

        AppendRequestParameter(requestUrl, "market=" & marketName)
        AppendRequestParameter(requestUrl, "type=sell")

        Dim sellOrders As New List(Of SellOrder)

        Dim ticksJObject As Json.Linq.JObject = GetResponseJson(requestUrl)
        If ticksJObject("success").ToString.Equals("true", StringComparison.CurrentCultureIgnoreCase) Then
            For Each sellOrderToken As Json.Linq.JToken In ticksJObject("result").Children
                sellOrders.Add(sellOrderToken.ToObject(Of SellOrder))
            Next
        Else
            Throw New Exception("Bittrex sell orders download failed - " & ticksJObject("message").ToString)
        End If

        Return sellOrders
    End Function


    Class BuyOrder
        Public Quantity As Double
        Public Rate As Double
    End Class

    Class SellOrder
        Public Quantity As Double
        Public Rate As Double
    End Class

    Class Currency
        Public Currency As String
        Public CurrencyLong As String
        Public MinConfirmation As Integer
        Public TxFee As Double
        Public IsActive As Boolean
        Public CoinType As String
        Public BaseAddress As String
    End Class

    Class Market
        Public MarketCurrency As String
        Public BaseCurrency As String
        Public MarketCurrencyLong As String
        Public BaseCurrencyLong As String
        Public MinTradeSize As Double
        Public MarketName As String
        Public IsActive As Boolean
        Public Created As Date
        Public LogoUrl As String
    End Class

    Class MarketHistory
        Public Id As Integer
        Public TimeStamp As Date
        Public Quantity As Double
        Public Price As Double
        Public Total As Double
        Public FillType As String
        Public OrderType As String
    End Class

    Class Ticker
        Friend ReadOnly CreatedDate As Date = Now
        Public Bid As Double
        Public Ask As Double
        Public Last As Double
    End Class

    Class Tick

        Public T As DateTime
        Public O As Double
        Public C As Double
        Public H As Double
        Public L As Double
        Public V As Double
        Public BV As Double

    End Class

    Class MarketSummary

        Public MarketName As String
        Public High As Double
        Public Low As Double
        Public Volume As Double
        Public Last As Double
        Public BaseVolume As Double
        Public TimeStamp As Date
        Public Bid As Double
        Public Ask As Double
        Public OpenBuyOrders As Integer
        Public OpenSellOrders As Integer
        Public PrevDay As Double
        Public Created As Date
        Public DisplayMarketName As String

    End Class

#End Region

#Region "Account"

    Const URL_GetWallets As String = "https://bittrex.com/api/v1.1/account/getbalances"
    Const URL_GetClosedOrders As String = "https://bittrex.com/api/v1.1/account/getorderhistory"
    Const URL_GetOrder As String = "https://bittrex.com/api/v1.1/account/getorder"

    Function GetWallets() As List(Of Wallet)
        If Me.ApiKeyRead Is Nothing Then
            Throw New Exception("You must call SetApiKeyRead() to use this function")
        End If

        Dim wallets As New List(Of Wallet)

        Dim walletsJObject As Json.Linq.JObject = GetResponseJson(URL_GetWallets, Me.ApiKeyRead, True)
        If walletsJObject("success").ToString.Equals("true", StringComparison.CurrentCultureIgnoreCase) Then
            For Each walletToken As Json.Linq.JToken In walletsJObject("result").Children()
                wallets.Add(walletToken.ToObject(Of Wallet)())
            Next
        Else
            Throw New Exception("Wallet download failed - " & walletsJObject("message").ToString)
        End If

        Return wallets
    End Function
    Function GetClosedOrders() As List(Of ClosedOrder)
        If Me.ApiKeyRead Is Nothing Then
            Throw New Exception("You must call SetApiKeyRead() to use this function")
        End If

        Dim closedOrders As New List(Of ClosedOrder)

        Dim closedOrdersJObject As Json.Linq.JObject = GetResponseJson(URL_GetClosedOrders, Me.ApiKeyRead, True)
        If closedOrdersJObject("success").ToString.Equals("true", StringComparison.CurrentCultureIgnoreCase) Then
            For Each OrderToken As Json.Linq.JToken In closedOrdersJObject("result").Children()
                closedOrders.Add(OrderToken.ToObject(Of ClosedOrder)())
            Next
        Else
            Throw New Exception("Order download failed - " & closedOrdersJObject("message").ToString)
        End If

        Return closedOrders
    End Function
    Function GetOrder(orderUuid As String) As Order
        If Me.ApiKeyRead Is Nothing Then
            Throw New Exception("You must call SetApiKeyRead() to use this function")
        End If

        Dim requestUrl As String = URL_GetOrder
        AppendRequestParameter(requestUrl, "uuid=" & orderUuid)


        Dim orderJObject As Json.Linq.JObject = GetResponseJson(requestUrl, Me.ApiKeyRead, True)
        If orderJObject("success").ToString.Equals("true", StringComparison.CurrentCultureIgnoreCase) Then
            Return orderJObject("result").ToObject(Of Order)
        Else
            Throw New Exception("Order download failed - " & orderJObject("message").ToString)
        End If
    End Function

    Class Wallet

        Public Currency As String
        Public Balance As Double
        Public Available As Double
        Public Pending As Double
        Public CryptoAddress As String

    End Class

    Class ClosedOrder

        Public OrderUuid As String
        Public Exchange As String
        Public TimeStamp As Date
        Public OrderType As String
        Public Limit As Double
        Public Quantity As Double
        Public QuantityRemaining As Double
        Public Commission As Double
        Public Price As Double
        Public PricePerUnit As Double
        Public IsConditional As Boolean
        Public Condition As String
        Public ConditionTarget As String
        Public ImmediateOrCancel As Boolean

        Public ReadOnly Property Type As String
            Get
                Return Me.OrderType
            End Get
        End Property
        Public ReadOnly Property Market As String
            Get
                Return Me.Exchange
            End Get
        End Property
        Public ReadOnly Property WhenClosed As DateTime
            Get
                Return Me.TimeStamp
            End Get
        End Property
        Public ReadOnly Property OrderLimit As Double
            Get
                Return Me.Limit
            End Get
        End Property
        Public ReadOnly Property OrderQuantity As Double
            Get
                Return Me.Quantity
            End Get
        End Property
    End Class

    Class Order

        Public ClosedDate As DateTime

        Public Property Closed As Object
            Get
                Return ""
            End Get
            Set(value As Object)
                If IsDate(value) Then
                    Me.ClosedDate = CDate(value)
                Else
                    Me.ClosedDate = Nothing
                End If
            End Set
        End Property

        Public OrderUuid As String
        Public Exchange As String
        Public Type As String
        Public Quantity As Double
        Public QuantityRemaining As Double
        Public Limit As Double
        Public Reserved As Double
        Public ReserveRemaining As Double
        Public CommissionReserved As Double
        Public CommissionReserveRemaining As Double
        Public CommissionPaid As Double
        Public Price As Double
        Public PricePerUnit As String
        Public Opened As Date
        'Public Closed As Date
        Public IsOpen As Boolean
        Public Sentinel As String
        Public CancelInitiated As Boolean
        Public ImmediateOrCancel As Boolean
        Public IsConditional As Boolean
        Public Condition As String
        Public ConditionTarget As String


    End Class
#End Region

#Region "Market"

    Const URL_GetOpenOrders As String = "https://bittrex.com/api/v1.1/market/getopenorders"
    Const URL_BuyLimit As String = "https://bittrex.com/api/v1.1/market/buylimit"
    Const URL_SellLimit As String = "https://bittrex.com/api/v1.1/market/selllimit"
    Const URL_Cancel As String = "https://bittrex.com/api/v1.1/market/cancel"

    Const URL_BuyTrade As String = "https://bittrex.com/api/v2.0/auth/market/TradeBuy"
    Const URL_SellTrade As String = "https://bittrex.com/api/v2.0/auth/market/TradeSell"

    Enum TimeInEffect
        GOOD_TIL_CANCELED
        IMMEDIATE_OR_CANCEL
    End Enum

    Function GetOpenOrders() As List(Of OpenOrder)
        If Me.ApiKeyRead Is Nothing Then
            Throw New Exception("You must call SetApiKeyRead() to use this function")
        End If

        Dim openOrders As New List(Of OpenOrder)

        Dim openOrdersJObject As Json.Linq.JObject = GetResponseJson(URL_GetOpenOrders, Me.ApiKeyRead, True)
        If openOrdersJObject("success").ToString.Equals("true", StringComparison.CurrentCultureIgnoreCase) Then
            For Each OrderToken As Json.Linq.JToken In openOrdersJObject("result").Children()
                openOrders.Add(OrderToken.ToObject(Of OpenOrder)())
            Next
        Else
            Throw New Exception("Order download failed - " & openOrdersJObject("message").ToString)
        End If

        Return openOrders
    End Function

    Function CreateBuyLimit(marketName As String, quantity As Double, rate As Double) As BuyLimit

        If Me.ApiKeyWrite Is Nothing Then
            Throw New Exception("You must call SetApiKeyWrite() to use this function")
        End If

        Dim requestUrl As String = URL_BuyLimit

        AppendRequestParameter(requestUrl, "market=" & marketName)
        AppendRequestParameter(requestUrl, "quantity=" & quantity.ToString)
        AppendRequestParameter(requestUrl, "rate=" & rate.ToString)

        Dim sellLimitJObject As Json.Linq.JObject = GetResponseJson(requestUrl, Me.ApiKeyWrite, True)
        If sellLimitJObject("success").ToString.Equals("true", StringComparison.CurrentCultureIgnoreCase) Then
            Return sellLimitJObject("result").ToObject(Of BuyLimit)
        Else
            Throw New Exception("Sell limit failed - " & sellLimitJObject("message").ToString)
        End If

    End Function
    Function CreateSellLimit(marketName As String, quantity As Double, rate As Double) As SellLimit
        If Me.ApiKeyWrite Is Nothing Then
            Throw New Exception("You must call SetApiKeyWrite() to use this function")
        End If

        Dim requestUrl As String = URL_SellLimit

        AppendRequestParameter(requestUrl, "market=" & marketName)
        AppendRequestParameter(requestUrl, "quantity=" & quantity.ToString)
        AppendRequestParameter(requestUrl, "rate=" & rate.ToString)

        Dim sellLimitJObject As Json.Linq.JObject = GetResponseJson(requestUrl, Me.ApiKeyWrite, True)
        If sellLimitJObject("success").ToString.Equals("true", StringComparison.CurrentCultureIgnoreCase) Then
            Return sellLimitJObject("result").ToObject(Of SellLimit)
        Else
            Throw New Exception("Sell limit failed - " & sellLimitJObject("message").ToString)
        End If
    End Function
    Sub CancelOrder(uuid As String)
        If Me.ApiKeyWrite Is Nothing Then
            Throw New Exception("You must call SetApiKeyWrite() to use this function")
        End If

        Dim requestUrl As String = URL_Cancel
        AppendRequestParameter(requestUrl, "uuid=" & uuid)

        Dim sellLimitJObject As Json.Linq.JObject = GetResponseJson(requestUrl, Me.ApiKeyWrite, True)
        If Not sellLimitJObject("success").ToString.Equals("true", StringComparison.CurrentCultureIgnoreCase) Then
            Throw New Exception("Cancel order failed - " & sellLimitJObject("message").ToString)
        End If
    End Sub

    Function CreateTradeBuy(marketName As String, quantity As Double, rate As Double, timeInEffect As TimeInEffect) As Trade

        If Me.ApiKeyWrite Is Nothing Then
            Throw New Exception("You must call SetApiKeyWrite() to use this function")
        End If

        Dim requestUrl As String = URL_BuyTrade

        AppendRequestParameter(requestUrl, "market=" & marketName)
        AppendRequestParameter(requestUrl, "quantity=" & quantity.ToString)
        AppendRequestParameter(requestUrl, "rate=" & rate.ToString)
        AppendRequestParameter(requestUrl, "OrderType=Limit")
        AppendRequestParameter(requestUrl, "TimeInEffect=" & timeInEffect.ToString)

        Dim tradeBuyJObject As Json.Linq.JObject = GetResponseJson(requestUrl, Me.ApiKeyWrite, True)
        If tradeBuyJObject("success").ToString.Equals("true", StringComparison.CurrentCultureIgnoreCase) Then
            Return tradeBuyJObject("result").ToObject(Of Trade)
        Else
            Throw New Exception("Trace buy failed - " & tradeBuyJObject("message").ToString)
        End If

    End Function
    Function CreateTradeSell(marketName As String, quantity As Double, rate As Double, timeInEffect As TimeInEffect) As Trade

        If Me.ApiKeyWrite Is Nothing Then
            Throw New Exception("You must call SetApiKeyWrite() to use this function")
        End If

        Dim requestUrl As String = URL_SellTrade

        AppendRequestParameter(requestUrl, "market=" & marketName)
        AppendRequestParameter(requestUrl, "quantity=" & quantity.ToString)
        AppendRequestParameter(requestUrl, "rate=" & rate.ToString)
        AppendRequestParameter(requestUrl, "OrderType=Limit")
        AppendRequestParameter(requestUrl, "TimeInEffect=" & timeInEffect.ToString)

        Dim tradeSellJObject As Json.Linq.JObject = GetResponseJson(requestUrl, Me.ApiKeyWrite, True)
        If tradeSellJObject("success").ToString.Equals("true", StringComparison.CurrentCultureIgnoreCase) Then
            Return tradeSellJObject("result").ToObject(Of Trade)
        Else
            Throw New Exception("Trace sell failed - " & tradeSellJObject("message").ToString)
        End If

    End Function

    Class Trade

        Public BuyOrSell As String
        Public MarketCurrency As String
        Public MarketName As String
        Public OrderId As String
        Public OrderType As String
        Public Quantity As Double
        Public Rate As Double

    End Class

    Class OpenOrder

        Public Uuid As String
        Public OrderUuid As String
        Public Exchange As String
        Public OrderType As String
        Public Quantity As Double
        Public QuantityRemaining As Double
        Public Limit As Double
        Public CommissionPaid As Double
        Public Price As Double
        Public PricePerUnit As String
        Public Opened As Date
        Public Closed As String
        Public CancelInitiated As Boolean
        Public ImmediateOrCancel As Boolean
        Public IsConditional As Boolean
        Public Condition As String
        Public ConditionTarget As String

        Public ReadOnly Property OrderLimit As Double
            Get
                Return Me.Limit
            End Get
        End Property
        Public ReadOnly Property Type As String
            Get
                Return Me.OrderType
            End Get
        End Property
        Public ReadOnly Property Market As String
            Get
                Return Me.Exchange
            End Get
        End Property
        Public ReadOnly Property WhenOpened As DateTime
            Get
                Return Me.Opened
            End Get
        End Property
        Public ReadOnly Property OrderQuantity As Double
            Get
                Return Me.Quantity
            End Get
        End Property
        Public ReadOnly Property NumRemaining As Double
            Get
                Return Me.QuantityRemaining
            End Get
        End Property
    End Class

    Class SellLimit

        Public UUID As String

    End Class

    Class BuyLimit

        Public UUID As String

    End Class

#End Region

    Class ApiKey

        Private ReadOnly Encoding As Encoding = Encoding.ASCII

        Public ReadOnly Key As String
        Public ReadOnly Secret As String
        Private ReadOnly HashFunction As HMACSHA512

        Sub New(key As String, secret As String)
            Me.Key = key
            Me.Secret = secret
            Me.HashFunction = New HMACSHA512(Me.Encoding.GetBytes(secret))
        End Sub

        Sub SignWebRequest(webRequest As WebRequest)
            Dim messageBytes As Byte() = Me.Encoding.GetBytes(webRequest.RequestUri.ToString)
            Dim hashMessage As Byte() = Me.HashFunction.ComputeHash(messageBytes)
            Dim signature As String = BitConverter.ToString(hashMessage).Replace("-"c, "")

            webRequest.Headers.Add("apisign", signature)
        End Sub

    End Class
End Class