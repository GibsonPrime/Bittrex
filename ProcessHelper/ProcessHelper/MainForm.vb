Imports System.Threading
Imports System.Timers

Public Class MainForm
    Private Delegate Sub HandleExceptionDelegate(ByVal ex As Exception)
    Private _handleException As HandleExceptionDelegate = New HandleExceptionDelegate(AddressOf handleException)
    Private _executablePath As String
    Private _processName As String
    Private _monitorThread As Thread = New Thread(New ThreadStart(AddressOf doMonitor))
    Private _doStop As Boolean = False
    Private _restartTime As Integer
    Private _recentRestart As Boolean = False
    Private _restartTimer As Timers.Timer

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
                Me._handleException(New Exception("Invalid executable path specified."))
            ElseIf (txt_ProcessName.Text = "") Then
                Me._handleException(New Exception("No process name specified."))
            ElseIf (Not IO.File.Exists(txt_ExecutablePath.Text))
                Me._handleException(New Exception("Specified executable does not exist."))
            Else
                Try
                    Me._restartTime = CInt(Me.txt_RestartTime.Text)
                    Me._restartTimer = New Timers.Timer(_restartTime + 500)
                    AddHandler Me._restartTimer.Elapsed, New ElapsedEventHandler(AddressOf resetRecentRestart)
                    Me._executablePath = txt_ExecutablePath.Text
                    Me._processName = txt_ProcessName.Text
                    Me._doStop = False
                    Me._monitorThread = New Thread(New ThreadStart(AddressOf doMonitor))
                    'Me._monitorThread.Start()
                    doMonitor()
                Catch ex As Exception
                    Me._handleException(ex)
                End Try
            End If
        End If
    End Sub

    Private Sub btn_Stop_Click(sender As Object, e As EventArgs) Handles btn_Stop.Click
        Me._doStop = True
    End Sub

    Private Sub handleException(ByVal ex As Exception)
        If (Me.InvokeRequired) Then
            Me._handleException(ex)
        Else
            Me._doStop = True
            MsgBox(ex.Message, MsgBoxStyle.OkOnly, "Error")
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
            Me._handleException(ex)
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
            If (Me._recentRestart) Then
                Throw New Exception("Frequent restart detected.  Possible recurrent fialure or incorrect process name.")
            End If
            Process.Start(Me._executablePath)
            Thread.Sleep(Me._restartTime)
            If (getProcessID(Process.GetProcesses()) = 0) Then
                Throw New Exception("Failed to start/restart application.")
            End If
            Me._recentRestart = True
            Me._restartTimer.Start()
        Catch ex As Exception
            Me._handleException(ex)
        End Try
    End Sub

    Private Sub resetRecentRestart(sender As Object, e As EventArgs)
        Me._recentRestart = False
        Me._restartTimer.Stop()
    End Sub
End Class
