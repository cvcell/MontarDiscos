Imports System.ComponentModel
Imports System.Text

Namespace UsbEject
    ''' <summary>
    ''' A generic base class for physical devices.
    ''' </summary>
    <TypeConverter(GetType(ExpandableObjectConverter))>
    Public Class Device
        Implements IComparable
        Private _path As String
        Private _deviceClass As DeviceClass
        Private _description As String
        Private _class As String
        Private _classGuid As String
        Private _parent As Device
        Private _index As Integer
        Private _capabilities As DeviceCapabilities = DeviceCapabilities.Unknown
        Private _removableDevices As List(Of Device)
        Private _friendlyName As String
        Private _deviceInfoData As Native.SP_DEVINFO_DATA

        Friend Sub New(deviceClass As DeviceClass, deviceInfoData As Native.SP_DEVINFO_DATA, path As String, index As Integer)
            If deviceClass Is Nothing Then
                Throw New ArgumentNullException("deviceClass")
            End If

            If deviceInfoData Is Nothing Then
                Throw New ArgumentNullException("deviceInfoData")
            End If

            _deviceClass = deviceClass
            _path = path
            ' may be null
            _deviceInfoData = deviceInfoData
            _index = index
        End Sub

        ''' <summary>
        ''' Gets the device's index.
        ''' </summary>
        Public ReadOnly Property Index() As Integer
            Get
                Return _index
            End Get
        End Property

        ''' <summary>
        ''' Gets the device's class instance.
        ''' </summary>
        <Browsable(False)>
        Public ReadOnly Property DeviceClass() As DeviceClass
            Get
                Return _deviceClass
            End Get
        End Property

        ''' <summary>
        ''' Gets the device's path.
        ''' </summary>
        Public ReadOnly Property Path() As String
            Get
                If _path Is Nothing Then
                End If
                Return _path
            End Get
        End Property

        ''' <summary>
        ''' Gets the device's instance handle.
        ''' </summary>
        Public ReadOnly Property InstanceHandle() As Integer
            Get
                Return _deviceInfoData.devInst
            End Get
        End Property

        ''' <summary>
        ''' Gets the device's class name.
        ''' </summary>
        Public ReadOnly Property [Class]() As String
            Get
                If _class Is Nothing Then
                    _class = _deviceClass.GetProperty(_deviceInfoData, Native.SPDRP_CLASS, "")
                End If
                Return _class
            End Get
        End Property

        ''' <summary>
        ''' Gets the device's class Guid as a string.
        ''' </summary>
        Public ReadOnly Property ClassGuid() As String
            Get
                If _classGuid Is Nothing Then
                    _classGuid = _deviceClass.GetProperty(_deviceInfoData, Native.SPDRP_CLASSGUID, "")
                End If
                Return _classGuid
            End Get
        End Property

        ''' <summary>
        ''' Gets the device's description.
        ''' </summary>
        Public ReadOnly Property Description() As String
            Get
                If _description Is Nothing Then
                    _description = _deviceClass.GetProperty(_deviceInfoData, Native.SPDRP_DEVICEDESC, "")
                End If
                Return _description
            End Get
        End Property

        ''' <summary>
        ''' Gets the device's friendly name.
        ''' </summary>
        Public ReadOnly Property FriendlyName() As String
            Get
                If _friendlyName Is Nothing Then
                    _friendlyName = _deviceClass.GetProperty(_deviceInfoData, Native.SPDRP_FRIENDLYNAME, "")
                End If
                Return _friendlyName
            End Get
        End Property

        ''' <summary>
        ''' Gets the device's capabilities.
        ''' </summary>
        Public ReadOnly Property Capabilities() As DeviceCapabilities
            Get
                If _capabilities = DeviceCapabilities.Unknown Then
                    _capabilities = DirectCast(_deviceClass.GetProperty(_deviceInfoData, Native.SPDRP_CAPABILITIES, 0), DeviceCapabilities)
                End If
                Return _capabilities
            End Get
        End Property

        ''' <summary>
        ''' Gets a value indicating whether this device is a USB device.
        ''' </summary>
        Public Overridable ReadOnly Property IsUsb() As Boolean
            Get
                If [Class] = "USB" Then
                    Return True
                End If

                If Parent Is Nothing Then
                    Return False
                End If

                Return Parent.IsUsb
            End Get
        End Property

        ''' <summary>
        ''' Gets the device's parent device or null if this device has not parent.
        ''' </summary>
        Public ReadOnly Property Parent() As Device
            Get
                If _parent Is Nothing Then
                    Dim parentDevInst As Integer = 0
                    Dim hr As Integer = Native.CM_Get_Parent(parentDevInst, _deviceInfoData.devInst, 0)
                    If hr = 0 Then
                        _parent = New Device(_deviceClass, _deviceClass.GetInfo(parentDevInst), Nothing, -1)
                    End If
                End If
                Return _parent
            End Get
        End Property

        ''' <summary>
        ''' Gets this device's list of removable devices.
        ''' Removable devices are parent devices that can be removed.
        ''' </summary>
        Public Overridable ReadOnly Property RemovableDevices() As List(Of Device)
            Get
                If _removableDevices Is Nothing Then
                    _removableDevices = New List(Of Device)()

                    If (Capabilities And DeviceCapabilities.Removable) <> 0 Then
                        _removableDevices.Add(Me)
                    Else
                        If Parent IsNot Nothing Then
                            For Each device As Device In Parent.RemovableDevices
                                _removableDevices.Add(device)
                            Next
                        End If
                    End If
                End If
                Return _removableDevices
            End Get
        End Property

        ''' <summary>
        ''' Ejects the device.
        ''' </summary>
        ''' <param name="allowUI">Pass true to allow the Windows shell to display any related UI element, false otherwise.</param>
        ''' <returns>null if no error occured, otherwise a contextual text.</returns>
        Public Function Eject(allowUI As Boolean) As String
            For Each device As Device In RemovableDevices
                If allowUI Then
                    ' don't handle errors, there should be a UI for this
                    Dim prueba As Integer = device.InstanceHandle
                    Native.CM_Request_Device_Eject_NoUi(device.InstanceHandle, IntPtr.Zero, Nothing, 0, 0)
                Else
                    Dim sb As New StringBuilder(1024)

                    Dim veto As Native.PNP_VETO_TYPE
                    Dim hr As Integer = Native.CM_Request_Device_Eject(device.InstanceHandle, veto, sb, sb.Capacity, 0)
                    If hr <> 0 Then
                        Throw New Win32Exception(hr)
                    End If

                    If veto <> Native.PNP_VETO_TYPE.Ok Then
                        Return veto.ToString()
                    End If

                End If
            Next
            Return Nothing
        End Function

        ''' <summary>
        ''' Compares the current instance with another object of the same type.
        ''' </summary>
        ''' <param name="obj">An object to compare with this instance.</param>
        ''' <returns>A 32-bit signed integer that indicates the relative order of the comparands.</returns>
        Public Overridable Function CompareTo(obj As Object) As Integer Implements IComparable.CompareTo
            Dim device As Device = TryCast(obj, Device)
            If device Is Nothing Then
                Throw New ArgumentException()
            End If

            Return Index.CompareTo(device.Index)
        End Function
    End Class
End Namespace
