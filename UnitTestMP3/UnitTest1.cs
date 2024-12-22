using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Security.Principal;
using System.Windows;

namespace UnitTestMP3
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            Window window = new Window();

            window.Show();
            window.Close();
            Assert.IsFalse(window.IsVisible);
        }
    }
}
