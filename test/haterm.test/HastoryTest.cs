using System;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace haterm.test
{
    [TestClass]
    public class HastoryTest
    {
        [TestMethod]
        public void AddGetHistory()
        {
            var guid = Guid.NewGuid();
            var his = new Hastory(guid, "test");
            his.Load();
            his.Count.Should().Be(0);
            his.Add("ab123");
            his.Count.Should().Be(1);

            var his2 = new Hastory(Guid.NewGuid(), "test");
            his2.Load();
            his2.Count.Should().Be(0);

            var his3 = new Hastory(guid, "test");
            his3.Load();
            his3.Count.Should().Be(0);

            his.Save();
            his3.Load();
            his3.Count.Should().Be(1);
        }

        [TestMethod]
        public void SearchHistory()
        {
            var guid = Guid.NewGuid();
            var his = new Hastory(guid, "test");
            his.Load();
            his.Count.Should().Be(0);
            his.Add("ab123");
            his.Add("bc123");

            var combo = his.Search("a");
            combo.Line.Should().Be("ab123");
            combo.Id.Should().Be(0);

            var combo1 = his.Search("");
            combo1.Line.Should().Be("bc123");
            combo1.Id.Should().Be(1);

            var combo2 = his.Search("56");
            combo2.Should().Be(SearchCombo.NotFound);
        }

        [TestMethod]
        public void SearchHistoryWithId()
        {
            var guid = Guid.NewGuid();
            var his = new Hastory(guid, "test");
            his.Load();
            his.Count.Should().Be(0);
            his.Add("at00");
            his.Add("bt01");
            his.Add("at02");
            his.Add("bt03");
            his.Add("bt04");
            his.Add("at05");

            var combo = his.Search("a");
            combo.Line.Should().Be("at05");
            combo.Id.Should().Be(5);

            combo = his.Search("a", 5);
            combo.Line.Should().Be("at02");
            combo.Id.Should().Be(2);

            combo = his.Search("a", 2);
            combo.Line.Should().Be("at00");
            combo.Id.Should().Be(0);
        }

        [TestMethod]
        public void SearchHistoryWithIdForward()
        {
            var guid = Guid.NewGuid();
            var his = new Hastory(guid, "test");
            his.Load();
            his.Count.Should().Be(0);
            his.Add("at00");
            his.Add("bt01");
            his.Add("at02");
            his.Add("bt03");
            his.Add("bt04");
            his.Add("at05");

            var combo = his.Search("a");
            combo.Line.Should().Be("at05");
            combo.Id.Should().Be(5);

            combo = his.Search("a", 5, false);
            combo.Should().Be(SearchCombo.NotFound);

            combo = his.Search("b", 1, false);
            combo.Line.Should().Be("bt03");
            combo.Id.Should().Be(3);
        }
    }
}
