using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OxfordWordExplainer
{
    public class Report
    {
        public bool Correct { get; set; }
        public string Word { get; set; }
        public List<string> Etymologies { get; set; }
        public List<string> Senses { get; set; }
        public List<string> Subsenses { get; set; }

        public string ErrorMessage { get; set; }

        public Report() { }

        public Report(string word, string errorMessage)
        {
            Word = word;
            ErrorMessage = errorMessage;
            Correct = false;
        }

        public Report(Result result)
        {
            Word = result.word;

            var entries = result.lexicalEntries?.First()?.entries;
            Etymologies = entries?.First()?.etymologies;

            var senses = entries?.First()?.senses;
            Senses = senses?.First()?.definitions;
            Subsenses = senses?.First()?.subsenses?.Select(q => q?.definitions?.First()).ToList();

            Correct = true;
        }

        public string GenerateHtmlString()
        {
            var sb = new StringBuilder();

            sb.Append("<!DOCTYPE html><html><head><meta charset='utf-8'></head><body>");

            if (Correct)
            {
                sb.Append($"<h2>Requested word: {Word}</h2></br>");
                sb.Append("<label style='color: gray;'>Etymologies</label>");

                if (Etymologies?.Any() ?? false)
                {
                    sb.Append("<ul>");
                    foreach (var etymologie in Etymologies)
                        if (etymologie != null)
                            sb.Append($"<li>{etymologie}</li>");
                    sb.Append("</ul>");
                }
                else
                    sb.Append("<p>No data</p>");

                sb.Append("<label style='color: gray;'>Senses</label>");

                if (Senses?.Any() ?? false)
                {
                    sb.Append("<ul>");
                    foreach (var sense in Senses)
                        if (sense != null)
                            sb.Append($"<li>{sense}</li>");
                    sb.Append("</ul>");
                }
                else
                    sb.Append("<p>No data</p>");

                sb.Append("<label style='color: gray;'>Subsenses</label>");

                if (Subsenses?.Any() ?? false)
                {
                    sb.Append("<ul>");
                    foreach (var subsense in Subsenses)
                        if (subsense != null)
                            sb.Append($"<li>{subsense}</li>");
                    sb.Append("</ul>");
                }
                else
                    sb.Append("<p>No data</p>");
            }
            else
            {
                sb.Append(String.IsNullOrEmpty(Word)
                    ? "<h2 style='color: red;'>You requested empty string</h2>"
                    : $"<h2 style='color: red;'>Request failed with word: {Word}</h2>");

                sb.Append($"<p><strong>Error message: </strong>{ErrorMessage}</p>");
            }

            sb.Append($"<h4>Requested date: {DateTime.Now.ToShortDateString()}</h4>");
            sb.Append("</body></html>");

            return sb.ToString();
        }
    }
}
