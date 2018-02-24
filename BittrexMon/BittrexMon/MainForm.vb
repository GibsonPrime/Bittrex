Imports System.ComponentModel
Imports System.Threading

Public Delegate Sub UpdateStatusDelegate(ByVal text As String)

Public Class MainForm
    ' Constants
    Private Const READ_KEY As String = "5659a83b81de4cc9b15fc05124c06e4e"
    Private Const READ_SECRET As String = "e2fa7ab96a684ecd8b7fdb7a52a21868"
    Private Const URL_MARKET As String = "https://bittrex.com/Market/Index?MarketName="
    Private Const CHECK_MESSAGE As String = "Checking your browser"
    Private Const DELIST_MESSAGE As String = "This market will be delisted on"
    Private Const REMOVE_MESSAGE As String = "market will be removed on"
    Private Const DGVOPENORDERS_MARKETCOL As Integer = 4

    ' Tools
    Private updateStatusMehtod As UpdateStatusDelegate = New UpdateStatusDelegate(AddressOf UpdateStatus)
    Private _bittrexAPI As New BittrexAPI()

    ' Objects
    Private _openOrders As New BindingList(Of Result)
    Private _openOrdersBS As BindingSource = New BindingSource()
    Private _completedOrders As New BindingList(Of BittrexAPI.ClosedOrder)
    Private _completedOrdersBS As BindingSource = New BindingSource()

    Sub New()
        ' Components
        InitializeComponent()
        statusStripLabel.Text = "Idle."
        _openOrdersBS.DataSource = _openOrders
        dgv_OpenOrders.DataSource = _openOrdersBS
        _completedOrdersBS.DataSource = _completedOrders
        dgv_CompletedOrders.DataSource = _completedOrdersBS

        ' Class objects
        _bittrexAPI.SetApiKeyRead(READ_KEY, READ_SECRET)
    End Sub

    Private Sub btn_GetOrders_Click(sender As Object, e As EventArgs) Handles btn_GetOrders.Click
        'Clear
        _openOrders.Clear()
        _completedOrders.Clear()

        'Get completed orders
        UpdateStatus("Getting open orders...")
        Dim completedOrders As List(Of BittrexAPI.ClosedOrder) = _bittrexAPI.GetClosedOrders()
        For Each compltedOrder As BittrexAPI.ClosedOrder In completedOrders
            _completedOrders.Add(compltedOrder)
        Next
        dgv_CompletedOrders.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells)

        'Get open orders
        UpdateStatus("Getting open orders...")
        Dim openOrders As List(Of BittrexAPI.OpenOrder) = _bittrexAPI.GetOpenOrders()
        For Each openOrder As BittrexAPI.OpenOrder In openOrders
            Me._openOrders.Add(New Result(openOrder, _bittrexAPI.GetMarketSummary(openOrder.Exchange)))
        Next
        dgv_OpenOrders.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells)

        'Check for markets flagged for removal
        checkMarketsForDelist()
    End Sub

    Sub checkMarketsForDelist()
        For Each order As BittrexAPI.OpenOrder In _openOrders
            Dim browser As WebBrowser = New WebBrowser()
            browser.ScriptErrorsSuppressed = True
            AddHandler browser.DocumentCompleted, New WebBrowserDocumentCompletedEventHandler(AddressOf MarketPageLoaded)
            browser.Navigate(URL_MARKET + order.Market)
            Thread.Sleep(500)
        Next
    End Sub

    Sub MarketPageLoaded(ByVal sender As Object, ByVal e As WebBrowserDocumentCompletedEventArgs)
        Dim browser As WebBrowser = CType(sender, WebBrowser)
        Dim market As String = browser.Url.ToString().Substring(browser.Url.ToString().IndexOf("=") + 1)

        Dim delist = False
        Dim html As String = browser.Document.Body.InnerHtml.ToString().ToUpper()
        If (Not html.Contains(CHECK_MESSAGE.ToUpper())) Then
            If (html.Contains(DELIST_MESSAGE.ToUpper()) Or html.Contains(REMOVE_MESSAGE.ToUpper())) Then
                delist = True
            End If
            For Each row As DataGridViewRow In dgv_OpenOrders.Rows
                If (row.Cells(DGVOPENORDERS_MARKETCOL).Value = market) Then
                    If delist Then
                        row.DefaultCellStyle.BackColor = Color.LightPink
                    Else
                        row.DefaultCellStyle.BackColor = Color.LightGreen
                    End If
                End If
            Next
            updateStatusMehtod("Checked " + market)
            browser.Dispose()
        End If
    End Sub

    Private Sub UpdateStatus(Text As String)
        If (statusStrip.InvokeRequired) Then
            statusStrip.Invoke(New UpdateStatusDelegate(AddressOf UpdateStatus), Text)
        Else
            statusStripLabel.Text = Text
            statusStrip.Refresh()
        End If
    End Sub

    Private Sub btn_CheckDelist_Click(sender As Object, e As EventArgs) Handles btn_CheckDelist.Click
        For Each row As DataGridViewRow In dgv_OpenOrders.Rows
            row.DefaultCellStyle.BackColor = Color.White
        Next
        Me.checkMarketsForDelist()
    End Sub

    Private Sub dgv_OpenOrders_CellDoubleClick(sender As Object, e As DataGridViewCellEventArgs) Handles dgv_OpenOrders.CellDoubleClick
        Dim dgv As DataGridView = sender
        Process.Start(URL_MARKET + dgv.Rows(e.RowIndex).Cells(DGVOPENORDERS_MARKETCOL).Value)
    End Sub

    Class Result
        Inherits BittrexAPI.OpenOrder

        Public MarketLastVal As Double
        Public MarketPreviousDayVal As Double

        Public ReadOnly Property MarketLast As Double
            Get
                Return Me.MarketLastVal
            End Get
        End Property
        Public ReadOnly Property MarketPreviousDay As Double
            Get
                Return Me.MarketPreviousDayVal
            End Get
        End Property

        Public Sub New(ByVal order As BittrexAPI.OpenOrder, ByVal marketSummary As BittrexAPI.MarketSummary)
            Me.CancelInitiated = order.CancelInitiated
            Me.Closed = order.Closed
            Me.CommissionPaid = order.CommissionPaid
            Me.Condition = order.Condition
            Me.ConditionTarget = order.ConditionTarget
            Me.Exchange = order.Exchange
            Me.ImmediateOrCancel = order.ImmediateOrCancel
            Me.IsConditional = order.IsConditional
            Me.Limit = order.Limit
            Me.Opened = order.Opened
            Me.OrderType = order.OrderType
            Me.OrderUuid = order.OrderUuid
            Me.Price = order.Price
            Me.PricePerUnit = order.PricePerUnit
            Me.Quantity = order.Quantity
            Me.QuantityRemaining = order.QuantityRemaining
            Me.Uuid = order.Uuid
            Me.MarketLastVal = marketSummary.Last
            Me.MarketPreviousDayVal = marketSummary.PrevDay
        End Sub
    End Class
End Class
