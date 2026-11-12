namespace DeviceInfoLibrary.Results;

public record class BaseResult
    (
        bool Success,
        string Message,
        string Error,
        int StatusCode
    );