using SpreadsheetUtilities;
using System.Text.RegularExpressions;

namespace FormulaTests
{

    /// <summary>
    /// This method is used to extensivly test the Formula.cs class that it has a dependency on
    /// I have created 42 tests to thoroughly test the class. With these tests I have achieved 
    /// 98.8% (252/255) code coverage, which I see as very good as I covered 252 lines of a possible 255 lines.
    /// Which I see as very sufficient as the remaining 3 lines were hard to reach. 
    /// </summary>
    [TestClass]
    public class FormulaTests
    {
        // private formula so i don't have to initiate the first part every time
        private Formula form;

        /// <summary>
        /// Refer to Title
        /// </summary>
        [TestMethod]
        public void TestCreatingValidFormula()
        {
            form = new Formula("3 + 2 - 6");
            Assert.IsNotNull(form);
        }

        /// <summary>
        /// Refer to Title
        /// </summary>
        [TestMethod]
        public void TestCreatingValidFormulaWithParentheses()
        {
            form = new Formula("3 + 2 - 6 * (3+2)");
            Assert.IsNotNull(form);
        }

        /// <summary>
        /// Refer to Title
        /// </summary>
        [TestMethod]
        public void TestCreatingValidFormulaWithMultipleParentheses()
        {
            form = new Formula("3 + (2 - 6) * (3+2)");
            Assert.IsNotNull(form);
        }

        /// <summary>
        /// Refer to Title
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestEmptyFormula()
        {
            form = new Formula("");
        }

        /// <summary>
        /// Refer to Title
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestInvalidDoubleNumFormula()
        {
            form = new Formula("3 3");
        }

        /// <summary>
        /// Refer to Title
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestInvalidDoubleOperatoinFormula()
        {
            form = new Formula("3 + + 24 -3");
        }

        /// <summary>
        /// Refer to Title
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestStartWithInvalidInput()
        {
            form = new Formula(" + 4 - 2", s => s, s => false);
        }

        /// <summary>
        /// Refer to Title
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestMoreClosedThanOpenParenthesesSingle()
        {
            form = new Formula("3 + 4 - 2(");
        }

        /// <summary>
        /// Refer to Title
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestMoreClosedThanOpenParenthesesMultiple()
        {
            form = new Formula("3 + (4 - 2) * 3 + (12-1))");
        }

        /// <summary>
        /// Refer to Title
        /// </summary>
        [TestMethod]
        public void TestCreatingVariables()
        {
            form = new Formula("3 + (2 - 6) * (3+2) + s");
            Assert.IsNotNull(form);
        }

        /// <summary>
        /// Refer to Title
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestUsingInvalidVariableNames()
        {
            form = new Formula("3 + (4 - 2) * 3 + (12-1) + x1_ + $$ - (", s => s ,s=> true);
        }

        /// <summary>
        /// Refer to Title
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestUsingInvalidVariables()
        {
            form = new Formula("3 + (4 - 2) * 3 + (12-1) + x1_", s => s, s => false);
        }

        /// <summary>
        /// Refer to Title
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestMoreOpenParenthesesThanClosed()
        {
            form = new Formula("(((3 + (4 - 2) * 3 + (12-1) + x)", s => s, s => true);
        }

        /// <summary>
        /// Refer to Title
        /// </summary>
        [TestMethod]
        public void testEvaluateBasic()
        {
            form = new Formula("3");
            Assert.AreEqual(form.Evaluate(s => 1.0), 3.0);
        }

        /// <summary>
        /// Refer to Title
        /// </summary>
        [TestMethod]
        public void testEvaluateAdd()
        {
            form = new Formula("3 + 2");
            Assert.AreEqual(form.Evaluate(s => 1.0), 5.0);
        }

        /// <summary>
        /// Refer to Title
        /// </summary>
        [TestMethod]
        public void testEvaluateSubtract()
        {
            form = new Formula("3 - 2");
            Assert.AreEqual(form.Evaluate(s => 1.0), 1.0);
        }

