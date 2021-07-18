using DifferLib;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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

        public void OnPost([FromForm] string before, [FromForm] string after)
        {
            Before = before;
            After = after;

            Response.Cookies.Append("Before", Before);
            Response.Cookies.Append("After", After);

            var differ = new Differ<char>(Before.ToCharArray(), After.ToCharArray());
            var (deletes, inserts) = differ.Compute();

            var highlighted = Highlighter.Highlight(Before, After, deletes, inserts);

            Highlighted = highlighted.Select(h => (h.Before?.Blocks, h.After?.Blocks)).ToArray();
        }
    }
}
