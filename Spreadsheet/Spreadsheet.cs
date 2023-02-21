// implementation written by Nate Stovak for CS 3500, February 2023.
// Version 1.1 (Create Cell class)
// Version 1.2 (Do Set methods and other helper methods)
// Version 1.3 (Finished debugging)

/// Author:    Nate Stovak
/// Partner:   None
/// Date:      9 - Feb - 2023
/// Course:    CS 3500, University of Utah, School of Computing
/// GitHub ID: Stovakk
/// Date:	   16 - Feb - 2023
/// Solution:  Spreadsheet
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

namespace SS
{

    /// <summary>
    /// This class creates a spreadsheet using cells in a cell class I created
    /// at the bottom, the class is able to set and get cells and their contents
    /// along with find any excpetions needed
    /// </summary>
    public class Spreadsheet : AbstractSpreadsheet
    {
        // initiating spreadsheet, dependency graph, and changed boolean
        private Dictionary<String, cell> spread;
        private DependencyGraph dg;
        private bool changed;

        /// <summary>
        /// Getter and setter for changed, should be false when the spreadsheet is saved
        /// or initiated, true when a cell has been added after being not changed
        /// </summary>
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
        public Spreadsheet() : base(s => true, s => s, "default") 
        {
            spread = new Dictionary<string, cell>();
            dg = new DependencyGraph();

            // has not been changed
            changed = false;
        }

        /// <summary>
        /// Creates a new spreadsheet with isValid function
        /// Normalize function, and version string
        /// </summary>
        /// <param name="isValid"> isValid function delegate </param>
        /// <param name="Normalize"> Normalize function delegate</param>
        /// <param name="version"> String version </param>
        public Spreadsheet(Func<String, bool> isValid, Func<String, String> Normalize, String version)
            : base(isValid, Normalize, version)
        {
            // creates new spreadsheet
            spread = new Dictionary<string, cell>();
            dg = new DependencyGraph();

            // has not been changed
            changed = false;
        }

        public Spreadsheet(String filePath, Func<String, bool> isValid, Func<String, String> Normalize, String version)
            : base(isValid, Normalize, version)
        {
            spread = new Dictionary<string, cell>();
            dg = new DependencyGraph();


            // happens when the filepath doesn't aline with the version
            if(!(GetSavedVersion(filePath).Equals(version)))
            {
                throw new SpreadsheetReadWriteException("filepath doesn't match the version");
            }

            // reads file and then inherently fills the spreadsheet
            readFile(filePath);

            // has not been changed
            changed = false;
        }

        /// <inheritdoc/>
        public override object GetCellContents(string name)
        {
            if(!((Regex.IsMatch(name, @"^[a-zA-Z_](?: [a-zA-Z_]|\d)*$", RegexOptions.Singleline))))
                throw new InvalidNameException();

            cell content;

            // normalizes name
            name = Normalize(name);

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
        protected override IList<string> SetCellContents(string name, double number)
        {
            if (name == "") throw new InvalidNameException();

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

            // replace the cells relying on the current cell
            dg.ReplaceDependees(name, new HashSet<String>());

            // create hashset of cells to recalculate after creating new one
            IList<String> recalculateDependees = GetCellsToRecalculate(name).ToList();
            return recalculateDependees;
        }

        /// <inheritdoc/>
        protected override IList<string> SetCellContents(string name, string text)
        {
            if (name == "") throw new InvalidNameException();

            // if cell has no content, remove it from dictionary
            if (text.Equals(""))
            {
                spread.Remove(name);
            }
            else
            { 
                cell cell = new cell(text);

                // if cell already has something in it, replace it, otherwise make new cell
                if (spread.ContainsKey(name)) { spread[name] = cell; }
                else { spread.Add(name, cell); }
            }

            // replace the cells relying on the current cell
            dg.ReplaceDependees(name, new HashSet<String>());

            // create hashset of cells to recalculate after creating new one
            IList<String> recalculateDependees = GetCellsToRecalculate(name).ToList();
            return recalculateDependees;
        }

        /// <inheritdoc/>
        protected override IList<string> SetCellContents(string name, Formula formula)
        {

            // used in case cirularException occurs
            HashSet<String> exDependees = dg.GetDependees(name).ToHashSet<String>();

            // replace the cells relying on the current cell
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
                IList<String> recalculateDependees = GetCellsToRecalculate(name).ToList();
                return recalculateDependees;
            }
            catch (CircularException)
            {
                // replaces the cells again to it's previous state before the error
                dg.ReplaceDependees(name, exDependees);
                throw new CircularException();
            }
        }

        /// <inheritdoc/>
        protected override IEnumerable<string> GetDirectDependents(string name)
        {
            //// if invalid name, throw exception
            //if (!(Regex.IsMatch(name, @"^[a-zA-Z_](?: [a-zA-Z_]|\d)*$", RegexOptions.Singleline)))
            //{ throw new InvalidNameException(); }

            return dg.GetDependents(name);
        }

