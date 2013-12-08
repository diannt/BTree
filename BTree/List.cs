using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BTree
{
    public class List
    {
        private ListNode First;
        private ListNode Current;
        private ListNode Last;
        private uint size;
        
        public List()
        {
            size = 0;
            First = Current = Last = null;
        }

        public bool isEmpty //check for emptyness
        {
            get
            {
                return size == 0;
            }
        }

        public void Insert_Index(object newElement, uint index) //insert by index
        {
            if (index < 1 || index > size) //throw exception, if the index was wrong
            {
                throw new InvalidOperationException();
            }
            else if (index == 1) //if list begins
            {
                Push_Front(newElement);
            }
            else if (index == size) //if list ends
            {
                Push_Back(newElement);
            }
            else //or lets search by this index
            {
                uint count = 1;
                Current = First;
                while (Current != null && count != index)
                {
                    Current = Current.Next;
                    count++;
                }
                ListNode newListNode = new ListNode(newElement); //create object
                Current.Prev.Next = newListNode;
                newListNode.Prev = Current.Prev;
                Current.Prev = newListNode;
                newListNode.Next = Current;
            }
        }

        public void Push_Front(object newElement)
        {
            ListNode newListNode = new ListNode(newElement);

            if (First == null)
            {
                First = Last = newListNode;
            }
            else
            {
                newListNode.Next = First;
                First = newListNode; //First and newListNode points on one object
                newListNode.Next.Prev = First;
            }
            Count++;
        }

        public ListNode Pop_Front()
        {
            if (First == null)
            {
                throw new InvalidOperationException();
            }
            else
            {
                ListNode temp = First;
                if (First.Next != null)
                {
                    First.Next.Prev = null;
                }
                First = First.Next;
                Count--;
                return temp;
            }
        }

        public void Push_Back(object newElement)
        {
            ListNode newListNode = new ListNode(newElement);

            if (First == null)
            {
                First = Last = newListNode;
            }
            else
            {
                Last.Next = newListNode;
                newListNode.Prev = Last;
                Last = newListNode;
            }
            Count++;
        }

        public ListNode Pop_Back()
        {
            if (Last == null)
            {
                throw new InvalidOperationException();
            }
            else
            {
                ListNode temp = Last;
                if (Last.Prev != null)
                {
                    Last.Prev.Next = null;
                }
                Last = Last.Prev;
                Count--;
                return temp;
            }
        }

        public void ClearList() //полностью очистить список
        {
            while (!isEmpty)
            {
                Pop_Front();
            }
        }

        public uint Count //свойство для size
        {
            get { return size; }
            set { size = value; }
        }

        public void Display() //show
        {
            if (First == null)
            {
                Console.WriteLine("List is empty");
                return;
            }
            Current = First;
            uint count = 1;
            while (Current != null)
            {
                Console.WriteLine("Element " + count.ToString() + " : " + Current.Value.ToString());
                count++;
                Current = Current.Next;
            }
        }

        public void DeleteElement(uint index)
        { //delete element by the index
            if (index < 1 || index > size)
            {
                throw new InvalidOperationException();
            }
            else if (index == 1)
            {
                Pop_Front();
            }
            else if (index == size)
            {
                Pop_Back();
            }
            else
            {
                uint count = 1;
                Current = First;
                while (Current != null && count != index)
                {
                    Current = Current.Next;
                    count++;
                }
                Current.Prev.Next = Current.Next;
                Current.Next.Prev = Current.Prev;
            }
        }

        public ListNode FindListNode(object Data) //find by data
        {
            Current = First;
            while (Current != null)
            {
                Current = Current.Next;
            }
            return Current;
        }

        public object FindObject(int _index)
        {
            Current = First;
            int index = 1;
            while (Current != null)
            {
                if (index == _index)
                {
                    return Current.Value;
                }
                index++;
                Current = Current.Next;
            }
            return null;
        }
        
        public uint GetIndex(object Data) //take index by data
        {
            Current = First;
            uint index = 1;
            while (Current != null)
            {
                Current = Current.Next;
                index++;
            }
            return index;

        }

    }
}
