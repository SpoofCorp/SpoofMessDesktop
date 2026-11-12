using DeviceInfoLibrary.Results;
using System.Runtime.InteropServices;

namespace DeviceInfoLibrary;

public static partial class SmbiosUuidReader
{
    private const uint RSMB = 0x52534D42;

    [LibraryImport("kernel32.dll", SetLastError = true)]
    private static partial uint GetSystemFirmwareTable(
        uint firmwareTableProviderSignature,
        uint firmwareTableID,
        IntPtr pFirmwareTableBuffer,
        uint bufferSize);

    public static Result<Guid> GetMotherboardUuid()
    {
        uint size = GetSystemFirmwareTable(RSMB, 0, IntPtr.Zero, 0);
        if (size == 0) return Result<Guid>.ErrorResult("Failed to obtain SMBIOS size");

        IntPtr buffer = Marshal.AllocHGlobal((int)size);
        try
        {
            if (GetSystemFirmwareTable(RSMB, 0, buffer, size) != size)
                return Result<Guid>.ErrorResult("SMBIOS read error.");

            byte[] data = new byte[size];
            Marshal.Copy(buffer, data, 0, (int)size);

            int offset = 8;
            while (offset + 4 < data.Length)
            {
                byte type = data[offset];
                byte length = data[offset + 1];

                if (type == 1 && length >= 24 && offset + 24 <= data.Length)
                    return Result<Guid>.OkResult(new Guid(data.AsSpan(offset + 8, 16)));

                offset += length;

                while (offset + 1 < data.Length && !(data[offset] == 0 && data[offset + 1] == 0))
                    offset++;

                offset += 2;
            }
        }
        finally
        {
            Marshal.FreeHGlobal(buffer);
        }

        return Result<Guid>.ErrorResult("Undefined Motherboard");
    }
}