        /// <inheritdoc/>
        public override IList<string> SetContentsOfCell(string name, string content)
        {
            HashSet<String> dependents;

            name = Normalize(name);

            // if name is invalid, throw exception
            if (!IsValid(name))
                throw new InvalidNameException();
            
            // if content is a double, add it as a cell that is a double
            if(Double.TryParse(content, out double doubleContent))
            {
                // sets the cell conetents and returns the variables the formula depends on
                dependents = new HashSet<String>(SetCellContents(name, doubleContent));
            }

            // if content is a formula, make a new formula with it, and it as a cell
            // that is a formula
            else if (content.StartsWith("="))
            {
                String formulaString = content[1..];

                Formula form = new Formula(formulaString, Normalize, IsValid);

                // sets the cell conetents and returns the variables the formula depends on
                dependents = new HashSet<String>(SetCellContents(name, form));
            }
            else
            {
                // sets the cell conetents and returns the variables the formula depends on
                dependents = new HashSet<String>(SetCellContents(name, content));
            }

            // for all the dependents to the cell added, reevaluate
            foreach (string key in dependents)
            {
                if(spread.TryGetValue(key, out cell dependentCell))
                {
                    dependentCell.reEvaluate(LookupVals);
                }
            }

            // cell added so the spreadsheet has been changed
            this.changed = true;
            return dependents.ToImmutableList<String>();
        }

        /// <inheritdoc/>
        public override string GetSavedVersion(string filename)
        {
            try
            {
                // read file
                using (XmlReader xmlReads = XmlReader.Create(filename))
                {
                    while (xmlReads.Read())
                    {
                        // if spreadsheet exists, return the name of the current version
                        if (xmlReads.Name.Equals("spreadsheet"))
                            return xmlReads.GetAttribute("version");
                    }
                }
            }
            catch (Exception e)
            {
                throw new SpreadsheetReadWriteException(e.Message);
            }

            // when filename doesn't match the version
            throw new SpreadsheetReadWriteException("filename doesn't match the version");
        }

        /// <inheritdoc/>
        public override void Save(string filename)
        {
            // if name is empty, throw exception
            if (filename.Equals(""))
                throw new SpreadsheetReadWriteException("The filename is empty");

            try
            {
                // indents the settings
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

                        String cellContents = "";
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
            catch(IOException)      
            {   throw new SpreadsheetReadWriteException("Unnable to read the file");    }

        }


        public override object GetCellValue(string name)
        {

            // if invalid name, throw exception
//            if (!(Regex.IsMatch(name, @"^[a-zA-Z_](?: [a-zA-Z_]|\d)*$", RegexOptions.Singleline)))
//            { throw new InvalidNameException(); }

            if (spread.TryGetValue(name, out cell cell))
                return cell.Value;
            else
                return "";
        }

        /// <summary>
        /// This method reads the files 
        /// </summary>
        /// <param name="filename"></param>
        /// <exception cref="SpreadsheetReadWriteException"></exception>
        private void readFile(string filename)
        {
            if (filename.Equals(""))
                throw new SpreadsheetReadWriteException("The filename cannot be empty");

            try
            {

                // ignores whitespace
                XmlReaderSettings settings= new XmlReaderSettings();
                settings.IgnoreWhitespace = true;
                
                using (XmlReader reader = XmlReader.Create(filename, settings))
                {
                    // name of cell
                    String name = "";
                    // contents of the cell
                    String contents = "";

                    // booleans only used if spreadsheet is empty
                    bool nameSet = false;
                    bool contentsSet = false;

                    // while reader has elements to read
                    while (reader.Read())
                    {
                        // if the reader is at the start element
                        if (reader.IsStartElement())
                        {
                            // switch to know which case block to execute
                            switch(reader.Name)
                            {
                                // if it's a name, set the name as what it is, then break
                                case "name":
                                    reader.Read();
                                    name = reader.ReadContentAsString();
                                    name = name.Trim();
                                    nameSet = true;
                                    break;

                                // if it's a content, set the content as what it is, then break
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

                    // happens with empty spredsheet
                    if(!contentsSet && !nameSet)
                    {
                        throw new Exception("empty spreadsheet");
                    }
                }
            }
            // catches the empty spreadsheet exception
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

            /// <summary>
            /// Method to re-evaluate the dependees of a cell when the cell contents
            /// have been changed
            /// </summary>
            /// <param name="lookup"> 
            /// the lookup method that gets the number corresponding
            /// to a variable
            /// </param>
            public void reEvaluate(Func<string, double> lookup)
            {
                // if it's a formula, reevaluate the formula with the new
                // cell contents it relies on
                if(contentType == "Formula")
                {
                    // cast the content to a formula as the string
                    // content still has the = sign
                    Formula form = (Formula)Content;
                    Value = form.Evaluate(lookup);
                }
            }
        }

        /// <summary>
        /// Method used to lookup the value for variables
        /// </summary>
        /// <param name="a"> cell name </param>
        /// <returns> the double inside the parameter cell</returns>
        /// <exception cref="ArgumentException"> 
        /// the parameter doesn;t
        /// lead to a cell or an empty cell
        /// </exception>
        private double LookupVals(string a)
        {
            cell lookupCell;

            // check to see if parameter cell exists
            if (spread.TryGetValue(a, out lookupCell))
            {
                // see if cell is a double
                if (lookupCell.Value is double)
                    return (double)lookupCell.Value;
                // cell is not a double
                else 
                    throw new ArgumentException();
            }
            // if spreadsheet doesn't contain parameter cell
            else
                throw new ArgumentException();

        }

    }
}
