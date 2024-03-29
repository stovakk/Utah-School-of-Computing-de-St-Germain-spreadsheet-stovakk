﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DependencyGraphTests
{
    /// <summary>
    ///This is a test class for DependencyGraphTest and is intended
    ///to contain all DependencyGraphTest Unit Tests
    ///</summary>
    [TestClass()]
    public class DependencyGraphTest
    {
        private const int V = 0;

        /// <summary>
        ///Empty graph should contain nothing
        ///</summary>
        [TestMethod()]
        public void SimpleEmptyTest()
        {
            SpreadsheetUtilities.DependencyGraph t = new SpreadsheetUtilities.DependencyGraph();
            Assert.AreEqual(0, t.Size);
        }
        /// <summary>
        ///Empty graph should contain nothing
        ///</summary>
        [TestMethod()]
        public void SimpleEmptyRemoveTest()
        {
            SpreadsheetUtilities.DependencyGraph t = new SpreadsheetUtilities.DependencyGraph();
            t.AddDependency("x", "y");
            Assert.AreEqual(1, t.Size);
            t.RemoveDependency("x", "y");
            Assert.AreEqual(0, t.Size);
        }
        /// <summary>
        ///Empty graph should contain nothing
        ///</summary>
        [TestMethod()]
        public void EmptyEnumeratorTest()
        {
            SpreadsheetUtilities.DependencyGraph t = new SpreadsheetUtilities.DependencyGraph();
            t.AddDependency("x", "y");
            IEnumerator<string> e1 = t.GetDependees("y").GetEnumerator();
            Assert.IsTrue(e1.MoveNext());
            Assert.AreEqual("x", e1.Current);
            IEnumerator<string> e2 = t.GetDependents("x").GetEnumerator();
            Assert.IsTrue(e2.MoveNext());
            Assert.AreEqual("y", e2.Current);
            t.RemoveDependency("x", "y");
            Assert.IsFalse(t.GetDependees("y").GetEnumerator().MoveNext());
            Assert.IsFalse(t.GetDependents("x").GetEnumerator().MoveNext());
        }
        /// <summary>
        ///Replace on an empty DG shouldn't fail
        ///</summary>
        [TestMethod()]
        public void SimpleReplaceTest()
        {
            SpreadsheetUtilities.DependencyGraph t = new SpreadsheetUtilities.DependencyGraph();
            t.AddDependency("x", "y");
            Assert.AreEqual(t.Size, 1);
            t.RemoveDependency("x", "y");
            t.ReplaceDependents("x", new HashSet<string>());
            t.ReplaceDependees("y", new HashSet<string>());
        }
        ///<summary>
        ///It should be possibe to have more than one DG at a time.
        ///</summary>
        [TestMethod()]
        public void StaticTest()
        {
            SpreadsheetUtilities.DependencyGraph t1 = new SpreadsheetUtilities.DependencyGraph();
            SpreadsheetUtilities.DependencyGraph t2 = new SpreadsheetUtilities.DependencyGraph();
            t1.AddDependency("x", "y");
            Assert.AreEqual(1, t1.Size);
            Assert.AreEqual(0, t2.Size);
        }
        /// <summary>
        ///Non-empty graph contains something
        ///</summary>
        [TestMethod()]
        public void SizeTest()
        {
            SpreadsheetUtilities.DependencyGraph t = new SpreadsheetUtilities.DependencyGraph();
            t.AddDependency("a", "b");
            t.AddDependency("a", "c");
            t.AddDependency("c", "b");
            t.AddDependency("b", "d");
            Assert.AreEqual(4, t.Size);
        }
        /// <summary>
        ///Non-empty graph contains something
        ///</summary>
        [TestMethod()]
        public void EnumeratorTest()
        {
            SpreadsheetUtilities.DependencyGraph t = new SpreadsheetUtilities.DependencyGraph();
            t.AddDependency("a", "b");
            t.AddDependency("a", "c");
            t.AddDependency("c", "b");
            t.AddDependency("b", "d");
            IEnumerator<string> e = t.GetDependees("a").GetEnumerator();
            Assert.IsFalse(e.MoveNext());
            e = t.GetDependees("b").GetEnumerator();
            Assert.IsTrue(e.MoveNext());
            String s1 = e.Current;
            Assert.IsTrue(e.MoveNext());
            String s2 = e.Current;
            Assert.IsFalse(e.MoveNext());
            Assert.IsTrue(((s1 == "a") && (s2 == "c")) || ((s1 == "c") && (s2 == "a")));
            e = t.GetDependees("c").GetEnumerator();
            Assert.IsTrue(e.MoveNext());
            Assert.AreEqual("a", e.Current);
            Assert.IsFalse(e.MoveNext());
            e = t.GetDependees("d").GetEnumerator();
            Assert.IsTrue(e.MoveNext());
            Assert.AreEqual("b", e.Current);
            Assert.IsFalse(e.MoveNext());
        }
        /// <summary>
        ///Non-empty graph contains something
        ///</summary>
        [TestMethod()]
        public void ReplaceThenEnumerate()
        {
            SpreadsheetUtilities.DependencyGraph t = new SpreadsheetUtilities.DependencyGraph();
            t.AddDependency("x", "b");
            t.AddDependency("a", "z");
            t.ReplaceDependents("b", new HashSet<string>());
            t.AddDependency("y", "b");
            t.ReplaceDependents("a", new HashSet<string>() { "c" });
            t.AddDependency("w", "d");
            t.ReplaceDependees("b", new HashSet<string>() { "a", "c" });
            t.ReplaceDependees("d", new HashSet<string>() { "b" });
            IEnumerator<string> e = t.GetDependees("a").GetEnumerator();
            Assert.IsFalse(e.MoveNext());
            e = t.GetDependees("b").GetEnumerator();
            Assert.IsTrue(e.MoveNext());
            String s1 = e.Current;
            Assert.IsTrue(e.MoveNext());
            String s2 = e.Current;
            Assert.IsFalse(e.MoveNext());
            Assert.IsTrue(((s1 == "a") && (s2 == "c")) || ((s1 == "c") && (s2 == "a")));
            e = t.GetDependees("c").GetEnumerator();
            Assert.IsTrue(e.MoveNext());
            Assert.AreEqual("a", e.Current);
            Assert.IsFalse(e.MoveNext());
            e = t.GetDependees("d").GetEnumerator();
            Assert.IsTrue(e.MoveNext());
            Assert.AreEqual("b", e.Current);
            Assert.IsFalse(e.MoveNext());
        }
        /// <summary>
        ///Using lots of data
        ///</summary>
        [TestMethod()]
        public void StressTest()
        {
            // Dependency graph
            SpreadsheetUtilities.DependencyGraph t = new SpreadsheetUtilities.DependencyGraph();
            // A bunch of strings to use
            const int SIZE = 200;
            string[] letters = new string[SIZE];
            for (int i = 0; i < SIZE; i++)
            {
                letters[i] = ("" + (char)('a' + i));
            }
            // The correct answers
            HashSet<string>[] dents = new HashSet<string>[SIZE];
            HashSet<string>[] dees = new HashSet<string>[SIZE];
            for (int i = 0; i < SIZE; i++)
            {
                dents[i] = new HashSet<string>();
                dees[i] = new HashSet<string>();
            }
            // Add a bunch of dependencies
            for (int i = 0; i < SIZE; i++)
            {
                for (int j = i + 1; j < SIZE; j++)
                {
                    t.AddDependency(letters[i], letters[j]);
                    dents[i].Add(letters[j]);
                    dees[j].Add(letters[i]);
                }
            }
            // Remove a bunch of dependencies
            for (int i = 0; i < SIZE; i++)
            {
                for (int j = i + 4; j < SIZE; j += 4)
                {
                    t.RemoveDependency(letters[i], letters[j]);
                    dents[i].Remove(letters[j]);
                    dees[j].Remove(letters[i]);
                }
            }
            // Add some back
            for (int i = 0; i < SIZE; i++)
            {
                for (int j = i + 1; j < SIZE; j += 2)
                {
                    t.AddDependency(letters[i], letters[j]);
                    dents[i].Add(letters[j]);
                    dees[j].Add(letters[i]);
                }
            }
            // Remove some more
            for (int i = 0; i < SIZE; i += 2)
            {
                for (int j = i + 3; j < SIZE; j += 3)
                {
                    t.RemoveDependency(letters[i], letters[j]);
                    dents[i].Remove(letters[j]);
                    dees[j].Remove(letters[i]);
                }
            }
            // Make sure everything is right
            for (int i = 0; i < SIZE; i++)
            {
                Assert.IsTrue(dents[i].SetEquals(new
        HashSet<string>(t.GetDependents(letters[i]))));
                Assert.IsTrue(dees[i].SetEquals(new
        HashSet<string>(t.GetDependees(letters[i]))));
            }
        }

        /// <summary>
        /// Duplicate add shouldn't work
        /// </summary>
        [TestMethod()]
        public void testDuplicate()
        {
            SpreadsheetUtilities.DependencyGraph t = new SpreadsheetUtilities.DependencyGraph();
            t.AddDependency("a", "b");
            t.AddDependency("a", "b");
            Assert.IsFalse(t.Size == 2);
            Assert.IsTrue(t.Size == 1);
        }

        /// <summary>
        /// Testing "This" method
        /// </summary>
        [TestMethod()]
        public void testThisMethodMany()
        {
            SpreadsheetUtilities.DependencyGraph t = new SpreadsheetUtilities.DependencyGraph();
            t.AddDependency("a", "b");
            t.AddDependency("c", "b");
            t.AddDependency("d", "b");
            t.AddDependency("r", "b");
            t.AddDependency("t", "b");
            t.AddDependency("b", "b");
            t.AddDependency("a", "t");
            t.AddDependency("b", "q");
            t.AddDependency("b", "e");
            Assert.IsTrue(t["b"] == 6);
        }

        /// <summary>
        /// Testing "This" method empty
        /// </summary>
        [TestMethod()]
        public void testThisMethodNone()
        {
            SpreadsheetUtilities.DependencyGraph t = new SpreadsheetUtilities.DependencyGraph();
            t.AddDependency("a", "b");
            t.AddDependency("c", "b");
            t.AddDependency("d", "b");
            t.AddDependency("r", "b");
            t.AddDependency("t", "b");
            t.AddDependency("b", "b");
            t.AddDependency("a", "t");
            t.AddDependency("b", "q");
            t.AddDependency("b", "e");
            Assert.IsTrue(t["r"] == 0);
        }

        /// <summary>
        /// Testing has Dependnts True
        /// </summary>
        [TestMethod()]
        public void testHasDependentsTrue()
        {
            SpreadsheetUtilities.DependencyGraph t = new SpreadsheetUtilities.DependencyGraph();
            t.AddDependency("a", "b");
            t.AddDependency("c", "b");
            Assert.IsTrue(t.HasDependents("a"));
            Assert.IsFalse(t.HasDependents("b"));
        }

        /// <summary>
        /// Testing HasDependents False
        /// </summary>
        [TestMethod()]
        public void testHasDependentsFalse()
        {
            SpreadsheetUtilities.DependencyGraph t = new SpreadsheetUtilities.DependencyGraph();
            t.AddDependency("d", "b");
            t.AddDependency("r", "b");
            Assert.IsTrue(t.HasDependents("d"));
            Assert.IsFalse(t.HasDependents("b"));
        }

        /// <summary>
        /// Testing has Dependees True
        /// </summary>
        [TestMethod()]
        public void testHasDependeesTrue()
        {
            SpreadsheetUtilities.DependencyGraph t = new SpreadsheetUtilities.DependencyGraph();
            t.AddDependency("a", "b");
            t.AddDependency("c", "b");
            t.AddDependency("", "");
            Assert.IsTrue(t.HasDependees("b"));
            Assert.IsFalse(t.HasDependees("a"));
            Assert.IsTrue(t.Size == 3);
        }

        /// <summary>
        /// Testing size is zero
        /// </summary>
        [TestMethod()]
        public void testSizeZero()
        {
            SpreadsheetUtilities.DependencyGraph t = new SpreadsheetUtilities.DependencyGraph();
            Assert.IsTrue(t.Size == 0);
        }

        /// <summary>
        /// Testing size after remove, should be one lower
        /// </summary>
        [TestMethod()]
        public void testSizeAfterRemove()
        {
            SpreadsheetUtilities.DependencyGraph t = new SpreadsheetUtilities.DependencyGraph();
            t.AddDependency("a", "b");
            t.AddDependency("b", "b");
            t.RemoveDependency("a", "b");
            Assert.IsTrue(t.Size == 1);
            t.RemoveDependency("b", "b");
            Assert.IsTrue(t.Size == 0);
        }

        /// <summary>
        /// Testing removing something that doesn't exist
        /// </summary>
        [TestMethod()]
        public void testRemoveNA()
        {
            SpreadsheetUtilities.DependencyGraph t = new SpreadsheetUtilities.DependencyGraph();
            t.AddDependency("a", "b");
            t.AddDependency("b", "b");
            t.RemoveDependency("c", "b");
            Assert.IsTrue(t.Size == 2);
            t.AddDependency("b", "b");
            Assert.IsTrue(t.Size == 2);
        }

        /// <summary>
        /// Testing adding duplicate
        /// </summary>
        [TestMethod()]
        public void testAddNA()
        {
            SpreadsheetUtilities.DependencyGraph t = new SpreadsheetUtilities.DependencyGraph();
            t.AddDependency("a", "b");
            t.AddDependency("b", "b");
            t.AddDependency("b", "b");
            Assert.IsTrue(t.Size == 2);
        }

        /// <summary>
        /// Testing has Dependees false
        /// </summary>
        [TestMethod()]
        public void testHasDependeesFalse()
        {
            SpreadsheetUtilities.DependencyGraph t = new SpreadsheetUtilities.DependencyGraph();
            t.AddDependency("a", "b");
            t.AddDependency("b", "b");
            t.AddDependency("c", "b");
            Assert.IsFalse(t.HasDependees("c"));
        }

        /// <summary>
        /// Testing removing the last num in dependency
        /// </summary>
        [TestMethod()]
        public void testRemoveLastNum()
        {
            SpreadsheetUtilities.DependencyGraph t = new SpreadsheetUtilities.DependencyGraph();
            t.AddDependency("a", "b");
            t.AddDependency("b", "b");
            t.AddDependency("c", "b");
            t.RemoveDependency("a", "b");
            Assert.IsFalse(t.HasDependents("a"));
        }

        /// <summary>
        /// Testing size of NA dependents chain
        /// </summary>
        [TestMethod()]
        public void testGetEmptyDependents()
        {
            SpreadsheetUtilities.DependencyGraph t = new SpreadsheetUtilities.DependencyGraph();
            t.AddDependency("a", "b");
            t.AddDependency("b", "b");
            t.AddDependency("c", "b");
            t.RemoveDependency("a", "b");
            IEnumerable<String> arr = t.GetDependents("r");
            int i = arr.Count();
            Assert.IsTrue(i == 0);
        }

        /// <summary>
        /// Testing NA dependees list
        /// </summary>
        [TestMethod()]
        public void testGetEmptyDependees()
        {
            SpreadsheetUtilities.DependencyGraph t = new SpreadsheetUtilities.DependencyGraph();
            t.AddDependency("a", "b");
            t.AddDependency("b", "b");
            t.AddDependency("c", "b");
            t.RemoveDependency("a", "b");
            IEnumerable<String> arr = t.GetDependees("r");
            int i = arr.Count();
            Assert.IsTrue(i == 0);
        }

        [TestMethod()]
        public void testCopyOfGetDependeesFalse()
        {
            SpreadsheetUtilities.DependencyGraph t = new SpreadsheetUtilities.DependencyGraph();
            t.AddDependency("a", "b");
            t.AddDependency("b", "b");
            t.AddDependency("c", "b");
            IEnumerable<String> result = t.GetDependees("b");
            t.AddDependency("r", "b");
            Assert.AreNotEqual(result, t.GetDependees("b"));
        }

        [TestMethod()]
        public void testCopyOfGetDependeesTrue()
        {
            SpreadsheetUtilities.DependencyGraph t = new SpreadsheetUtilities.DependencyGraph();
            t.AddDependency("a", "b");
            t.AddDependency("b", "b");
            t.AddDependency("c", "b");
            IEnumerable<String> result = t.GetDependees("b");
            Assert.AreNotEqual(result, t.GetDependees("b"));
        }

        [TestMethod()]
        public void testCopyOfGetDependentsTrue()
        {
            SpreadsheetUtilities.DependencyGraph t = new SpreadsheetUtilities.DependencyGraph();
            t.AddDependency("a", "b");
            t.AddDependency("b", "b");
            t.AddDependency("c", "b");
            IEnumerable<String> result = t.GetDependents("b");
            Assert.AreNotEqual(result, t.GetDependents("b"));
        }

        [TestMethod()]
        public void testCopyOfGetDependentsFalse()
        {
            SpreadsheetUtilities.DependencyGraph t = new SpreadsheetUtilities.DependencyGraph();
            t.AddDependency("a", "b");
            t.AddDependency("b", "b");
            t.AddDependency("c", "b");
            IEnumerable<String> result = t.GetDependents("b");
            t.AddDependency("b", "r");
            Assert.AreNotEqual(result, t.GetDependents("b"));
        }
    }
}

