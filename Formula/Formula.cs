// Skeleton written by Joe Zachary for CS 3500, September 2013
// Read the entire skeleton carefully and completely before you
// do anything else!

// Version 1.1 (9/22/13 11:45 a.m.)

// Change log:
//  (Version 1.1) Repaired mistake in GetTokens
//  (Version 1.1) Changed specification of second constructor to
//                clarify description of how validation works

// (Daniel Kopta) 
// Version 1.2 (9/10/17) 

// Change log:
//  (Version 1.2) Changed the definition of equality with regards
//                to numeric tokens


using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;

namespace SpreadsheetUtilities
{
    /// <summary>
    /// Represents formulas written in standard infix notation using standard precedence
    /// rules.  The allowed symbols are non-negative numbers written using double-precision 
    /// floating-point syntax (without unary preceeding '-' or '+'); 
    /// variables that consist of a letter or underscore followed by 
    /// zero or more letters, underscores, or digits; parentheses; and the four operator 
    /// symbols +, -, *, and /.  
    /// 
    /// Spaces are significant only insofar that they delimit tokens.  For example, "xy" is
    /// a single variable, "x y" consists of two variables "x" and y; "x23" is a single variable; 
    /// and "x 23" consists of a variable "x" and a number "23".
    /// 
    /// Associated with every formula are two delegates:  a normalizer and a validator.  The
    /// normalizer is used to convert variables into a canonical form, and the validator is used
    /// to add extra restrictions on the validity of a variable (beyond the standard requirement 
    /// that it consist of a letter or underscore followed by zero or more letters, underscores,
    /// or digits.)  Their use is described in detail in the constructor and method comments.
    /// </summary>
    public class Formula
    {
        private List<String> formulaTokens;
        private HashSet<String> normalVars;

        /// <summary>
        /// Creates a Formula from a string that consists of an infix expression written as
        /// described in the class comment.  If the expression is syntactically invalid,
        /// throws a FormulaFormatException with an explanatory Message.
        /// 
        /// The associated normalizer is the identity function, and the associated validator
        /// maps every string to true.  
        /// </summary>
        public Formula(String formula) :
            this(formula, s => s, s => true)
        {
        }

        /// <summary>
        /// Creates a Formula from a string that consists of an infix expression written as
        /// described in the class comment.  If the expression is syntactically incorrect,
        /// throws a FormulaFormatException with an explanatory Message.
        /// 
        /// The associated normalizer and validator are the second and third parameters,
        /// respectively.  
        /// 
        /// If the formula contains a variable v such that normalize(v) is not a legal variable, 
        /// throws a FormulaFormatException with an explanatory message. 
        /// 
        /// If the formula contains a variable v such that isValid(normalize(v)) is false,
        /// throws a FormulaFormatException with an explanatory message.
        /// 
        /// Suppose that N is a method that converts all the letters in a string to upper case, and
        /// that V is a method that returns true only if a string consists of one letter followed
        /// by one digit.  Then:
        /// 
        /// new Formula("x2+y3", N, V) should succeed
        /// new Formula("x+y3", N, V) should throw an exception, since V(N("x")) is false
        /// new Formula("2x+y3", N, V) should throw an exception, since "2x+y3" is syntactically incorrect.
        /// </summary>
        public Formula(String formula, Func<string, string> normalize, Func<string, bool> isValid)
        {
            // initiating all the IEnumberables we will use throughout
            formulaTokens = GetTokens(formula).ToList();
            normalVars = new HashSet<String>();

            emptyCheck();

            // initiating all the variables we will use 
            int openParentheses = 0;
            int closedParentheses = 0;
            double tokenValue = 0;
            String previousToken = null;

            CreateFormula(normalize, isValid, ref openParentheses, ref closedParentheses, ref tokenValue, ref previousToken);
        }

