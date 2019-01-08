using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Modix.Common.Results
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public readonly struct Result<T> : IEquatable<Result<T>>
    {
        private Result(IFault fault)
        {
            IsFaulted = true;
            Fault = fault;
            Value = default;
        }

        private Result(T value)
        {
            IsFaulted = false;
            Fault = default;
            Value = value;
        }

        public IFault Fault { get; }

        public bool IsFaulted { get; }

        public T Value { get; }

        private string DebuggerDisplay
            => IsFaulted
                ? "Faulted: " + Fault.Message
                : "Succeeded: " + Value.ToString();

        public static Result<T> FromFault(IFault fault)
            => new Result<T>(fault);

        public static Result<T> FromSuccess(T value)
            => new Result<T>(value);

        public Result ContinueWith(Action continuation)
        {
            if (IsFaulted)
                return Result.FromFault(Fault);

            continuation();

            return Result.Success;
        }

        public Result<T> ContinueWith(Func<Result<T>> continuation)
            => IsFaulted
                ? this
                : continuation();

        public Result ContinueWith(Func<Result> continuation)
            => IsFaulted
                ? Result.FromFault(Fault)
                : continuation();

        public async Task<Result> ContinueWithAsync(Func<Task> continuation)
        {
            if (IsFaulted)
                return Result.FromFault(Fault);

            await continuation();

            return Result.Success;
        }

        public async Task<Result<T>> ContinueWithAsync(Func<Task<Result<T>>> continuation)
            => IsFaulted
                ? this
                : await continuation();

        public async Task<Result> ContinueWithAsync(Func<Task<Result>> continuation)
            => IsFaulted
                ? Result.FromFault(Fault)
                : await continuation();

        public bool Equals(Result<T> other) => throw new NotImplementedException();

        public static implicit operator Result<T>(T value)
            => new Result<T>(value);
    }
}
