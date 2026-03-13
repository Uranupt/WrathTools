using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WrathTools.Unity
{
	public static class TextFormatter
	{

		public sealed class Builder : IEnumerable<object>
		{

			private readonly List<object> _content = new();

			public static implicit operator string(Builder builder)
			{
				string resl = builder._content[0].ToString();
				for(int i = 1; i < builder._content.Count; i++)
				{
					resl += builder._content[i].ToString();
				}
				return resl;
			}

			internal Builder(object center)
			{
        _content.Add(center);
			}

			public IEnumerator<object> GetEnumerator() => _content.GetEnumerator();
			IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
			public override string ToString() => this;

			public Builder Wrap(object left, object right) => Prepend(left).Append(right);

			public Builder Append(object obj)
			{
				_content.Add(obj);
				return this;
			}

			public Builder Prepend(object obj)
			{
				_content.Insert(0, obj);
				return this;
			}

    }

		public static Builder Color(this object content, Color color) => new Builder(content).Color(color);
		public static Builder Color(this Builder builder, Color color) => builder.Wrap($"<color=#{ColorUtility.ToHtmlStringRGB(color)}>", "</color>");
		public static Builder Link(this object content, string link) => new Builder(content).Link(link);
		public static Builder Link(this Builder builder, string link) => builder.Wrap($"<link=\"{link}\"", "</link");
		public static Builder Underline(this object content) => new Builder(content).Underline();
    public static Builder Underline(this Builder builder) => builder.Wrap("<u>", "</u>");
    public static Builder Strikethrough(this object content) => new Builder(content).Strikethrough();
    public static Builder Strikethrough(this Builder builder) => builder.Wrap("<s>", "</s>");
    public static Builder Italics(this object content) => new Builder(content).Italics();
    public static Builder Italics(this Builder builder) => builder.Wrap("<i>", "</i>");
    public static Builder Bold(this object content) => new Builder(content).Bold();
    public static Builder Bold(this Builder builder) => builder.Wrap("<b>", "</b>");

	}
}