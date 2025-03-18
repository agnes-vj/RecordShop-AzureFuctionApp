using System;
namespace InputBindingDb.Model
{
    public class RecordShopException : Exception
    {
        public ExecutionStatus Status { get; }

        public RecordShopException(ExecutionStatus status, string message)
            : base(message)
        {
            Status = status;
        }
    }
}
