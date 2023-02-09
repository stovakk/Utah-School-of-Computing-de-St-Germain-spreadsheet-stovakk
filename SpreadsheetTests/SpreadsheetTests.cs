using SpreadsheetUtilities;
using SS;
using System.Security.Cryptography;

namespace SpreadsheetTests
{
    [TestClass]
    public class SpreadsheetTests
    {
        SS.Spreadsheet ss = new SS.Spreadsheet();

        // Name of test explains what it does
        [TestMethod]
        public void TestSetCellDouble()
        {
            ss.SetCellContents("a1", 2.0);
            Assert.IsNotNull(ss.GetCellContents("a1"));
        }

        // Name of test explains what it does
        [TestMethod]
        public void TestSetCellString() 
        {
            ss.SetCellContents("a1", "5");
            Assert.IsNotNull(ss.GetCellContents("a1"));
        }

        // Name of test explains what it does
        [TestMethod]
        public void TestSetCellFormula()
        {
            Formula form = new Formula("3+1 + a2");
            ss.SetCellContents("a1", form);
            Assert.IsNotNull(ss.GetCellContents("a1"));
        }

        // Name of test explains what it does
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestNullNameDouble()
        {
            ss.SetCellContents(null, 2.0);
        }

        // Name of test explains what it does
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestNullNameString()
        {
            ss.SetCellContents(null, "5");
        }

        // Name of test explains what it does
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestNullValueString()
        {
            String s = null;
            ss.SetCellContents("a1", s);
        }

        // Name of test explains what it does
        [TestMethod]
        public void TestReplaceCell()
        {
            ss.SetCellContents("a1", new Formula("3"));
            ss.SetCellContents("a1", new Formula("2"));
            Assert.IsNotNull(ss.GetCellContents("a1"));
        }

        // Name of test explains what it does
        [TestMethod]
        public void TestGetEmptyCell()
        {
            ss.SetCellContents("a1", "a");
            ss.SetCellContents("a1", "");
            Assert.IsTrue(ss.GetCellContents("a1").Equals(""));
        }

        // Name of test explains what it does
        [TestMethod]
        public void TestReplaceStringCell()
        {
            ss.SetCellContents("a1", "a");
            ss.SetCellContents("a1", "b");
            Assert.IsTrue(ss.GetCellContents("a1").Equals("b"));
        }

        // Name of test explains what it does
        [TestMethod]
        public void TestReplaceDoubleCell()
        {
            ss.SetCellContents("a1", 2.0);
            ss.SetCellContents("a1", 3.0);
            Assert.IsTrue(ss.GetCellContents("a1").Equals(3.0));
        }

        // Name of test explains what it does
        [TestMethod]
        public void TestGetNamesNonEmptyCells()
        {
            ss.SetCellContents("a1", "a");
            ss.SetCellContents("a2", "b");
            ss.SetCellContents("a3", "");
            ss.SetCellContents("b1", "ra");
            ss.SetCellContents("b2", "ba");
            ss.SetCellContents("b3", "d");
            HashSet<String> hs = ss.GetNamesOfAllNonemptyCells().ToHashSet<String>();
            Assert.IsTrue(hs.Contains("a1"));
            Assert.IsTrue(hs.Contains("a2"));
            Assert.IsTrue(hs.Contains("b1"));
            Assert.IsTrue(hs.Contains("b2"));
            Assert.IsTrue(hs.Contains("b3"));
            Assert.IsTrue(hs.Count == 5);
        }

        // Name of test explains what it does
        [TestMethod()]
        [ExpectedException(typeof(CircularException))]
        public void TestCircularExceptionFormula()
        {
            ss.SetCellContents("a1", new Formula("a2 + 3"));
            ss.SetCellContents("a2", new Formula("a1 - 3"));
        }

        // Name of test explains what it does
        [TestMethod()]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestExceptionInvalidNameGetCell()
        {
            ss.GetCellContents("2a");
        }

        // Name of test explains what it does
        [TestMethod()]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestExceptionInvalidNameCreateFormula()
        {
            String s = null;
            ss.SetCellContents(s, new Formula("3"));
        }

        // Name of test explains what it does
        [TestMethod()]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestExceptionNullFormula()
        {
            String s = null;
            ss.SetCellContents("a1", new Formula(s));
        }
    }
}