        /// <summary>
        /// Refer to Title
        /// </summary>
        [TestMethod]
        public void testEvaluateMultiplication()
        {
            form = new Formula("3 * 2");
            Assert.AreEqual(form.Evaluate(s => 1.0), 6.0);
        }

        /// <summary>
        /// Refer to Title
        /// </summary>
        [TestMethod]
        public void testEvaluateDivision()
        {
            form = new Formula("4 / 2");
            Assert.AreEqual(form.Evaluate(s => 1.0), 2.0);
        }

        /// <summary>
        /// Refer to Title
        /// </summary>
        [TestMethod]
        public void testEvaluateParentheses()
        {
            form = new Formula("3 + 2 * (12-1)");
            Assert.AreEqual(form.Evaluate(s=>0.0), 25.0);
        }

        /// <summary>
        /// Refer to Title
        /// </summary>
        [TestMethod]
        public void testEvaluateMultipleParentheses()
        {
            form = new Formula("3 + 2 * ((12-1)) + 1 - 1");
            Assert.AreEqual(form.Evaluate(s => 0.0), 25.0);
        }

        /// <summary>
        /// Refer to Title
        /// </summary>
        [TestMethod]
        public void testVariableEvaluate()
        {
            form = new Formula("3 * 2 * (12-1) * s");
            Assert.AreEqual(form.Evaluate(s => 0.0), 0.0);
        }

        /// <summary>
        /// Refer to Title
        /// </summary>
        [TestMethod]
        public void testDivdingEvaluate()
        {
            form = new Formula("((3 + 2 * (12-1)) / 5)");
            Assert.AreEqual(form.Evaluate(s => 0.0), 5.0);
        }

        /// <summary>
        /// Refer to Title
        /// </summary>
        [TestMethod]
        public void testDivdingEvaluateInParentheses()
        {
            form = new Formula("1 + 3/(1 + 2)");
            Assert.AreEqual(form.Evaluate(s => 0.0), 2.0);
        }

        /// <summary>
        /// Refer to Title
        /// </summary>
        [TestMethod]
        public void testComplexWithParentheses()
        {
            form = new Formula("2+3*5+(3+4*8)*5+2");
            Assert.AreEqual(form.Evaluate(s => 0.0), 194.0);
        }

        /// <summary>
        /// Refer to Title
        /// </summary>
        [TestMethod]
        public void testLargeSetWithParentheses()
        {
            form = new Formula("2+3*5+(3+4*8)*5+2");
            Assert.AreEqual(form.Evaluate(s => 0.0), 194.0);
        }

        /// <summary>
        /// Refer to Title
        /// </summary>
        [TestMethod]
        public void testGetVariables()
        {
            form = new Formula("r1 + d2 + k3");
            HashSet<String> variables = form.GetVariables().ToHashSet();
            HashSet<String> clone = new HashSet<String>();
            clone.Add("r1");
            clone.Add("d2");
            clone.Add("k3");
            foreach(var var in clone)
            {
                Assert.IsTrue(variables.Contains(var));
            }
        }

        /// <summary>
        /// Refer to Title
        /// </summary>
        [TestMethod]
        public void testGetVariablesEmpty()
        {
            form = new Formula("1 + 2");
            HashSet<String> variables = form.GetVariables().ToHashSet();
            Assert.IsTrue(variables.Count == 0);
        }

        /// <summary>
        /// Refer to Title
        /// </summary>
        [TestMethod]
        public void testToStringEasy()
        {
            form = new Formula("1 + 2");
            Formula form2 = new Formula("1+2");
            Assert.AreEqual(form2.ToString(), form.ToString());
        }

        /// <summary>
        /// Refer to Title
        /// </summary>
        [TestMethod]
        public void testToStringEasy1()
        {
            form = new Formula("1 + 2");
            Formula form2 = new Formula("1+2");
            Assert.AreEqual(form2.ToString(), form.ToString());
        }

        /// <summary>
        /// Refer to Title
        /// </summary>
        [TestMethod]
        public void testStringVariables()
        {
            form = new Formula("1 + 2 + x1", s => s.ToUpper(), s => true);
            Formula form2 = new Formula("1+2 +X1");
            Assert.AreEqual(form2.ToString(), form.ToString());
        }

