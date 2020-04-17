Namespace UsbEject
    ''' <summary>
    ''' The device class for disk devices.
    ''' </summary>
    Public Class DiskDeviceClass
        Inherits DeviceClass
        ''' <summary>
        ''' Initializes a new instance of the DiskDeviceClass class.
        ''' </summary>
        Public Sub New()
            MyBase.New(New Guid(Native.GUID_DEVINTERFACE_DISK))
        End Sub
    End Class
End Namespace
