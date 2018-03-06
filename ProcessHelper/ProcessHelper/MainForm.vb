Imports System.Threading
Imports System.Data.SqlClient

Public Class MainForm
    Private Const FREQUENT_RESTART_LIMIT As Integer = 100

    Private Delegate Sub ReportErrorDelegate(errorMessage As String)
    Private _reportError As ReportErrorDelegate = New ReportErrorDelegate(AddressOf reportError)
    Private _monitorThread As Thread = New Thread(New ThreadStart(AddressOf doMonitor))
    Private _executablePath As String
    Private _processName As String
    Private _sqlConnection As SqlConnection
    Private _sqlProcedureString As String
    Private _sqlProcedureOutput As String
    Private _doStop As Boolean = False
    Private _lastRestartTime As DateTime = Nothing

#Region "Form Controls"
    Private Sub btn_Browse_Click(sender As Object, e As EventArgs) Handles btn_Browse.Click
        Dim openFileDialog As OpenFileDialog = New OpenFileDialog()

        openFileDialog.Title = "Select application executable"
        openFileDialog.InitialDirectory = "C:\"
        openFileDialog.Filter = "Executables | *.exe"
        openFileDialog.FilterIndex = 0
        openFileDialog.RestoreDirectory = True

        If openFileDialog.ShowDialog() = DialogResult.OK Then
            Me.txt_ExecutablePath.Text = openFileDialog.FileName
            Me.txt_ProcessName.Text = openFileDialog.SafeFileName.Remove(openFileDialog.SafeFileName.Length - 4)
        End If
    End Sub

    Private Sub btn_Start_Click(sender As Object, e As EventArgs) Handles btn_Start.Click
        If (Not (Me._monitorThread.ThreadState = ThreadState.Stopped Or Me._monitorThread.ThreadState = ThreadState.Unstarted)) Then
            MsgBox("Process helper is already monitoring " + _processName + ".", MsgBoxStyle.OkOnly, "Already started")
        Else
            If (Not txt_ExecutablePath.Text.Contains(".exe")) Then
                Me._reportError("Invalid executable path specified.")
            ElseIf (txt_ProcessName.Text = "") Then
                Me._reportError("No process name specified.")
            ElseIf (Not IO.File.Exists(txt_ExecutablePath.Text))
                Me._reportError("Specified executable does not exist.")
            ElseIf (txt_SqlConnection.Text = "" Or txt_SqlProcedure.Text = "" Or txt_ProcedureOutput.Text = "") Then
                Me._reportError("Database parameter(s) not specified.")
            Else
                Try
                    Me._executablePath = txt_ExecutablePath.Text
                    Me._processName = txt_ProcessName.Text
                    Me._sqlConnection = New SqlClient.SqlConnection(txt_SqlConnection.Text)
                    Me._sqlConnection.Open()
                    Me._sqlProcedureString = txt_SqlProcedure.Text
                    Me._sqlProcedureOutput = txt_ProcedureOutput.Text
                    Me._doStop = False
                    Me._monitorThread = New Thread(New ThreadStart(AddressOf doMonitor))
                    Me._monitorThread.Start()
                Catch ex As Exception
                    Me._reportError(ex.Message)
                End Try
            End If
        End If
    End Sub

    Private Sub btn_Stop_Click(sender As Object, e As EventArgs) Handles btn_Stop.Click
        Me._doStop = True
    End Sub

    Private Sub MainForm_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing
        Me._doStop = True
    End Sub
#End Region

#Region "Monitoring"
    Private Sub reportError(ByVal errorMessage As String)
        If (InvokeRequired) Then
            Me.Invoke(Me._reportError, New Object() {errorMessage})
        Else
            Me._doStop = True
            MsgBox(errorMessage, MsgBoxStyle.OkOnly, "Error")
        End If
    End Sub

    Private Sub doMonitor()
        Try
            'Start process if not found
            If (Not isProcessRunning()) Then
                restartProcess()
            End If

            'Monitor
            While (Not Me._doStop)
                ' Checkin with database
                If (Not databaseCheckin()) Then
                    killProcess()
                End If

                ' Check if app is running
                If (Not isProcessRunning()) Then
                    restartProcess()
                End If
                Thread.Sleep(1000)
            End While
        Catch ex As Exception
            Me.Invoke(Me._reportError, ex.Message)
        End Try
    End Sub

    Private Function isProcessRunning() As Boolean
        For Each p As Process In Process.GetProcesses()
            If (p.ProcessName = Me._processName) Then
                Return True
            End If
        Next
        Return False
    End Function

    Private Sub killProcess()
        For Each p As Process In Process.GetProcesses()
            If (p.ProcessName = Me._processName) Then
                p.Kill()
            End If
        Next
    End Sub

    Private Sub restartProcess()
        Try
            If (Not (Me._lastRestartTime = Nothing) And DateTime.Now.Subtract(Me._lastRestartTime).TotalMilliseconds < MainForm.FREQUENT_RESTART_LIMIT) Then
                Throw New Exception("Frequent restart detected.  Possibly caused by incorrect process name Or frequent And rapid application failure.")
            End If
            Process.Start(Me._executablePath)
            Me._lastRestartTime = DateTime.Now
        Catch ex As Exception
            Me.Invoke(Me._reportError, ex.Message)
        End Try
    End Sub

    Private Function databaseCheckin() As Boolean
        ' Prepare and execute
        Dim sqlCommand As New SqlCommand(Me._sqlProcedureString, Me._sqlConnection)
        sqlCommand.CommandType = CommandType.StoredProcedure
        Dim sqlParameter As New SqlParameter(Me._sqlProcedureOutput, SqlDbType.Bit, 1)
        sqlParameter.Direction = ParameterDirection.Output
        sqlCommand.Parameters.Add(sqlParameter)
        sqlCommand.ExecuteNonQuery()

        ' Get result and return
        For Each parameter As SqlParameter In sqlCommand.Parameters
            If parameter.ParameterName.Equals(Me._sqlProcedureOutput, StringComparison.CurrentCultureIgnoreCase) Then
                If IsDBNull(parameter.Value) Then
                    Return Nothing
                Else
                    Return CBool(parameter.Value)
                End If
            End If
        Next
        Throw New Exception("Database checkin procedure did not return or parameter name is incorrect.")
    End Function
#End Region
End Class
