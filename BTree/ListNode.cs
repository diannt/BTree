using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BTree
{
    public class ListNode
    {
        private object _Data;
        private ListNode _Next;
        private ListNode _Prev;
        public object Value
        {
            get { return _Data; }
            set { _Data = value; }
        }
        public ListNode(object Data)
        {
            this._Data = Data;
        }
        public ListNode Next
        {
            get { return this._Next; }
            set { this._Next = value; }
        }
        public ListNode Prev
        {
            get { return this._Prev; }
            set { this._Prev = value; }
        }
    }
}
