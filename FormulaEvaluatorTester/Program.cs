
using FormulaEvaluator;

/// <summary>
/// This method is used to test the FormulaEvaluator class on it's Evaluate
/// method, and does this by many boolean tests below
/// </summary>
public static class FormulaEvaluatorTest
{
    /// <summary>
    /// This method tests adding 2 numbers together
    /// </summary>
    /// <returns></returns>
    public static bool TestAdd2num()
    {
        Evaluator ev = new Evaluator();
        int value;
        value = Evaluator.Evaluate("3 + 2 * (12-1)", s => 1);
        return value == 25;
    }

    /// <summary>
    /// This method tests adding 3 numbsers together
    /// </summary>
    /// <returns></returns>
    public static bool TestAdd3num()
    {
        Evaluator ev = new Evaluator();
        int value;
        value = Evaluator.Evaluate("1 + 3 + 5", s => 1);
        return value == 9;
    }

    /// <summary>
    /// This method tests subtracting 2 numbers
    /// </summary>
    /// <returns></returns>
    public static bool TestSubtract2num()
    {
        Evaluator ev = new Evaluator();
        int value;
        value = Evaluator.Evaluate("2 - 1", s => 1);
        return value == 1;
    }

    /// <summary>
    ///     /// This method tests subtracting 3 numbers
    /// </summary>
    /// <returns></returns>
    public static bool TestSubtract3num()
    {
        Evaluator ev = new Evaluator();
        int value;
        value = Evaluator.Evaluate("9 - 4 - 2", s => 1);
        return value == 3;
    }

    /// <summary>
    /// This method tests multiplying 2 numbers
    /// </summary>
    /// <returns></returns>
    public static bool TestMultiply2num()
    {
        Evaluator ev = new Evaluator();
        int value;
        value = Evaluator.Evaluate("1 * 1", s => 1);
        return value == 1;
    }

    /// <summary>
    /// This method tests multiplying 3 numbers
    /// </summary>
    /// <returns></returns>
    public static bool TestMultiply3num()
    {
        Evaluator ev = new Evaluator();
        int value;
        value = Evaluator.Evaluate("2 * 4 * 2", s => 1);
        return value == 16;
    }

    /// <summary>
    /// This method tests dividing 2 numbers
    /// </summary>
    /// <returns></returns>
    public static bool TestDivide2num()
    {
        Evaluator ev = new Evaluator();
        int value;
        value = Evaluator.Evaluate("1 / 1", s => 1);
        return value == 1;
    }

    /// <summary>
    /// This method tests dividing 3 numbers
    /// </summary>
    /// <returns></returns>
    public static bool TestDivide3num()
    {
        Evaluator ev = new Evaluator();
        int value;
        value = Evaluator.Evaluate("6 / 2 / 3", s => 1);
        return value == 1;
    }

    /// <summary>
    /// This method tests a single number
    /// </summary>
    /// <returns></returns>
    public static bool TestSingleNum()
    {
        Evaluator ev = new Evaluator();
        int value;
        value = Evaluator.Evaluate("6", s => 1);
        return value == 6;
    }

    /// <summary>
    /// This mehtod tests a double digit number
    /// </summary>
    /// <returns></returns>
    public static bool TestDoubleNum()
    {
        Evaluator ev = new Evaluator();
        int value;
        value = Evaluator.Evaluate("02", s => 1);
        return value == 02;
    }

    /// <summary>
    /// This method tests a signle full Parentheses
    /// </summary>
    /// <returns></returns>
    public static bool TestSingleParentheses()
    {
        Evaluator ev = new Evaluator();
        int value;
        value = Evaluator.Evaluate("(4)", s => 1);
        return value == 4;
    }

    /// <summary>
    /// This method tests a double full parentheses
    /// </summary>
    /// <returns></returns>
    public static bool TestDoubleParentheses()
    {
        Evaluator ev = new Evaluator();
        int value;
        value = Evaluator.Evaluate("((3))", s => 1);
        return value == 3;
    }

    /// <summary>
    /// This method tests doing a plus then minus operation
    /// </summary>
    /// <returns></returns>
    public static bool TestPlusMinus()
    {
        Evaluator ev = new Evaluator();
        int value;
        value = Evaluator.Evaluate("3+7-2", s => 1);
        return value == 8;
    }

    /// <summary>
    /// This method tests doing a minus then plus operation
    /// </summary>
    /// <returns></returns>
    public static bool TestMinusPlus()
    {
        Evaluator ev = new Evaluator();
        int value;
        value = Evaluator.Evaluate("10-4+3", s => 1);
        return value == 9;
    }

