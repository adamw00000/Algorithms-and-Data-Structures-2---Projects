
using System;
using System.Collections.Generic;

namespace ASD
{

public interface IPriorityQueue
    {
    void Put(int p);     // wstawia element do kolejki
    int GetMax();        // pobiera maksymalny element z kolejki (element jest usuwany z kolejki)
    int ShowMax();       // pokazuje maksymalny element kolejki (element pozostaje w kolejce)
    int Count { get; }   // liczba elementów kolejki
    }


public class LazyPriorityQueue : MarshalByRefObject, IPriorityQueue
    {
        Node head;
        int count;

        public LazyPriorityQueue()
        {
            head = null;
        }

    public void Put(int p)
        {
            head = new Node(p, head);
            count++;
        }

    public int GetMax()
        {
            CheckIfQueueIsEmpty();
            
            FindMax(out Node p, out Node pp);

            if (head != null)
            {
                if (pp != null)
                {
                    pp.next = p.next;
                }
                else
                {
                    head = p.next;
                }
                count--;
                return p.value;
            }
            return 0;
        }

        private void FindMax(out Node pMax, out Node ppMax)
        {
            pMax = head;
            ppMax = null;
            Node p = head, pp = null;
            while (p != null)
            {
                if (p.value > pMax.value)
                {
                    pMax = p;
                    ppMax = pp;
                }

                pp = p;
                p = p.next;
            }
        }

        private void CheckIfQueueIsEmpty()
        {
            if (head == null)
            {
                throw new InvalidOperationException("Access to empty queue");
            }
        }

        public int ShowMax()
        {
            CheckIfQueueIsEmpty();

            if (head != null)
            {
                FindMax(out Node p, out Node pp);
                return p.value;
            }
            return 0;
        }

    public int Count
        {
            get
            {
                return count;
            }
        }

    } // LazyPriorityQueue


public class EagerPriorityQueue : MarshalByRefObject, IPriorityQueue
    {
        Node head;
        int count;
        public EagerPriorityQueue()
        {
            head = null;
        }

        private void CheckIfQueueIsEmpty()
        {
            if (head == null)
            {
                throw new InvalidOperationException("Access to empty queue");
            }
        }
        private void FindPosition(out Node p, out Node pp, int v)
        {
            p = head;
            pp = null;
            while (p != null && p.value >= v)
            {
                pp = p;
                p = p.next;
            }
        }
        public void Put(int v)
        {
            FindPosition(out Node p, out Node pp, v);
            count++;

            if (pp == null)
            {
                head = new Node(v, head);
                return;
            }

            pp.next = new Node(v, p);
        }

    public int GetMax()
        {
            CheckIfQueueIsEmpty();
            if (head != null)
            {
                Node p = head;
                head = head.next;
                count--;
                return p.value;
            }
            return 0;
        }

    public int ShowMax()
        {
            CheckIfQueueIsEmpty();
            if (head != null)
            {
                return head.value;
            }
            return 0;
        }

    public int Count
        {
        get {
                return count;
            }
        }

    } // EagerPriorityQueue

    class Node
    {
        public int value { get; }
        public Node next { get; set; }

        public Node(int v, Node n)
        {
            value = v;
            next = n;
        }

        public Node(int v)
        {
            value = v;
            next = null;
        }
    }

public class HeapPriorityQueue : MarshalByRefObject, IPriorityQueue
    {
        List<int> heap;
        int size;
    public HeapPriorityQueue()
        {
            heap = new List<int>();
            heap.Add(Int32.MaxValue);
        }
        private void CheckIfQueueIsEmpty()
        {
            if (size == 0)
            {
                throw new InvalidOperationException("Access to empty queue");
            }
        }
        void UpHeap(int i)
        {
            int v = heap[i];

            while (heap[i/2] < v)
            {
                heap[i] = heap[i/2];
                i = i / 2;
            }
            heap[i] = v;
        }
        void DownHeap(int i)
        {
            if (size == 0)
                return;
            int v = heap[i];
            int k = 2 * i;

            while (k <= size)
            {
                if (k+1 <= size)
                {
                    if (heap[k + 1] > heap[k])
                        k = k + 1;
                }

                if (heap[k] > v)
                {
                    heap[i] = heap[k];
                    i = k;
                    k = 2 * i;
                }
                else
                    break;
            }
            heap[i] = v;
        }
        public void Put(int p)
        {
            heap.Add(p);
            size++;
            UpHeap(size);
        }

    public int GetMax()
        {
            CheckIfQueueIsEmpty();

            if (size > 0)
            {
                int v = heap[1];
                heap[1] = heap[size];
                heap.RemoveAt(size);
                size--;
                DownHeap(1);
                return v;
            }
            return 0;
        }

    public int ShowMax()
        {
            CheckIfQueueIsEmpty();

            if (size > 0)
            {
                return heap[1];
            }
            return 0;
        }

    public int Count
        {
        get {
                return size;
            }
        }

    } // HeapPriorityQueue

}
