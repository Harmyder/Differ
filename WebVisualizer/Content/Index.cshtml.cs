using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace WebVisualizer.Content
{
    public class IndexModel : PageModel
    {
        public string Before { get; set; } = "";
        public string After { get; set; } = "";
        public string BeforeComputed { get; set; } = "";
        public string AfterComputed { get; set; } = "";

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
            Before =  before;
            After = after;

            Response.Cookies.Append("Before", Before);
            Response.Cookies.Append("After", After);

            var differ = new DifferLib.Differ<char>(Before.ToCharArray(), After.ToCharArray());
            var (deletes, inserts) = differ.Compute();

            var curBeforeIndex = 0;
            foreach (var delete in deletes)
            {
                if (delete.StartOriginal > curBeforeIndex) BeforeComputed += Before.Substring(curBeforeIndex, delete.StartOriginal - curBeforeIndex);
                BeforeComputed += "<span class=\"deleteBlock\">" + Before.Substring(delete.StartOriginal, delete.Length) + "</span>";
                curBeforeIndex = delete.StartOriginal + delete.Length;
            }
            BeforeComputed += Before.Substring(curBeforeIndex);

            var curAfterIndex = 0;
            foreach (var insert in inserts)
            {
                if (insert.StartNew > curAfterIndex) AfterComputed += After.Substring(curAfterIndex, insert.StartNew - curAfterIndex);
                AfterComputed += "<span class=\"insertBlock\">" + After.Substring(insert.StartNew, insert.Length) + "</span>";
                curAfterIndex = insert.StartNew + insert.Length;
            }
            AfterComputed += After.Substring(curAfterIndex);
        }
    }
}
