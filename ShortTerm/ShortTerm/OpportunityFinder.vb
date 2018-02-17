Public Class OpportunityFinder
    Shared Function FindOpportunitys(ByVal hourTicks As List(Of CoinTradingAPI.Tick), ByVal span As Integer, ByVal highestLowestPercent As Decimal) As List(Of Opporunity)
        Dim opportunities As List(Of Opporunity) = New List(Of Opporunity)

        ' Looks for reinforced lows in hour ticks in SPAN ticks
        ' For all possible starting points in hour ticks
        For startTickIndex As Integer = span * 24 To hourTicks.Count - 1 Step +1
            'Get list of ticks over SPAN
            Dim spanTicks(span * 24) As CoinTradingAPI.Tick
            hourTicks.CopyTo(startTickIndex - span * 24, spanTicks, 0, spanTicks.Length)

            'Get lists of lowest lows and highest highs
            Dim lowest(spanTicks.Count * highestLowestPercent) As CoinTradingAPI.Tick
            SortByLowesetLow(spanTicks).CopyTo(0, lowest, 0, lowest.Length)
            Dim h(spanTicks.Count * highestLowestPercent) As CoinTradingAPI.Tick
            SortByHighestHigh(spanTicks).CopyTo(0, h, 0, h.Length)
            Dim highest As List(Of CoinTradingAPI.Tick) = h.ToList()

            'Get list of lowest lows separated by highest highs
            For i As Integer = 0 To lowest.Count - 3 Step +1
                For Each highTick In highest
                    Dim alreadyAdded = False
                    Dim added = False
                    If (highTick.T.CompareTo(lowest(i).T) > 0 And highTick.T.CompareTo(lowest(i + 1).T) < 0) Then
                        For j = highest.IndexOf(highTick) To highest.Count - 1 Step +1
                            If (highest(j).T.CompareTo(lowest(i + 1).T) > 0 And highest(j).T.CompareTo(lowest(i + 2).T) < 0) Then
                                For Each r In opportunities
                                    If r.startTick.T = lowest(i + 1).T Then
                                        alreadyAdded = True
                                        Exit For
                                    End If
                                Next

                                If (Not alreadyAdded) Then
                                    opportunities.Add(New Opporunity(lowest(i + 2), hourTicks.IndexOf(lowest(i + 2))))
                                    added = True
                                    Exit For
                                End If
                            End If
                        Next
                    End If

                    If (added Or alreadyAdded) Then
                        Exit For
                    End If
                Next
            Next
        Next

        Return opportunities
    End Function
    Private Shared Function SortByLowesetLow(ByVal inArray As CoinTradingAPI.Tick()) As List(Of CoinTradingAPI.Tick)
        Dim outList As List(Of CoinTradingAPI.Tick) = New List(Of CoinTradingAPI.Tick)

        For Each inArrayTick As CoinTradingAPI.Tick In inArray
            Dim insertIndex = 0
            For Each outListTick As CoinTradingAPI.Tick In outList
                If (outListTick.L < inArrayTick.L) Then
                    insertIndex += 1
                End If
            Next
            outList.Insert(insertIndex, inArrayTick)
        Next

        Return outList
    End Function
    Private Shared Function SortByHighestHigh(ByVal inArray As CoinTradingAPI.Tick()) As List(Of CoinTradingAPI.Tick)
        Dim outList As List(Of CoinTradingAPI.Tick) = New List(Of CoinTradingAPI.Tick)

        For Each inArrayTick As CoinTradingAPI.Tick In inArray
            Dim insertIndex = 0
            For Each outListTick As CoinTradingAPI.Tick In outList
                If (outListTick.H > inArrayTick.H) Then
                    insertIndex += 1
                End If
            Next
            outList.Insert(insertIndex, inArrayTick)
        Next

        Return outList
    End Function

    Shared Function AssessOpportunities(ByVal goal As Decimal, ByVal opportunities As List(Of Opporunity), ByVal hourTicks As List(Of CoinTradingAPI.Tick)) As List(Of Result)
        Dim results As List(Of Result) = New List(Of Result)

        For Each opportunity As Opporunity In opportunities
            Dim result As Result = New Result(opportunity.startTick)
            Dim curTick = result.startTick

            ' Best case - buy at low, sell at high
            Dim goalmet As Boolean = False
            Dim lastGain As Double = 0
            For i = opportunity.startTickIndex + 1 To hourTicks.Count - 1
                curTick = hourTicks(i)
                Dim curGain = (curTick.H - result.startTick.L) / result.startTick.L

                If curGain >= goal Then
                    goalmet = True
                    Exit For
                End If
            Next

            result.goalMet = goalmet
            result.endTick = curTick
            result.daysTaken = result.endTick.T.Subtract(result.startTick.T).TotalDays
            results.Add(result)
        Next

        Return results
    End Function

    Public Class Opporunity
        Public startTick As CoinTradingAPI.Tick
        Public startTickIndex As Integer

        Public Sub New(ByVal tick As CoinTradingAPI.Tick, ByVal tickIndex As Integer)
            Me.startTick = tick
            Me.startTickIndex = tickIndex
        End Sub
    End Class
    Public Class Result
        Public startTick As CoinTradingAPI.Tick
        Public endTick As CoinTradingAPI.Tick
        Public goalMet As Boolean = False
        Public daysTaken As Double

        Public ReadOnly Property gain As Double
            Get
                Return ((Me.endTick.H - Me.startTick.L) / Me.startTick.L) * 100
            End Get
        End Property

        Public Sub New(ByVal startTick As CoinTradingAPI.Tick)
            Me.startTick = startTick
        End Sub
    End Class
End Class
