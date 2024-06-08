using System.Linq;

namespace Rewired.Utils.Libraries.TinyJson;

public static class JsonExtensions
{
	/// <summary>
	/// 
	/// </summary>
	/// <param name="json"></param>
	/// <param name="indent"></param>
	/// <returns>Prettified version of the provided JSON string</returns>
	public static string Prettify(this string json, string indent = "\t") {
		var indentation = 0;
		var quoteCount = 0;
		var escapeCount = 0;

		var result =
			from ch in json ?? string.Empty
			let escaped = (ch == '\\' ? escapeCount++ : escapeCount > 0 ? escapeCount-- : escapeCount) > 0
			let quotes = ch == '"' && !escaped ? quoteCount++ : quoteCount
			let unquoted = quotes % 2 == 0
			let colon = ch == ':' && unquoted ? ": " : null
			let nospace = char.IsWhiteSpace(ch) && unquoted ? string.Empty : null
			let lineBreak = ch == ',' && unquoted ? ch + System.Environment.NewLine + string.Concat(Enumerable.Repeat(indent, indentation)) : null
			let openChar = (ch == '{' || ch == '[') && unquoted ? ch + System.Environment.NewLine + string.Concat(Enumerable.Repeat(indent, ++indentation)) : ch.ToString()
			let closeChar = (ch == '}' || ch == ']') && unquoted ? System.Environment.NewLine + string.Concat(Enumerable.Repeat(indent, --indentation)) + ch : ch.ToString()
			select colon ?? nospace ?? lineBreak ?? (
				openChar.Length > 1 ? openChar : closeChar
			);

		return string.Concat(result);
	}
}