# Cherry Collections
[![Build](https://github.com/apoorvkhurasia/cherry/workflows/Build/badge.svg)](https://github.com/apoorvkhurasia/cherry/actions?query=workflow%3ABuild) [![License: MIT](https://img.shields.io/badge/License-MIT-red.svg)](https://github.com/apoorvkhurasia/cherry/blob/master/LICENSE)

This library provides collections that are missing in the .Net SDK as of .Net 5.0.

The following collections are provided:

## PriorityQueue
This is a thread unsafe priority queue. You can add any type of
items to this queue so long as you have a way to assign them
relative priority. This can be achieved in two ways:

  1. Having the items implement `IComparable<T>` interface. This can
  be a natural way to prioritise items based on their properties.
  2. Passing a `Comparision<T>` function or by passing 
  an `IComparer<T>` implementation. Using either of these two techniques
  you can delegate the responsibility of assigning relative priority
  to another class.
  
Whether the queue is a max priority queue or a min priority queue will
depend on your choice of assigning relative priorities. By default, if
you store `IComparable<T>` objects in this queue and do not specify 
any other ordering then the queue will behave as a max priority queue.

### Examples
#### Max Priority Queue of numbers
```cs
var someNumbers = new List<int> { 1, 8, 11, 21, 13, 4, -50 };
var pq = new PriorityQueue<int>(someNumbers);
Console.WriteLine(pq.Count); //Prints 7
Console.WriteLine(pq.Dequeue()); //Prints 21
Console.WriteLine(pq.Peek()); //Prints 13
Console.WriteLine(pq.Dequeue()); //Prints 13
Console.WriteLine(pq.Contains(1)); //Prints True
Console.WriteLine(pq.Contains(-4)); //Prints False
Console.WriteLine(pq.Replace(4, 28)); //Prints True
Console.WriteLine(pq.Dequeue()); //Prints 28
```

#### Min Priority Queue of numbers
```cs
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
```
#### Custom comparision method and priority updates
```cs
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
```
with
```cs
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
```
