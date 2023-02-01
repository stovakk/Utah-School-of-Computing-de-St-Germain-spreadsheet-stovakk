// Skeleton implementation written by Joe Zachary for CS 3500, September 2013.
// Version 1.1 (Fixed error in comment for RemoveDependency.)
// Version 1.2 - Daniel Kopta 
//               (Clarified meaning of dependent and dependee.)
//               (Clarified names in solution/project structure.)
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace SpreadsheetUtilities

{
    /// <summary>
    /// (s1,t1) is an ordered pair of strings
    /// t1 depends on s1; s1 must be evaluated before t1
    /// 
    /// A DependencyGraph can be modeled as a set of ordered pairs of strings.  Two 
    ///  ordered pairs
    /// (s1,t1) and (s2,t2) are considered equal if and only if s1 equals s2 and t1 
    ///  equals t2.
    /// Recall that sets never contain duplicates.  If an attempt is made to add an 
    ///  element to a
    /// set, and the element is already in the set, the set remains unchanged.
    /// 
    /// Given a DependencyGraph DG:
    /// 
    ///    (1) If s is a string, the set of all strings t such that (s,t) is in DG is
    /// called dependents(s).
    ///        (The set of things that depend on s)    
    ///        
    ///    (2) If s is a string, the set of all strings t such that (t,s) is in DG is
    /// called dependees(s).
    ///        (The set of things that s depends on) 
    //
    // For example, suppose DG = {("a", "b"), ("a", "c"), ("b", "d"), ("d", "d")}
    //     dependents("a") = {"b", "c"}
    //     dependents("b") = {"d"}
    //     dependents("c") = {}
    //     dependents("d") = {"d"}
    //     dependees("a") = {}
    //     dependees("b") = {"a"}
    //     dependees("c") = {"a"}
    //     dependees("d") = {"b", "d"}
    /// </summary>
    public class DependencyGraph
    {
        //Private instance methods we will use in the class
        private Dictionary<String, HashSet<String>> dependents;
        private Dictionary<String, HashSet<String>> dependees;
        private int pairs;

        /// <summary>
        /// Creates an empty DependencyGraph.
        /// </summary>
        public DependencyGraph()
        {
            dependents = new Dictionary<string, HashSet<String>>();
            dependees = new Dictionary<string, HashSet<String>>();
        }
        /// <summary>
        /// The number of ordered pairs in the DependencyGraph.
        /// </summary>
        public int Size
        {
            get { return pairs; }

        }
        /// <summary>
        /// The size of dependees(s).
        /// This property is an example of an indexer.  If dg is a DependencyGraph, you
        /// would invoke it like this:
        /// dg["a"]
        /// It should return the size of dependees("a")
        /// </summary>
        public int this[string s]
        {
            get
            {
                if (dependees.ContainsKey(s))
                {
                    return dependees[s].Count;
                }
                else
                {
                    return 0;
                }
            }
        }
        /// <summary>
        /// Reports whether dependents(s) is non-empty.
        /// </summary>
        public bool HasDependents(string s)
        {
            if (dependents.ContainsKey(s) && dependents.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// Reports whether dependees(s) is non-empty.
        /// </summary>
        public bool HasDependees(string s)
        {
            if (dependees.ContainsKey(s) && dependees.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// Enumerates dependents(s).
        /// </summary>
        public IEnumerable<string> GetDependents(string s)
        {
            if (!dependents.ContainsKey(s))
            {
                return new HashSet<String>();
            }
            String[] results = new string[dependents[s].Count];
            dependents[s].CopyTo(results);
            return results;
        }
        /// <summary>
        /// Enumerates dependees(s).
        /// </summary>
        public IEnumerable<string> GetDependees(string s)
        {
            if (!dependees.ContainsKey(s))
            {
                return new HashSet<String>();
            }
            String[] results = new string[dependees[s].Count];
            dependees[s].CopyTo(results);
            return results;
        }
        /// <summary>
        /// <para>Adds the ordered pair (s,t), if it doesn't exist</para>
        /// 
        /// <para>This should be thought of as:</para>   
        /// 
        ///   t depends on s
        ///
        /// </summary>
        /// <param name="s"> s must be evaluated first. T depends on S</param>
        /// <param name="t"> t cannot be evaluated until s is</param>        /// 
        public void AddDependency(string s, string t)
        {
            // if neither dependent or dependee exist in the Dictionaries
            if (!dependents.ContainsKey(s) && !dependees.ContainsKey(t))
            {
                // creates two new hashSet's, adds them to their assigned 
                // s and t location, and adds their adversary accordingly
                HashSet<String> newDependents = new HashSet<String>();
                HashSet<String> newDependees = new HashSet<String>();
                dependents.Add(s, newDependents);
                dependees.Add(t, newDependees);
                dependents[s].Add(t);
                dependees[t].Add(s);
            }
            // if the dependency exits in the set, return and don't do anything
            else if (dependents.ContainsKey(s) && dependees.ContainsKey(t) && dependents[s].Contains(t))
            {
                return;
            }
            // if dependents hashset exists and dependees does not
            else if (dependents.ContainsKey(s) && !dependees.ContainsKey(t))
            {
                // then add new hashset to dependees, and add it to the according spot
                HashSet<String> newDependees = new HashSet<String>();
                dependents[s].Add(t);
                dependees.Add(t, newDependees);
                dependees[t].Add(s);
            }
            // if dependees hashset exists, and dependents does not
            else if (!dependents.ContainsKey(s) && dependees.ContainsKey(t))
            {
                // then add new hashset to dependetns, and add it to the according spot
                HashSet<String> newDependents = new HashSet<String>();
                dependents.Add(s, newDependents);
                dependents[s].Add(t);
                dependees[t].Add(s);
            }
            // else if dependees and dependents both have hashset's, but it doesn't exist in either
            else
            {
                dependents[s].Add(t);
                dependees[t].Add(s);
            }
            // adds pair only if it doesn't already exist
            pairs++;
        }
        /// <summary>
        /// Removes the ordered pair (s,t), if it exists
        /// </summary>
        /// <param name="s"></param>
        /// <param name="t"></param>
        public void RemoveDependency(string s, string t)
        {
            // if the dependency to remove exits where it should in both dependents and dependees
            if (dependents.ContainsKey(s) && dependees.ContainsKey(t))
            {
                dependents[s].Remove(t);

                // if the dependents list is now empty, remove it
                if (dependents[s].Count == 0)
                {
                    dependents.Remove(s);
                }

                dependees[t].Remove(s);

                //if the dependees list is now empty, remove it
                if (dependees[t].Count == 0)
                {
                    dependees.Remove(t);
                }
                // if it removed something, it will lower the pairs by 1
                pairs--;
            }

        }
        /// <summary>
        /// Removes all existing ordered pairs of the form (s,r).  Then, for each
        /// t in newDependents, adds the ordered pair (s,t).
        /// </summary>
        public void ReplaceDependents(string s, IEnumerable<string> newDependents)
        {
            // if dependents contains the key, it will remove all dependees associated with it
            if (dependents.ContainsKey(s))
            {
                HashSet<String> replace = dependents[s];
                foreach (string r in replace)
                {
                    RemoveDependency(r, s);
                }
            }

            // for each string in newDependents it will be added to Dependents
            foreach (string t in newDependents)
            {
                AddDependency(s, t);
            }
        }
        /// <summary>
        /// Removes all existing ordered pairs of the form (r,s).  Then, for each 
        /// t in newDependees, adds the ordered pair (t,s).
        /// </summary>
        public void ReplaceDependees(string s, IEnumerable<string> newDependees)
        {

            // if dependees contains the key, it will remove all dependees associated with it
            if (dependees.ContainsKey(s))
            {
                HashSet<String> replace = dependees[s];
                // removes all dependencys in the dependees set by 
                // using the remove dependency backwards as it works the same way
                foreach (string r in replace)
                {
                    RemoveDependency(r, s);
                }
            }

            // for each string in newDependents it will be added to Dependees,
            // which is done by adding dependency backwards
            foreach (string t in newDependees)
            {
                AddDependency(t, s);
            }
           
        }

    }
}