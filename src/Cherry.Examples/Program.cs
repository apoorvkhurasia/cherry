using Cherry.Collection;
using System;
using System.Collections.Generic;

namespace Cherry.Examples
{
    class PriorityQueueExamples
    {
        static void Main()
        {
            MaxQueueExample();
            MinQueueExample();
            UpdateExample();
        }

        private static void MaxQueueExample()
        {
            var someNumbers = new List<int> { 1, 8, 11, 21, 13, 4, -50 };
            var maxQueue = new PriorityQueue<int>(someNumbers);
            Console.WriteLine(maxQueue.Count); //Prints 7
            Console.WriteLine(maxQueue.Dequeue()); //Prints 21
            Console.WriteLine(maxQueue.Peek()); //Prints 13
            Console.WriteLine(maxQueue.Dequeue()); //Prints 13
            Console.WriteLine(maxQueue.Contains(1)); //Prints True
            Console.WriteLine(maxQueue.Contains(-4)); //Prints False
        }

        private static void MinQueueExample()
        {
            var someNumbers = new List<int> { 1, 8, 11, 21, 13, 4, -50 };
            var minQueue = new PriorityQueue<int>(someNumbers,
                (e1, e2) => e2.CompareTo(e1));
            Console.WriteLine(minQueue.Count); //Prints 7
            Console.WriteLine(minQueue.Dequeue()); //Prints -50
            Console.WriteLine(minQueue.Peek()); //Prints 1
            Console.WriteLine(minQueue.Dequeue()); //Prints 1
            Console.WriteLine(minQueue.Contains(1)); //Prints False
            Console.WriteLine(minQueue.Contains(-4)); //Prints False
            Console.WriteLine(minQueue.Dequeue()); //Prints 4
        }

        private static void UpdateExample()
        {
            var a = new ExampleItem("A", 1);
            var b = new ExampleItem("B", 2);
            var c = new ExampleItem("C", 3);
            var d = new ExampleItem("D", 4);
            var e = new ExampleItem("E", 5);
            var f = new ExampleItem("F", 6);
            var items = new List<ExampleItem> { a, b, c, d, e, f };

            // Should be F, E, D, C, B, A
            var queue = new PriorityQueue<ExampleItem>(items,
                (e1, e2) => e1.Priority.CompareTo(e2.Priority));

            queue.UpdatePriority(c, item => item.Priority = 50);
            //Will print C F E D B A
            while (queue.Count > 0)
            {
                Console.Write(queue.Dequeue() + " ");
            }
        }

        private class ExampleItem
        {
            public int Priority { get; set; }

            public string Name { get; }

            public ExampleItem(string name, int priority)
            {
                Name = name;
                Priority = priority;
            }

            public override bool Equals(object other)
            {
                return other is ExampleItem i && i.Name == this.Name;
            }

            public override int GetHashCode() => Name.GetHashCode();

            public override string ToString() => Name;
        }
    }
}

