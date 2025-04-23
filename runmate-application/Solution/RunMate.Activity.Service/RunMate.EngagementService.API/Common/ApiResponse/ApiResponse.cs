namespace RunMate.EngagementService.RunMate.EngagementService.API.Common.ApiResponse
{
    public class ApiResponse<T>(T data, bool success = true, string? message = null, object? errors = null)
    {
        public bool Success { get; set; } = success;
        public string? Message { get; set; } = message;
        public T Data { get; set; } = data;
        public object? Errors { get; set; } = errors;

        public static ApiResponse<T> SuccessResponse(T data, string? message = null)
        {
            return new ApiResponse<T>(data, true, message);
        }

        public static ApiResponse<T> ErrorResponse(string message, object? errors = null)
        {
            return new ApiResponse<T>(default, false, message, errors);
        }
    }
}