    /// <summary>
    /// This method tests doing an add then multiplication operation
    /// </summary>
    /// <returns></returns>
    public static bool TestAddMult()
    {
        Evaluator ev = new Evaluator();
        int value;
        value = Evaluator.Evaluate("4 + 3 * 6", s => 1);
        return value == 22;
    }

    /// <summary>
    /// This method tests doing a very large expression
    /// </summary>
    /// <returns></returns>
    public static bool TestBigExpression()
    {
        Evaluator ev = new Evaluator();
        int value;
        value = Evaluator.Evaluate("(3 + 4) * 2 /(7)", s => 1);
        return value == 2;
    }

    /// <summary>
    /// This method tests what extra spaces does to the operations
    /// </summary>
    /// <returns></returns>
    public static bool TestExtraSpaces()
    {
        Evaluator ev = new Evaluator();
        int value;
        value = Evaluator.Evaluate("  1+2  / 2  * 4", s => 1);
        return value == 5;
    }

    /// <summary>
    /// This method tests using many parentheses
    /// </summary>
    /// <returns></returns>
    public static bool TestManyParentheses()
    {
        Evaluator ev = new Evaluator();
        int value;
        value = Evaluator.Evaluate("((3+ 4) * 2) / 2 +9", s => 1);
        return value == 16;
    }

    /// <summary>
    /// This method tests the PEMDAS rules
    /// </summary>
    /// <returns></returns>
    public static bool TestPEMDAS()
    {
        Evaluator ev = new Evaluator();
        int value;
        value = Evaluator.Evaluate("1+2*3/6-1+6", s => 1);
        return value == 7;
    }

    /// <summary>
    /// This method tests the argument exception on a negative number
    /// </summary>
    /// <returns></returns>
    public static bool TestArgumentExceptionNegative()
    {
        try
        {
            Evaluator ev = new Evaluator();
            int value;
            value = Evaluator.Evaluate("-5", s => 1);
            return value == 0;
        }
        catch (ArgumentException)
        {
            //My Code recognized it is only supposed to deal with non-negative integers, making this
            //throw an arugment exception, as planned
            return true;
        }
    }

    /// <summary>
    /// This method tests the argument exception on a plus but only one num
    /// </summary>
    /// <returns></returns>
    public static bool TestArgumentExceptionPos()
    {
        try
        {
            Evaluator ev = new Evaluator();
            int value;
            value = Evaluator.Evaluate("+5", s => 1);
            return value == 0;
        }
        catch (ArgumentException)
        {
            //My Code recognized there is only one number next to the 5, which is an error
            //and throws an exception accordingly
            return true;
        }
    }

    /// <summary>
    /// This method tests the argument exception on only one left parentheses
    /// </summary>
    /// <returns></returns>
    public static bool TestArgException1ParenthesesLeft()
    {
        try
        {
            Evaluator ev = new Evaluator();
            int value;
            value = Evaluator.Evaluate("(12", s => 1);
            return value == 0;
        }
        catch (ArgumentException)
        {
            //My Code recognized that only one parentheses doesn't work, and throws an exception
            return true;
        }
    }

    /// <summary>
    /// This method tests the argument exception on only one right parentheses
    /// </summary>
    /// <returns></returns>
    public static bool TestArgException1ParenthesesRight()
    {
        try
        {
            Evaluator ev = new Evaluator();
            int value;
            value = Evaluator.Evaluate("3)", s => 1);
            return value == 0;
        }
        catch (ArgumentException)
        {
            //My Code recognized that only one parentheses doesn't work, and throws an exception
            return true;
        }
    }

    /// <summary>
    /// This method tests the argument exception on only one number with a multiplication sign
    /// </summary>
    /// <returns></returns>
    public static bool TestArgOneMult()
    {
        try
        {
            Evaluator ev = new Evaluator();
            int value;
            value = Evaluator.Evaluate("*12", s => 1);
            return value == 0;
        }
        catch (ArgumentException)
        {
            //My Code recognized it is only one number next to the multiplication sign and throws 
            //an exception accordingly
            return true;
        }
    }

    /// <summary>
    /// This method tests the argument exception on only one number with a division sign
    /// </summary>
    /// <returns></returns>
    public static bool TestArgOneDivide()
    {
        try
        {
            Evaluator ev = new Evaluator();
            int value;
            value = Evaluator.Evaluate("/12", s => 1);
            return value == 0;
        }
        catch (ArgumentException)
        {
            //My Code recognized it is only one number next to the divison sign and throws 
            //an exception accordingly
            return true;
        }
    }

