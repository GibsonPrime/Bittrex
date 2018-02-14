Public Class BittrexUploader

    Private BittrexAPI As BittrexAPI
    Private CoinTradingAPI As CoinTradingAPI

    Sub New(bittrexAPI As BittrexAPI, coinTradingAPI As CoinTradingAPI)
        Me.BittrexAPI = bittrexAPI
        Me.CoinTradingAPI = coinTradingAPI
    End Sub

    Sub UploadCurrencies()
        For Each currency As BittrexAPI.Currency In Me.BittrexAPI.GetCurrencies
            With currency
                Me.CoinTradingAPI.UploadCurrency(.Currency, .CurrencyLong, .MinConfirmation, .TxFee, .IsActive, .CoinType, .BaseAddress)
            End With
        Next
    End Sub

    Sub UploadMarkets()
        For Each market As BittrexAPI.Market In Me.BittrexAPI.GetMarkets
            With market
                Me.CoinTradingAPI.UploadMarket(.MarketName, .BaseCurrency, .MarketCurrency, .MinTradeSize, .IsActive, .Created, .LogoUrl)
            End With
        Next
    End Sub

    Sub UploadMarketSummaries()
        For Each marketSummary As BittrexAPI.MarketSummary In Me.BittrexAPI.GetMarketSummaries
            With marketSummary
                Me.CoinTradingAPI.UploadMarketSummary(.MarketName, .TimeStamp, .High, .Low, .Last, .Bid, .Ask, .Volume, .BaseVolume, .OpenBuyOrders, .OpenSellOrders, .PrevDay)
            End With
        Next
    End Sub

    Sub UploadFiveMinuteTicks(marketName As String)
        For Each fiveMinuteTick As BittrexAPI.Tick In Me.BittrexAPI.GetTicks(marketName, BittrexAPI.TickInterval.fivemin)
            With fiveMinuteTick
                Me.CoinTradingAPI.UploadFiveMinuteTick(marketName, .T, .O, .C, .H, .L, .V, .BV)
            End With
        Next
    End Sub

    Sub UploadFiveMinuteTicks(markets As List(Of CoinTradingAPI.Market))
        For Each market As CoinTradingAPI.Market In markets
            Me.UploadFiveMinuteTicks(market.MarketName)
        Next
    End Sub

    Sub UploadHourTicks(marketName As String)
        For Each hourTick As BittrexAPI.Tick In Me.BittrexAPI.GetTicks(marketName, BittrexAPI.TickInterval.hour)
            With hourTick
                Me.CoinTradingAPI.UploadHourTick(marketName, .T, .O, .C, .H, .L, .V, .BV)
            End With
        Next
    End Sub

    Sub UploadHourTicks(markets As List(Of CoinTradingAPI.Market))
        Dim caughtUp As Boolean = True
        For Each market As CoinTradingAPI.Market In markets
            If market.IsActive Then
                If market.MarketName = "BTC-SC" Then
                    caughtUp = True
                End If
                If caughtUp Then
                    Me.UploadHourTicks(market.MarketName)
                End If
            End If
        Next
    End Sub
End Class