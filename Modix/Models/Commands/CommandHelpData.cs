using System.Collections.Generic;

namespace Modix.Models.Commands
{
    public class CommandHelpData
    {
        public string Name { get; set; }

        public string Summary { get; set; }

        public IReadOnlyCollection<string> Aliases { get; set; }

        public IReadOnlyCollection<ParameterHelpData> Parameters { get; set; }
    }
}
