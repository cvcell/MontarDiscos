Namespace UsbEject
    ''' <summary>
    ''' Contains constants for determining devices capabilities.
    ''' This enumeration has a FlagsAttribute attribute that allows a bitwise combination of its member values.
    ''' </summary>
    <Flags>
    Public Enum DeviceCapabilities
        Unknown = &H0
        ' matches cfmgr32.h CM_DEVCAP_* definitions

        LockSupported = &H1
        EjectSupported = &H2
        Removable = &H4
        DockDevice = &H8
        UniqueId = &H10
        SilentInstall = &H20
        RawDeviceOk = &H40
        SurpriseRemovalOk = &H80
        HardwareDisabled = &H100
        NonDynamic = &H200
    End Enum
End Namespace
