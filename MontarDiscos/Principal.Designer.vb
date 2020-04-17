<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class Principal
    Inherits System.Windows.Forms.Form

    'Form reemplaza a Dispose para limpiar la lista de componentes.
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

    'Requerido por el Diseñador de Windows Forms
    Private components As System.ComponentModel.IContainer

    'NOTA: el Diseñador de Windows Forms necesita el siguiente procedimiento
    'Se puede modificar usando el Diseñador de Windows Forms.  
    'No lo modifique con el editor de código.
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(Principal))
        Me.LnkMonitor = New System.Windows.Forms.Timer(Me.components)
        Me.Configuracion = New System.Windows.Forms.NotifyIcon(Me.components)
        Me.MenuConfiguracion = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.MenuConfig = New System.Windows.Forms.ToolStripMenuItem()
        Me.MenuWeb = New System.Windows.Forms.ToolStripMenuItem()
        Me.MenuSalir = New System.Windows.Forms.ToolStripMenuItem()
        Me.MenuConfiguracion.SuspendLayout()
        Me.SuspendLayout()
        '
        'LnkMonitor
        '
        '
        'Configuracion
        '
        Me.Configuracion.BalloonTipIcon = System.Windows.Forms.ToolTipIcon.Info
        Me.Configuracion.BalloonTipText = "contenido"
        Me.Configuracion.BalloonTipTitle = "titulo"
        Me.Configuracion.ContextMenuStrip = Me.MenuConfiguracion
        Me.Configuracion.Icon = CType(resources.GetObject("Configuracion.Icon"), System.Drawing.Icon)
        Me.Configuracion.Text = "USBMount"
        Me.Configuracion.Visible = True
        '
        'MenuConfiguracion
        '
        Me.MenuConfiguracion.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.MenuConfig, Me.MenuWeb, Me.MenuSalir})
        Me.MenuConfiguracion.Name = "MenuConfiguracion"
        Me.MenuConfiguracion.Size = New System.Drawing.Size(182, 70)
        Me.MenuConfiguracion.Text = "pppppp"
        '
        'MenuConfig
        '
        Me.MenuConfig.Checked = True
        Me.MenuConfig.CheckOnClick = True
        Me.MenuConfig.CheckState = System.Windows.Forms.CheckState.Checked
        Me.MenuConfig.Name = "MenuConfig"
        Me.MenuConfig.Size = New System.Drawing.Size(181, 22)
        Me.MenuConfig.Text = "Iniciar con Windows"
        '
        'MenuWeb
        '
        Me.MenuWeb.Name = "MenuWeb"
        Me.MenuWeb.Size = New System.Drawing.Size(181, 22)
        Me.MenuWeb.Text = "Visita Nuestra Web"
        '
        'MenuSalir
        '
        Me.MenuSalir.Name = "MenuSalir"
        Me.MenuSalir.Size = New System.Drawing.Size(181, 22)
        Me.MenuSalir.Text = "Salir"
        '
        'Principal
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(262, 105)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Name = "Principal"
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "MontarUSB"
        Me.WindowState = System.Windows.Forms.FormWindowState.Minimized
        Me.MenuConfiguracion.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents LnkMonitor As Timer
    Friend WithEvents Configuracion As NotifyIcon
    Friend WithEvents MenuConfiguracion As ContextMenuStrip
    Friend WithEvents MenuConfig As ToolStripMenuItem
    Friend WithEvents MenuWeb As ToolStripMenuItem
    Friend WithEvents MenuSalir As ToolStripMenuItem
End Class
