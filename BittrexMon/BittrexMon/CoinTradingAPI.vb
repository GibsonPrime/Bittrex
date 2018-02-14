Imports System.Data.SqlClient

Public Class CoinTradingAPI
    Public ReadOnly SqlConnection As SqlConnection

    Sub New(sqlConnection As SqlConnection)
        Me.SqlConnection = sqlConnection
        Me.SqlConnection.Open()
    End Sub

    Function GetCurrencyIDFromName(currencyName As String) As Integer
        Dim sqlCommand As New SqlCommand("Bittrex.Currency_GetIDFromName", Me.SqlConnection)
        sqlCommand.CommandType = CommandType.StoredProcedure
        AddSQLParameter(sqlCommand, "CurrencyName", SqlDbType.VarChar, 10, currencyName)
        AddSQLOutputParameter(sqlCommand, "CurrencyID", SqlDbType.Int)

        sqlCommand.ExecuteNonQuery()

        Return GetSQLParameterIntegerValue(sqlCommand, "CurrencyID")
    End Function
    Function GetMarketIDFromName(currencyName As String) As Integer
        Dim sqlCommand As New SqlCommand("Bittrex.Market_GetIDFromName", Me.SqlConnection)
        sqlCommand.CommandType = CommandType.StoredProcedure
        AddSQLParameter(sqlCommand, "MarketName", SqlDbType.VarChar, 10, currencyName)
        AddSQLOutputParameter(sqlCommand, "MarketID", SqlDbType.Int)

        sqlCommand.ExecuteNonQuery()

        Return GetSQLParameterIntegerValue(sqlCommand, "MarketID")
    End Function

    Function DownloadMarketIDs() As List(Of Integer)
        Dim sqlCommand As New SqlCommand("Bittrex.Markets_DownloadIDs", Me.SqlConnection)

        Dim marketIDs As New List(Of Integer)

        For Each dataRow As DataRow In DownloadSqlDataset(sqlCommand).Tables(0).Rows
            marketIDs.Add(CInt(dataRow.Item("MarketID")))
        Next

        Return marketIDs
    End Function

    Function DownloadCurrencyIDs() As List(Of Integer)
        Dim sqlCommand As New SqlCommand("Bittrex.Currencies_DownloadIDs", Me.SqlConnection)

        Dim currencyIDs As New List(Of Integer)

        For Each dataRow As DataRow In DownloadSqlDataset(sqlCommand).Tables(0).Rows
            currencyIDs.Add(CInt(dataRow.Item("CurrencyID")))
        Next

        Return currencyIDs
    End Function

    Function DownloadMarkets() As List(Of Market)
        Dim markets As New List(Of Market)

        For Each marketID As Integer In Me.DownloadMarketIDs
            markets.Add(New Market(Me, marketID))
        Next

        Return markets
    End Function

    Function DownloadCurrencies() As List(Of Currency)
        Dim currencies As New List(Of Currency)

        For Each currencyID As Integer In Me.DownloadCurrencyIDs
            currencies.Add(New Currency(Me, currencyID))
        Next

        Return currencies
    End Function

    Class Currency
        Private ReadOnly CoinTradingAPI As CoinTradingAPI

        Private C_CurrencyID As Integer
        Private C_CurrencyName As String
        Private C_CurrencyLongName As String
        Private C_MinimumConfirmation As Integer
        Private C_TransactionFee As Decimal
        Private C_IsActive As Boolean
        Private C_CoinType As String
        Private C_BaseAddress As String


        Sub New(coinTradingAPI As CoinTradingAPI, currencyID As Integer)
            Me.CoinTradingAPI = coinTradingAPI
            Me.DownloadDetails(currencyID)
        End Sub
        Sub New(coinTradingAPI As CoinTradingAPI, currencyName As String)
            Me.New(coinTradingAPI, coinTradingAPI.GetCurrencyIDFromName(currencyName))
        End Sub

        Private Sub DownloadDetails(currencyID As Integer)
            Dim sqlCommand As New SqlCommand("Bittrex.Currency_GetDetails", Me.CoinTradingAPI.SqlConnection)
            sqlCommand.CommandType = CommandType.StoredProcedure
            AddSQLParameter(sqlCommand, "CurrencyID", SqlDbType.Int, currencyID)
            AddSQLOutputParameter(sqlCommand, "CurrencyName", SqlDbType.VarChar, 10)
            AddSQLOutputParameter(sqlCommand, "CurrencyLongName", SqlDbType.VarChar, 50)
            AddSQLOutputParameter(sqlCommand, "MinimumConfirmation", SqlDbType.Int)
            AddSQLOutputParameter(sqlCommand, "TransactionFee", SqlDbType.Decimal)
            AddSQLOutputParameter(sqlCommand, "IsActive", SqlDbType.Bit)
            AddSQLOutputParameter(sqlCommand, "CoinType", SqlDbType.VarChar, 50)
            AddSQLOutputParameter(sqlCommand, "BaseAddress", SqlDbType.VarChar, 50)

            sqlCommand.ExecuteNonQuery()

            Me.C_CurrencyID = currencyID
            Me.C_CurrencyName = GetSQLParameterStringValue(sqlCommand, "CurrencyName")
            Me.C_CurrencyLongName = GetSQLParameterStringValue(sqlCommand, "CurrencyLongName")
            Me.C_MinimumConfirmation = GetSQLParameterIntegerValue(sqlCommand, "MinimumConfirmation")
            Me.C_TransactionFee = GetSQLParameterDecimalValue(sqlCommand, "TransactionFee")
            Me.C_IsActive = GetSQLParameterBooleanValue(sqlCommand, "IsActive")
            Me.C_CoinType = GetSQLParameterStringValue(sqlCommand, "CoinType")
            Me.C_BaseAddress = GetSQLParameterStringValue(sqlCommand, "BaseAddress")
        End Sub

        Sub Update()
            Me.DownloadDetails(Me.C_CurrencyID)
        End Sub

        Overloads Function Equals(currencyName As String) As Boolean
            Return Me.C_CurrencyName.Trim.Equals(currencyName.Trim, StringComparison.CurrentCultureIgnoreCase)
        End Function
        Overloads Function Equals(currency As Currency) As Boolean
            Return Me.Equals(currency.C_CurrencyName)
        End Function

        ReadOnly Property CurrencyID As Integer
            Get
                Return Me.C_CurrencyID
            End Get
        End Property
        ReadOnly Property CurrencyName As String
            Get
                Return Me.C_CurrencyName
            End Get
        End Property
        ReadOnly Property CurrencyLongName As String
            Get
                Return Me.C_CurrencyLongName
            End Get
        End Property
        ReadOnly Property MinimumConfirmation As Integer
            Get
                Return Me.C_MinimumConfirmation
            End Get
        End Property
        ReadOnly Property TransactionFee As Decimal
            Get
                Return Me.C_TransactionFee
            End Get
        End Property
        ReadOnly Property IsActive As Boolean
            Get
                Return Me.C_IsActive
            End Get
        End Property
        ReadOnly Property CoinType As String
            Get
                Return Me.C_CoinType
            End Get
        End Property
        ReadOnly Property BaseAddress As String
            Get
                Return Me.C_BaseAddress
            End Get
        End Property
    End Class

    Class Market

        Private ReadOnly CoinTradingAPI As CoinTradingAPI

        Private C_MarketID As Integer
        Private C_MarketName As String
        Private C_BaseCurrencyID As Integer
        Private C_MarketCurrencyID As Integer
        Private C_MinimumTradeSize As Decimal
        Private C_IsActive As Boolean
        Private C_Created As Date
        Private C_LogoURL As String

        Private C_BaseCurrency As Currency
        Private C_MarketCurrency As Currency

        Sub New(coinTradingAPI As CoinTradingAPI, marketID As Integer)
            Me.CoinTradingAPI = coinTradingAPI

            Me.DownloadDetails(marketID)
        End Sub
        Sub New(coinTradingAPI As CoinTradingAPI, marketName As String)
            Me.New(coinTradingAPI, coinTradingAPI.GetMarketIDFromName(marketName))
        End Sub

        Sub Update()
            Me.DownloadDetails(Me.C_MarketID)
        End Sub

        Private Sub DownloadDetails(marketID As Integer)
            Dim sqlCommand As New SqlCommand("Bittrex.Market_GetDetails", Me.CoinTradingAPI.SqlConnection)
            sqlCommand.CommandType = CommandType.StoredProcedure
            AddSQLParameter(sqlCommand, "MarketID", SqlDbType.Int, marketID)
            AddSQLOutputParameter(sqlCommand, "MarketName", SqlDbType.VarChar, 20)
            AddSQLOutputParameter(sqlCommand, "BaseCurrencyID", SqlDbType.VarChar, 10)
            AddSQLOutputParameter(sqlCommand, "MarketCurrencyID", SqlDbType.VarChar, 10)
            AddSQLOutputParameter(sqlCommand, "MinimumTradeSize", SqlDbType.Decimal)
            AddSQLOutputParameter(sqlCommand, "IsActive", SqlDbType.Bit)
            AddSQLOutputParameter(sqlCommand, "Created", SqlDbType.DateTime)
            AddSQLOutputParameter(sqlCommand, "LogoURL", SqlDbType.VarChar, 200)

            sqlCommand.ExecuteNonQuery()

            Me.C_MarketID = marketID
            Me.C_MarketName = GetSQLParameterStringValue(sqlCommand, "MarketName")
            Me.C_BaseCurrencyID = GetSQLParameterIntegerValue(sqlCommand, "BaseCurrencyID")
            Me.C_MarketCurrencyID = GetSQLParameterIntegerValue(sqlCommand, "MarketCurrencyID")
            Me.C_MinimumTradeSize = GetSQLParameterDecimalValue(sqlCommand, "MinimumTradeSize")
            Me.C_IsActive = GetSQLParameterBooleanValue(sqlCommand, "IsActive")
            Me.C_Created = GetSQLParameterDateValue(sqlCommand, "Created")
            Me.C_LogoURL = GetSQLParameterStringValue(sqlCommand, "LogoURL")
        End Sub


        Overloads Function Equals(marketName As String) As Boolean
            Return Me.MarketName.Trim.Equals(marketName.Trim, StringComparison.CurrentCultureIgnoreCase)
        End Function
        Overloads Function Equals(market As Market) As Boolean
            Return Me.Equals(market.MarketName)
        End Function


        ReadOnly Property MarketID As Integer
            Get
                Return Me.C_MarketID
            End Get
        End Property
        ReadOnly Property MarketName As String
            Get
                Return Me.C_MarketName
            End Get
        End Property
        ReadOnly Property BaseCurrencyID As Integer
            Get
                Return Me.C_BaseCurrencyID
            End Get
        End Property
        ReadOnly Property MarketCurrencyID As Integer
            Get
                Return Me.C_MarketCurrencyID
            End Get
        End Property
        ReadOnly Property MinimumTradeSize As Decimal
            Get
                Return Me.C_MinimumTradeSize
            End Get
        End Property
        ReadOnly Property IsActive As Boolean
            Get
                Return Me.C_IsActive
            End Get
        End Property
        ReadOnly Property Created As Date
            Get
                Return Me.C_Created
            End Get
        End Property
        ReadOnly Property LogoURL As String
            Get
                Return Me.C_LogoURL
            End Get
        End Property

        ReadOnly Property BaseCurrency As Currency
            Get
                If Me.C_BaseCurrency Is Nothing Then
                    Me.C_BaseCurrency = New CoinTradingAPI.Currency(Me.CoinTradingAPI, Me.C_BaseCurrencyID)
                End If

                Return Me.C_BaseCurrency
            End Get
        End Property
        ReadOnly Property MarketCurrency As Currency
            Get
                If Me.C_MarketCurrency Is Nothing Then
                    Me.C_MarketCurrency = New CoinTradingAPI.Currency(Me.CoinTradingAPI, Me.C_MarketCurrencyID)
                End If

                Return Me.C_MarketCurrency
            End Get
        End Property
    End Class


