using System.Runtime.CompilerServices;

namespace FormulaEvaluator

{
    using System.Text.RegularExpressions;

    /// <summary>
    /// This class evalues a given mathematical expression in PEMDAS
    /// progression. It can also use variable notation and find the correct
    /// answer. Part of a larger project of creating a application simmilar to 
    /// Microsoft Excel
    /// </summary>
    public class Evaluator
    {
        /// <summary>
        /// This Lookup delegate method is used 
        /// for Variables to lookup their value/if they have a 
        /// value. If not it throws an exception, if it does it returns
        /// the integer associated with the variable
        /// </summary>
        /// <param name="variable_name">
        /// Paramater is the variable that will be looked at
        /// </param>
        /// <returns>
        /// Return is an integer of the associated variable, or
        /// throws an exception if no integer is associated
        /// </returns>
        public delegate int Lookup(String variable_name);

        /// <summary>
        /// This method is used to evaluate the paramater
        /// expression given, using PEMDAS and stacks. 
        /// Can also evaluate variables
        /// </summary>
        /// <param name="expression">
        /// This is a string of the expression that needs to be
        /// evaluated
        /// </param>
        /// <param name="variableEvaluator">
        /// This paramater is the evaluator for any given variables
        /// </param>
        /// <returns>
        /// returns an int of the evaluated expression or 
        /// throws an exception if the method doesn't work
        /// </returns>
        /// <exception cref="ArgumentException"></exception>
        public static int Evaluate(String expression,
                                   Lookup variableEvaluator)
        {
            // The following lines split the String into tokens in an array
            // to be iterated over
            if(expression.Equals(""))
            {
                throw new ArgumentException();
            }
            string[] substrings =
                    Regex.Split(expression, "(\\()|(\\))|(-)|(\\+)|(\\*)|(/)");
            Stack<int> numStack = new Stack<int>();
            Stack<String> opStack = new Stack<String>();

            int currentSum;
            int value1;
            int value2;

            for (int i = 0; i < substrings.Length; i++)
            {
                string s = substrings[i];
                // doesn't go through if it's a blank token
                if (s == " ")
                {
                    continue;
                }
                if(s == "xx")
                {
                    throw new ArgumentException();
                }
                // takes out the blank spaces
                s = s.Replace(" ", "");

                // if s is an integer in string form, assigns value2 to be the int version of s
                if (int.TryParse(s, out int value3))
                {
                    value1 = IntegerEvaluation(value3, opStack, numStack);
                    numStack.Push(value1);
                }

                else if (Regex.IsMatch(s, @"[a-zA-Z]+\d+"))
                {
                    value3 = variableEvaluator(s);
                    value1 = IntegerEvaluation(value3, opStack, numStack);
                    numStack.Push(value1);

                }

                // if token is plus or minus, proceed
                else if (s == "+" || s == "-")
                {

                    if (opStack.Count > 0 && (opStack.Peek() == "+" || opStack.Peek() == "-"))
                    {

                        // if expression won't evaluate, throw exception
                        if (numStack.Count < 2)
                        {
                            throw new ArgumentException();
                        }

                        value1 = numStack.Pop();
                        value2 = numStack.Pop();

                        //if token is plus, finds the sum
                        if (opStack.Peek() == "+")
                        {
                            opStack.Pop();

                            currentSum = value1 + value2;
                            numStack.Push(currentSum);
                        }
                        //if token is minus, finds the difference
                        else if (opStack.Peek() == "-")
                        {
                            opStack.Pop();

                            currentSum = value2 - value1;
                            numStack.Push(currentSum);
                        }

                    }

                    // if token is plus, push into opStack
                    if (s == "+")
                    {
                        opStack.Push("+");

                    }
                    else
                    {
                        opStack.Push("-");
                    }
                }
                // if token is * or / add to opStack
                else if (s == "*" || s == "/")
                {
                    if (s == "*")
                    {
                        opStack.Push("*");
                    }
                    else
                    {
                        opStack.Push("/");
                    }
                }
                // if token is (, add to opStack
                else if (s == "(")
                {
                    opStack.Push("(");
                }
                // if token is ), proceed with following steps
                else if (s == ")")
                {
                    String opPeek = "";
                    if (opStack.Count > 0)
                    {
                        opPeek = opStack.Peek();
                    }

                    if (opPeek == "+" || opPeek == "-")
                    {
                        if (numStack.Count < 2)
                        {
                            throw new ArgumentException();
                        }
                        // if it says to add, add
                        else if (opPeek == "+")
                        {
                            opStack.Pop();

                            value1 = numStack.Pop();
                            value2 = numStack.Pop();
                            currentSum = value1 + value2;

                            numStack.Push(currentSum);
                        }
                        // if operator stack says to subtract, subtract
                        else
                        {
                            opStack.Pop();

                            value1 = numStack.Pop();
                            value2 = numStack.Pop();
                            currentSum = value2 - value1;

                            numStack.Push(currentSum);
                        }
                    }

                    if (opStack.Count() > 0 && opStack.Peek() == "(")
                    {
                        opStack.Pop();
                    }
                    // if expression won't evaluate, throw exception
                    else
                    {
                        throw new ArgumentException();
                    }

                    if (opStack.Count > 0 && (opStack.Peek() == "*" || opStack.Peek() == "/"))
                    {
                        // if expression doesn't work, throw exception
                        if (numStack.Count < 2)
                        {
                            throw new ArgumentException();
                        }
                        // if says to multiply, multiply
                        else if (opStack.Peek() == "*")
                        {
                            currentSum = MultiplyStack(numStack, opStack);
                            numStack.Push(currentSum);
                        }
                        // if says to divide, divide
                        else
                        {
                            currentSum = DivideStack(numStack, opStack);
                            numStack.Push(currentSum);
                        }
                    }
                }

            }

            // if still need to do an operation
            if (opStack.Count != 0)
            {
                // if expression won't evaluate, throw exception
                if (opStack.Count != 1 || numStack.Count != 2)
                {
                    throw new ArgumentException();
                }

                // if says to add, add
                if (opStack.Peek() == "+")
                {
                    opStack.Pop();

                    value1 = numStack.Pop();
                    value2 = numStack.Pop();
                    currentSum = value1 + value2;
                    numStack.Push(currentSum);
                }
                // else it should be minus, so minus
                else
                {
                    opStack.Pop();
                    value1 = numStack.Pop();
                    value2 = numStack.Pop();
                    currentSum = value2 - value1;
                    numStack.Push(currentSum);
                }
            }

            return numStack.Pop();
        }