    /// <summary>
    /// This method tests dividing by 0
    /// </summary>
    /// <returns></returns>
    public static bool TestArgDivideBy0()
    {
        try
        {
            Evaluator ev = new Evaluator();
            int value;
            value = Evaluator.Evaluate("3/0", s => 1);
            return value == 0;
        }
        catch (ArgumentException)
        {
            //My Code recognized it is attempting to divide by zero, and throws an exception
            //accordingly
            return true;
        }
    }

    /// <summary>
    /// This method tests too many plus signs
    /// </summary>
    /// <returns></returns>
    public static bool TestTooManyOpsPlus()
    {
        try
        {
            Evaluator ev = new Evaluator();
            int value;
            value = Evaluator.Evaluate("3 + 1 +", s => 1);
            return value == 0;
        }
        catch (ArgumentException)
        {
            //My Code recognized there are too many operations, and threw an exception accordingly
            return true;
        }
    }

    /// <summary>
    /// This method tests too many minus signs
    /// </summary>
    /// <returns></returns>
    public static bool TestTooManyOpsMinus()
    {
        try
        {
            Evaluator ev = new Evaluator();
            int value;
            value = Evaluator.Evaluate("3 - 1 -", s => 1);
            return value == 0;
        }
        catch (ArgumentException)
        {
            //My Code recognized there are too many operations, and threw an exception accordingly
            return true;
        }
    }

    /// <summary>
    /// This method tests too many multiplaction signs
    /// </summary>
    /// <returns></returns>
    public static bool TestTooManyOpsMult()
    {
        try
        {
            Evaluator ev = new Evaluator();
            int value;
            value = Evaluator.Evaluate("3 * 1 *", s => 1);
            return value == 0;
        }
        catch (ArgumentException)
        {
            //My Code recognized there are too many operations, and threw an exception accordingly
            return true;
        }
    }

    /// <summary>
    /// This method tests too many division signs
    /// </summary>
    /// <returns></returns>
    public static bool TestTooManyOpsDivide()
    {
        try
        {
            Evaluator ev = new Evaluator();
            int value;
            value = Evaluator.Evaluate("3 / 1 /", s => 1);
            return value == 0;
        }
        catch (ArgumentException)
        {
            //My Code recognized there are too many operations, and threw an exception accordingly
            return true;
        }
    }

    /// <summary>
    /// This code tests a very basic just delegate test case
    /// </summary>
    /// <returns></returns>
    public static bool TestDelegateBasic()
    {
        Evaluator ev = new Evaluator();
        int value;
        value = Evaluator.Evaluate("s", s => 2);
        return value == 2;
    }

    /// <summary>
    /// This method is used to test using a delegate in an elaborate large expression
    /// </summary>
    /// <returns></returns>
    public static bool TestDelegateLargeExpression()
    {
        Evaluator ev = new Evaluator();
        int value;
        value = Evaluator.Evaluate("S + 4 + (3 * 2) / 6", S => 2);
        return value == 7;
    }

    /// <summary>
    /// This main function is where all the other methods are tested
    /// </summary>
    /// <param name="args"></param>
    static void Main(string[] args)
    {
        Console.WriteLine(TestAdd2num());
        Console.WriteLine(TestAdd3num());
        Console.WriteLine(TestSubtract2num());
        Console.WriteLine(TestSubtract3num());
        Console.WriteLine(TestMultiply2num());
        Console.WriteLine(TestMultiply3num());
        Console.WriteLine(TestDivide2num());
        Console.WriteLine(TestDivide3num());
        Console.WriteLine(TestSingleNum());
        Console.WriteLine(TestDoubleNum());
        Console.WriteLine(TestSingleParentheses());
        Console.WriteLine(TestDoubleParentheses());
        Console.WriteLine(TestPlusMinus());
        Console.WriteLine(TestMinusPlus());
        Console.WriteLine(TestAddMult());
        Console.WriteLine(TestBigExpression());
        Console.WriteLine(TestExtraSpaces());
        Console.WriteLine(TestManyParentheses());
        Console.WriteLine(TestPEMDAS());
        Console.WriteLine(TestArgumentExceptionNegative());
        Console.WriteLine(TestArgException1ParenthesesLeft());
        Console.WriteLine(TestArgException1ParenthesesRight());
        Console.WriteLine(TestArgOneDivide());
        Console.WriteLine(TestArgOneMult());
        Console.WriteLine(TestArgDivideBy0());
        Console.WriteLine(TestArgumentExceptionPos());
        Console.WriteLine(TestTooManyOpsPlus());
        Console.WriteLine(TestTooManyOpsMinus());
        Console.WriteLine(TestTooManyOpsMult());
        Console.WriteLine(TestTooManyOpsDivide());
        Console.WriteLine(TestDelegateBasic());
        Console.WriteLine(TestDelegateLargeExpression());
    }

}