        /// <summary>
        /// Refer to Title
        /// </summary>
        [TestMethod]
        public void testEqualsSame()
        {
            form = new Formula("1 + 3");
            Assert.IsTrue(form.Equals("1 + 3"));
        }

        /// <summary>
        /// Refer to Title
        /// </summary>
        [TestMethod]
        public void testEqualsLarge() 
        {
            form = new Formula("1 + 3 / 2 + 4 - (12 + 9)");
            Assert.IsTrue(form.Equals("1+3/2+4-(12+9)"));
        }

        /// <summary>
        /// Refer to Title
        /// </summary>
        [TestMethod]
        public void testEqualsLargeVariables()
        {
            form = new Formula("1 + r1 / 2 + 4 - (12 + s9)");
            Assert.IsTrue(form.Equals("1+r1/2+4-(12+s9)"));
        }

        /// <summary>
        /// Refer to Title
        /// </summary>
        [TestMethod]
        public void testEqualsLargeVariablesFalse()
        {
            form = new Formula("1 + r1 / 2 + 4 - (12 + s9)");
            Assert.IsFalse(form.Equals("1+r1/2+4-(12+s8)"));
        }

        /// <summary>
        /// Refer to Title
        /// </summary>
        [TestMethod]
        public void testEqualsLargeFalse()
        {
            form = new Formula("1 + 1 / 2 + 4 - (12 + 9)");
            Assert.IsFalse(form.Equals("1+1/2+4-(12+8)"));
        }

        /// <summary>
        /// Refer to Title
        /// </summary>
        [TestMethod]
        public void testEqualsLargeVariablesEqualsSigns()
        {
            form = new Formula("1 + r1 / 2 + 4 - (12 + s9)");
            Formula form2 = new Formula("1+r1/2+4-(12+s9)");
            Assert.IsTrue(form == form2);
        }

        /// <summary>
        /// Refer to Title
        /// </summary>
        [TestMethod]
        public void testNotEqualSignsTrue()
        {
            form = new Formula("1 + r1 / 2 + 4 - (12 + s9)");
            Formula form2 = new Formula("1+r1/2+4-(12+39)");
            Assert.IsTrue(form != form2);
        }

        /// <summary>
        /// Refer to Title
        /// </summary>
        [TestMethod]
        public void testNotEqualSignsFalse()
        {
            form = new Formula("1 + r1 / 2 + 4 - (12 + s9)");
            Formula form2 = new Formula("1+r1/2+4-(12+s9)");
            Assert.IsFalse(form != form2);
        }

        /// <summary>
        /// Refer to Title
        /// </summary>
        [TestMethod]
        public void testGetHashCodeTrue()
        {
            form = new Formula("1 + r1 / 2 + 4 - (12 + s9)");
            Formula form2 = new Formula("1+r1/2+4-(12+s9)");
            Assert.IsTrue(form.GetHashCode() == form2.GetHashCode());
        }

        /// <summary>
        /// Refer to Title
        /// </summary>
        [TestMethod]
        public void testGetHashCodeFalse()
        {
            form = new Formula("1 + r1 / 2 + 4 - (12 + s9)");
            Formula form2 = new Formula("1+r1/2+4-(12+39)");
            Assert.IsFalse(form.GetHashCode() == form2.GetHashCode());
        }

        /// <summary>
        /// Refer to Title
        /// </summary>
        [TestMethod]
        public void testDivideByZero()
        {
            form = new Formula("2/0");
            Object formEvaluate = form.Evaluate(s => 0.0);
            Assert.IsInstanceOfType(formEvaluate, typeof(FormulaError));
        }

        /// <summary>
        /// Refer to Title
        /// </summary>
        [TestMethod]
        public void testVariableNoAssociation()
        {
            form = new Formula("2/r1");
            Object formEvaluate = form.Evaluate(r => 0.0);
            Assert.IsInstanceOfType(formEvaluate, typeof(FormulaError));
        }
    }
}