using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using BTree;

namespace ConsoleTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Student temp=null;
            int key;
            var bTree = new BTree<int, Student>(Degree);
            var tempEntry = new Entry<int, Student>();
            while (true)
            {
                Console.WriteLine("------------------------------------------");
                Console.WriteLine("Enter 0 to generate the student");
                Console.WriteLine("Enter 1 to insert into the tree");
                Console.WriteLine("Enter 2 to search in the tree by the key");
                Console.WriteLine("Enter 3 to delete from the tree by the key");
                Console.WriteLine("Enter 9 to exit");
                Console.WriteLine("------------------------------------------");
                int k = -1;
                if (int.TryParse(Console.ReadLine(), out k))
                {
                    switch (k)
                    {
                        case 0:
                            temp = null;
                            temp = new Student();
                            Console.WriteLine("Student is generated!");
                            Console.WriteLine(temp.Print());
                            break;
                        case 1:
                            if (temp != null)
                            {
                                Console.WriteLine("Enter new key!");
                                if (int.TryParse(Console.ReadLine(), out key))
                                {
                                    Console.WriteLine("key " + key + " student: " + temp.Print());
                                    bTree.Insert(key, temp);
                                }
                                else
                                {
                                    Console.WriteLine("You wrote not number, try again!");
                                }
                            }
                            else
                            {
                                Console.WriteLine("Generate student before!");
                            }
                            break;
                        case 2:
                            Console.WriteLine("Enter search key!");
                            if (int.TryParse(Console.ReadLine(), out key))
                            {
                                Console.WriteLine("key " + key);
                                tempEntry = bTree.Search(key);
                                Console.WriteLine("Entry: " + ((Student) tempEntry.Pointer).Print());
                            }
                            else
                            {
                                Console.WriteLine("You wrote not number, try again!");
                            }
                            break;
                        case 3:
                            Console.WriteLine("Enter deleting key!");
                            if (int.TryParse(Console.ReadLine(), out key))
                            {
                                Console.WriteLine("key " + key);
                                bTree.Delete(key);
                                Console.WriteLine("Successfully deleted!");
                            }
                            else
                            {
                                Console.WriteLine("You wrote not number, try again!");
                            }
                            break;
                        case 9:
                            Environment.Exit(0);
                            break;
                    }
                }
                else
                {
                    Console.WriteLine("You have entered wrong command! Try again.");
                }
            }
        }
        private const int Degree = 2;

        private readonly int[] testKeyData = new int[] { 10, 20, 30, 50 };
        private readonly int[] testPointerData = new int[] { 50, 60, 40, 20 };

        public void CreateBTree()
        {
            var btree = new BTree<int, int>(Degree);

            TreeNode<int, int> root = btree.Root;
        }

        public void InsertOneNode()
        {
            var btree = new BTree<int, int>(Degree);
            this.InsertTestDataAndValidateTree(btree, 0);
        }

        public void InsertMultipleNodesToSplit()
        {
            var btree = new BTree<int, int>(Degree);

            for (int i = 0; i < this.testKeyData.Length; i++)
            {
                this.InsertTestDataAndValidateTree(btree, i);
            }
        }

        public void DeleteNodes()
        {
            var btree = new BTree<int, int>(Degree);

            for (int i = 0; i < this.testKeyData.Length; i++)
            {
                this.InsertTestData(btree, i);
            }

            for (int i = 0; i < this.testKeyData.Length; i++)
            {
                btree.Delete(this.testKeyData[i]);
            }
        }

        public void SearchNodes()
        {
            var btree = new BTree<int, int>(Degree);

            for (int i = 0; i < this.testKeyData.Length; i++)
            {
                this.InsertTestData(btree, i);
                this.SearchTestData(btree, i);
            }
        }

        public void SearchNonExistingNode()
        {
            var btree = new BTree<int, int>(Degree);

            // search an empty tree
            Entry<int, int> nonExisting = btree.Search(9999);

            for (int i = 0; i < this.testKeyData.Length; i++)
            {
                this.InsertTestData(btree, i);
                this.SearchTestData(btree, i);
            }

            // search a populated tree
            nonExisting = btree.Search(9999);
        }

        private void InsertTestData(BTree<int, int> btree, int testDataIndex)
        {
            btree.Insert(this.testKeyData[testDataIndex], this.testPointerData[testDataIndex]);
        }

        private void InsertTestDataAndValidateTree(BTree<int, int> btree, int testDataIndex)
        {
            btree.Insert(this.testKeyData[testDataIndex], this.testPointerData[testDataIndex]);
        }

        private void SearchTestData(BTree<int, int> btree, int testKeyDataIndex)
        {
            for (int i = 0; i <= testKeyDataIndex; i++)
            {
                Entry<int, int> entry = btree.Search(this.testKeyData[i]);
            }
        }

    }
}
