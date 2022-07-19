namespace SimpleCut.Common.Dtos
{
    public class OperationResult<T> : OperationResult
    {
        public T? Result { get; set; }

        public OperationResult(T result)
        {
            Result = result;
        }

        public OperationResult() => Result = default;
    }

    public class OperationResult
    {
        private readonly List<OperationError> errors = new List<OperationError>();

        public IEnumerable<OperationError> Errors => errors.AsEnumerable();
        public bool Success => !Errors.Any();

        public OperationResult()
        { }

        public OperationResult(string error, string? key = null)
        {
            AddError(error, key);
        }

        public OperationResult(OperationError error)
        {
            AddError(error);
        }

        public OperationResult(IEnumerable<OperationError> errors)
        {
            AddErrors(errors);
        }

        public OperationResult(OperationResult operationResult)
        {
            AddErrors(operationResult);
        }

        public OperationResult(IEnumerable<OperationResult> operationResults)
        {
            AddErrors(operationResults);
        }

        public void AddError(string reason, string? key = null)
        {
            errors.Add(new OperationError(reason, key));
        }

        public void AddError(OperationError reason)
        {
            errors.Add(reason);
        }

        public void AddErrors(IEnumerable<OperationError> errors)
        {
            this.errors.AddRange(errors);
        }

        public void AddErrors(OperationResult operationResult)
        {
            AddErrors(operationResult.Errors);
        }

        public void AddErrors(IEnumerable<OperationResult> results)
        {
            foreach (var canExecuteResult in results)
                AddErrors(canExecuteResult.Errors);
        }

        public string JoinErrors(string? separator = null)
        {
            separator = separator ?? Environment.NewLine;

            return string.Join(separator, errors.Select(x => x.Error));
        }

        public static OperationResult Yes() => new OperationResult();

        public static OperationResult No(string error, string? key = null) => new OperationResult(error, key);

        public static OperationResult No(OperationError error) => new OperationResult(error);

        public static OperationResult No(IEnumerable<OperationError> errors) => new OperationResult(errors);
    };
}