<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class MainForm
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
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
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.txt_ProcessName = New System.Windows.Forms.TextBox()
        Me.btn_Stop = New System.Windows.Forms.Button()
        Me.btn_Start = New System.Windows.Forms.Button()
        Me.txt_ExecutablePath = New System.Windows.Forms.TextBox()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.btn_Browse = New System.Windows.Forms.Button()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.txt_RestartTime = New System.Windows.Forms.TextBox()
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
        Me.txt_ProcessName.Location = New System.Drawing.Point(90, 38)
        Me.txt_ProcessName.Name = "txt_ProcessName"
        Me.txt_ProcessName.Size = New System.Drawing.Size(332, 20)
        Me.txt_ProcessName.TabIndex = 1
        Me.txt_ProcessName.Text = "notepad++"
        '
        'btn_Stop
        '
        Me.btn_Stop.Location = New System.Drawing.Point(90, 90)
        Me.btn_Stop.Name = "btn_Stop"
        Me.btn_Stop.Size = New System.Drawing.Size(75, 23)
        Me.btn_Stop.TabIndex = 2
        Me.btn_Stop.Text = "Stop"
        Me.btn_Stop.UseVisualStyleBackColor = True
        '
        'btn_Start
        '
        Me.btn_Start.Location = New System.Drawing.Point(11, 90)
        Me.btn_Start.Name = "btn_Start"
        Me.btn_Start.Size = New System.Drawing.Size(75, 23)
        Me.btn_Start.TabIndex = 3
        Me.btn_Start.Text = "Start"
        Me.btn_Start.UseVisualStyleBackColor = True
        '
        'txt_ExecutablePath
        '
        Me.txt_ExecutablePath.Location = New System.Drawing.Point(90, 12)
        Me.txt_ExecutablePath.Name = "txt_ExecutablePath"
        Me.txt_ExecutablePath.Size = New System.Drawing.Size(332, 20)
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
        Me.btn_Browse.Location = New System.Drawing.Point(428, 10)
        Me.btn_Browse.Name = "btn_Browse"
        Me.btn_Browse.Size = New System.Drawing.Size(75, 23)
        Me.btn_Browse.TabIndex = 6
        Me.btn_Browse.Text = "Browse"
        Me.btn_Browse.UseVisualStyleBackColor = True
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(12, 64)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(67, 13)
        Me.Label3.TabIndex = 7
        Me.Label3.Text = "Restart Time"
        '
        'txt_RestartTime
        '
        Me.txt_RestartTime.Location = New System.Drawing.Point(90, 64)
        Me.txt_RestartTime.Name = "txt_RestartTime"
        Me.txt_RestartTime.Size = New System.Drawing.Size(332, 20)
        Me.txt_RestartTime.TabIndex = 8
        Me.txt_RestartTime.Text = "500"
        '
        'MainForm
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(515, 121)
        Me.Controls.Add(Me.txt_RestartTime)
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
    Friend WithEvents Label3 As Label
    Friend WithEvents txt_RestartTime As TextBox
End Class
