using DeviceInfoLibrary.Results;
using Microsoft.Win32;
using Microsoft.Win32.SafeHandles;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;

namespace DeviceInfoLibrary;

public static partial class HardwareMacReader
{
    private const uint IOCTL_NDIS_QUERY_GLOBAL_STATS = 0x00170002;
    private const uint OID_802_3_PERMANENT_ADDRESS = 0x01010101;
    private const uint OID_802_3_CURRENT_ADDRESS = 0x01010102;

    [LibraryImport("kernel32.dll", EntryPoint = "CreateFileW", SetLastError = true, StringMarshalling = StringMarshalling.Utf16)]
    private static partial SafeFileHandle CreateFile(
        string lpFileName,
        uint dwDesiredAccess,
        uint dwShareMode,
        IntPtr lpSecurityAttributes,
        uint dwCreationDisposition,
        uint dwFlagsAndAttributes,
        IntPtr hTemplateFile);

    [LibraryImport("kernel32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool DeviceIoControl(
        SafeFileHandle hDevice,
        uint dwIoControlCode,
        ref uint lpInBuffer,
        uint nInBufferSize,
        byte[] lpOutBuffer,
        uint nOutBufferSize,
        out uint lpBytesReturned,
        IntPtr lpOverlapped);

    public static Result GetPhysicalAdaptersMac()
    {
        if (!OperatingSystem.IsWindows()) return Result.ErrorResult("Unsupported OS");
        var result = new List<(string, string, string, bool)>();

        foreach (NetworkInterface nic in NetworkInterface.GetAllNetworkInterfaces())
        {
            if (nic.NetworkInterfaceType != NetworkInterfaceType.Ethernet &&
                nic.NetworkInterfaceType != NetworkInterfaceType.Wireless80211)
                continue;

            if (!IsHardwareBusAdapter(nic.Id))
                continue;

            using SafeFileHandle hAdapter = CreateFile($@"\\.\{nic.Id}", 0, 3, IntPtr.Zero, 3, 0, IntPtr.Zero);
            if (hAdapter.IsInvalid) continue;

            byte[] permBytes = new byte[6];
            byte[] currBytes = new byte[6];

            uint oidPerm = OID_802_3_PERMANENT_ADDRESS;
            uint oidCurr = OID_802_3_CURRENT_ADDRESS;

            bool hasPerm = DeviceIoControl(hAdapter, IOCTL_NDIS_QUERY_GLOBAL_STATS, ref oidPerm, sizeof(uint), permBytes, 6, out uint ret1, IntPtr.Zero) && ret1 == 6;
            bool hasCurr = DeviceIoControl(hAdapter, IOCTL_NDIS_QUERY_GLOBAL_STATS, ref oidCurr, sizeof(uint), currBytes, 6, out uint ret2, IntPtr.Zero) && ret2 == 6;

            if (hasPerm)
            {
                string permMac = BitConverter.ToString(permBytes);
                string currMac = hasCurr ? BitConverter.ToString(currBytes) : permMac;
                bool isSpoofed = !string.Equals(permMac, currMac, StringComparison.OrdinalIgnoreCase);

                result.Add((nic.Description, permMac, currMac, isSpoofed));
            }
        }

        return result.Count > 0 ? Result.OkResult(result[0].Item2) : Result.ErrorResult("Undefined Ethernet card");
    }
    private static bool IsHardwareBusAdapter(string adapterGuid)
    {
        if (!OperatingSystem.IsWindows()) return false;
        const string classKeyPath = @"SYSTEM\CurrentControlSet\Control\Class\{4d36e972-e325-11ce-bfc1-08002be10318}";
        using RegistryKey? baseKey = Registry.LocalMachine.OpenSubKey(classKeyPath);
        if (baseKey == null) return false;

        foreach (string subKeyName in baseKey.GetSubKeyNames())
        {
            if (subKeyName == "Properties") continue;
            using RegistryKey? subKey = baseKey.OpenSubKey(subKeyName);
            string? netCfgInstanceId = subKey?.GetValue("NetCfgInstanceId")?.ToString();

            if (string.Equals(netCfgInstanceId, adapterGuid, StringComparison.OrdinalIgnoreCase))
            {
                string? instanceId = subKey?.GetValue("DeviceInstanceID")?.ToString();
                return instanceId != null &&
                       (instanceId.StartsWith("PCI\\", StringComparison.OrdinalIgnoreCase) ||
                        instanceId.StartsWith("USB\\", StringComparison.OrdinalIgnoreCase));
            }
        }
        return false;
    }
}