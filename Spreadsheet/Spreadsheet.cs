// implementation written by Nate Stovak for CS 3500, February 2023.
// Version 1.1 (Create Cell class)
// Version 1.2 (Do Set methods and other helper methods)
// Version 1.3 (Finished debugging)

/// Author: Nate Stovak
/// Partner: None
/// Date:      9 - Feb - 2023
/// Course: CS 3500, University of Utah, School of Computing
/// GitHub ID: Stovakk
/// Date:	   9 - Feb - 2023
/// Solution: Spreadsheet
/// Copyright: CS 3500 and Nate Stovak,


using SpreadsheetUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace SS
{

    /// <summary>
    /// This class creates a spreadsheet using cells in a cell class I created
    /// at the bottom, the class is able to set and get cells and their contents
    /// along with find any excpetions needed
    /// </summary>
    public class Spreadsheet : AbstractSpreadsheet
    {
        private Dictionary<String, cell> spread;
        private DependencyGraph dg;

        /// <summary>
        /// Creates a new spreadsheet
        /// </summary>
        public Spreadsheet() 
        {
            spread = new Dictionary<string, cell>();
            dg = new DependencyGraph();
        }

        /// <inheritdoc/>
        public override object GetCellContents(string name)
        {
            if(name == null || !((Regex.IsMatch(name, @"^[a-zA-Z_](?: [a-zA-Z_]|\d)*$", RegexOptions.Singleline))))
                throw new InvalidNameException();

            cell content;

            // If the cell has something in it, return it
            if (spread.TryGetValue(name, out content))
                return content.Content;
            else 
                return "";
        }

        /// <inheritdoc/>
        public override IEnumerable<string> GetNamesOfAllNonemptyCells()
        {
            return spread.Keys;
        }

        /// <inheritdoc/>
        public override ISet<string> SetCellContents(string name, double number)
        {
            checkExceptionsSetCell(name, "NA");

            cell cell = new cell(number);

            // if cell already has something in it, replace it, otherwise make new cell
            if (spread.ContainsKey(name))
            {
                spread[name] = cell;
            }
            else
            {
                spread.Add(name, cell);
            }

            dg.ReplaceDependees(name, new HashSet<String>());

            // create hashset of cells to recalculate after creating new one
            HashSet<String> recalculateDependees = GetCellsToRecalculate(name).ToHashSet<String>();
            return recalculateDependees;
        }

        /// <inheritdoc/>
        public override ISet<string> SetCellContents(string name, string text)
        {
            checkExceptionsSetCell(name, text);

            // if cell has no content, remove it from dictionary
            if (text.Equals(""))
            {
                spread.Remove(name);
            }
            else
            { 
                cell cell = new cell(text);

                // if cell already has something in it, replace it, otherwise make new cell
                if (spread.ContainsKey(name))
                {
                    spread[name] = cell;
                }
                else
                {
                    spread.Add(name, cell);
                }
            }

            dg.ReplaceDependees(name, new HashSet<String>());

            // create hashset of cells to recalculate after creating new one
            HashSet<String> recalculateDependees = GetCellsToRecalculate(name).ToHashSet<String>();
            return recalculateDependees;
        }

        /// <inheritdoc/>
        public override ISet<string> SetCellContents(string name, Formula formula)
        {
            checkExceptionsFormula(name, formula);

            // used in case cirularException occurs
            HashSet<String> exDependees = dg.GetDependees(name).ToHashSet<String>();

            dg.ReplaceDependees(name, formula.GetVariables());
            try
            {
                cell cell = new cell(formula);

                // if cell already has something in it, replace it, otherwise make new cell
                if (spread.ContainsKey(name))
                {
                    spread[name] = cell;
                }
                else
                {
                    spread.Add(name, cell);
                }

                // create hashset of cells to recalculate after creating new one
                HashSet<String> recalculateDependees = GetCellsToRecalculate(name).ToHashSet<String>();
                return recalculateDependees;
            }
            catch (CircularException e)
            {
                dg.ReplaceDependees(name, exDependees);
                throw new CircularException();
            }
        }

        /// <summary>
        /// This method checks the exceptions in the setCellContents for
        /// the formula case, and throwing exceptions accordingly
        /// </summary>
        /// <param name="name"> name of cell</param>
        /// <param name="formula"> formula in cell </param>
        /// <exception cref="InvalidNameException"> if name is null</exception>
        /// <exception cref="ArgumentNullException"> if formula is null</exception>
        private static void checkExceptionsFormula(string name, Formula formula)
        {
            if (name == null)
            {
                throw new InvalidNameException();
            }

            if (formula is null)
            {
                throw new ArgumentNullException();
            }
        }

        /// <inheritdoc/>
        protected override IEnumerable<string> GetDirectDependents(string name)
        {
            if(name is null)
            { throw new ArgumentNullException(); }

            // if invalid name, throw exception
            if(!(Regex.IsMatch(name, @"^[a-zA-Z_](?: [a-zA-Z_]|\d)*$", RegexOptions.Singleline)))
            { throw new InvalidNameException(); }

            return dg.GetDependents(name);
        }

        /// <summary>
        /// Checks the exceptions for setCellContents for the double
        /// and string case, and throws exceptions accordingly
        /// </summary>
        /// <param name="name"> name of cell </param>
        /// <param name="text"> text in cell </param>
        /// <exception cref="InvalidNameException"> if name is null or invalid </exception>
        /// <exception cref="ArgumentNullException"> if stuff inside cell is invalid, only applies to string </exception>
        private static void checkExceptionsSetCell(string name, string text)
        {
            if (String.IsNullOrEmpty(name)) throw new InvalidNameException();

            if (text == null) throw new ArgumentNullException();
        }

        /// <summary>
        /// This class creates a cell to go inside the spreadsheet.
        /// It's functionality is to hold the content and value of the 
        /// cell, which routinely are the same. It also holds the type of 
        /// content of value that are inside the cell
        /// </summary>
        private class cell
        {
            /// <summary>
            /// Getter and setter for the cell.content
            /// </summary>
            public Object Content
            {
                get; private set;
            }

            /// <summary>
            /// Getter and setter for the cell.value
            /// </summary>
            public Object Value
            {
                get; private set;
            }

            // initiate content and value type to be used below
            String contentType;
            String valueType;

            /// <summary>
            /// Create cell for a string cell
            /// </summary>
            /// <param name="contents"> content for string type cell </param>
            public cell(String contents)
            {
                Content = contents;
                Value = contents;
                contentType = "String";
                valueType = "String";
            }

            /// <summary>
            /// Create cell for a double cell
            /// </summary>
            /// <param name="contents"> content for a double type cell </param>
            public cell(double contents)
            {
                Content = contents;
                Value = contents;
                contentType = "double";
                valueType = "double";
            }

            /// <summary>
            /// Create cell for a formula cell
            /// </summary>
            /// <param name="contents"> content for a formula type cell </param>
            public cell(Formula contents)
            {
                Content = contents;
                contentType = "Formula";

            }
        }

    }
}
