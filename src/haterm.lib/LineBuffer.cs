﻿using System.Text;

namespace haterm
{
    public class LineBuffer
    {
        private StringBuilder builder = new StringBuilder();
        public int CurrentIndex { get; set; }

        public string Line => builder.ToString();
        public string LineToCur => builder.ToString(0, CurrentIndex);

        public void Replace(string newStr)
        {
            builder.Clear();
            builder.Append(newStr);
            this.CurrentIndex = newStr.Length;
        }

        public void ReplaceLastSegment(string newStr)
        {
            var l = this.builder.Length;
            var i = l - 1;
            while (i >= 0 && builder[i] != ' ') --i;
            if (i < 0)
            {
                this.Replace(newStr);
                return;
            }

            this.builder.Remove(i + 1, l - i - 1);
            this.builder.Append(newStr);
            this.CurrentIndex = this.builder.Length;
        }

        public void Add(char ch)
        {
            if (CurrentIndex == this.builder.Length)
            {
                builder.Append(ch);
            }
            else
            {
                builder[CurrentIndex] = ch;
            }
            this.CurrentIndex++;
        }

        public void Add(string str)
        {
            foreach (var ch in str)
            {
                this.Add(ch);
            }
        }

        public bool Back()
        {
            if (this.CurrentIndex > 0)
            {
                this.CurrentIndex--;
                return true;
            }

            return false;
        }

        public bool Forward()
        {
            if (this.CurrentIndex < this.builder.Length)
            {
                this.CurrentIndex++;
                return true;
            }

            return false;
        }

        public void Backspace()
        {
            if (this.CurrentIndex > 0)
            {
                this.builder.Remove(this.CurrentIndex - 1, 1);
                this.CurrentIndex--;
            }
        }
    }
}
