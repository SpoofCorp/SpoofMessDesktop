using DeviceInfoLibrary.Results;
using Microsoft.Win32;

namespace DeviceInfoLibrary;

public static class WindowsIdReader
{
    public static Result GetMachineGuid()
    {
        if (!OperatingSystem.IsWindows()) return Result.ErrorResult("Unsupported OS");
        using RegistryKey baseKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64);
        using RegistryKey? cryptoKey = baseKey.OpenSubKey(@"SOFTWARE\Microsoft\Cryptography");

        string? guidString = cryptoKey?.GetValue("MachineGuid")?.ToString();

        return Guid.TryParse(guidString, out Guid result) ? Result.OkResult(result.ToString()) : Result.ErrorResult("Invalid Machine UUID");
    }
}
