Imports System.ComponentModel
Imports System.Runtime.InteropServices
Imports System.Text

Namespace UsbEject
    ''' <summary>
    ''' A volume device.
    ''' </summary>
    Public Class Volume
        Inherits Device
        'Implements IComparable
        Private _volumeName As String
        Private _logicalDrive As String
        Private _diskNumbers As Integer()
        Private _disks As List(Of Device)
        Private _removableDevices As List(Of Device)

        Friend Sub New(deviceClass As DeviceClass, deviceInfoData As Native.SP_DEVINFO_DATA, path As String, index As Integer)
            MyBase.New(deviceClass, deviceInfoData, path, index)
        End Sub

        ''' <summary>
        ''' Gets the volume's name.
        ''' </summary>
        Public ReadOnly Property VolumeName() As String
            Get
                If _volumeName Is Nothing Then
                    Dim sb As New StringBuilder(1024)
                    ' throw new Win32Exception(Marshal.GetLastWin32Error());

                    If Not Native.GetVolumeNameForVolumeMountPoint(Path + "\", sb, sb.Capacity) Then
                    End If

                    If sb.Length > 0 Then
                        _volumeName = sb.ToString()
                    End If
                End If
                Return _volumeName
            End Get
        End Property

        ''' <summary>
        ''' Gets the volume's logical drive in the form [letter]:\
        ''' </summary>
        Public ReadOnly Property LogicalDrive() As String
            Get
                If (_logicalDrive Is Nothing) AndAlso (VolumeName IsNot Nothing) Then
                    DirectCast(DeviceClass, VolumeDeviceClass)._logicalDrives.TryGetValue(VolumeName, _logicalDrive)
                End If
                Return _logicalDrive
            End Get
        End Property

        ''' <summary>
        ''' Gets a value indicating whether this volume is a based on USB devices.
        ''' </summary>
        Public Overrides ReadOnly Property IsUsb() As Boolean
            Get
                If Disks IsNot Nothing Then
                    For Each disk As Device In Disks
                        If disk.IsUsb Then
                            Return True
                        End If
                    Next
                End If
                Return False
            End Get
        End Property

        ''' <summary>
        ''' Gets a list of underlying disks for this volume.
        ''' </summary>
        Public ReadOnly Property Disks() As List(Of Device)
            Get
                If _disks Is Nothing Then
                    _disks = New List(Of Device)()

                    If DiskNumbers IsNot Nothing Then
                        Dim disks__1 As New DiskDeviceClass()
                        For Each index As Integer In DiskNumbers
                            If index < disks__1.Devices.Count Then
                                _disks.Add(disks__1.Devices(index))
                            End If
                        Next
                    End If
                End If
                Return _disks
            End Get
        End Property

        Private ReadOnly Property DiskNumbers() As Integer()
            Get
                If _diskNumbers Is Nothing Then
                    Dim numbers As New List(Of Integer)()
                    If LogicalDrive IsNot Nothing Then

                        'IntPtr hFile = Native.CreateFile(@"\\.\" + LogicalDrive, Native.GENERIC_READ, Native.FILE_SHARE_READ | Native.FILE_SHARE_WRITE, IntPtr.Zero, Native.OPEN_EXISTING, 0, IntPtr.Zero);
                        Dim hFile As IntPtr = Native.CreateFile(Convert.ToString("\\.\") & LogicalDrive, 0, Native.FILE_SHARE_READ Or Native.FILE_SHARE_WRITE, IntPtr.Zero, Native.OPEN_EXISTING, 0, _
                            IntPtr.Zero)
                        If hFile.ToInt32() = Native.INVALID_HANDLE_VALUE Then
                            Throw New Win32Exception(Marshal.GetLastWin32Error())
                        End If

                        Dim size As Integer = &H400
                        ' some big size
                        Dim buffer As IntPtr = Marshal.AllocHGlobal(size)
                        Dim bytesReturned As Integer = 0
                        Try
                            ' do nothing here on purpose
                            If Not Native.DeviceIoControl(hFile, Native.IOCTL_VOLUME_GET_VOLUME_DISK_EXTENTS, IntPtr.Zero, 0, buffer, size, _
                                bytesReturned, IntPtr.Zero) Then
                            End If
                        Finally
                            Native.CloseHandle(hFile)
                        End Try

                        If bytesReturned > 0 Then
                            Dim numberOfDiskExtents As Integer = CInt(Marshal.PtrToStructure(buffer, GetType(Integer)))
                            For i As Integer = 0 To numberOfDiskExtents - 1
                                Dim extentPtr As New IntPtr(buffer.ToInt64() + Marshal.SizeOf(GetType(Long)) + i * Marshal.SizeOf(GetType(Native.DISK_EXTENT)))
                                Dim extent As Native.DISK_EXTENT = DirectCast(Marshal.PtrToStructure(extentPtr, GetType(Native.DISK_EXTENT)), Native.DISK_EXTENT)
                                numbers.Add(extent.DiskNumber)
                            Next
                        End If
                        Marshal.FreeHGlobal(buffer)
                    End If

                    _diskNumbers = New Integer(numbers.Count - 1) {}
                    numbers.CopyTo(_diskNumbers)
                End If
                Return _diskNumbers
            End Get
        End Property

        ''' <summary>
        ''' Gets a list of removable devices for this volume.
        ''' </summary>
        Public Overrides ReadOnly Property RemovableDevices() As List(Of Device)
            Get
                If _removableDevices Is Nothing Then
                    _removableDevices = New List(Of Device)()
                    If Disks Is Nothing Then
                        _removableDevices = MyBase.RemovableDevices
                    Else
                        For Each disk As Device In Disks
                            For Each device As Device In disk.RemovableDevices
                                _removableDevices.Add(device)
                            Next
                        Next
                    End If
                End If
                Return _removableDevices
            End Get
        End Property

        ''' <summary>
        ''' Compares the current instance with another object of the same type.
        ''' </summary>
        ''' <param name="obj">An object to compare with this instance.</param>
        ''' <returns>A 32-bit signed integer that indicates the relative order of the comparands.</returns>
        Public Overrides Function CompareTo(obj As Object) As Integer
            Dim device As Volume = TryCast(obj, Volume)
            If device Is Nothing Then
                Throw New ArgumentException()
            End If

            If LogicalDrive Is Nothing Then
                Return 1
            End If

            If device.LogicalDrive Is Nothing Then
                Return -1
            End If

            Return LogicalDrive.CompareTo(device.LogicalDrive)
        End Function
    End Class
End Namespace
