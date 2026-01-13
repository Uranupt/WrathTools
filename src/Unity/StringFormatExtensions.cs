using UnityEngine;

namespace WrathTools.Unity
{
	public static class StringFormatExtensions
	{

		public static string Color(this string content, Color color)
		{
			string resl = "<color=#" + ColorUtility.ToHtmlStringRGB(color) + ">";
			resl += content;
			resl += "</color>";
			return resl;
		}

		public static string Link(this string content, string link)
		{
			string resl = "<link=\"" + link + "\">";
			resl += content;
			resl += "</link>";
			return resl;
		}

		public static string Underline(this string content)
		{
			return "<u>" + content + "</u>";
		}

		public static string Strikethrough(this string content)
		{
			return "<s>" + content + "</s>";
		}

		public static string Italics(this string content)
		{
			return "<i>" + content + "</i>";
		}

		public static string Bold(this string content)
		{
			return "<b>" + content + "</b>";
		}

	}
}