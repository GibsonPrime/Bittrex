Imports System.ComponentModel

Public Class Form1
    Private Const MINSPAN = 1
    Private Const MAXSPAN = 10
    Private Const HIGHESTLOWESETPERCENT = 0.1
    Private Const GOAL = 0.3

    Private sqlConnection As SqlClient.SqlConnection = New SqlClient.SqlConnection("Server=LENOVO\SQLEXPRESS;Database=CoinTrading;Trusted_Connection=True; ")
    Private coinTradingAPI As New CoinTradingAPI(sqlConnection)

    Private parametersResults As BindingList(Of ParametersResult) = New BindingList(Of ParametersResult)
    Private spanResults As BindingList(Of SpanResult) = New BindingList(Of SpanResult)
    Private dgv1Bind As BindingSource = New BindingSource()
    Private dgv2Bind As BindingSource = New BindingSource()

    Public Sub New()
        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        dgv1Bind.DataSource = Me.spanResults
        DataGridView1.DataSource = dgv1Bind
        dgv2Bind.DataSource = Me.parametersResults
        DataGridView2.DataSource = dgv2Bind
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Dim markets As List(Of Integer) = coinTradingAPI.DownloadMarketIDs()

        ' For all markets
        For Each market As Integer In markets
            If (Not market = 185) Then  ' Skipping ETH-LGD
                'For market = 185 To 185 Step +1
                Dim hourTicks As List(Of CoinTradingAPI.Tick) = coinTradingAPI.DownloadHourTicks(market)

                ' For all spans
                For span = MINSPAN To MAXSPAN Step +1
                    Dim opportunities As List(Of OpportunityFinder.Opporunity) = OpportunityFinder.FindOpportunitys(hourTicks, span, HIGHESTLOWESETPERCENT)
                    Dim spanResults As List(Of OpportunityFinder.Result) = OpportunityFinder.AssessOpportunities(GOAL, opportunities, hourTicks)
                    Me.spanResults.Add(New SpanResult(coinTradingAPI.GetMarketNameFromID(market), span, spanResults))
                Next
            End If
        Next
        DataGridView1.AutoResizeColumns(DataGridViewAutoSizeColumnMode.AllCells)

        For span = MINSPAN To MAXSPAN Step +1
            Dim currentSpanResults As List(Of SpanResult) = GetAllSpanResultsForSpan(span)
            parametersResults.Add(New ParametersResult(span, currentSpanResults))
        Next
        DataGridView2.AutoResizeColumns(DataGridViewAutoSizeColumnMode.AllCells)
    End Sub

    Private Function GetAllSpanResultsForSpan(ByVal span As Integer) As List(Of SpanResult)
        Dim results As List(Of SpanResult) = New List(Of SpanResult)

        For Each sr In Me.spanResults
            If (sr.Span = span) Then
                results.Add(sr)
            End If
        Next

        Return results
    End Function

    Class ParametersResult
        Private spanDays As Integer
        Private totalResults As Integer
        Private resultsTerminated As Integer
        Private gainAverageTerminated As Double
        Private gainMin As Double
        Private gainMinAverage As Double
        Private gainAverage As Double
        Private gainMax As Double
        Private gainMaxAverage As Double
        Private daysMin As Double
        Private daysMinAverage As Double
        Private daysAverage As Double
        Private daysMax As Double
        Private daysMaxAverage As Double

        Public ReadOnly Property Span As Integer
            Get
                Return Me.spanDays
            End Get
        End Property
        Public ReadOnly Property NumResults As Integer
            Get
                Return Me.totalResults
            End Get
        End Property
        Public ReadOnly Property NumTerminated As Integer
            Get
                Return Me.resultsTerminated
            End Get
        End Property
        Public ReadOnly Property TerminatedAverageGain As Double
            Get
                Return Me.gainAverageTerminated
            End Get
        End Property
        Public ReadOnly Property MinGain As Double
            Get
                Return Me.gainMin
            End Get
        End Property
        Public ReadOnly Property AverageMinGain As Double
            Get
                Return Me.gainMinAverage
            End Get
        End Property
        Public ReadOnly Property AverageGain As Double
            Get
                Return Me.gainAverage
            End Get
        End Property
        Public ReadOnly Property MaxGain As Double
            Get
                Return Me.gainMax
            End Get
        End Property
        Public ReadOnly Property AverageMaxGain As Double
            Get
                Return Me.gainMaxAverage
            End Get
        End Property
        Public ReadOnly Property MinDays As Double
            Get
                Return Me.daysMin
            End Get
        End Property
        Public ReadOnly Property AverageMinDays As Double
            Get
                Return Me.daysMinAverage
            End Get
        End Property
        Public ReadOnly Property AverageDays As Double
            Get
                Return Me.daysAverage
            End Get
        End Property
        Public ReadOnly Property MaxDays As Double
            Get
                Return Me.daysMax
            End Get
        End Property
        Public ReadOnly Property AverageMaxDays As Double
            Get
                Return Me.daysMaxAverage
            End Get
        End Property

        Public Sub New(ByVal span As Integer, ByVal spanResults As List(Of SpanResult))
            Me.spanDays = span
            Me.totalResults = 0
            Me.resultsTerminated = 0
            Me.gainAverageTerminated = 0
            Me.gainMin = 0
            Me.gainMinAverage = 0
            Me.gainAverage = 0
            Me.gainMax = 0
            Me.gainMaxAverage = 0
            Me.daysMin = 0
            Me.daysMinAverage = 0
            Me.daysAverage = 0
            Me.daysMax = 0
            Me.daysMaxAverage = 0

            If (spanResults.Count > 0) Then
                Me.gainMin = spanResults(0).MinGain
                Me.gainMax = spanResults(0).MaxGain
                Me.daysMin = spanResults(0).MinDays
                Me.daysMax = spanResults(0).MaxDays

                For Each r As SpanResult In spanResults
                    Me.totalResults += r.NumOpportunities
                    Me.resultsTerminated += r.NumTerminated

                    Me.gainAverageTerminated += r.TerminatedAverageGain

                    Me.gainMinAverage += r.MinGain
                    Me.gainAverage += r.AverageGain
                    Me.gainMaxAverage += r.MaxGain
                    If (r.MinGain < Me.gainMin) Then
                        Me.gainMin = r.MinGain
                    End If
                    If (r.MaxGain > Me.gainMax) Then
                        Me.gainMax = r.MaxGain
                    End If

                    Me.daysMinAverage += r.MinDays
                    Me.daysAverage += r.AverageDays
                    Me.daysMaxAverage += r.MaxDays
                    If (r.MinDays < Me.daysMin) Then
                        Me.daysMin = r.MinDays
                    End If
                    If (r.MaxDays > Me.daysMax) Then
                        Me.daysMax = r.MaxDays
                    End If
                Next
                Me.gainAverageTerminated = Me.gainAverageTerminated / spanResults.Count
                Me.gainMinAverage = Me.gainMinAverage / spanResults.Count
                Me.gainAverage = Me.gainAverage / spanResults.Count
                Me.gainMaxAverage = Me.gainMaxAverage / spanResults.Count
                Me.daysMinAverage = Me.daysMinAverage / spanResults.Count
                Me.daysAverage = Me.daysAverage / spanResults.Count
                Me.daysMaxAverage = Me.daysMaxAverage / spanResults.Count
            End If
        End Sub
    End Class
    Class SpanResult
        Private marketName As String
        Private spanDays As Integer
        Private resultsTerminated As Integer
        Private gainAverageTerminated As Double
        Private gainMin As Double
        Private gainAverage As Double
        Private gainMax As Double
        Private daysMin As Double
        Private daysAverage As Double
        Private daysMax As Double

        Private results As List(Of OpportunityFinder.Result)

        Public ReadOnly Property Market As String
            Get
                Return Me.marketName
            End Get
        End Property
        Public ReadOnly Property Span As Integer
            Get
                Return Me.spanDays
            End Get
        End Property
        Public ReadOnly Property NumOpportunities As Integer
            Get
                Return Me.results.Count
            End Get
        End Property
        Public ReadOnly Property NumTerminated As Integer
            Get
                Return Me.resultsTerminated
            End Get
        End Property
        Public ReadOnly Property TerminatedAverageGain As Double
            Get
                Return Me.gainAverageTerminated
            End Get
        End Property
        Public ReadOnly Property MinGain As Double
            Get
                Return Me.gainMin
            End Get
        End Property
        Public ReadOnly Property MaxGain As Double
            Get
                Return Me.gainMax
            End Get
        End Property
        Public ReadOnly Property AverageGain As Double
            Get
                Return Me.gainAverage
            End Get
        End Property
        Public ReadOnly Property MinDays As Double
            Get
                Return Me.daysMin
            End Get
        End Property
        Public ReadOnly Property MaxDays As Double
            Get
                Return Me.daysMax
            End Get
        End Property
        Public ReadOnly Property AverageDays As Double
            Get
                Return Me.daysAverage
            End Get
        End Property

        Public Sub New(ByVal marketname As String, ByVal span As Integer, ByVal results As List(Of OpportunityFinder.Result))
            Me.marketName = marketname
            Me.spanDays = span
            Me.results = results
            Me.resultsTerminated = 0
            Me.gainAverageTerminated = 0
            Me.gainMin = 0
            Me.gainAverage = 0
            Me.gainMax = 0
            Me.daysMin = 0
            Me.daysAverage = 0
            Me.daysMax = 0

            If (Me.results.Count > 0) Then
                Me.gainMin = results(0).gain
                Me.daysMin = results(0).daysTaken

                For Each r As OpportunityFinder.Result In Me.results
                    If (r.goalMet) Then
                        Me.resultsTerminated += 1
                        Me.gainAverageTerminated += r.gain
                    End If

                    Me.gainAverage += r.gain
                    If (r.gain < Me.gainMin) Then
                        Me.gainMin = r.gain
                    End If
                    If (r.gain > Me.gainMax) Then
                        Me.gainMax = r.gain
                    End If

                    Me.daysAverage += r.daysTaken
                    If (r.daysTaken < Me.daysMin) Then
                        Me.daysMin = r.daysTaken
                    End If
                    If (r.daysTaken > Me.daysMax) Then
                        Me.daysMax = r.daysTaken
                    End If
                Next
                If (Me.resultsTerminated > 0) Then
                    Me.gainAverageTerminated = Me.gainAverageTerminated / Me.resultsTerminated
                Else
                    Me.gainAverageTerminated = 0
                End If
                Me.gainAverage = Me.gainAverage / Me.results.Count
                    Me.daysAverage = Me.daysAverage / Me.results.Count
                End If
        End Sub
    End Class
End Class
