using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cherry.Collections;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Cherry.Tests.Collections
{
    [TestClass]
    public class PriorityQueueTest
    {

        [TestMethod]
        public void EnsureExceptionOnIncomparable()
        {
            var pq = new PriorityQueue<MutableItem>();
            pq.Enqueue(new MutableItem("A", 1));
            Assert.ThrowsException<ArgumentException>(
                () => pq.Enqueue(new MutableItem("B", 2)));
        }

        [TestMethod]
        public void EnsureExceptionOnNullEnumerable()
        {
            Assert.ThrowsException<ArgumentNullException>(
                () => new PriorityQueue<int>((IEnumerable<int>)null));
        }

        [TestMethod]
        public void EnsureExceptionOnNullComparer()
        {
            IComparer<int> cmp = null;
            Assert.ThrowsException<ArgumentNullException>(
                () => new PriorityQueue<int>(cmp));
            Assert.ThrowsException<ArgumentNullException>(
                () => new PriorityQueue<int>(new int[] { 1, 2, 3 }, cmp));
        }

        [TestMethod]
        public void EnsureExceptionOnNullComparasion()
        {
            Comparison<int> cmp = null;
            Assert.ThrowsException<ArgumentNullException>(
                () => new PriorityQueue<int>(cmp));
            Assert.ThrowsException<ArgumentNullException>(
                () => new PriorityQueue<int>(new int[] { 1, 2, 3 }, cmp));
        }

        [TestMethod]
        public void EnsureExceptionOnEmptyDequeue()
        {
            var pq = new PriorityQueue<int>();
            Assert.ThrowsException<InvalidOperationException>(
                () => pq.Peek());
            Assert.ThrowsException<InvalidOperationException>(
                () => pq.Dequeue());
        }

        [TestMethod]
        public void TestSynchronisation()
        {
            var semaphore = new object();
            var pq = new PriorityQueue<int>(
                new List<int> { 1, 8, 11, 21, 13, 4, -50 });
            var collected = new ConcurrentBag<int>();
            Thread t = new(() =>
            {
                lock (pq.SyncRoot)
                {
                    foreach (var item in pq)
                    {
                        collected.Add(item);
                    }
                }
            });
            Thread blocker = new(() =>
            {
                lock (pq.SyncRoot)
                {
                    lock (semaphore)
                    {
                        Monitor.Wait(semaphore);
                    }
                }
            });
            blocker.Start();
            Thread.Sleep(500);
            t.Start();
            Thread.Sleep(5000);
            /* By now, had synchronisation failed,
             * we should have executed a portion of the for loop. */
            Assert.AreEqual(0, collected.Count);
            lock (semaphore)
            {
                Monitor.Pulse(semaphore);
            }
            while (t.IsAlive)
            {
                Thread.Sleep(500);
            }
            Assert.AreEqual(7, collected.Count);
        }

        [TestMethod]
        public void TestQueueCreation()
        {
            var someNumbers = new List<int> { 1, 8, 11, 21, 13, 4, -50 };
            var pq = new PriorityQueue<int>(someNumbers);
            Assert.AreEqual(7, pq.Count);
            VerifyDequeueOrder(pq, 21, 13, 11, 8, 4, 1, -50);
        }

        [TestMethod]
        public void TestQueueCopy()
        {
            var someNumbers = new List<int> { 1, 8, 11, 21, 13, 4, -50 };
            var pq = new PriorityQueue<int>(someNumbers);
            var pqCopy = new PriorityQueue<int>(pq);
            Assert.AreEqual(7, pqCopy.Count);
            VerifyDequeueOrder(pqCopy, 21, 13, 11, 8, 4, 1, -50);
        }

        [TestMethod]
        public void TestMinQueueCreation()
        {
            var someNumbers = new List<int> { 1, 8, 11, 21, 13, 4, -50 };
            var pq = new PriorityQueue<int>(someNumbers,
                (e1, e2) => e2.CompareTo(e1));
            Assert.AreEqual(7, pq.Count);
            VerifyDequeueOrder(pq, -50, 1, 4, 8, 11, 13, 21);
        }

        [TestMethod]
        public void TestItemAddition()
        {
            var pq = new PriorityQueue<int>();
            pq.Enqueue(1);
            pq.Enqueue(8);
            pq.Enqueue(11);
            pq.Add(21);
            pq.Add(13);
            pq.Enqueue(4);
            pq.Enqueue(-50);
            Assert.AreEqual(7, pq.Count);
            VerifyDequeueOrder(pq, 21, 13, 11, 8, 4, 1, -50);
        }

        [TestMethod]
        public void TestItemRemoval()
        {
            var pq = new PriorityQueue<int>(
                new[] { 10, 5, 9, 2, 3, 7, 8 });

            Assert.AreEqual(7, pq.Count);

            Assert.IsTrue(pq.Remove(2));
            Assert.AreEqual(6, pq.Count);

            Assert.IsFalse(pq.Remove(2));
            Assert.AreEqual(6, pq.Count);

            Assert.IsTrue(pq.Remove(8));
            Assert.AreEqual(5, pq.Count);
            Assert.IsFalse(pq.Remove(8));
            Assert.AreEqual(5, pq.Count);

            VerifyDequeueOrder(pq, 10, 9, 7, 5, 3);
            Assert.IsFalse(pq.Remove(10));
            Assert.IsFalse(pq.Remove(9));
            Assert.IsFalse(pq.Remove(7));
            Assert.IsFalse(pq.Remove(5));
            Assert.IsFalse(pq.Remove(3));
        }

        [TestMethod]
        public void TestPriorityUpdate()
        {
            var a = new MutableItem("A", 21);
            var b = new MutableItem("B", 12);
            var c = new MutableItem("C", -3);
            var d = new MutableItem("D", 41);
            var e = new MutableItem("E", 15);
            var f = new MutableItem("F", 10);
            var g = new MutableItem("G", 5);
            var h = new MutableItem("H", 1);
            var i = new MutableItem("I", 81);
            var j = new MutableItem("J", 9);

            static int comp(MutableItem e1, MutableItem e2)
                => e1.Priority.CompareTo(e2.Priority);

            var items = new[] { a, b, c, d, e, f, g, h, i, j };

            var pq = new PriorityQueue<MutableItem>(items, comp);

            //Edge case; update priority of largest to even higher
            pq.UpdatePriority(i, item => item.Priority = 82);
            VerifyDequeueOrder(pq, i, d, a, e, b, f, j, g, h, c);

            //Re-create the queue
            pq = new PriorityQueue<MutableItem>(items, comp);

            //Edge case; update priority of a lowest priority node
            pq.UpdatePriority(c, item => item.Priority = -10);
            VerifyDequeueOrder(pq, i, d, a, e, b, f, j, g, h, c);

            //Re-create the queue; decrease priority of an item
            pq = new PriorityQueue<MutableItem>(items, comp);
            pq.UpdatePriority(i, item => item.Priority = 16);
            VerifyDequeueOrder(pq, d, a, i, e, b, f, j, g, h, c);

            //Re-create the queue; increase some priorities
            pq = new PriorityQueue<MutableItem>(items, comp);
            pq.UpdatePriority(c, item => item.Priority = 810);
            pq.UpdatePriority(h, item => item.Priority = 801);
            VerifyDequeueOrder(pq, c, h, d, a, i, e, b, f, j, g);
        }

        [TestMethod]
        public void TestAddRange()
        {
            var a = new MutableItem("A", 21);
            var b = new MutableItem("B", 12);
            var c = new MutableItem("C", -3);
            var d = new MutableItem("D", 41);
            var e = new MutableItem("E", 15);
            var f = new MutableItem("F", 10);
            var g = new MutableItem("G", 5);
            var h = new MutableItem("H", 1);
            var i = new MutableItem("I", 81);
            var j = new MutableItem("J", 9);

            var pq = new PriorityQueue<MutableItem>(
                new[] { a, b, c, d, e, f, g, h, i, j },
                (e1, e2) => e1.Priority.CompareTo(e2.Priority));

            var k = new MutableItem("K", 11);
            var l = new MutableItem("L", 18);
            var m = new MutableItem("M", 31);
            var n = new MutableItem("N", 2);
            pq.AddRange(new[] { k, l, m, n });

            VerifyDequeueOrder(pq, i, d, m, a, l, e, b, k, f, j, g, n, h, c);
        }

        [TestMethod]
        public void TestAddRangeWithStreams()
        {
            var a = new MutableItem("A", 21);
            var b = new MutableItem("B", 12);
            var c = new MutableItem("C", -3);
            var d = new MutableItem("D", 41);
            var e = new MutableItem("E", 15);
            var f = new MutableItem("F", 10);
            var g = new MutableItem("G", 5);
            var h = new MutableItem("H", 1);
            var i = new MutableItem("I", 81);
            var j = new MutableItem("J", 9);

            var pq = new PriorityQueue<MutableItem>(
                new[] { a, b, c, d, e, f, g, h, i, j },
                (e1, e2) => e1.Priority.CompareTo(e2.Priority));

            var k = new MutableItem("K", 11);
            var l = new MutableItem("L", 18);
            var m = new MutableItem("M", 31);
            var n = new MutableItem("N", 2);
            pq.AddRange(new[] { k, l, m, n }.Skip(2));

            VerifyDequeueOrder(pq, i, d, m, a, e, b, f, j, g, n, h, c);
        }

        [TestMethod]
        public void TestSamePriorityElements()
        {
            var a = new MutableItem("A", 1);
            var b = new MutableItem("B", 1);
            var c = new MutableItem("C", 2);
            var d = new MutableItem("D", 2);
            var e = new MutableItem("E", 50);

            var pq = new PriorityQueue<MutableItem>(
                new[] { a, b, c, d, e },
                (e1, e2) => e1.Priority.CompareTo(e2.Priority));

            AnyEqual(pq.Dequeue(), e);
            AnyEqual(pq.Dequeue(), d, c);
            AnyEqual(pq.Dequeue(), d, c);
            AnyEqual(pq.Dequeue(), a, b);
            AnyEqual(pq.Dequeue(), a, b);
        }

        [TestMethod]
        public void TestContains()
        {
            var a1 = new MutableItem("A", 1);
            var a2 = new MutableItem("A", 2);
            var b1 = new MutableItem("B", 3);
            var b2 = new MutableItem("B", 4);
            var c1 = new MutableItem("C", 5);

            var pq = new PriorityQueue<MutableItem>(
                new[] { a1, a2, b1, b2, c1 },
                (e1, e2) => e1.Priority.CompareTo(e2.Priority));

            foreach (var item in new[] { a1, a2, b1, b2, c1 })
            {
                Assert.IsTrue(pq.Contains(item));
            }

            Assert.IsTrue(pq.Remove(b2));
            Assert.IsTrue(pq.Remove(b2));
            Assert.IsFalse(pq.Remove(b2));
            Assert.IsFalse(pq.Remove(b1));
            Assert.IsFalse(pq.Contains(b2));
            Assert.IsFalse(pq.Contains(b1));
        }

        [TestMethod]
        public void TestEnumeration()
        {
            var items = new List<int> { 1903, 14, -104, 4234, 12, 191 };
            var pq = new PriorityQueue<int>(items);
            int i = 0;
            foreach (var item in pq)
            {
                i += 1;
                items.Remove(item);
            }
            Assert.AreEqual(pq.Count, i);
            Assert.AreEqual(0, items.Count);
        }

        [TestMethod]
        public void TestCopyTo()
        {
            var items = new int[] { 1903, 14, 104, 4234, 12, 191 };
            var pq = new PriorityQueue<int>(items);
            var array = new int[pq.Count];
            pq.CopyTo(array, 0);
            foreach (var item in items)
            {
                Assert.IsTrue(array.Contains(item));
            }

            var arr2 = new int[pq.Count + 2];
            arr2[0] = -1;
            arr2[1] = -2;
            pq.CopyTo((Array)arr2, 2);
            Assert.AreEqual(-1, arr2[0]);
            Assert.AreEqual(-2, arr2[1]);
            foreach (var item in items)
            {
                Assert.IsTrue(array.Contains(item));
            }
        }

        [TestMethod]
        public void EnsureExceptionOnCopyToInsufficientSpace()
        {
            var items = new int[] { 1903, 14, 104, 4234, 12, 191 };
            var pq = new PriorityQueue<int>(items);
            var array = new int[pq.Count - 3];
            Assert.ThrowsException<ArgumentException>(
                () => pq.CopyTo(array, 0));
        }

        [TestMethod]
        public void EnsureExceptionOnCopyToOutOfRangeIndex()
        {
            var items = new int[] { 1903, 14, 104, 4234, 12, 191 };
            var pq = new PriorityQueue<int>(items);
            var array = new int[pq.Count];
            Assert.ThrowsException<ArgumentOutOfRangeException>(
                () => pq.CopyTo(array, -1));
        }

        [TestMethod]
        public void EnsureExceptionOnCopyToWrongDestinationType()
        {
            var items = new int[] { 1903, 14, 104, 4234, 12, 191 };
            var pq = new PriorityQueue<int>(items);
            Array array = new string[pq.Count];
            Assert.ThrowsException<ArrayTypeMismatchException>(
                () => pq.CopyTo(array, 0));
        }

        [TestMethod]
        public void EnsureExceptionOnCopyToRankMismatch()
        {
            var items = new int[] { 1903, 14, 104, 4234, 12, 191 };
            var pq = new PriorityQueue<int>(items);
            Array array = new string[pq.Count, 2];
            Assert.ThrowsException<RankException>(
                () => pq.CopyTo(array, 0));
        }

        [TestMethod]
        public void EnsureExceptionOnCopyToNullTargetArray()
        {
            var items = new int[] { 1903, 14, 104, 4234, 12, 191 };
            var pq = new PriorityQueue<int>(items);
            Assert.ThrowsException<ArgumentNullException>(
                () => pq.CopyTo(null, 0));
        }

        [TestMethod]
        public void TestClear()
        {
            var items = GetUniqueRandomInts();
            var pq = new PriorityQueue<int>(items);
            pq.Clear();
            Assert.AreEqual(0, pq.Count);
            foreach (var item in items)
            {
                Assert.IsFalse(pq.Contains(item));
            }
        }

        [TestMethod]
        public void TestClone()
        {
            var items = GetUniqueRandomInts();
            var pq = new PriorityQueue<int>(items);
            var clone = (PriorityQueue<int>)pq.Clone();
            Assert.AreEqual(pq.Count, clone.Count);
            while (pq.Count > 0)
            {
                Assert.AreEqual(pq.Dequeue(), clone.Dequeue());
            }
            Assert.AreEqual(0, clone.Count);
        }

        [TestMethod]
        public void TestCloneSideEffects()
        {
            var a = new MutableItem("A", 21);
            var b = new MutableItem("B", 12);
            var c = new MutableItem("C", -3);
            var d = new MutableItem("D", 41);
            var e = new MutableItem("E", 15);
            var f = new MutableItem("F", 10);
            var g = new MutableItem("G", 5);
            var h = new MutableItem("H", 1);
            var i = new MutableItem("I", 81);
            var j = new MutableItem("J", 9);

            static int comp(MutableItem e1, MutableItem e2)
                => e1.Priority.CompareTo(e2.Priority);

            var items = new[] { a, b, c, d, e, f, g, h, i, j };

            var pq = new PriorityQueue<MutableItem>(items, comp);
            var clone = (PriorityQueue<MutableItem>)pq.Clone();
            VerifyDequeueOrder(pq, i, d, a, e, b, f, j, g, h, c);
            VerifyDequeueOrder(clone, i, d, a, e, b, f, j, g, h, c);
        }

        [TestMethod]
        public void TestCloneSynchronisation()
        {
            var semaphore = new object();
            var pq = new PriorityQueue<int>(
                new List<int> { 1, 8, 11, 21, 13, 4, -50 });
            var clone = (PriorityQueue<int>)pq.Clone();
            var collected = new ConcurrentBag<int>();
            Thread t = new(() =>
            {
                lock (pq.SyncRoot)
                {
                    foreach (var item in pq)
                    {
                        collected.Add(item);
                    }
                }
            });
            Thread blocker = new(() =>
            {
                lock (clone.SyncRoot)
                {
                    lock (semaphore)
                    {
                        Monitor.Wait(semaphore);
                    }
                }
            });
            blocker.Start();
            Thread.Sleep(500);
            t.Start();
            while (t.IsAlive)
            {
                Thread.Sleep(500);
            }
            Assert.AreEqual(7, collected.Count, "SyncRoot shared by clones");
            lock (semaphore)
            {
                Monitor.Pulse(semaphore);
            }
            while (blocker.IsAlive)
            {
                Thread.Sleep(500);
            }
        }

        #region Helpers

        private static void AnyEqual<E>(E actual, params E[] anyExpected)
        {
            Assert.IsTrue(anyExpected.Any(e => Equals(actual, e)));
        }

        private static ISet<int> GetUniqueRandomInts()
        {
            const int SMPL_SIZE = 10;
            var items = new HashSet<int>(SMPL_SIZE);
            var random = new Random();
            while (items.Count < SMPL_SIZE)
            {
                items.Add(random.Next());
            }
            return items;
        }

        private class MutableItem
        {
            public int Priority { get; set; }

            public string Name { get; }

            public MutableItem(string name, int priority)
            {
                Name = name;
                Priority = priority;
            }

            public override bool Equals(object obj) =>
                obj is MutableItem m && m.Name == Name;

            public override int GetHashCode() => Name.GetHashCode();

            public override string ToString() =>
                $"Name: { Name }, Priority { Priority }";
        }

        private static void VerifyDequeueOrder<E>(PriorityQueue<E> queue,
                                                  params E[] expectedOrder)
        {
            int numElemsRemaining = expectedOrder.Length;
            foreach (var item in expectedOrder)
            {
                Assert.AreEqual(numElemsRemaining--, queue.Count);
                Assert.AreEqual(item, queue.Peek());
                Assert.AreEqual(item, queue.Dequeue());
            }
        }

        #endregion

    }
}