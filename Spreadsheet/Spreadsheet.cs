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
    public class Spreadsheet : AbstractSpreadsheet
    {
        private Dictionary<String, cell> spread;
        private DependencyGraph dg;

        public Spreadsheet() 
        {
            spread = new Dictionary<string, cell>();
            dg = new DependencyGraph();
        }

        public override object GetCellContents(string name)
        {
            return spread[name];
        }

        public override IEnumerable<string> GetNamesOfAllNonemptyCells()
        {
            return spread.Keys;
        }

        public override ISet<string> SetCellContents(string name, double number)
        {
            if (name.Equals(null)) throw new InvalidNameException();

            cell cell = new cell(number);

            if   (spread.ContainsKey(name)) { spread[name] = cell;    }
            else                            { spread.Add(name, cell); }

            dg.ReplaceDependees(name, new HashSet<String>());

            HashSet<String> recalculateDependees = GetCellsToRecalculate(name).ToHashSet<String>();
            return recalculateDependees;
        }

        public override ISet<string> SetCellContents(string name, string text)
        {
            if (name.Equals(null)) throw new InvalidNameException();

            if (text.Equals(null)) throw new ArgumentNullException();

            if (text.Equals(""))
            {
                spread.Remove(name);
            }
            else
            {
                cell cell = new cell(text);

                if   (spread.ContainsKey(name)) { spread[name] = cell;    } 
                else                            { spread.Add(name, cell); }
            }

            dg.ReplaceDependees(name, new HashSet<String>());

            HashSet<String> recalculateDependees = GetCellsToRecalculate(name).ToHashSet<String>();
            return recalculateDependees;
        }

        public override ISet<string> SetCellContents(string name, Formula formula)
        {
            if (name.Equals(null))
            {
                throw new InvalidNameException();
            }

            if(formula.Equals(null) || !((Regex.IsMatch(name, @"^[a-zA-Z_](?: [a-zA-Z_]|\d)*$", RegexOptions.Singleline))))
            {
                throw new ArgumentNullException();
            }

            cell cell = new cell(formula);
            HashSet<String> exDependees = dg.GetDependees(name).ToHashSet<String>();

            dg.ReplaceDependees(name, formula.GetVariables());
            try
            {
                if (spread.ContainsKey(name)) { spread[name] = cell;    }
                else                          { spread.Add(name, cell); }

                dg.ReplaceDependees(name, new HashSet<String>());

                HashSet<String> recalculateDependees = GetCellsToRecalculate(name).ToHashSet<String>();
                return recalculateDependees;
            }
            catch (CircularException)
            {
                dg.ReplaceDependees(name, exDependees);
                throw new CircularException();
            }
        }

        protected override IEnumerable<string> GetDirectDependents(string name)
        {
            throw new NotImplementedException();
        }

        private class cell
        {
            public Object Content
            {
                get; private set;
            }
            public Object Value
            {
                get; private set;
            }

            String contentType;
            String valueType;

            public cell(String contents)
            {
                Content = contents;
                Value = contents;
                contentType = "String";
                valueType = "String";
            }

            public cell(double contents)
            {
                Content = contents;
                Value = contents;
                contentType = "double";
                valueType = "double";
            }

            public cell(Formula contents)
            {
                Content = contents;
                contentType = "Formula";

            }
        }

    }
}
