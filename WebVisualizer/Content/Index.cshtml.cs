using DifferLib;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Text;

namespace WebVisualizer.Content
{
    public class IndexModel : PageModel
    {
        public string Before { get; set; } = "";
        public string After { get; set; } = "";
        public string BeforeComputed { get; set; } = "";
        public string AfterComputed { get; set; } = "";

        private const string InsertLineStart = "<span class=\"insertLine\">";
        private const string InsertBlockStart = "<span class=\"insertBlock\">";
        private const string DeleteLineStart = "<span class=\"deleteLine\">";
        private const string DeleteBlockStart = "<span class=\"deleteBlock\">";
        private const string End = "</span>";

        private readonly Highlighter _highlighter;

        private readonly ILogger<IndexModel> _logger;

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;

            var deleteSettings = new HighlighterSettings(DeleteLineStart, End, DeleteBlockStart, End);
            var insertSettings = new HighlighterSettings(InsertLineStart, End, InsertBlockStart, End);
            _highlighter = new Highlighter(deleteSettings, insertSettings);
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

            BeforeComputed = _highlighter.HighlightDelete(Before, deletes);
            AfterComputed = _highlighter.HighlightInsert(After, inserts);
        }
    }
}