        /// <summary>
        /// This helper method is so I don't have to write the divide
        /// program again
        /// </summary>
        /// <param name="numStack"></param>
        /// <param name="opStack"></param>
        /// <returns></returns>
        /// <exception cref="DivideByZeroException"></exception>
        private static int DivideStack(Stack<int> numStack, Stack<String> opStack)
        {
            opStack.Pop();

            int value1 = numStack.Pop();
            int value2 = numStack.Pop();

            if (value2 == 0)
            {
                throw new ArgumentException();
            }
            int currentSum = value2 / value1;
            return currentSum;
        }

        /// <summary>
        /// This helper method is so I don't have to write the divide
        /// program again
        /// </summary>
        /// <param name="numStack"></param>
        /// <param name="opStack"></param>
        /// <param name="value2"></param>
        /// <returns></returns>
        /// <exception cref="DivideByZeroException"></exception>
        private static int DivideStack(Stack<int> numStack, Stack<String> opStack, int value2)
        {
            opStack.Pop();

            int value1 = numStack.Pop();

            if (value2 == 0)
            {
                throw new ArgumentException();
            }
            int currentSum = value1 / value2;
            return currentSum;
        }

        /// <summary>
        /// This helper method is so I don't have to write the multiply
        /// program again
        /// </summary>
        /// <param name="numStack"></param>
        /// <param name="opStack"></param>
        /// <returns></returns>
        private static int MultiplyStack(Stack<int> numStack, Stack<String> opStack)
        {
            opStack.Pop();

            int value1 = numStack.Pop();
            int value2 = numStack.Pop();
            int currentSum = value1 * value2;
            return currentSum;
        }

        /// <summary>
        /// This helper method is so I don't have to write the multiply
        /// program again
        /// </summary>
        /// <param name="numStack"></param>
        /// <param name="opStack"></param>
        /// <param name="value2"></param>
        /// <returns></returns>
        private static int MultiplyStack(Stack<int> numStack, Stack<String> opStack, int value2)
        {
            opStack.Pop();

            int value1 = numStack.Pop();
            int currentSum = value1 * value2;
            return currentSum;
        }

        private static int IntegerEvaluation(int val, Stack<String> opStack, Stack<int> numStack)
        {
            int currentSum;
            if (opStack.Count > 0 && (opStack.Peek() == "*" || opStack.Peek() == "/") && numStack.Count == 0)
            {
                throw new ArgumentException();
            }

            // if needs to multiply, pop's it and multiplies
            if (opStack.Count > 0 && opStack.Peek() == "*")
            {
                currentSum = MultiplyStack(numStack, opStack, val);
                return currentSum;
            }
            // if needs to divide, pop's it and divides
            else if (opStack.Count > 0 && opStack.Peek() == "/")
            {
                currentSum = DivideStack(numStack, opStack, val);
                return currentSum;
            }
            // if no need to multiply or divide, push into stack
            else
            {
                return val;
            }
        }
    }
}
