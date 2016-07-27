using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace haterm.test
{
    [TestClass]
    public class LineBufferTest
    {
        [TestMethod]
        public void BasicTest()
        {
            var lb = new LineBuffer();
            lb.Add('1');
            lb.Add('2');
            lb.Line.Should().Be("12");
        }

        [TestMethod]
        public void BackForwardEditTest()
        {
            var lb = new LineBuffer();
            lb.Add('1');
            lb.Add('2');
            lb.Add('3');
            lb.Add('4');
            lb.Add('5');
            lb.Back();
            lb.Back();
            lb.Back();
            lb.Back();
            lb.LineToCur.Should().Be("1");
            lb.Add('a');
            lb.Line.Should().Be("1a345");
            lb.LineToCur.Should().Be("1a");
            lb.Forward();
            lb.LineToCur.Should().Be("1a3");
            lb.Add('b');
            lb.Line.Should().Be("1a3b5");
        }

        [TestMethod]
        public void BackspaceTest()
        {
            var lb = new LineBuffer();
            lb.Add('1');
            lb.Add('2');
            lb.Add('3');
            lb.Add('4');
            lb.Add('5');
            lb.Backspace();
            lb.Line.Should().Be("1234");
            lb.LineToCur.Should().Be("1234");

            lb.Back();
            lb.Back();
            lb.LineToCur.Should().Be("12");
            lb.Backspace();
            lb.Line.Should().Be("134");
            lb.LineToCur.Should().Be("1");
        }

        [TestMethod]
        public void ReplaceTest()
        {
            var lb = new LineBuffer();
            lb.Add('1');
            lb.Add('2');
            lb.Add('3');
            lb.Add('4');
            lb.Add('5');
            lb.Back();
            lb.Line.Should().Be("12345");
            lb.LineToCur.Should().Be("1234");

            lb.Replace("abcde");
            lb.Line.Should().Be("abcde");
            lb.LineToCur.Should().Be("abcde");
        }

        [TestMethod]
        public void ReplaceLastSegmentTest()
        {
            var lb = new LineBuffer();
            lb.Add('1');
            lb.Add('2');
            lb.Add(' ');
            lb.Add('4');
            lb.Add('5');
            lb.Line.Should().Be("12 45");

            lb.ReplaceLastSegment("abcde");
            lb.Line.Should().Be("12 abcde");
        }

        [TestMethod]
        public void ReplaceLastSegmentWithNoSpaceTest()
        {
            var lb = new LineBuffer();
            lb.Add('1');
            lb.Add('2');
            lb.Add('3');
            lb.Add('4');
            lb.Add('5');
            lb.Line.Should().Be("12345");

            lb.ReplaceLastSegment("abcde");
            lb.Line.Should().Be("abcde");
        }
    }
}
