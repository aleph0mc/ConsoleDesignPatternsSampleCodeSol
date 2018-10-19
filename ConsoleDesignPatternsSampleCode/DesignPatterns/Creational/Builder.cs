using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleDesignPatternsSampleCode.DesignPatterns.Creational
{
    class HtmlElement
    {
        private const int INDENT_SIZE = 2;

        public string Name;
        public string Text;
        public List<HtmlElement> Elements = new List<HtmlElement>();

        private string ToStringImpl(int Indent)
        {
            var sb = new StringBuilder();
            var ind = new string(' ', INDENT_SIZE * Indent);

            sb.AppendLine($"{ind}<{Name}>");

            if (!string.IsNullOrWhiteSpace(Text))
            {
                sb.Append(new string(' ', INDENT_SIZE * (Indent + 1)));
                sb.AppendLine(Text);
            }

            foreach (var e in Elements)
                sb.Append(e.ToStringImpl(Indent + 1));

            sb.AppendLine($"{ind}</{Name}>");

            return sb.ToString();
        }

        public HtmlElement() { }

        public HtmlElement(string Name, string Text)
        {
            this.Name = Name ?? throw new ArgumentNullException(paramName: nameof(Name));
            this.Text = Text ?? throw new ArgumentNullException(paramName: nameof(Text));
        }

        public override string ToString()
        {
            return ToStringImpl(0);
        }
    }

    // HtmlBuilder
    public class Builder
    {
        private readonly string _rootName;
        private HtmlElement _root = new HtmlElement();

        public Builder(string RootName)
        {
            _rootName = RootName;
            _root.Name = _rootName;
        }

        public void AddChild(string ChildName, string ChildText)
        {
            var e = new HtmlElement(ChildName, ChildText);
            _root.Elements.Add(e);
        }

        // Fluent Interface
        public Builder AddChildFluent(string ChildName, string ChildText)
        {
            var e = new HtmlElement(ChildName, ChildText);
            _root.Elements.Add(e);
            return this;
        }

        public override string ToString()
        {
            return _root.ToString();
        }

        public void Clear()
        {
            _root = new HtmlElement { Name = _rootName };
        }
    }
}
