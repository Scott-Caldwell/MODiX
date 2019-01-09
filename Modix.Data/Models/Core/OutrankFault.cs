using Modix.Common.Results;

namespace Modix.Data.Models.Core
{
    public readonly struct OutrankFault : IFault
    {
        public string Message => "You must outrank the user that you are attempting to moderate.";

        public override bool Equals(object obj)
            => obj is OutrankFault otherFault
                && this == otherFault;

        public bool Equals(IFault other)
            => other is OutrankFault otherFault
                && this == otherFault;

        public override int GetHashCode()
            => 0;

        public static bool operator ==(OutrankFault left, OutrankFault right)
            => true;

        public static bool operator !=(OutrankFault left, OutrankFault right)
            => false;
    }
}
