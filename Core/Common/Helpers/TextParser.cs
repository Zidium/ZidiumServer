using System;

namespace Zidium.Core.Common.Helpers
{
    public class TextFragment
    {
        public string Source { get; protected set; }

        public int From { get; protected set; }

        public int To { get; protected set; }

        public TextFragment(string source, int from, int to)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            Source = source;
            From = from;
            To = to;
        }

        public bool NotFound
        {
            get { return From == -1; }
        }

        public bool Exists
        {
            get { return From > -1; }
        }

        public string Substring
        {
            get { return Source.Substring(From, To - From); }
        }

        public int Length
        {
            get { return To - From; }
        }
    }

    public class TextParser
    {
        public string Source { get; protected set; }

        public TextFragment CurrentFragment { get; protected set; }

        public StringComparison StringComparison { get; set; }

        public TextParser(string source)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            StringComparison = StringComparison.CurrentCulture;
            Source = source;
            CurrentFragment = new TextFragment(source, 0, 0);
        }

        protected TextFragment NewFragment(int from, int to)
        {
            var fragment = new TextFragment(Source, from, to);
            CurrentFragment = fragment;
            return fragment;
        }

        public TextFragment First(string search)
        {
            int index = Source.IndexOf(search, CurrentFragment.To, StringComparison);
            if (index == -1)
            {
                throw new Exception("Text not found");
            }
            return NewFragment(index, index + search.Length);
        }

        public TextFragment FirstOrNull(string search)
        {
            int index = Source.IndexOf(search, CurrentFragment.To, StringComparison);
            if (index == -1)
            {
                return null;
            }
            return NewFragment(index, index + search.Length);
        }

        public TextFragment GetBetween(int index1, int index2)
        {
            if (index1 < index2)
            {
                return new TextFragment(Source, index1, index2);
            }
            return new TextFragment(Source, index2, index1);
        }
    }
}
