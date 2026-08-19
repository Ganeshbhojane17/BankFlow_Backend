namespace CustomerService.Shared
{
    public class Result
    {
        public bool Success { get; }

        public string Message { get; }

        public List<string> Errors { get; }

        protected Result(bool success, string message, List<string>? errors = null)
        {
            Success = success;
            Message = message;
            Errors = errors ?? new();
        }

        public static Result Ok(string message = "Success")
            => new(true, message);

        public static Result Failure(string message, List<string>? errors = null)
            => new(false, message, errors);
    }

    public class Result<T> : Result
    {
        public T? Data { get; }

        private Result(bool success, T? data, string message, List<string>? errors = null)
            : base(success, message, errors)
        {
            Data = data;
        }

        public static Result<T> Ok(T data, string message = "Success")
            => new(true, data, message);

        public static Result<T> Failure(string message, List<string>? errors = null)
            => new(false, default, message, errors);
    }
}
