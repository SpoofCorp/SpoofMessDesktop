using DeviceInfoLibrary;
using DeviceInfoLibrary.Results;

Result CPU = CPUIdService.GetCPUId();

var info = GC.GetGCMemoryInfo();
long totalBytes = info.TotalAvailableMemoryBytes;
Console.WriteLine($"{totalBytes / (1024.0 * 1024.0 * 1024.0)}Gb");
Console.WriteLine($"MotherBoard UUID: {SmbiosUuidReader.GetMotherboardUuid()}");
Console.WriteLine($"System UUID: {WindowsIdReader.GetMachineGuid()}");
Result mac = HardwareMacReader.GetPhysicalAdaptersMac();
Console.ReadKey();