#Region "Uploads"

    Sub UploadCurrency(currencyName As String, currencyLongName As String, minimumConfirmation As Integer, transactionFee As Double, isActive As Boolean, coinType As String, baseAddress As String)

        Dim sqlCommand As New SqlCommand("Bittrex.Currencies_Upload", Me.SqlConnection)
        sqlCommand.CommandType = CommandType.StoredProcedure
        AddSQLParameter(sqlCommand, "CurrencyName", SqlDbType.VarChar, 10, currencyName)
        AddSQLParameter(sqlCommand, "CurrencyLongName", SqlDbType.VarChar, 50, currencyLongName)
        AddSQLParameter(sqlCommand, "MinimumConfirmation", SqlDbType.Int, minimumConfirmation)
        AddSQLParameter(sqlCommand, "TransactionFee", SqlDbType.Float, transactionFee)
        AddSQLParameter(sqlCommand, "IsActive", SqlDbType.Bit, isActive)
        AddSQLParameter(sqlCommand, "CoinType", SqlDbType.VarChar, 50, coinType)
        AddSQLParameter(sqlCommand, "BaseAddress", SqlDbType.VarChar, 50, baseAddress)

        sqlCommand.ExecuteNonQuery()

    End Sub
    Sub UploadMarket(marketName As String, baseCurrencyName As String, marketCurrencyName As String, minimumTradeSize As Double, isActive As Boolean, created As Date, logoURL As String)
        Dim sqlCommand As New SqlCommand("Bittrex.Markets_Upload", Me.SqlConnection)
        sqlCommand.CommandType = CommandType.StoredProcedure
        AddSQLParameter(sqlCommand, "MarketName", SqlDbType.VarChar, 20, marketName)
        AddSQLParameter(sqlCommand, "BaseCurrencyName", SqlDbType.VarChar, 10, baseCurrencyName)
        AddSQLParameter(sqlCommand, "MarketCurrencyName", SqlDbType.VarChar, 10, marketCurrencyName)
        AddSQLParameter(sqlCommand, "MinimumTradeSize", SqlDbType.Float, minimumTradeSize)
        AddSQLParameter(sqlCommand, "IsActive", SqlDbType.Bit, isActive)
        AddSQLParameter(sqlCommand, "Created", SqlDbType.DateTime, created)
        AddSQLParameter(sqlCommand, "LogoURL", SqlDbType.VarChar, 200, logoURL)

        sqlCommand.ExecuteNonQuery()
    End Sub
    Sub UploadMarketSummary(marketName As String, timeStamp As Date, high As Double, low As Double, last As Double, bid As Double, ask As Double, volume As Double, baseVolume As Double, openBuyOrders As Integer, openSellOrders As Integer, previousDay As Double)
        Dim numRetries As Integer = 5
        Dim retry As Boolean = True
        While (retry)
            Dim sqlCommand As New SqlCommand("Bittrex.MarketSummaries_Upload", Me.SqlConnection)
            sqlCommand.CommandType = CommandType.StoredProcedure
            AddSQLParameter(sqlCommand, "MarketName", SqlDbType.VarChar, 20, marketName)
            AddSQLParameter(sqlCommand, "TimeStamp", SqlDbType.DateTime, timeStamp)
            AddSQLParameter(sqlCommand, "High", SqlDbType.Float, high)
            AddSQLParameter(sqlCommand, "Low", SqlDbType.Float, low)
            AddSQLParameter(sqlCommand, "Last", SqlDbType.Float, last)
            AddSQLParameter(sqlCommand, "Bid", SqlDbType.Float, bid)
            AddSQLParameter(sqlCommand, "Ask", SqlDbType.Float, ask)
            AddSQLParameter(sqlCommand, "Volume", SqlDbType.Float, volume)
            AddSQLParameter(sqlCommand, "BaseVolume", SqlDbType.Float, baseVolume)
            AddSQLParameter(sqlCommand, "OpenBuyOrders", SqlDbType.Int, openBuyOrders)
            AddSQLParameter(sqlCommand, "OpenSellOrders", SqlDbType.Float, openSellOrders)
            AddSQLParameter(sqlCommand, "PreviousDay", SqlDbType.Float, previousDay)
            Try
                sqlCommand.ExecuteNonQuery()
                retry = False
            Catch ex As Exception
                numRetries -= 1
                If (numRetries < 0) Then
                    retry = False
                End If
            End Try
        End While
    End Sub
    Sub UploadFiveMinuteTick(marketName As String, timeStamp As Date, open As Double, close As Double, high As Double, low As Double, volume As Double, baseVolume As Double)
        Dim sqlCommand As New SqlCommand("Bittrex.FiveMinuteTicks_Upload", Me.SqlConnection)
        sqlCommand.CommandType = CommandType.StoredProcedure
        AddSQLParameter(sqlCommand, "MarketName", SqlDbType.VarChar, 20, marketName)
        AddSQLParameter(sqlCommand, "TimeStamp", SqlDbType.DateTime, timeStamp)
        AddSQLParameter(sqlCommand, "Open", SqlDbType.Float, open)
        AddSQLParameter(sqlCommand, "Close", SqlDbType.Float, close)
        AddSQLParameter(sqlCommand, "High", SqlDbType.Float, high)
        AddSQLParameter(sqlCommand, "Low", SqlDbType.Float, low)
        AddSQLParameter(sqlCommand, "Volume", SqlDbType.Float, volume)
        AddSQLParameter(sqlCommand, "BaseVolume", SqlDbType.Float, baseVolume)
        sqlCommand.ExecuteNonQuery()
    End Sub
    Sub UploadHourTick(marketName As String, timeStamp As Date, open As Double, close As Double, high As Double, low As Double, volume As Double, baseVolume As Double)

        Dim sqlCommand As New SqlCommand("Bittrex.HourTicks_Upload", Me.SqlConnection)
        sqlCommand.CommandType = CommandType.StoredProcedure
        AddSQLParameter(sqlCommand, "MarketName", SqlDbType.VarChar, 20, marketName)
        AddSQLParameter(sqlCommand, "TimeStamp", SqlDbType.DateTime, timeStamp)
        AddSQLParameter(sqlCommand, "Open", SqlDbType.Float, open)
        AddSQLParameter(sqlCommand, "Close", SqlDbType.Float, close)
        AddSQLParameter(sqlCommand, "High", SqlDbType.Float, high)
        AddSQLParameter(sqlCommand, "Low", SqlDbType.Float, low)
        AddSQLParameter(sqlCommand, "Volume", SqlDbType.Float, volume)
        AddSQLParameter(sqlCommand, "BaseVolume", SqlDbType.Float, baseVolume)
        sqlCommand.ExecuteNonQuery()
    End Sub

