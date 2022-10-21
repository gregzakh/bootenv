using System;
using System.Runtime.InteropServices;

namespace BootEnvironment {
  internal static class NativeMethods {
    internal const Int32 STATUS_SUCCESS = 0;
    internal const Int32 SystemBootEnvironmentInformation = 90;

    [StructLayout(LayoutKind.Sequential)]
    internal struct GUID {
      internal UInt32 Data1;
      internal UInt16 Data2;
      internal UInt16 Data3;
      [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
      internal Byte[] Data4;

      internal String Guid {
        get {
          return Program.StrMsg(
            "{{{0:X}-{1:X}-{2:X}-{3}}}", this.Data1, this.Data2, this.Data3,
            BitConverter.ToString(this.Data4).Replace("-", "").Insert(4, "-")
          );
        }
      }
    } // GUID

    internal enum FIRMWARE_TYPE : uint {
      FirmwareTypeUnknown = 0,
      FirmwareTypeBios = 1,
      FirmwareTypeUefi = 2,
      FirmwareTypeMax = 3
    } // FIRMWARE_TYPE

    /* [StructLayout(LayoutKind.Explicit, Size = 8)]
    internal struct BOOT_FLAGS {
      [FieldOffset(0)]internal UInt64 DbgMenuOsSelection;
      [FieldOffset(0)]internal UInt64 DbgHiberBoot;
      [FieldOffset(0)]internal UInt64 DbgSoftBoot;
      [FieldOffset(0)]internal UInt64 DbgMeasuredLaunch;
      [FieldOffset(0)]internal UInt64 DbgMeasuredLaunchCapable;
      [FieldOffset(0)]internal UInt64 DbgSystemHiveReplace;
      [FieldOffset(0)]internal UInt64 DbgMeasuredLaunchSmmProtections;
      [FieldOffset(0)]internal UInt64 DbgMeasuredLaunchSmmLevel;
    } // BOOT_FLAGS */

    [StructLayout(LayoutKind.Sequential)]
    internal struct SYSTEM_BOOT_ENVIRONMENT_INFORMATION {
      internal GUID BootIdentifier;
      internal FIRMWARE_TYPE FirmwareType;
      internal /* BOOT_FLAGS */ UInt64 BootFlags;

      internal Int32 Size {
        get { return Marshal.SizeOf(this); }
      }
    } // SYSTEM_BOOT_ENVIRONMENT_INFORMATION

    [DllImport("ntdll.dll")]
    internal static extern Int32 NtQuerySystemInformation(
       Int32 SystemInformationClass,
       out SYSTEM_BOOT_ENVIRONMENT_INFORMATION SystemInformation,
       Int32 SystemInformationLength,
       Byte[] ReturnLength // out UInt32 ReturnLength
    );

    [DllImport("ntdll.dll")]
    internal static extern Int32 RtlNtStatusToDosError(
       Int32 Status
    );

    internal static Boolean IsWin8OrHigher() {
      Int32[] ver = new Int32[2];
      Marshal.Copy((IntPtr)0x7FFE026C, ver, 0, ver.Length);
      return (ver[0] * 10 + ver[1]) >= 80;
    }
  } // NativeMethods
}
