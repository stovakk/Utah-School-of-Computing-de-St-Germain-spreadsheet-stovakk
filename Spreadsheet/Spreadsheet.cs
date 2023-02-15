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
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
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
        private bool changed;

        public override bool Changed 
        { 
            get
            {
                return changed;
            }
            protected set
            {
                changed = value;
            }
        }

        /// <summary>
        /// Creates a new spreadsheet
        /// </summary>
        public Spreadsheet() : base(s => true, s => s, "original") 
        {
            spread = new Dictionary<string, cell>();
            dg = new DependencyGraph();

            changed = false;
        }

        public Spreadsheet(Func<String, bool> isValid, Func<String, String> Normalize, String version)
            : base(isValid, Normalize, version)
        {
            spread = new Dictionary<string, cell>();
            dg = new DependencyGraph();

            changed = false;
        }

        public Spreadsheet(String filePath, Func<String, bool> isValid, Func<String, String> Normalize, String version)
            : base(isValid, Normalize, version)
        {
            spread = new Dictionary<string, cell>();
            dg = new DependencyGraph();

            changed = false;

            if(!(GetSavedVersion(filePath).Equals(version)))
            {
                throw new SpreadsheetReadWriteException("filepath doesn't match the version");
            }

            readFile(filePath);
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
                cell cell = new cell(formula, LookupVals);

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

        public override IList<string> SetContentsOfCell(string name, string content)
        {
            HashSet<String> dependents;

            // if name is invalid, throw exception
            if (!(IsValid(name)))
                throw new InvalidNameException();
            
            // if content is a double, add it as a cell that is a double
            if(Double.TryParse(content, out double doubleContent))
            {
                dependents = new HashSet<String>(SetCellContents(name, doubleContent));
            }

            // if content is a formula, make a new formula with it, and it as a cell
            // that is a formula
            else if (content.StartsWith("="))
            {
                String formulaString = content[1..];

                Formula form = new Formula(formulaString, Normalize, isValid);

                dependents = new HashSet<String>(SetCellContents(name, form));
            }
            else
            {
                dependents = new HashSet<String>(SetCellContents(name, content));
            }

            changed = true;

            foreach(string key in dependents)
            {
                if(spread.TryGetValue(key, out cell dependentCell))
                {
                    dependentCell.reEvaluate(LookupVals);
                }
            }

            return dependents.ToImmutableList<String>();
        }

        public override string GetSavedVersion(string filename)
        {
            try
            {
                using (XmlReader xmlReads = XmlReader.Create(filename))
                {
                    while (xmlReads.Read())
                    {
                        if (xmlReads.Name.Equals("spreadsheet"))
                        {
                            return xmlReads.GetAttribute("version");
                        }
                    }
                }
            }
            catch (Exception e)
            {
                throw new SpreadsheetReadWriteException(e.Message);
            }

            throw new SpreadsheetReadWriteException("Version does not exist in the current context");
        }

        public override void Save(string filename)
        {
            if (filename.Equals(""))
                throw new SpreadsheetReadWriteException("The filename is empty");

            try
            {
                XmlWriterSettings settings = new XmlWriterSettings();
                settings.Indent = true;

                using (XmlWriter xmlWrites = XmlWriter.Create(filename, settings))
                {
                    // start xml document
                    xmlWrites.WriteStartDocument(); 
                    // open with tag "spreadsheet"
                    xmlWrites.WriteStartElement("spreadsheet");              
                    xmlWrites.WriteAttributeString("version", null, Version);

                    foreach (string cell in spread.Keys)
                    {
                        // create tag "cell"
                        xmlWrites.WriteStartElement("cell");   
                        // create tag of name of cell
                        xmlWrites.WriteElementString("name", cell);

                        // cell contents
                        string cellContents;

                        // if cell is a double, store it as a string using toString()
                        if (spread[cell].Content is double)
                        {                        
                            cellContents = spread[cell].Content.ToString();
                        }
                        // else if the cell is a formula, store it with an equals sign using toString()
                        else if (spread[cell].Content is Formula)
                        {   
                            cellContents = "=";
                            cellContents += spread[cell].Content.ToString();
                        }
                        // else it's a string, treat it as such
                        else
                        {  
                            cellContents = (string)spread[cell].Content;
                        }

                        // creates content tag and puts in contents of the cell
                        xmlWrites.WriteElementString("contents", cellContents);  
                        // closes cell tag
                        xmlWrites.WriteEndElement();
                    }
                    // closes spreadsheet tag
                    xmlWrites.WriteEndElement();
                    // closes document
                    xmlWrites.WriteEndDocument();

                }

            }
            catch(XmlException e)
            {
                throw new SpreadsheetReadWriteException(e.ToString());
            }
            catch(IOException e)
            {
                throw new SpreadsheetReadWriteException("Unnable to read the file");
            }

        }

        public override object GetCellValue(string name)
        {
            if (spread.TryGetValue(name, out cell cell))
                return cell.Value;
            else
                return "";
        }

        private void readFile(string filename)
        {
            if (filename is null)
                throw new SpreadsheetReadWriteException("file name is null");
            if (filename.Equals(""))
                throw new SpreadsheetReadWriteException("The filename cannot be empty");

            try
            {

                XmlReaderSettings settings= new XmlReaderSettings();
                settings.IgnoreWhitespace = true;
                
                using (XmlReader reader = XmlReader.Create(filename, settings))
                {
                    // name of cell
                    String name = "";
                    // contents of the cell
                    String contents = "";
                    bool nameSet = false;
                    bool contentsSet = false;

                    // while reader has elements to read
                    while (reader.Read())
                    {
                        if (reader.IsStartElement())
                        {
                            switch(reader.Name)
                            {
                                case "name":
                                    reader.Read();
                                    name = reader.ReadContentAsString();
                                    name = name.Trim();
                                    nameSet = true;
                                    break;

                                case "contents":
                                    reader.Read();
                                    contents = reader.ReadContentAsString();
                                    contents = contents.Trim();
                                    SetContentsOfCell(name, contents);
                                    contentsSet = true;
                                    break;
                            }
                        }
                    }

                    if(!contentsSet && !nameSet)
                    {
                        throw new Exception("empty spreadsheet");
                    }
                }
            }
            catch(Exception e)
            {
                throw new SpreadsheetReadWriteException(e.Message);
            }
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
            public cell(Formula contents, Func<string, double> lookup)
            {
                Content = contents;
                contentType = "Formula";
                Value = contents.Evaluate(lookup);
                valueType = "double";
            }

            public void reEvaluate(Func<string, double> lookup)
            {
                if(contentType == "Formula")
                {
                    Formula form = (Formula)Content;
                    Value = form.Evaluate(lookup);
                }
            }
        }
        private double LookupVals(string a)
        {
            cell lookupCell; // value of name

            // if the dictionary contains the parameter
            if (spread.TryGetValue(a, out lookupCell))
            {
                // check if cell is a double
                if (lookupCell.Value is double)
                    return (double)lookupCell.Value;
                // else throw exception
                else 
                    throw new ArgumentException();
            }
            // if spreadsheet doesn't contains s, throw exception
            else
                throw new ArgumentException();

        }

    }
}
