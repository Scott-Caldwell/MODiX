using System;

using Modix.Common.Results;

namespace Modix.Data.Models.Core
{
    public readonly struct MissingClaimFault : IFault
    {
        public MissingClaimFault(AuthorizationClaim missingClaim)
        {
            MissingClaim = missingClaim;
            Message = $"The following claim was missing: {missingClaim}.";
        }

        public AuthorizationClaim MissingClaim { get; }

        public string Message { get; }

        public override bool Equals(object obj)
            => obj is MissingClaimFault otherFault
                && this == otherFault;

        public bool Equals(IFault other)
            => other is MissingClaimFault otherFault
                && this == otherFault;

        public override int GetHashCode()
            => HashCode.Combine(MissingClaim, Message);

        public static bool operator ==(MissingClaimFault left, MissingClaimFault right)
            => left.MissingClaim == right.MissingClaim
                && left.Message == right.Message;

        public static bool operator !=(MissingClaimFault left, MissingClaimFault right)
            => left.MissingClaim != right.MissingClaim
                || left.Message != right.Message;
    }
}
