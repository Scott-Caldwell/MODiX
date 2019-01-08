using System;

namespace Modix.Common.Results
{
    public interface IFault : IEquatable<IFault>
    {
        string Message { get; }
    }
}
