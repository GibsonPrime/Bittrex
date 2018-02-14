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
        Me.dgv_OpenOrders = New System.Windows.Forms.DataGridView()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.dgv_CompletedOrders = New System.Windows.Forms.DataGridView()
        Me.btn_GetOrders = New System.Windows.Forms.Button()
        Me.statusStrip = New System.Windows.Forms.StatusStrip()
        Me.statusStripLabel = New System.Windows.Forms.ToolStripStatusLabel()
        Me.btn_CheckDelist = New System.Windows.Forms.Button()
        CType(Me.dgv_OpenOrders, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.dgv_CompletedOrders, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.statusStrip.SuspendLayout()
        Me.SuspendLayout()
        '
        'dgv_OpenOrders
        '
        Me.dgv_OpenOrders.AllowUserToAddRows = False
        Me.dgv_OpenOrders.AllowUserToDeleteRows = False
        Me.dgv_OpenOrders.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.dgv_OpenOrders.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dgv_OpenOrders.Location = New System.Drawing.Point(12, 54)
        Me.dgv_OpenOrders.Name = "dgv_OpenOrders"
        Me.dgv_OpenOrders.RowHeadersVisible = False
        Me.dgv_OpenOrders.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me.dgv_OpenOrders.Size = New System.Drawing.Size(1040, 185)
        Me.dgv_OpenOrders.TabIndex = 0
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(12, 38)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(67, 13)
        Me.Label1.TabIndex = 1
        Me.Label1.Text = "Open Orders"
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(12, 242)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(91, 13)
        Me.Label2.TabIndex = 2
        Me.Label2.Text = "Completed Orders"
        '
        'dgv_CompletedOrders
        '
        Me.dgv_CompletedOrders.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.dgv_CompletedOrders.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dgv_CompletedOrders.Location = New System.Drawing.Point(10, 258)
        Me.dgv_CompletedOrders.Name = "dgv_CompletedOrders"
        Me.dgv_CompletedOrders.RowHeadersVisible = False
        Me.dgv_CompletedOrders.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me.dgv_CompletedOrders.Size = New System.Drawing.Size(1040, 185)
        Me.dgv_CompletedOrders.TabIndex = 3
        '
        'btn_GetOrders
        '
        Me.btn_GetOrders.Location = New System.Drawing.Point(12, 12)
        Me.btn_GetOrders.Name = "btn_GetOrders"
        Me.btn_GetOrders.Size = New System.Drawing.Size(75, 23)
        Me.btn_GetOrders.TabIndex = 4
        Me.btn_GetOrders.Text = "Get Orders"
        Me.btn_GetOrders.UseVisualStyleBackColor = True
        '
        'statusStrip
        '
        Me.statusStrip.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.statusStripLabel})
        Me.statusStrip.Location = New System.Drawing.Point(0, 446)
        Me.statusStrip.Name = "statusStrip"
        Me.statusStrip.Size = New System.Drawing.Size(1062, 22)
        Me.statusStrip.TabIndex = 5
        '
        'statusStripLabel
        '
        Me.statusStripLabel.Name = "statusStripLabel"
        Me.statusStripLabel.Size = New System.Drawing.Size(121, 17)
        Me.statusStripLabel.Text = "ToolStripStatusLabel1"
        '
        'btn_CheckDelist
        '
        Me.btn_CheckDelist.Location = New System.Drawing.Point(93, 12)
        Me.btn_CheckDelist.Name = "btn_CheckDelist"
        Me.btn_CheckDelist.Size = New System.Drawing.Size(75, 23)
        Me.btn_CheckDelist.TabIndex = 6
        Me.btn_CheckDelist.Text = "Check Delist"
        Me.btn_CheckDelist.UseVisualStyleBackColor = True
        '
        'MainForm
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1062, 468)
        Me.Controls.Add(Me.btn_CheckDelist)
        Me.Controls.Add(Me.statusStrip)
        Me.Controls.Add(Me.btn_GetOrders)
        Me.Controls.Add(Me.dgv_CompletedOrders)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.dgv_OpenOrders)
        Me.Name = "MainForm"
        Me.Text = "Bittrex"
        CType(Me.dgv_OpenOrders, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.dgv_CompletedOrders, System.ComponentModel.ISupportInitialize).EndInit()
        Me.statusStrip.ResumeLayout(False)
        Me.statusStrip.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents dgv_OpenOrders As DataGridView
    Friend WithEvents Label1 As Label
    Friend WithEvents Label2 As Label
    Friend WithEvents dgv_CompletedOrders As DataGridView
    Friend WithEvents btn_GetOrders As Button
    Friend WithEvents statusStrip As StatusStrip
    Friend WithEvents statusStripLabel As ToolStripStatusLabel
    Friend WithEvents btn_CheckDelist As Button
End Class
