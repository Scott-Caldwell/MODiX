using System.Collections.Generic;

namespace Modix.Models.Commands
{
    public class ParameterHelpData
    {
        public string Name { get; set; }

        public string Summary { get; set; }

        public string Type { get; set; }

        public bool IsOptional { get; set; }

        public IReadOnlyCollection<string> Options { get; set; }
    }
}
