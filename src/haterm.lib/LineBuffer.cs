using System.Text;

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
