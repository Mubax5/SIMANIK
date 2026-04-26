namespace SIMANIK.Services
{
    public class ServiceResult
    {
        public bool Success { get; private set; }
        public string Message { get; private set; }

        protected ServiceResult(bool success, string message)
        {
            Success = success;
            Message = message;
        }

        public static ServiceResult Ok(string message)
        {
            return new ServiceResult(true, message);
        }

        public static ServiceResult Fail(string message)
        {
            return new ServiceResult(false, message);
        }
    }
}
