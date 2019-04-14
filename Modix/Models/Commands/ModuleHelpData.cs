using System.Collections.Generic;

namespace Modix.Models.Commands
{
    public class ModuleHelpData
    {
        public string Name { get; set; }

        public string Summary { get; set; }

        public IReadOnlyCollection<CommandHelpData> Commands { get; set; }
    }
}
