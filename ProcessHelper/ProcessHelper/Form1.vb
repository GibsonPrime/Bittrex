Imports System.Threading

Public Class MainForm
    Private Delegate Sub HandleExceptionDelegate(ByVal ex As Exception)
    Private _handleException As HandleExceptionDelegate = New HandleExceptionDelegate(AddressOf handleException)
    Private _executablePath As String
    Private _processName As String
    Private _monitorThread As Thread
    Private _restartTime As Integer

    Private Sub btn_Browse_Click(sender As Object, e As EventArgs) Handles btn_Browse.Click
        Dim openFileDialog As OpenFileDialog = New OpenFileDialog()

        openFileDialog.Title = "Select application executable"
        openFileDialog.InitialDirectory = "C:\"
        openFileDialog.Filter = "Executables | *.exe"
        openFileDialog.FilterIndex = 2
        openFileDialog.RestoreDirectory = True

        If openFileDialog.ShowDialog() = DialogResult.OK Then
            Me.txt_ExecutablePath.Text = openFileDialog.FileName
            Me.txt_ProcessName.Text = openFileDialog.SafeFileName
        End If
    End Sub

    Private Sub btn_Start_Click(sender As Object, e As EventArgs) Handles btn_Start.Click
        If (Not txt_ExecutablePath.Text.Contains(".exe")) Then
            Me._handleException(New Exception("Invalid executable path specified."))
        ElseIf (txt_ProcessName.Text = "") Then
            Me._handleException(New Exception("No process name specified."))
        Else
            Try
                Me._restartTime = CInt(Me.txt_RestartTime.Text)
            Catch ex As Exception
                Me._handleException(ex)
            End Try

            Me._executablePath = txt_ExecutablePath.Text
            Me._processName = txt_ProcessName.Text

            Me._monitorThread = New Thread(New ThreadStart(AddressOf doMonitor))
            Me._monitorThread.Start()
        End If
    End Sub

    Private Sub btn_Stop_Click(sender As Object, e As EventArgs) Handles btn_Stop.Click
        killMonitorThread()
    End Sub

    Private Sub handleException(ByVal ex As Exception)
        If (Me.InvokeRequired) Then
            Me._handleException(ex)
        Else
            killMonitorThread()
            MsgBox("Process helper encountered an exception:  " + ex.Message, MsgBoxStyle.OkOnly, "Exception")
        End If
    End Sub

    Private Sub killMonitorThread()
        If (Me._monitorThread.IsAlive) Then
            Me._monitorThread.Abort()
        End If
    End Sub

    Private Sub doMonitor()
        Try
            Dim processes As Process() = Process.GetProcesses()
            Dim pid As Integer = getProcessID(processes)

            'Start process if not found
            If (pid = 0) Then  'PID 0 is reserved for pseudo processes
                restartApplication()
                processes = Process.GetProcesses()
                pid = getProcessID(processes)
            End If

            'Monitor
            Dim foundProcess As Boolean = False
            While (True)
                processes = Process.GetProcesses()
                For Each p As Process In processes
                    If (p.Id = pid) Then
                        foundProcess = True
                    End If
                Next

                If (Not foundProcess) Then
                    restartApplication()
                End If

                Thread.Sleep(1000)
                foundProcess = False
            End While
        Catch ex As Exception
            Me._handleException(ex)
        End Try
    End Sub

    Private Function getProcessID(ByRef processes As Process()) As Integer
        For Each p As Process In processes
            If (p.ProcessName = Me._processName) Then
                Return p.Id
            End If
        Next
        Return 0
    End Function

    Private Sub restartApplication()
        Try
            Process.Start(Me._executablePath)
            Thread.Sleep(Me._restartTime)
            If (getProcessID(Process.GetProcesses()) = 0) Then
                Throw New Exception("Failed to start/restart application.")
            End If
        Catch ex As Exception
            Me._handleException(ex)
        End Try
    End Sub
End Class
