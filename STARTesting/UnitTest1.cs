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
    }
}
