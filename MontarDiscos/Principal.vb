Imports System.IO
Imports Microsoft.Win32
Imports MontarUSB.UsbEject

Public Class Principal
    Private Escritorio As String = My.Computer.FileSystem.SpecialDirectories.Desktop
    Private Const DispositivoCambiado As Integer = &H219
    Private Const DispositivoConectado As Integer = &H8000
    Private Const DispositivoRemovido As Integer = &H8004
    Dim dispositivo() As Device
    Dim Usb() As String

    Private Sub Form2_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Dim MonitorLnk As FileSystemWatcher = New FileSystemWatcher
        Dim hklm As RegistryKey = Registry.LocalMachine
        hklm = hklm.OpenSubKey("SOFTWARE\Microsoft\Windows\CurrentVersion\Run", True)
        If hklm.GetValue(Application.ProductName) Is Nothing Then
            MenuConfig.Checked = False
        Else
            MenuConfig.Checked = True
        End If
        hklm.Close()
        MonitorLnk.Path = Escritorio
        MonitorLnk.Filter = "* (?).lnk"
        DiscosConectados()
        ActualizarAccesos()
        LnkMonitor.Enabled = True
    End Sub

    Private Sub DiscosConectados()
        Dim allDiscos() As DriveInfo = DriveInfo.GetDrives
        Dim d As DriveInfo
        Dim Lugar As Integer = 0
        Dim pos As Integer = 0
        Erase Usb
        Erase dispositivo

        Dim volumeDeviceClass As New VolumeDeviceClass()
        For Each device As Volume In volumeDeviceClass.Devices
            If (Not device.IsUsb) Then
                Continue For
            End If
            ReDim Preserve dispositivo(Lugar)
            dispositivo(Lugar) = device
            Lugar += 1
        Next

        For Each d In allDiscos
            If d.IsReady = True Then
                If d.DriveType = DriveType.Removable Then
                    ReDim Preserve Usb(pos)
                    Usb(pos) = Escritorio & "\" & d.VolumeLabel & " (" & Mid(d.Name, 1, 1) & ").lnk"
                    pos += 1
                End If
            End If
        Next

    End Sub

    Private Sub CrearAcceso(RutaUnidad As String, RutaAcceso As String)
        If Not File.Exists(RutaAcceso) Then
            Dim WSHShell As Object = CreateObject("WScript.Shell")
            Dim Shortcut As Object = WSHShell.CreateShortcut(RutaAcceso)
            Shortcut.TargetPath = RutaUnidad
            Shortcut.Save()
        End If
    End Sub

    Private Sub ActualizarAccesos()
        EliminarAcceso()
        If Usb IsNot Nothing Then
            For i As Integer = 0 To Usb.Length - 1
                CrearAcceso(Mid(Usb(i), Usb(i).Length - 5, 1) & ":\", Usb(i))
            Next
        End If
    End Sub

    Private Sub EliminarAcceso()
        Dim Lnk(0) As String
        Dim CountLnk As Integer = FileIO.FileSystem.GetFiles(Escritorio, FileIO.SearchOption.SearchTopLevelOnly, "* (?).lnk").Count

        For i As Integer = 0 To CountLnk - 1
            ReDim Preserve Lnk(i)
            Lnk(i) = FileIO.FileSystem.GetFiles(Escritorio, FileIO.SearchOption.SearchTopLevelOnly, "* (?).lnk").Item(i)
        Next

        For e As Integer = 0 To CountLnk - 1
            File.Delete(Lnk(e))
        Next

    End Sub

    Protected Overrides Sub WndProc(ByRef m As System.Windows.Forms.Message)
        If m.Msg = DispositivoCambiado Then
            Select Case m.WParam
                Case DispositivoConectado
                    LnkMonitor.Enabled = False
                    DiscosConectados()
                    ActualizarAccesos()
                    LnkMonitor.Enabled = True
                Case DispositivoRemovido
                    LnkMonitor.Enabled = False
                    DiscosConectados()
                    ActualizarAccesos()
                    LnkMonitor.Enabled = True
            End Select
        End If
        MyBase.WndProc(m)
    End Sub

    Private Sub ExpulsarUSB(Nivel As Integer)
        Dim device As Device = dispositivo(Nivel)
        LnkMonitor.Enabled = False

        Dim s As String = device.Eject(True)
        If s IsNot Nothing Then
            MessageBox.Show(Me, s, Text, MessageBoxButtons.OK, MessageBoxIcon.[Error])
        Else
            Configuracion.BalloonTipText = "Ahora se puede quitar el dispositivo ""Dispositivo de almacenamiento USB"" de forma segura del equipo."
            Configuracion.BalloonTipTitle = "Es seguro quitar el hardware"
            Configuracion.ShowBalloonTip(10)
        End If
        LnkMonitor.Enabled = True
    End Sub

    Private Sub LnkMonitor_Tick(sender As Object, e As EventArgs) Handles LnkMonitor.Tick
        Dim Lnk(0) As String
        Dim d, a As Integer
        Dim Expulsar As Boolean = False
        Dim CountLnk As Integer = FileIO.FileSystem.GetFiles(Escritorio, FileIO.SearchOption.SearchTopLevelOnly, "* (?).lnk").Count

        For i As Integer = 0 To CountLnk - 1
            ReDim Preserve Lnk(i)
            Lnk(i) = FileIO.FileSystem.GetFiles(Escritorio, FileIO.SearchOption.SearchTopLevelOnly, "* (?).lnk").Item(i)
        Next

        If Usb IsNot Nothing Then
            d = 0
            a = 0
            For Each U In Usb
                For Each L In Lnk
                    If Usb(d) = Lnk(a) Then
                        Expulsar = False
                        Exit For
                    Else
                        Expulsar = True
                    End If
                    a += 1
                Next
                If Expulsar = True Then ExpulsarUSB(d)
                a = 0
                d += 1
            Next
        End If

    End Sub

    Private Sub MenuConfig_Click(sender As Object, e As EventArgs) Handles MenuConfig.Click
        Dim hklm As RegistryKey = Registry.LocalMachine
        If MenuConfig.Checked Then
            hklm = hklm.OpenSubKey("SOFTWARE\Microsoft\Windows\CurrentVersion\Run", True)
            Dim thisApp As String = Application.ExecutablePath
            hklm.SetValue(Application.ProductName, thisApp)
        Else
            hklm = hklm.OpenSubKey("SOFTWARE\Microsoft\Windows\CurrentVersion\Run", True)
            hklm.DeleteValue(Application.ProductName, False)
        End If
        hklm.Close()
    End Sub

    Private Sub MenuWeb_Click(sender As Object, e As EventArgs) Handles MenuWeb.Click
        Process.Start("http:\\cvcell.comoj.com")
    End Sub

    Private Sub MenuSalir_Click(sender As Object, e As EventArgs) Handles MenuSalir.Click
        Application.Exit()
    End Sub

End Class