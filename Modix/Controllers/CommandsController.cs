using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Modix.Services.CommandHelp;
using Modix.Services.Utilities;

namespace Modix.Controllers
{
    [Route("~/api")]
    public class CommandsController : Controller
    {
        private readonly ICommandHelpService _commandHelpService;

        public CommandsController(ICommandHelpService commandHelpService)
        {
            _commandHelpService = commandHelpService;
        }

        [HttpGet("commands")]
        public ActionResult<Models.Commands.ModuleHelpData[]> Commands()
        {
            var modules = _commandHelpService.GetModuleHelpData();

            var mapped = modules.Select(m => new Models.Commands.ModuleHelpData()
            {
                Name = m.Name,
                Summary = m.Summary,
                Commands = m.Commands.Select(c => new Models.Commands.CommandHelpData()
                {
                    Name = c.Name,
                    Summary = c.Summary,
                    Aliases = FormatUtilities.CollapsePlurals(c.Aliases),
                    Parameters = c.Parameters.Select(p => new Models.Commands.ParameterHelpData()
                    {
                        Name = p.Name,
                        Summary = p.Summary,
                        Type = p.Type,
                        IsOptional = p.IsOptional,
                        Options = p.Options,
                    }).ToArray(),
                }).ToArray(),
            }).ToArray();

            return Ok(mapped);
        }
    }
}