        private void CreateFormula(Func<string, string> normalize, Func<string, bool> isValid, ref int openParentheses, ref int closedParentheses, ref double tokenValue, ref string previousToken)
        {
            for (int i = 0; i < formulaTokens.Count(); i++)
            {
                String token = formulaTokens.ElementAt(i);

                tokenValue = TestFirstTokenFormula(tokenValue, i, token);

                // progression through token's to see if they are valid inputs
                if (token.Equals("("))
                {
                    openParentheses++;
                }

                else if (token.Equals(")"))
                {
                    closedParentheses++;
                    // if there are ever more closed parentheses than open ones, throw exception
                    if (closedParentheses > openParentheses)
                    {
                        throw new FormulaFormatException("Invalid formula, cannot have more closed" +
                            "parentheses than open parentheses");
                    }
                }
                //
                else if (token.Equals("+") || token.Equals("-") || token.Equals("*") || token.Equals("/"))
                { }
                // if it's a number, add the toString version to the formulaTokens
                // so it's easier to use .equals later on
                else if (double.TryParse(token, out tokenValue))
                { this.formulaTokens[i] = tokenValue.ToString(); }
                // if Variable is in the right format, and works when normalized, then add it to the 
                // formulaTokens so it's easier later on, and also to the hashSet of variables
                else if (Regex.IsMatch(token, @"[a-zA-Z_](?: [a-zA-Z_]|\d)*"))
                {
                    ChecksVariables(normalize, isValid, i, token);
                }
                // else it's something that we don't want
                else
                {
                    throw new FormulaFormatException("Invalid formula, at least one of the tokens in your" +
                        "formula is invalid");
                }

                tokenValue = TestInvalidLastToken(openParentheses, closedParentheses, tokenValue, i, token);

                tokenValue = TestInvalidFormula(tokenValue, previousToken, token);

                // sets previous token to current token for next time
                previousToken = token;
            }
        }

        private void emptyCheck()
        {
            // if formula is empty, throw exception
            if (formulaTokens.Count() == 0)
            {
                throw new FormulaFormatException("Invalid Formula, formula has no tokens in it");
            }
        }

        private static double TestFirstTokenFormula(double tokenValue, int i, string token)
        {
            // if its the first token, it must be a valid variable, number, or open parentheses
            if (i == 0)
            {
                if (!(Regex.IsMatch(token, @"[a-zA-Z_](?: [a-zA-Z_]|\d)*") || double.TryParse(token, out tokenValue) || token.Equals("(")))
                {
                    throw new FormulaFormatException("Invalid start to fomula, must begin with " +
                        "valid variable, number, or (");
                }
            }

            return tokenValue;
        }

        private void ChecksVariables(Func<string, string> normalize, Func<string, bool> isValid, int i, string token)
        {
            if (isValid(normalize(token)) && Regex.IsMatch(normalize(token), @"[a-zA-Z_](?: [a-zA-Z_]|\d)*"))
            {
                normalVars.Add(normalize(token));
                this.formulaTokens[i] = normalize(token);
            }
            else throw new FormulaFormatException("Invalid formula, a variable is invalid");
        }

        private static double TestInvalidFormula(double tokenValue, string previousToken, string token)
        {
            // checks previous tokens to current tokens
            if (previousToken != null && previousToken != "")
            {
                // if the previous token was an operand or an open parentheses, the next token must be a 
                // variable, number, or open parentheses
                if (previousToken.Equals("(") || previousToken.Equals("+") || previousToken.Equals("-")
                    || previousToken.Equals("*") || previousToken.Equals("/"))
                {
                    if (!(Regex.IsMatch(token, @"[a-zA-Z_](?: [a-zA-Z_]|\d)*") || double.TryParse(token, out _) || token.Equals("(")))
                    {
                        throw new FormulaFormatException("Invalid formula, after an opened parentheses" +
                            " or an operator (+,-,*,/), a number, variable, or opened parentheses must follow");
                    }
                }
                // if the previous token was a number, variable, or closed parentheses, the next token must be an
                // operand or closed parentheses
                else if (double.TryParse(previousToken, out tokenValue) || (Regex.IsMatch(token, @"[a-zA-Z_](?: [a-zA-Z_]|\d)*")
                         || previousToken.Equals(")")))
                {
                    if (!(token.Equals("+") || token.Equals("-") || token.Equals("*") || token.Equals("/")
                           || token.Equals(")")))
                    {
                        throw new FormulaFormatException("Invalid formula, after a valid number, variable" +
                            " or closed parentheses, an operand (+,-,*,/) or closed parentheses must follow");
                    }
                }
            }

            return tokenValue;
        }

        private double TestInvalidLastToken(int openParentheses, int closedParentheses, double tokenValue, int i, string token)
        {
            // if the last token doesn't have a valid input, throw an exception
            if (i == formulaTokens.Count - 1)
            {
                //if the last input is not a variable, number, or ) throw an exception
                if (!(Regex.IsMatch(token, @"[a-zA-Z_](?: [a-zA-Z_]|\d)*", RegexOptions.Singleline) ||
                    double.TryParse(token, out tokenValue) || token.Equals(")")))
                {
                    throw new FormulaFormatException("Invalid formula, must end in " +
                        "valid variable, number, or )");
                }
                // if after the last token, the parentheses don't add up, throw an exception
                else if (openParentheses != closedParentheses)
                {
                    throw new FormulaFormatException("Invalid Formula, not equal number of closed and open" +
                        "parentheses");
                }
            }

            return tokenValue;
        }

