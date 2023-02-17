// implementation written by Nate Stovak for CS 3500, February 2023.
// Version 1.1 (Write Tests and debugg Spreadsheet class)

/// Author:    Nate Stovak
/// Partner:   None
/// Date:      9 - Feb - 2023
/// Course:    CS 3500, University of Utah, School of Computing
/// GitHub ID: Stovakk
/// Date:	   16 - Feb - 2023
/// Solution:  SpreadsheetTests
/// Copyright: CS 3500 and Nate Stovak,

using SpreadsheetUtilities;
using SS;
using System.Xml;

namespace SpreadsheetTests
{
    [TestClass]
    public class SpreadsheetTests
    {

        /// <summary>
        /// Refer to name, testing constructors
        /// </summary>
        [TestMethod]
        public void TestConstructors()
        {
            using (XmlWriter writer = XmlWriter.Create("save.txt")) // NOTICE the file with no path
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("spreadsheet");
                writer.WriteAttributeString("version", "");

                writer.WriteStartElement("cell");
                writer.WriteElementString("name", "A1");
                writer.WriteElementString("contents", "hello");
                writer.WriteEndElement();

                writer.WriteEndElement();
                writer.WriteEndDocument();
            }


            Spreadsheet spread = new Spreadsheet();
            Spreadsheet spreadIsValid = new Spreadsheet(s =>true, s => s, "5.0 + x1");
            Spreadsheet spreadFile = new Spreadsheet("save.txt", s => true, s => s, "");
        }

             //////////   Exception Tests   \\\\\\\\\\\ 

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestInvalidGetCellContents()
        {
            Spreadsheet spread = new Spreadsheet();
            spread.GetCellContents("12");
        }

        /// <summary>
        /// Refer to name, testing invalid setContentsOfCell
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestEmptySetContentsOfCell()
        {
            Spreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell("", "5.0");
        }



        /// <summary>
        /// Refer to name, testing invalid formula
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestInvalidFormulaSetContents()
        {
            Spreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell("B1", "=+");
        }

