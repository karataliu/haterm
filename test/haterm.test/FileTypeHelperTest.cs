using System;
using System.Runtime.InteropServices;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace haterm.test
{
    [TestClass]
    public class FileTypeHelperTest
    {
        [TestMethod]
        public void GetTypeTest()
        {
            string fileName = @"C:\Windows\System32\cmd.exe";
            FileTypeHelper.FileTypeInfo(fileName).Should().Be("Application");
        }
    }
}