        /// <summary>
        /// Evaluates this Formula, using the lookup delegate to determine the values of
        /// variables.  When a variable symbol v needs to be determined, it should be looked up
        /// via lookup(normalize(v)). (Here, normalize is the normalizer that was passed to 
        /// the constructor.)
        /// 
        /// For example, if L("x") is 2, L("X") is 4, and N is a method that converts all the letters 
        /// in a string to upper case:
        /// 
        /// new Formula("x+7", N, s => true).Evaluate(L) is 11
        /// new Formula("x+7").Evaluate(L) is 9
        /// 
        /// Given a variable symbol as its parameter, lookup returns the variable's value 
        /// (if it has one) or throws an ArgumentException (otherwise).
        /// 
        /// If no undefined variables or divisions by zero are encountered when evaluating 
        /// this Formula, the value is returned.  Otherwise, a FormulaError is returned.  
        /// The Reason property of the FormulaError should have a meaningful explanation.
        ///
        /// This method should never throw an exception.
        /// </summary>
        public object Evaluate(Func<string, double> lookup)
        {
            // try catch to catch exceptions
            try
            {

                // inititating operator and number stack
                Stack<String> opStack = new Stack<String>();
                Stack<double> numStack = new Stack<double>();

                // go through all tokens
                foreach (String token in formulaTokens)
                {
                    // if the token is a number, treat it as so
                    if (double.TryParse(token, out double tokenValue))
                    {
                        doubleEvaluation(tokenValue, opStack, numStack);
                    }
                    // if the token is a plus or minus, treat it as so
                    else if (token.Equals("+") || token.Equals("-"))
                    {
                        plusMinusEvaluation(token, opStack, numStack);
                    }
                    // if the token is a multiplication or division sign or an opened parentheses, treat it as so
                    else if (token.Equals("*") || token.Equals("/") || token.Equals("("))
                    {
                        opStack.Push(token);
                    }
                    // if the token is a closed parentheses, treat it as one
                    else if (token.Equals(")"))
                    {
                        plusMinusEvaluation(token, opStack, numStack);

                        // this will be an opened parentheses
                        opStack.Pop();

                        multiplyDivideEvaluation(opStack, numStack);
                    }
                    // else it is a variable
                    else
                    {
                        double value = lookup(token);
                        doubleEvaluation(value, opStack, numStack);
                    }
                }

                // if this occurs it is a plus or minus sign
                if (opStack.Count > 0)
                {
                    plusMinusEvaluation("0", opStack, numStack);
                }

                return numStack.Pop();

            } 
            // happens if the lookup deleagate can't find an associating value
            catch(ArgumentException)
            {
                return new FormulaError("Invalid Formula, a variable is invalid");
            }
            // happens when divide by zero occurs
            catch(DivideByZeroException)
            {
                return new FormulaError("Invalid Formula, divide by zero");
            }
        
        }

        /// <summary>
        /// Helper method for when a number is found, checks to see if the operator stack
        /// has a multiply or divide sign, and uses it if it does. Otherwise it pushs the new token
        /// </summary>
        /// <param name="token"> token we just got</param>
        /// <param name="opStack"> operator stack</param>
        /// <param name="numStack"> number stack</param>
        /// <exception cref="DivideByZeroException"> if divide by zero occurs</exception>
        private void doubleEvaluation(double token, Stack<String> opStack, Stack<double> numStack)
        {
            if (opStack.Count > 0 && (opStack.Peek().Equals("*") || opStack.Peek().Equals("/")))
            {
                if (opStack.Peek() == "*")
                {
                    String op = opStack.Pop();
                    double value2 = numStack.Pop();
                    double value3 = value2 * token;
                    numStack.Push(value3);
                    return;
                }
                else
                {
                    String op = opStack.Pop();
                    double value2 = numStack.Pop();
                    if (token == 0) throw new DivideByZeroException();
                    double value3 = value2 / token;
                    numStack.Push(value3);
                    return;
                }
            }
            numStack.Push(token);
        }

        /// <summary>
        /// Helper method for when plus or minus token is found. First it checks to 
        /// see if there is another plus or minus sign at the top of the operator stack and
        /// does what it needs to do if so and push's the new number. Then regardless of the 
        /// previous step it pushes the operand to the stack
        /// </summary>
        /// <param name="token"> new plus or minus sign </param>
        /// <param name="opStack"> operator stack </param>
        /// <param name="numStack"> number stack </param>
        private void plusMinusEvaluation(String token, Stack<String> opStack, Stack<double> numStack)
        {
            if (opStack.Count > 0 && (opStack.Peek().Equals("+") || opStack.Peek().Equals("-")))
            {
                double value1 = numStack.Pop();
                double value2 = numStack.Pop();
                String op = opStack.Pop();

                //if token is plus, finds the sum, else subtract
                if (op.Equals("+")) 
                    numStack.Push(value2 + value1);
                else 
                    numStack.Push(value2 - value1);
            }
            
            // if the token is a closed parentheses, don't add it to the stack
            if(token != ")")    opStack.Push(token);
        }

