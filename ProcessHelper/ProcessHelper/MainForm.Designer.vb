<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class MainForm
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.txt_ProcessName = New System.Windows.Forms.TextBox()
        Me.btn_Stop = New System.Windows.Forms.Button()
        Me.btn_Start = New System.Windows.Forms.Button()
        Me.txt_ExecutablePath = New System.Windows.Forms.TextBox()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.btn_Browse = New System.Windows.Forms.Button()
        Me.txt_SqlConnection = New System.Windows.Forms.TextBox()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.txt_SqlProcedure = New System.Windows.Forms.TextBox()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.txt_ProcedureOutput = New System.Windows.Forms.TextBox()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.SuspendLayout()
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(8, 41)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(76, 13)
        Me.Label1.TabIndex = 0
        Me.Label1.Text = "Process Name"
        '
        'txt_ProcessName
        '
        Me.txt_ProcessName.Location = New System.Drawing.Point(99, 38)
        Me.txt_ProcessName.Name = "txt_ProcessName"
        Me.txt_ProcessName.Size = New System.Drawing.Size(429, 20)
        Me.txt_ProcessName.TabIndex = 1
        Me.txt_ProcessName.Text = "notepad++"
        '
        'btn_Stop
        '
        Me.btn_Stop.Location = New System.Drawing.Point(90, 142)
        Me.btn_Stop.Name = "btn_Stop"
        Me.btn_Stop.Size = New System.Drawing.Size(75, 23)
        Me.btn_Stop.TabIndex = 2
        Me.btn_Stop.Text = "Stop"
        Me.btn_Stop.UseVisualStyleBackColor = True
        '
        'btn_Start
        '
        Me.btn_Start.Location = New System.Drawing.Point(9, 142)
        Me.btn_Start.Name = "btn_Start"
        Me.btn_Start.Size = New System.Drawing.Size(75, 23)
        Me.btn_Start.TabIndex = 3
        Me.btn_Start.Text = "Start"
        Me.btn_Start.UseVisualStyleBackColor = True
        '
        'txt_ExecutablePath
        '
        Me.txt_ExecutablePath.Location = New System.Drawing.Point(99, 12)
        Me.txt_ExecutablePath.Name = "txt_ExecutablePath"
        Me.txt_ExecutablePath.Size = New System.Drawing.Size(429, 20)
        Me.txt_ExecutablePath.TabIndex = 5
        Me.txt_ExecutablePath.Text = "C:\Program Files (x86)\Notepad++\notepad++.exe"
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(8, 15)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(60, 13)
        Me.Label2.TabIndex = 4
        Me.Label2.Text = "Executable"
        '
        'btn_Browse
        '
        Me.btn_Browse.Location = New System.Drawing.Point(534, 10)
        Me.btn_Browse.Name = "btn_Browse"
        Me.btn_Browse.Size = New System.Drawing.Size(75, 23)
        Me.btn_Browse.TabIndex = 6
        Me.btn_Browse.Text = "Browse"
        Me.btn_Browse.UseVisualStyleBackColor = True
        '
        'txt_SqlConnection
        '
        Me.txt_SqlConnection.Location = New System.Drawing.Point(97, 64)
        Me.txt_SqlConnection.Name = "txt_SqlConnection"
        Me.txt_SqlConnection.Size = New System.Drawing.Size(431, 20)
        Me.txt_SqlConnection.TabIndex = 8
        Me.txt_SqlConnection.Text = "Server=LENOVO\SQLEXPRESS;Database=CoinTrading;Trusted_Connection=True; "
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(6, 67)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(85, 13)
        Me.Label3.TabIndex = 7
        Me.Label3.Text = "SQL Connection"
        '
        'txt_SqlProcedure
        '
        Me.txt_SqlProcedure.Location = New System.Drawing.Point(97, 90)
        Me.txt_SqlProcedure.Name = "txt_SqlProcedure"
        Me.txt_SqlProcedure.Size = New System.Drawing.Size(431, 20)
        Me.txt_SqlProcedure.TabIndex = 10
        Me.txt_SqlProcedure.Text = "Bittrex.Checkin"
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Location = New System.Drawing.Point(6, 93)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(80, 13)
        Me.Label4.TabIndex = 9
        Me.Label4.Text = "SQL Procedure"
        '
        'txt_ProcedureOutput
        '
        Me.txt_ProcedureOutput.Location = New System.Drawing.Point(97, 116)
        Me.txt_ProcedureOutput.Name = "txt_ProcedureOutput"
        Me.txt_ProcedureOutput.Size = New System.Drawing.Size(431, 20)
        Me.txt_ProcedureOutput.TabIndex = 12
        Me.txt_ProcedureOutput.Text = "Okay"
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Location = New System.Drawing.Point(6, 119)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(91, 13)
        Me.Label5.TabIndex = 11
        Me.Label5.Text = "Procedure Output"
        '
        'MainForm
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(617, 172)
        Me.Controls.Add(Me.txt_ProcedureOutput)
        Me.Controls.Add(Me.Label5)
        Me.Controls.Add(Me.txt_SqlProcedure)
        Me.Controls.Add(Me.Label4)
        Me.Controls.Add(Me.txt_SqlConnection)
        Me.Controls.Add(Me.Label3)
        Me.Controls.Add(Me.btn_Browse)
        Me.Controls.Add(Me.txt_ExecutablePath)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.btn_Start)
        Me.Controls.Add(Me.btn_Stop)
        Me.Controls.Add(Me.txt_ProcessName)
        Me.Controls.Add(Me.Label1)
        Me.Name = "MainForm"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "ProcessHelper"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents Label1 As Label
    Friend WithEvents txt_ProcessName As TextBox
    Friend WithEvents btn_Stop As Button
    Friend WithEvents btn_Start As Button
    Friend WithEvents txt_ExecutablePath As TextBox
    Friend WithEvents Label2 As Label
    Friend WithEvents btn_Browse As Button
    Friend WithEvents txt_SqlConnection As TextBox
    Friend WithEvents Label3 As Label
    Friend WithEvents txt_SqlProcedure As TextBox
    Friend WithEvents Label4 As Label
    Friend WithEvents txt_ProcedureOutput As TextBox
    Friend WithEvents Label5 As Label
End Class
