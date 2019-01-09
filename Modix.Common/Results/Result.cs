using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Modix.Common.Results
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public readonly struct Result : IEquatable<Result>
    {
        private Result(IFault fault)
        {
            IsFaulted = true;
            Fault = fault;
        }

        public static Result Success => new Result();

        public IFault Fault { get; }

        public bool IsFaulted { get; }

        private string DebuggerDisplay
            => IsFaulted
                ? "Faulted: " + Fault.Message
                : "Succeeded";

        public static Result FromFault(IFault fault)
            => new Result(fault);

        public bool TryGetFaulted(out Result result)
        {
            result = this;

            return IsFaulted;
        }

        public Result ContinueWith(Action continuation)
        {
            if (IsFaulted)
                return this;

            continuation();

            return Success;
        }

        public Result ContinueWith(Func<Result> continuation)
            => IsFaulted
                ? this
                : continuation();

        public Result<T> ContinueWith<T>(Func<Result<T>> continuation)
            => IsFaulted
                ? Result<T>.FromFault(Fault)
                : continuation();

        public async Task<Result> ContinueWithAsync(Func<Task> continuation)
        {
            if (IsFaulted)
                return this;

            await continuation();

            return Success;
        }

        public async Task<Result> ContinueWithAsync(Func<Task<Result>> continuation)
            => IsFaulted
                ? this
                : await continuation();

        public async Task<Result<T>> ContinueWithAsync<T>(Func<Task<Result<T>>> continuation)
            => IsFaulted
                ? Result<T>.FromFault(Fault)
                : await continuation();

        public override bool Equals(object obj)
            => obj is Result result
                && this == result;

        public bool Equals(Result other)
            => this == other;

        public override int GetHashCode()
            => IsFaulted
                ? Fault.GetHashCode()
                : 0;

        public static bool operator ==(Result left, Result right)
            => left.IsFaulted
                ? right.IsFaulted
                    && left.Fault.Equals(right.Fault)
                : !right.IsFaulted;

        public static bool operator !=(Result left, Result right)
            => left.IsFaulted
                ? !right.IsFaulted
                    || !left.Fault.Equals(right.Fault)
                : right.IsFaulted;
    }
}
