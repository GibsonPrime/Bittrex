Imports System.Threading
Imports System.Timers

Public Class MainForm
    Private Const FREQUENT_RESTART_LIMIT As Integer = 100

    Private Delegate Sub ReportErrorDelegate(errorMessage As String)
    Private _reportError As ReportErrorDelegate = New ReportErrorDelegate(AddressOf reportError)
    Private _executablePath As String
    Private _processName As String
    Private _monitorThread As Thread = New Thread(New ThreadStart(AddressOf doMonitor))
    Private _doStop As Boolean = False
    Private _lastRestartTime As DateTime = Nothing

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
        If (Me._monitorThread.ThreadState = ThreadState.Running) Then
            MsgBox("Process helper is already monitoring " + _processName, MsgBoxStyle.OkOnly, "Already started.")
        Else
            If (Not txt_ExecutablePath.Text.Contains(".exe")) Then
                Me._reportError("Invalid executable path specified.")
            ElseIf (txt_ProcessName.Text = "") Then
                Me._reportError("No process name specified.")
            ElseIf (Not IO.File.Exists(txt_ExecutablePath.Text))
                Me._reportError("Specified executable does not exist.")
            Else
                Try
                    Me._executablePath = txt_ExecutablePath.Text
                    Me._processName = txt_ProcessName.Text
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
            If (getProcessID(Process.GetProcesses()) = 0) Then
                restartApplication()
            End If

            'Monitor
            While (Not Me._doStop)
                If (getProcessID(Process.GetProcesses()) = 0) Then
                    restartApplication()
                End If
                Thread.Sleep(1000)
            End While
        Catch ex As Exception
            Me.Invoke(Me._reportError, ex.Message)
        End Try
    End Sub

    Private Function getProcessID(ByVal processes As Process()) As Integer
        For Each p As Process In processes
            If (p.ProcessName = Me._processName) Then
                Return p.Id
            End If
        Next
        Return 0 'PID = 0 is reserved for pseudo processes
    End Function

    Private Sub restartApplication()
        Try
            If (Not (Me._lastRestartTime = Nothing) And DateTime.Now.Subtract(Me._lastRestartTime).TotalMilliseconds < MainForm.FREQUENT_RESTART_LIMIT) Then
                Throw New Exception("Frequent restart detected.  Possible causes:" + vbCrLf + "-Recurrent fialure." + vbCrLf + "-Insufficient restart time." + vbCrLf + "-Incorrect process name.")
            End If
            Process.Start(Me._executablePath)
            Me._lastRestartTime = DateTime.Now
        Catch ex As Exception
            Me.Invoke(Me._reportError, ex.Message)
        End Try
    End Sub
End Class