        /// <summary>
        /// If new token is closed parentheses, this method calls, it's to check to see
        /// if at the top of the oeprator stack there is a multiplication or division sign 
        /// and if so it does what the sign tells it to do
        /// </summary>
        /// <param name="opStack"> operator stack </param>
        /// <param name="numStack"> number stack </param>
        /// <exception cref="DivideByZeroException"> if division by zero occurs </exception>
        private void multiplyDivideEvaluation(Stack<String> opStack, Stack<double> numStack)
        {
            if (opStack.Count > 0 && (opStack.Peek().Equals("*") || opStack.Peek().Equals("/")) && numStack.Count >= 2)
            {
                if (opStack.Peek().Equals("*"))
                {
                    String op = opStack.Pop();
                    double value1 = numStack.Pop();
                    double value2 = numStack.Pop();
                    double value3 = value2 * value1;
                    numStack.Push(value3);
                }
                else
                {
                    String op = opStack.Pop();
                    double value1 = numStack.Pop();
                    double value2 = numStack.Pop();
                    if (value1 == 0) throw new DivideByZeroException();
                    double value3 = value2 / value1;
                    numStack.Push(value3);
                }
            }
        }

        /// <summary>
        /// Enumerates the normalized versions of all of the variables that occur in this 
        /// formula.  No normalization may appear more than once in the enumeration, even 
        /// if it appears more than once in this Formula.
        /// 
        /// For example, if N is a method that converts all the letters in a string to upper case:
        /// 
        /// new Formula("x+y*z", N, s => true).GetVariables() should enumerate "X", "Y", and "Z"
        /// new Formula("x+X*z", N, s => true).GetVariables() should enumerate "X" and "Z".
        /// new Formula("x+X*z").GetVariables() should enumerate "x", "X", and "z".
        /// </summary>
        public IEnumerable<String> GetVariables()
        {
            HashSet<String> clone = new HashSet<String>(normalVars);
            return clone;
        }

        /// <summary>
        /// Returns a string containing no spaces which, if passed to the Formula
        /// constructor, will produce a Formula f such that this.Equals(f).  All of the
        /// variables in the string should be normalized.
        /// 
        /// For example, if N is a method that converts all the letters in a string to upper case:
        /// 
        /// new Formula("x + y", N, s => true).ToString() should return "X+Y"
        /// new Formula("x + Y").ToString() should return "x+Y"
        /// </summary>
        public override string ToString()
        {
            // all variables and doubles should be normalized
            // so we can just add them in
            String toString = "";
            foreach(String token in this.formulaTokens)
            {
                toString += token;
            }
            return toString;
        }

