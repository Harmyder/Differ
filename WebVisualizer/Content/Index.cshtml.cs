using DifferLib;
using DifferLib.Highlight;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;

namespace WebVisualizer.Content
{
    public class IndexModel : PageModel
    {
        public string Before { get; set; } = "";
        public string After { get; set; } = "";

        public (List<string> Before, List<string> After)[] Highlighted = new (List<string> Before, List<string> After)[] { };

        private readonly ILogger<IndexModel> _logger;

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
            Before = Request.Cookies["Before"];
            After = Request.Cookies["After"];
        }

        public void OnPost([FromForm] bool shouldPreferLines, [FromForm] string before, [FromForm] string after)
        {
            Before = before;
            After = after;

            Response.Cookies.Append("Before", Before);
            Response.Cookies.Append("After", After);

            var (deletes, inserts) = DifferWrapper.Compute(before, after, shouldPreferLines);

            var highlighted = Highlighter.Highlight(Before, After, deletes, inserts);

            Highlighted = highlighted.Select(h => (h.Before?.Blocks, h.After?.Blocks)).ToArray();
        }
    }
}
