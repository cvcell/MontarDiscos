Imports System.ComponentModel
Imports System.Runtime.InteropServices
Imports System.Text

Namespace UsbEject
    ''' <summary>
    ''' A generic base class for physical device classes.
    ''' </summary>
    Public MustInherit Class DeviceClass
        'Implements IDisposable
        Private _deviceInfoSet As IntPtr
        Private _classGuid As Guid
        Private _devices As List(Of Device)

        Protected Sub New(classGuid As Guid)
            Me.New(classGuid, IntPtr.Zero)
        End Sub

        Friend Overridable Function CreateDevice(deviceClass As DeviceClass, deviceInfoData As Native.SP_DEVINFO_DATA, path As String, index As Integer) As Device
            Return New Device(deviceClass, deviceInfoData, path, index)
        End Function

        ''' <summary>
        ''' Initializes a new instance of the DeviceClass class.
        ''' </summary>
        ''' <param name="classGuid">A device class Guid.</param>
        ''' <param name="hwndParent">The handle of the top-level window to be used for any user interface or IntPtr.Zero for no handle.</param>
        Protected Sub New(classGuid As Guid, hwndParent As IntPtr)
            _classGuid = classGuid

            _deviceInfoSet = Native.SetupDiGetClassDevs(_classGuid, 0, hwndParent, Native.DIGCF_DEVICEINTERFACE Or Native.DIGCF_PRESENT)
            If _deviceInfoSet.ToInt64() = Native.INVALID_HANDLE_VALUE Then
                Throw New Win32Exception(Marshal.GetLastWin32Error())
            End If
        End Sub

        ''' <summary>
        ''' Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        ''' </summary>
        Public Sub Dispose()
            If _deviceInfoSet <> IntPtr.Zero Then
                Native.SetupDiDestroyDeviceInfoList(_deviceInfoSet)
                _deviceInfoSet = IntPtr.Zero
            End If
        End Sub

        ''' <summary>
        ''' Gets the device class's guid.
        ''' </summary>
        Public ReadOnly Property ClassGuid() As Guid
            Get
                Return _classGuid
            End Get
        End Property

        ''' <summary>
        ''' Gets the list of devices of this device class.
        ''' </summary>
        Public ReadOnly Property Devices() As List(Of Device)
            Get
                If _devices Is Nothing Then
                    _devices = New List(Of Device)()
                    Dim index As Integer = 0
                    While True
                        Dim interfaceData As New Native.SP_DEVICE_INTERFACE_DATA()
                        interfaceData.cbSize = Marshal.SizeOf(interfaceData)

                        If Not Native.SetupDiEnumDeviceInterfaces(_deviceInfoSet, IntPtr.Zero, _classGuid, index, interfaceData) Then
                            Dim [error] As Integer = Marshal.GetLastWin32Error()
                            If [error] <> Native.ERROR_NO_MORE_ITEMS Then
                                Throw New Win32Exception([error])
                            End If
                            Exit While
                        End If
                        Dim devData As New Native.SP_DEVINFO_DATA()
                        Dim p As IntPtr = Marshal.AllocHGlobal(Marshal.SizeOf(devData))
                        Marshal.StructureToPtr(devData, p, True)
                        Dim size As UInt32 = 0
                        If Not Native.SetupDiGetDeviceInterfaceDetail(_deviceInfoSet, interfaceData, IntPtr.Zero, 0, size, p) Then
                            Dim [error] As Integer = Marshal.GetLastWin32Error()
                            If [error] <> Native.ERROR_INSUFFICIENT_BUFFER Then
                                Throw New Win32Exception([error])
                            End If
                        End If
                        Dim detailDataBuffer As New Native.SP_DEVICE_INTERFACE_DETAIL_DATA()
                        If IntPtr.Size = 8 Then
                            ' for 64 bit operating systems
                            detailDataBuffer.cbSize = 8
                        Else
                            ' for 32 bit systems
                            detailDataBuffer.cbSize = 4 + Marshal.SystemDefaultCharSize
                        End If
                        Dim pBuf As IntPtr = Marshal.AllocHGlobal(Marshal.SizeOf(detailDataBuffer))

                        Marshal.StructureToPtr(detailDataBuffer, pBuf, True)

                        If Not Native.SetupDiGetDeviceInterfaceDetail(_deviceInfoSet, interfaceData, pBuf, size, size, p) Then
                            Dim [error] As Integer = Marshal.GetLastWin32Error()
                            If [error] <> Native.ERROR_INSUFFICIENT_BUFFER Then
                                Throw New Win32Exception([error])
                            End If
                        End If
                        devData = DirectCast(Marshal.PtrToStructure(p, GetType(Native.SP_DEVINFO_DATA)), Native.SP_DEVINFO_DATA)
                        Marshal.FreeHGlobal(p)

                        detailDataBuffer = DirectCast(Marshal.PtrToStructure(pBuf, GetType(Native.SP_DEVICE_INTERFACE_DETAIL_DATA)), Native.SP_DEVICE_INTERFACE_DETAIL_DATA)
                        Marshal.FreeHGlobal(pBuf)

                        Dim devicePath As String = detailDataBuffer.DevicePath
                        Dim storageDeviceNumber As Native.STORAGE_DEVICE_NUMBER = GetDeviceNumber(devicePath)
                        Dim device As Device = CreateDevice(Me, devData, devicePath, storageDeviceNumber.DeviceNumber)
                        _devices.Add(device)

                        index += 1
                    End While
                    _devices.Sort()
                End If
                Return _devices
            End Get
        End Property

        Friend Function GetInfo(dnDevInst As Integer) As Native.SP_DEVINFO_DATA
            Dim sb As New StringBuilder(1024)
            Dim hr As Integer = Native.CM_Get_Device_ID(dnDevInst, sb, sb.Capacity, 0)
            If hr <> 0 Then
                Throw New Win32Exception(hr)
            End If

            Dim devData As New Native.SP_DEVINFO_DATA()
            devData.cbSize = Marshal.SizeOf(GetType(Native.SP_DEVINFO_DATA))
            If Not Native.SetupDiOpenDeviceInfo(_deviceInfoSet, sb.ToString(), IntPtr.Zero, 0, devData) Then
                Throw New Win32Exception(Marshal.GetLastWin32Error())
            End If

            Return devData
        End Function

        Friend Function GetProperty(devData As Native.SP_DEVINFO_DATA, [property] As Integer, defaultValue As String) As String
            If devData Is Nothing Then
                Throw New ArgumentNullException("devData")
            End If

            Dim propertyRegDataType As Integer = 0
            Dim requiredSize As Integer
            Dim propertyBufferSize As Integer = 1024

            Dim propertyBuffer As IntPtr = Marshal.AllocHGlobal(propertyBufferSize)
            If Not Native.SetupDiGetDeviceRegistryProperty(_deviceInfoSet, devData, [property], propertyRegDataType, propertyBuffer, propertyBufferSize, _
                requiredSize) Then
                Marshal.FreeHGlobal(propertyBuffer)
                Dim [error] As Integer = Marshal.GetLastWin32Error()
                If [error] <> Native.ERROR_INVALID_DATA Then
                    Throw New Win32Exception([error])
                End If
                Return defaultValue
            End If

            Dim value As String = Marshal.PtrToStringAuto(propertyBuffer)
            Marshal.FreeHGlobal(propertyBuffer)
            Return value
        End Function

        Friend Function GetProperty(devData As Native.SP_DEVINFO_DATA, [property] As Integer, defaultValue As Integer) As Integer
            If devData Is Nothing Then
                Throw New ArgumentNullException("devData")
            End If

            Dim propertyRegDataType As Integer = 0
            Dim requiredSize As Integer
            Dim propertyBufferSize As Integer = Marshal.SizeOf(GetType(Integer))

            Dim propertyBuffer As IntPtr = Marshal.AllocHGlobal(propertyBufferSize)
            If Not Native.SetupDiGetDeviceRegistryProperty(_deviceInfoSet, devData, [property], propertyRegDataType, propertyBuffer, propertyBufferSize, _
                requiredSize) Then
                Marshal.FreeHGlobal(propertyBuffer)
                Dim [error] As Integer = Marshal.GetLastWin32Error()
                If [error] <> Native.ERROR_INVALID_DATA Then
                    Throw New Win32Exception([error])
                End If
                Return defaultValue
            End If

            Dim value As Integer = CInt(Marshal.PtrToStructure(propertyBuffer, GetType(Integer)))
            Marshal.FreeHGlobal(propertyBuffer)
            Return value
        End Function

        Friend Function GetProperty(devData As Native.SP_DEVINFO_DATA, [property] As Integer, defaultValue As Guid) As Guid
            If devData Is Nothing Then
                Throw New ArgumentNullException("devData")
            End If

            Dim propertyRegDataType As Integer = 0
            Dim requiredSize As Integer
            Dim propertyBufferSize As Integer = Marshal.SizeOf(GetType(Guid))

            Dim propertyBuffer As IntPtr = Marshal.AllocHGlobal(propertyBufferSize)
            If Not Native.SetupDiGetDeviceRegistryProperty(_deviceInfoSet, devData, [property], propertyRegDataType, propertyBuffer, propertyBufferSize, _
                requiredSize) Then
                Marshal.FreeHGlobal(propertyBuffer)
                Dim [error] As Integer = Marshal.GetLastWin32Error()
                If [error] <> Native.ERROR_INVALID_DATA Then
                    Throw New Win32Exception([error])
                End If
                Return defaultValue
            End If

            Dim value As Guid = DirectCast(Marshal.PtrToStructure(propertyBuffer, GetType(Guid)), Guid)
            Marshal.FreeHGlobal(propertyBuffer)
            Return value
        End Function
        Friend Function GetDeviceNumber(devicePath As String) As Native.STORAGE_DEVICE_NUMBER
            Dim hFile As IntPtr = Native.CreateFile(devicePath.TrimEnd("\"c), 0, 0, IntPtr.Zero, Native.OPEN_EXISTING, 0, _
                IntPtr.Zero)
            If hFile.ToInt32() = Native.INVALID_HANDLE_VALUE Then
                Throw New Win32Exception(Marshal.GetLastWin32Error())
            End If
            Dim bytesReturned As Integer
            Dim storageDeviceNumber As New Native.STORAGE_DEVICE_NUMBER()
            Dim size As Integer = Marshal.SizeOf(storageDeviceNumber)
            Dim buffer As IntPtr = Marshal.AllocHGlobal(size)
            Try
                ' do nothing here on purpose
                If Not Native.DeviceIoControl(hFile, Native.IOCTL_STORAGE_GET_DEVICE_NUMBER, IntPtr.Zero, 0, buffer, size, _
                    bytesReturned, IntPtr.Zero) Then
                End If
            Finally
                Native.CloseHandle(hFile)
            End Try
            If bytesReturned > 0 Then
                storageDeviceNumber = DirectCast(Marshal.PtrToStructure(buffer, GetType(Native.STORAGE_DEVICE_NUMBER)), Native.STORAGE_DEVICE_NUMBER)
            End If
            Marshal.FreeHGlobal(buffer)
            Return storageDeviceNumber
        End Function

    End Class
End Namespace

