using DeviceInfoLibrary.Results;
using System.Runtime.Intrinsics.Arm;
using System.Runtime.Intrinsics.X86;
using System.Text;

namespace DeviceInfoLibrary;

internal class CPUIdService
{
    public static Result GetCPUId()
    {

        if (X86Base.IsSupported)
        {
            var brandString = new StringBuilder();

            for (uint i = 0x80000002; i <= 0x80000004; i++)
            {
                var (eax, ebx, ecx, edx) = X86Base.CpuId((int)i, 0);

                brandString.Append(Encoding.ASCII.GetString(BitConverter.GetBytes(eax)));
                brandString.Append(Encoding.ASCII.GetString(BitConverter.GetBytes(ebx)));
                brandString.Append(Encoding.ASCII.GetString(BitConverter.GetBytes(ecx)));
                brandString.Append(Encoding.ASCII.GetString(BitConverter.GetBytes(edx)));
            }

            return Result.OkResult(brandString.ToString().Trim());
        }
        else if (ArmBase.IsSupported)
        {
            return Result.ErrorResult("Unsupported CPU");
        }
        return Result.ErrorResult("Unsupported CPU");
    }
}
