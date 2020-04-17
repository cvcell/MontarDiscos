Imports System.Collections.Generic
Imports System.Text

Namespace UsbEject
    ''' <summary>
    ''' The device class for volume devices.
    ''' </summary>
    Public Class VolumeDeviceClass
        Inherits DeviceClass
        Friend _logicalDrives As New SortedDictionary(Of String, String)()

        ''' <summary>
        ''' Initializes a new instance of the VolumeDeviceClass class.
        ''' </summary>
        Public Sub New()
            MyBase.New(New Guid(Native.GUID_DEVINTERFACE_VOLUME))
            For Each drive As String In Environment.GetLogicalDrives()
                Dim sb As New StringBuilder(1024)
                If Native.GetVolumeNameForVolumeMountPoint(drive, sb, sb.Capacity) Then
                    _logicalDrives(sb.ToString()) = drive.Replace("\", "")
                End If
            Next
        End Sub

        Friend Overrides Function CreateDevice(deviceClass As DeviceClass, deviceInfoData As Native.SP_DEVINFO_DATA, path As String, index As Integer) As Device
            Return New Volume(deviceClass, deviceInfoData, path, index)
        End Function
    End Class
End Namespace