        /// <summary>
        ///  <change> make object nullable </change>
        ///
        /// If obj is null or obj is not a Formula, returns false.  Otherwise, reports
        /// whether or not this Formula and obj are equal.
        /// 
        /// Two Formulae are considered equal if they consist of the same tokens in the
        /// same order.  To determine token equality, all tokens are compared as strings 
        /// except for numeric tokens and variable tokens.
        /// Numeric tokens are considered equal if they are equal after being "normalized" 
        /// by C#'s standard conversion from string to double, then back to string. This 
        /// eliminates any inconsistencies due to limited floating point precision.
        /// Variable tokens are considered equal if their normalized forms are equal, as 
        /// defined by the provided normalizer.
        /// 
        /// For example, if N is a method that converts all the letters in a string to upper case:
        ///  
        /// new Formula("x1+y2", N, s => true).Equals(new Formula("X1  +  Y2")) is true
        /// new Formula("x1+y2").Equals(new Formula("X1+Y2")) is false
        /// new Formula("x1+y2").Equals(new Formula("y2+x1")) is false
        /// new Formula("2.0 + x7").Equals(new Formula("2.000 + x7")) is true
        /// </summary>
        public override bool Equals(object? obj)
        {
            // create new formuls with paramater, and initiate stings
            Formula objFormula = new Formula(obj.ToString());
            String objFormula_curr;
            String thisFormula_curr;

            // go through both formulas to see if they are equal
            // if at any time they aren't equal, it returns false, true otherwise
            for(int i = 0; i < this.formulaTokens.Count(); i++)
            {
                // get elements at their respective location
                objFormula_curr  = objFormula.formulaTokens.ElementAt(i);
                thisFormula_curr = this.formulaTokens.ElementAt(i);

                // if the tokens are both numbers, check if the numbers are the same
                if(double.TryParse(objFormula_curr, out double value1) && double.TryParse(thisFormula_curr, out double value2))
                {
                    // changes it into a toString() to make sure the floats will equal each other
                    if (value1.ToString() != value2.ToString())
                        return false;
                }
                // I don't have to normalize these current tokens as I already added the normalized ones to the 
                // forula tokens in the constructor method
                else if(objFormula_curr != thisFormula_curr)
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        ///   <change> We are now using Non-Nullable objects.  Thus neither f1 nor f2 can be null!</change>
        /// Reports whether f1 == f2, using the notion of equality from the Equals method.
        /// 
        /// </summary>
        public static bool operator == (Formula f1, Formula f2)
        {
            return f1.Equals((Object) f2);
        }

        /// <summary>
        ///   <change> We are now using Non-Nullable objects.  Thus neither f1 nor f2 can be null!</change>
        ///   <change> Note: != should almost always be not ==, if you get my meaning </change>
        ///   Reports whether f1 != f2, using the notion of equality from the Equals method.
        /// </summary>
        public static bool operator !=(Formula f1, Formula f2)
        {
            // if they are equal, then the operator is false
            if(f1 == f2)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Returns a hash code for this Formula.  If f1.Equals(f2), then it must be the
        /// case that f1.GetHashCode() == f2.GetHashCode().  Ideally, the probability that two 
        /// randomly-generated unequal Formulae have the same hash code should be extremely small.
        /// </summary>
        public override int GetHashCode()
        {
            int hashCode = 0;
            foreach(var token in this.formulaTokens)
            {
                hashCode += token.GetHashCode();
            }
            return hashCode;
        }


        /// <summary>
        /// Given an expression, enumerates the tokens that compose it.  Tokens are left paren;
        /// right paren; one of the four operator symbols; a string consisting of a letter or underscore
        /// followed by zero or more letters, digits, or underscores; a double literal; and anything that doesn't
        /// match one of those patterns.  There are no empty tokens, and no token contains white space.
        /// </summary>
        private static IEnumerable<string> GetTokens(String formula)
        {
            // Patterns for individual tokens
            String lpPattern = @"\(";
            String rpPattern = @"\)";
            String opPattern = @"[\+\-*/]";
            String varPattern = @"[a-zA-Z_](?: [a-zA-Z_]|\d)*";
            String doublePattern = @"(?: \d+\.\d* | \d*\.\d+ | \d+ ) (?: [eE][\+-]?\d+)?";
            String spacePattern = @"\s+";

            // Overall pattern
            String pattern = String.Format("({0}) | ({1}) | ({2}) | ({3}) | ({4}) | ({5})",
                                            lpPattern, rpPattern, opPattern, varPattern, doublePattern, spacePattern);

            // Enumerate matching tokens that don't consist solely of white space.
            foreach (String s in Regex.Split(formula, pattern, RegexOptions.IgnorePatternWhitespace))
            {
                if (!Regex.IsMatch(s, @"^\s*$", RegexOptions.Singleline))
                {
                    yield return s;
                }
            }

        }
    }

    /// <summary>
    /// Used to report syntactic errors in the argument to the Formula constructor.
    /// </summary>
    public class FormulaFormatException : Exception
    {
        /// <summary>
        /// Constructs a FormulaFormatException containing the explanatory message.
        /// </summary>
        public FormulaFormatException(String message)
            : base(message)
        {
        }
    }

    /// <summary>
    /// Used as a possible return value of the Formula.Evaluate method.
    /// </summary>
    public struct FormulaError
    {
        /// <summary>
        /// Constructs a FormulaError containing the explanatory reason.
        /// </summary>
        /// <param name="reason"></param>
        public FormulaError(String reason)
            : this()
        {
            Reason = reason;
        }

        /// <summary>
        ///  The reason why this FormulaError was created.
        /// </summary>
        public string Reason { get; private set; }
    }
}


// <change>
//   If you are using Extension methods to deal with common stack operations (e.g., checking for
//   an empty stack before peeking) you will find that the Non-Nullable checking is "biting" you.
//
//   To fix this, you have to use a little special syntax like the following:
//
//       public static bool OnTop<T>(this Stack<T> stack, T element1, T element2) where T : notnull
//
//   Notice that the "where T : notnull" tells the compiler that the Stack can contain any object
//   as long as it doesn't allow nulls!
// </change>
