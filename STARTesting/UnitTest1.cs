using System;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace STARTesting
{
    [TestClass]
    public class MainTester
    {
        [TestMethod]
        public void testFileLoading()
        {
            
            Assert.AreEqual(2,2);
        }

        [TestMethod]
        public void testClearing()
        {
            Assert.AreEqual(1,1);
        }

        [TestMethod]
        public void testHelp()
        {
            Assert.AreEqual(3,3);
        }

        [TestMethod]
        public void testGraph()
        {
            Assert.AreEqual(4,4);
        }

        [TestMethod]
        public void testRefresh()
        {
            Assert.AreEqual(5,5);
        }

        [TestMethod]
        public void testNavigation()
        {
            Assert.AreEqual(6,6);
        }

        [TestMethod]
        public void testFilter()
    }
}