#End Region

#Region "Shared"

    Shared Function DoesListContainString(list As List(Of String), value As String) As Boolean
        For Each listValue As String In list
            If listValue.Trim.Equals(value, StringComparison.CurrentCultureIgnoreCase) Then
                Return True
            End If
        Next

        Return False
    End Function


    Shared Sub AddSQLParameter(sqlCommand As SqlCommand, parameterName As String, dataType As SqlDbType, size As Integer, value As Object)
        Dim sqlParameter As New SqlParameter(parameterName, dataType, size)
        sqlParameter.Value = value
        sqlCommand.Parameters.Add(sqlParameter)
    End Sub
    Shared Sub AddSQLOutputParameter(sqlCommand As SqlCommand, parameterName As String, dataType As SqlDbType, size As Integer)
        Dim sqlParameter As New SqlParameter(parameterName, dataType, size)
        sqlParameter.Direction = ParameterDirection.Output
        If dataType = SqlDbType.Decimal Then
            sqlParameter.Precision = 30
            sqlParameter.Scale = 15
        End If
        sqlCommand.Parameters.Add(sqlParameter)
    End Sub
    Shared Sub AddSQLOutputParameter(sqlCommand As SqlCommand, parameterName As String, dataType As SqlDbType)
        AddSQLOutputParameter(sqlCommand, parameterName, dataType, -1)
    End Sub
    Shared Sub AddSQLParameter(sqlCommand As SqlCommand, parameterName As String, dataType As SqlDbType, value As Object)
        AddSQLParameter(sqlCommand, parameterName, dataType, -1, value)
    End Sub
    Shared Function GetSQLParameterStringValue(sqlCommand As SqlCommand, parameterName As String) As String
        For Each parameter As SqlParameter In sqlCommand.Parameters
            If parameter.ParameterName.Equals(parameterName, StringComparison.CurrentCultureIgnoreCase) Then
                If IsDBNull(parameter.Value) Then
                    Return Nothing
                Else
                    Return CStr(parameter.Value)
                End If
            End If
        Next

        Throw New Exception("Could not find parameter")
    End Function
    Shared Function GetSQLParameterDecimalValue(sqlCommand As SqlCommand, parameterName As String) As Decimal
        For Each parameter As SqlParameter In sqlCommand.Parameters
            If parameter.ParameterName.Equals(parameterName, StringComparison.CurrentCultureIgnoreCase) Then
                If IsDBNull(parameter.Value) Then
                    Return Nothing
                Else
                    Return CDec(parameter.Value)
                End If
            End If
        Next

        Throw New Exception("Could not find parameter")
    End Function
    Shared Function GetSQLParameterDoubleValue(sqlCommand As SqlCommand, parameterName As String) As Double
        For Each parameter As SqlParameter In sqlCommand.Parameters
            If parameter.ParameterName.Equals(parameterName, StringComparison.CurrentCultureIgnoreCase) Then
                If IsDBNull(parameter.Value) Then
                    Return Nothing
                Else
                    Return CDbl(parameter.Value)
                End If
            End If
        Next

        Throw New Exception("Could not find parameter")
    End Function
    Shared Function GetSQLParameterIntegerValue(sqlCommand As SqlCommand, parameterName As String) As Integer
        For Each parameter As SqlParameter In sqlCommand.Parameters
            If parameter.ParameterName.Equals(parameterName, StringComparison.CurrentCultureIgnoreCase) Then
                If IsDBNull(parameter.Value) Then
                    Return Nothing
                Else
                    Return CInt(parameter.Value)
                End If
            End If
        Next

        Throw New Exception("Could not find parameter")
    End Function
    Shared Function GetSQLParameterDateValue(sqlCommand As SqlCommand, parameterName As String) As Date
        For Each parameter As SqlParameter In sqlCommand.Parameters
            If parameter.ParameterName.Equals(parameterName, StringComparison.CurrentCultureIgnoreCase) Then
                If IsDBNull(parameter.Value) Then
                    Return Nothing
                Else
                    Return CDate(parameter.Value)
                End If
            End If
        Next

        Throw New Exception("Could not find parameter")
    End Function
    Shared Function GetSQLParameterBooleanValue(sqlCommand As SqlCommand, parameterName As String) As Boolean
        For Each parameter As SqlParameter In sqlCommand.Parameters
            If parameter.ParameterName.Equals(parameterName, StringComparison.CurrentCultureIgnoreCase) Then
                If IsDBNull(parameter.Value) Then
                    Return Nothing
                Else
                    Return CBool(parameter.Value)
                End If
            End If
        Next

        Throw New Exception("Could not find parameter")
    End Function
    Shared Function DownloadSqlDataset(sqlCommand As SqlCommand) As DataSet
        Dim sqlDataAdapter As New SqlDataAdapter(sqlCommand)
        Dim dataSet As New DataSet
        sqlDataAdapter.Fill(dataSet)
        Return dataSet
    End Function

#End Region
End Class