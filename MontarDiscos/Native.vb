Imports System.Runtime.InteropServices
Imports System.Text

Namespace UsbEject
    Friend NotInheritable Class Native
        ' from winuser.h
        Friend Const WM_DEVICECHANGE As Integer = &H219

        ' from winbase.h
        Friend Const INVALID_HANDLE_VALUE As Integer = -1
        Friend Const GENERIC_READ As Integer = &H80000000
        Friend Const FILE_SHARE_READ As Integer = &H1
        Friend Const FILE_SHARE_WRITE As Integer = &H2
        Friend Const OPEN_EXISTING As Integer = 3

        <DllImport("kernel32", CharSet:=CharSet.Auto, SetLastError:=True)> _
        Friend Shared Function GetVolumeNameForVolumeMountPoint(volumeName As String, uniqueVolumeName As StringBuilder, uniqueNameBufferCapacity As Integer) As Boolean
        End Function

        <DllImport("Kernel32.dll", SetLastError:=True)> _
        Friend Shared Function CreateFile(lpFileName As String, dwDesiredAccess As Integer, dwShareMode As Integer, lpSecurityAttributes As IntPtr, dwCreationDisposition As Integer, dwFlagsAndAttributes As Integer, _
            hTemplateFile As IntPtr) As IntPtr
        End Function

        <DllImport("Kernel32.dll", SetLastError:=True)> _
        Friend Shared Function DeviceIoControl(hDevice As IntPtr, dwIoControlCode As Integer, lpInBuffer As IntPtr, nInBufferSize As Integer, lpOutBuffer As IntPtr, nOutBufferSize As Integer, _
            ByRef lpBytesReturned As Integer, lpOverlapped As IntPtr) As Boolean
        End Function

        <DllImport("Kernel32.dll", SetLastError:=True)> _
        Friend Shared Function CloseHandle(hObject As IntPtr) As Boolean
        End Function

        ' from winerror.h
        Friend Const ERROR_NO_MORE_ITEMS As Integer = 259
        Friend Const ERROR_INSUFFICIENT_BUFFER As Integer = 122
        Friend Const ERROR_INVALID_DATA As Integer = 13

        ' from winioctl.h
        Friend Const GUID_DEVINTERFACE_VOLUME As String = "53f5630d-b6bf-11d0-94f2-00a0c91efb8b"
        Friend Const GUID_DEVINTERFACE_DISK As String = "53f56307-b6bf-11d0-94f2-00a0c91efb8b"
        Friend Const IOCTL_VOLUME_GET_VOLUME_DISK_EXTENTS As Integer = &H560000

        <StructLayout(LayoutKind.Sequential)> _
        Friend Structure DISK_EXTENT
            Friend DiskNumber As Integer
            Friend StartingOffset As Long
            Friend ExtentLength As Long
        End Structure

        ' from cfg.h
        Friend Enum PNP_VETO_TYPE
            Ok

            TypeUnknown
            LegacyDevice
            PendingClose
            WindowsApp
            WindowsService
            OutstandingOpen
            Device
            Driver
            IllegalDeviceRequest
            InsufficientPower
            NonDisableable
            LegacyDriver
        End Enum

        ' from cfgmgr32.h
        <DllImport("setupapi.dll")> _
        Friend Shared Function CM_Get_Parent(ByRef pdnDevInst As Integer, dnDevInst As Integer, ulFlags As Integer) As Integer
        End Function

        <DllImport("setupapi.dll")> _
        Friend Shared Function CM_Get_Device_ID(dnDevInst As Integer, buffer As StringBuilder, bufferLen As Integer, ulFlags As Integer) As Integer
        End Function

        <DllImport("setupapi.dll")> _
        Friend Shared Function CM_Request_Device_Eject(dnDevInst As Integer, ByRef pVetoType As PNP_VETO_TYPE, pszVetoName As StringBuilder, ulNameLength As Integer, ulFlags As Integer) As Integer
        End Function

        <DllImport("setupapi.dll", EntryPoint:="CM_Request_Device_Eject")> _
        Friend Shared Function CM_Request_Device_Eject_NoUi(dnDevInst As Integer, pVetoType As IntPtr, pszVetoName As StringBuilder, ulNameLength As Integer, ulFlags As Integer) As Integer
        End Function

        ' from setupapi.h
        Friend Const DIGCF_PRESENT As Integer = (&H2)
        Friend Const DIGCF_DEVICEINTERFACE As Integer = (&H10)

        Friend Const SPDRP_DEVICEDESC As Integer = &H0
        Friend Const SPDRP_CAPABILITIES As Integer = &HF
        Friend Const SPDRP_CLASS As Integer = &H7
        Friend Const SPDRP_CLASSGUID As Integer = &H8
        Friend Const SPDRP_FRIENDLYNAME As Integer = &HC

        <StructLayout(LayoutKind.Sequential)> _
        Friend Class SP_DEVINFO_DATA
            Friend cbSize As Integer = Marshal.SizeOf(GetType(SP_DEVINFO_DATA))
            Friend classGuid As Guid = Guid.Empty
            ' temp
            Friend devInst As Integer = 0
            ' dumy
            Friend reserved As IntPtr = IntPtr.Zero
        End Class


        <StructLayout(LayoutKind.Sequential, CharSet:=CharSet.Auto)> _
        Friend Structure SP_DEVICE_INTERFACE_DETAIL_DATA
            Friend cbSize As Int32
            <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=256)> _
            Friend DevicePath As String
        End Structure


        <StructLayout(LayoutKind.Sequential)> _
        Friend Structure SP_DEVICE_INTERFACE_DATA
            Friend cbSize As UInt32
            Friend interfaceClassGuid As Guid
            Friend flags As UInt32
            Friend reserved As UIntPtr
        End Structure

        <DllImport("setupapi.dll")> _
        Friend Shared Function SetupDiGetClassDevs(ByRef classGuid As Guid, enumerator As Integer, hwndParent As IntPtr, flags As Integer) As IntPtr
        End Function

        <DllImport("setupapi.dll", SetLastError:=True, CharSet:=CharSet.Auto)> _
        Friend Shared Function SetupDiEnumDeviceInterfaces(deviceInfoSet As IntPtr, deviceInfoData As IntPtr, ByRef interfaceClassGuid As Guid, memberIndex As UInt32, ByRef deviceInterfaceData As SP_DEVICE_INTERFACE_DATA) As Boolean
        End Function

        <DllImport("setupapi.dll")> _
        Friend Shared Function SetupDiOpenDeviceInfo(deviceInfoSet As IntPtr, deviceInstanceId As String, hwndParent As IntPtr, openFlags As Integer, deviceInfoData As SP_DEVINFO_DATA) As Boolean
        End Function

        <DllImport("setupapi.dll", SetLastError:=True, CharSet:=CharSet.Auto)> _
        Friend Shared Function SetupDiGetDeviceInterfaceDetail(deviceInfoSet As IntPtr, ByRef deviceInterfaceData As SP_DEVICE_INTERFACE_DATA, deviceInterfaceDetailData As IntPtr, deviceInterfaceDetailDataSize As UInt32, ByRef requiredSize As UInt32, deviceInfoData As IntPtr) As Boolean
        End Function


        <DllImport("setupapi.dll", CharSet:=CharSet.Auto, SetLastError:=True)> _
        Friend Shared Function SetupDiGetDeviceRegistryProperty(deviceInfoSet As IntPtr, deviceInfoData As SP_DEVINFO_DATA, [property] As Integer, ByRef propertyRegDataType As Integer, propertyBuffer As IntPtr, propertyBufferSize As Integer, _
            ByRef requiredSize As Integer) As Boolean
        End Function

        <DllImport("setupapi.dll")> _
        Friend Shared Function SetupDiDestroyDeviceInfoList(deviceInfoSet As IntPtr) As UInteger
        End Function

        <StructLayout(LayoutKind.Sequential)> _
        Friend Structure STORAGE_DEVICE_NUMBER
            Public DeviceType As Integer
            Public DeviceNumber As Integer
            Public PartitionNumber As Integer
        End Structure

        Friend Const IOCTL_STORAGE_GET_DEVICE_NUMBER As Integer = &H2D1080

        Private Sub New()
        End Sub
    End Class
End Namespace