        /// <summary>
        /// Refer to name, testing invalidCircularException off
        /// the SetContentsOfCell method
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(CircularException))]
        public void TestInvalidCircularExceptionSetContents()
        {
            Spreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell("b1", "= 5.0 + a1");
            sheet.SetContentsOfCell("a1", "= 5.0 + b1");
        }

        /// <summary>
        /// Refer to method name, testing if the class will throw an exception for empty
        /// save name
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void TestSaveEmptyName()
        {
            Spreadsheet sheet = new Spreadsheet(s => true, s => s, "5.0 + x3");
            sheet.SetContentsOfCell("a1", "5.0");
            sheet.Save("");
        }

        /// <summary>
        /// Refer to title, testing if exception if thrown if
        /// the getSavedVersion has empty parameter
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void testEmptyFilePathLocation()
        {
            using (XmlWriter writer = XmlWriter.Create("save.txt")) // NOTICE the file with no path
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("spreadsheet");
                writer.WriteAttributeString("version", "");

                writer.WriteStartElement("cell");
                writer.WriteElementString("name", "A1");
                writer.WriteElementString("contents", "hello");
                writer.WriteEndElement();

                writer.WriteEndElement();
                writer.WriteEndDocument();
            }
            Spreadsheet spread = new Spreadsheet("save.txt", s => true, s => s, "5.0");
            spread.GetSavedVersion("");

        }

        /// <summary>
        /// Test Save, expect exception as verions doesn't match the filePath
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void TestSaveSmallInvalid()
        {
            Spreadsheet spread = new Spreadsheet();
            spread.SetContentsOfCell("a1", "5.0");
            spread.SetContentsOfCell("b1", "20.5");
            spread.SetContentsOfCell("d1", "= 5*a1");
            spread.SetContentsOfCell("e1", "yes");
            spread.SetContentsOfCell("f1", "no");
            spread.SetContentsOfCell("g1", "a1+b1/d1");
            spread.Save("save.txt");

            Spreadsheet spread2 = new Spreadsheet("save.txt", s => true, s => s, "nonDefault");
        }

        /// <summary>
        /// Testing saving a file that has an empty filename
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void TestSaveEmptyFilename()
        {
            Spreadsheet spread = new Spreadsheet();
            spread.SetContentsOfCell("a1", "5.0");
            spread.SetContentsOfCell("b1", "20.5");
            spread.SetContentsOfCell("d1", "= 5*a1");
            spread.SetContentsOfCell("e1", "yes");
            spread.SetContentsOfCell("f1", "no");
            spread.SetContentsOfCell("g1", "a1+b1/d1");
            spread.Save("");
        }

        /// <summary>
        /// Refer to name, saving invalid empty name
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void TestGetSavedVersionEmptyName()
        {
            Spreadsheet spread = new Spreadsheet();
            spread.SetContentsOfCell("a1", "5.0");
            spread.SetContentsOfCell("b1", "20.5");
            spread.SetContentsOfCell("d1", "= 5*a1");
            spread.SetContentsOfCell("e1", "yes");
            spread.SetContentsOfCell("f1", "no");
            spread.SetContentsOfCell("g1", "a1+b1/d1");
            spread.Save("");
        }

        /// <summary>
        /// refer to title, excpeption thrown becuase save.txt is empty
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void TestReadFileEmpty()
        {
            Spreadsheet spread = new Spreadsheet();
            spread.Save("save.txt");
            Spreadsheet spread2 = new Spreadsheet("save.txt", s => true, s => s, "default");
        }

        /// <summary>
        /// Refer to title, testing non-existent file, should throw 
        /// an exception
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void TestNonExistentFile()
        {
            Spreadsheet spread = new Spreadsheet();
            String version = spread.GetSavedVersion("NA.txt");
        }

        //////////   Normal Tests   \\\\\\\\\\\ 

        /// <summary>
        /// Refer to name, testing setContents for double
        /// </summary>
        [TestMethod]
        public void TestSetContentDouble()
        {
            Spreadsheet spread = new Spreadsheet();
            spread.SetContentsOfCell("b1", "5.0");
            object num = spread.GetCellValue("b1");
            Assert.IsTrue(num.Equals(5.0));
        }

        /// <summary>
        /// Refer to name, testing setContents for String
        /// </summary>
        [TestMethod]
        public void TestSetContentsString()
        {
            Spreadsheet spread = new Spreadsheet();
            spread.SetContentsOfCell("a1", "5.0");
            spread.SetContentsOfCell("b1", "yes");
            object str = spread.GetCellValue("b1");
            Assert.IsTrue(str.Equals("yes"));
        }

        /// <summary>
        /// Refer to name, testing setContents for Formula
        /// </summary>
        [TestMethod]
        public void TestSetContentsFormula()
        {
            Spreadsheet spread = new Spreadsheet();
            spread.SetContentsOfCell("a1", "5.0");
            spread.SetContentsOfCell("b1", "yes");
            spread.SetContentsOfCell("c1", "= a1 + 3.0");
            object form = spread.GetCellValue("c1");
            Assert.IsTrue(form.Equals(8.0));
        }

        /// <summary>
        /// Testing if spreadsheet creates empty cells when created
        /// </summary>
        [TestMethod]
        public void TestEmtpyCell()
        {
            Spreadsheet spread = new Spreadsheet();
            object empty = spread.GetCellValue("b24");
            Assert.IsTrue("".Equals(empty));
        }

        /// <summary>
        /// Refer to Title, test cell contents empty
        /// </summary>
        [TestMethod]
        public void TestCellContentsEmtpy()
        {
            Spreadsheet spread = new Spreadsheet();
            Assert.IsTrue("".Equals(spread.GetCellContents("a1")));
        }

        /// <summary>
        /// Refer to title, testing get cell contents normal response
        /// </summary>
        [TestMethod]
        public void TestGetCellContentsNormal()
        {
            Spreadsheet spread = new Spreadsheet();
            spread.SetContentsOfCell("a1", "5.0");
            Assert.IsTrue(5.0.Equals(spread.GetCellContents("a1")));
        }

        /// <summary>
        /// Refer to name, messing with changed variable
        /// </summary>
        [TestMethod]
        public void TestChangedVariable()
        {
            Spreadsheet spread = new Spreadsheet();
            Assert.IsFalse(spread.Changed);
        }

        /// <summary>
        /// Testing setContents of a string that's empty
        /// </summary>
        [TestMethod]
        public void TestSetContentsEmptyString()
        {
            Spreadsheet spread = new Spreadsheet();
            spread.SetContentsOfCell("a1", "");
            Assert.IsTrue("".Equals(spread.GetCellContents("a1")));
        }

        /// <summary>
        /// Testing setContents of a string that already exists
        /// </summary>
        [TestMethod]
        public void TestSetContentsReplaceString()
        {
            Spreadsheet spread = new Spreadsheet();
            spread.SetContentsOfCell("a1", "5.0");
            spread.SetContentsOfCell("a1", "53.0");
            Assert.IsTrue(53.0.Equals(spread.GetCellContents("a1")));
        }

        /// <summary>
        /// Test empty file
        /// </summary>
        [TestMethod]
        public void TestSaveEmpty()
        {
            Spreadsheet spread = new Spreadsheet();
            spread.Save("save.txt");
        }

        /// <summary>
        /// Creating a spreadsheet, then saving it
        /// and creating another spreadhseet with it
        /// all variables should be the same
        /// </summary>
        [TestMethod]
        public void TestSaveSmallFile()
        {
            Spreadsheet spread = new Spreadsheet();
            spread.SetContentsOfCell("a1", "5.0");
            spread.SetContentsOfCell("b1", "20.5");
            spread.SetContentsOfCell("d1", "= 5*a1");
            spread.SetContentsOfCell("e1", "yes");
            spread.SetContentsOfCell("f1", "no");
            spread.SetContentsOfCell("g1", "a1+b1/d1");
            spread.Save("1.0");

            Spreadsheet spread2 = new Spreadsheet("1.0", s=>true, s=> s, "default");
            Assert.IsTrue(spread2.GetCellContents("a1").Equals(spread.GetCellContents("a1")));
            Assert.IsTrue(spread2.GetCellContents("b1").Equals(spread.GetCellContents("b1")));
            Assert.IsTrue(spread2.GetCellContents("d1").Equals(spread.GetCellContents("d1")));
            Assert.IsTrue(spread2.GetCellContents("e1").Equals(spread.GetCellContents("e1")));
            Assert.IsTrue(spread2.GetCellContents("f1").Equals(spread.GetCellContents("f1")));
            Assert.IsTrue(spread2.GetCellContents("g1").Equals(spread.GetCellContents("g1")));
        }

        /// <summary>
        /// Refer to title, tests reevaluate with variables
        /// </summary>
        [TestMethod]
        public void TestReEvaluateVariables()
        {
            Spreadsheet spread = new Spreadsheet();
            spread.SetContentsOfCell("a1", "2.0");
            spread.SetContentsOfCell("b1", "=a1 + 3.0");
            spread.SetContentsOfCell("a2", "12.0");
            Assert.IsTrue(spread.GetCellValue("a2").Equals(12.0));
            spread.SetContentsOfCell("a2", "=a1 + b1");
            Assert.IsTrue(spread.GetCellValue("a2").Equals(7.0));
        }

        /// <summary>
        /// refer to title
        /// </summary>
        [TestMethod]
        public void TestGetNamesOfAllNonEmptyCells()
        {
            Spreadsheet spread = new Spreadsheet();
            spread.SetContentsOfCell("a1", "2.0");
            spread.SetContentsOfCell("b1", "=a1 + 3.0");
            spread.SetContentsOfCell("a2", "12.0");
            spread.SetContentsOfCell("c1", "2.0");
            spread.SetContentsOfCell("d1", "=a1 + 3.0");
            spread.SetContentsOfCell("b2", "12.0");

            HashSet<String> hash = new HashSet<string>();
            hash.Add("a1"); hash.Add("b1");
            hash.Add("a2"); hash.Add("c1");
            hash.Add("d1"); hash.Add("b2");

            HashSet<String> keys = spread.GetNamesOfAllNonemptyCells().ToHashSet<String>();
            foreach(String key in keys)
            {
                Assert.IsTrue(hash.Contains(key));
            }
        }

        //////////   Stress Tests   \\\\\\\\\\\ 

        /// <summary>
        /// stress test for setting contents as doubles
        /// </summary>
        [TestMethod(), Timeout(2000)]
        public void TestStressSetContentsDouble()
        {
            Spreadsheet s = new Spreadsheet();
            IList<String> cells = new List<string>();

            for (int i = 1; i < 400; i++)
            {
                s.SetContentsOfCell("a1", i.ToString());
            }
            Assert.IsTrue(s.GetCellContents("a1").Equals(399));
        }

        /// <summary>
        /// stress test formulas
        /// </summary>
        [TestMethod(), Timeout(2000)]
        public void TestStressSetFormula()
        {
            Spreadsheet s = new Spreadsheet();
            IList<String> cells = new List<string>();
            s.SetContentsOfCell("b1", "1.0");
            for (int i = 1; i < 400; i++)
            {
                s.SetContentsOfCell("a1", "= b1 + " + i.ToString());
            }
        }

        /// <summary>
        /// Stress test on get Cell contents
        /// </summary>
        [TestMethod(), Timeout(2000)]
        public void TestStressGetCellContents()
        {
            Spreadsheet s = new Spreadsheet();
            IList<String> cells = new List<string>();

            for (int i = 1; i < 400; i++)
            {
                s.SetContentsOfCell("a1", i.ToString());
                double r = i;
                Assert.IsTrue(s.GetCellContents("a1").Equals(r));
            }
        }


    }
}