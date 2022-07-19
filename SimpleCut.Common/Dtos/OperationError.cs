namespace SimpleCut.Common.Dtos
{
    public class OperationError
    {
        public string Error { get; }
        public string? Key { get; }

        public OperationError(string error, string? key = null)
        {
            Error = error;
            Key = key;
        }
    }
}
