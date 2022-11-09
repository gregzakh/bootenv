using System;
using System.IO;
using System.Linq;
using System.Globalization;
using System.ComponentModel;

namespace BootEnvironment {
  internal sealed class Program {
    internal static String StrMsg(String msg, params Object[] list) {
      return String.Format(CultureInfo.InvariantCulture, msg, list);
    }

    static void Main() {
      // in Linux the approach is something like this
      // Directory.Exists("/sys/firmware/efi") ? "UEFI" : "Legacy";
      if (!NativeMethods.IsWin8OrHigher()) {
        Console.WriteLine(StrMsg("Legacy operating system (seems {0}).",
          new String[] {"Legacy", "UEFI"} [File.Exists(
            Path.Combine(Environment.SystemDirectory, "winload.efi")
          ) ? 1 : 0]
        ));
        return;
      }

      // seems a great shot, but we aren't looking for easy ways...
      // Console.WriteLine(Environment.GetEnvironmentVariable("FIRMWARE_TYPE"));
      var bei = new NativeMethods.SYSTEM_BOOT_ENVIRONMENT_INFORMATION();
      var nts = NativeMethods.NtQuerySystemInformation(
        NativeMethods.SystemBootEnvironmentInformation, out bei, bei.Size, null
      );

      if (NativeMethods.STATUS_SUCCESS != nts) {
        Console.WriteLine(new Win32Exception(
          NativeMethods.RtlNtStatusToDosError(nts)
        ).Message);
        return;
      }

      Console.WriteLine(StrMsg("Boot identifier: {0}", bei.BootIdentifier.Guid));
      Console.WriteLine(StrMsg("Firmware type:   {0}", bei.FirmwareType));
    }
  } // Program
}
