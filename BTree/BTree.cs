namespace BTree
{
    using System;
    using System.Diagnostics;
    using System.Linq;

    /// <typeparam name="TK">Type of BTree Key.</typeparam>
    /// <typeparam name="TP">Type of BTree Pointer associated with each Key.</typeparam>
    public class BTree<TK, TP> where TK : IComparable<TK>
    {
        public BTree(int degree)
        {
            if (degree < 2)
            {
                throw new ArgumentException("BTree degree must be at least 2", "degree");
            }

            this.Root = new TreeNode<TK, TP>(degree);
            this.Degree = degree;
            this.Height = 1;
        }

        public TreeNode<TK, TP> Root { get; private set; }

        public int Degree { get; private set; }

        public int Height { get; private set; }

        /// <summary>
        /// Searches a key in the BTree, returning the entry with it and with the pointer.
        /// </summary>
        /// <param name="key">Key being searched.</param>
        /// <returns>Entry for that key, null otherwise.</returns>
        public Entry<TK, TP> Search(TK key)
        {
            return this.SearchInternal(this.Root, key);
        }

        /// <summary>
        /// Inserts a new key associated with a pointer in the BTree. This
        /// operation splits TreeNodes as required to keep the BTree properties.
        /// </summary>
        /// <param name="newKey">Key to be inserted.</param>
        /// <param name="newPointer">Pointer to be associated with inserted key.</param>
        public void Insert(TK newKey, TP newPointer)
        {
            // there is space in the root TreeNode
            if (!this.Root.HasReachedMaxEntries)
            {
                this.InsertNonFull(this.Root, newKey, newPointer);
                return;
            }

            // need to create new TreeNode and have it split
            TreeNode<TK, TP> oldRoot = this.Root;
            this.Root = new TreeNode<TK, TP>(this.Degree);
            this.Root.Children.Add(oldRoot);
            this.SplitChild(this.Root, 0, oldRoot);
            this.InsertNonFull(this.Root, newKey, newPointer);

            this.Height++;
        }

        /// <summary>
        /// Deletes a key from the BTree. This operations moves keys and TreeNodes
        /// as required to keep the BTree properties.
        /// </summary>
        /// <param name="keyToDelete">Key to be deleted.</param>
        public void Delete(TK keyToDelete)
        {
            this.DeleteInternal(this.Root, keyToDelete);

            // if root's last entry was moved to a child TreeNode, remove it
            if (this.Root.Entries.Count == 0 && !this.Root.IsLeaf)
            {
                this.Root = this.Root.Children.Single();
                this.Height--;
            }
        }

        /// <summary>
        /// Internal method to delete keys from the BTree
        /// </summary>
        /// <param name="TreeNode">TreeNode to use to start search for the key.</param>
        /// <param name="keyToDelete">Key to be deleted.</param>
        private void DeleteInternal(TreeNode<TK, TP> TreeNode, TK keyToDelete)
        {
            int i = TreeNode.Entries.TakeWhile(entry => keyToDelete.CompareTo(entry.Key) > 0).Count();

            // found key in TreeNode, so delete if from it
            if (i < TreeNode.Entries.Count && TreeNode.Entries[i].Key.CompareTo(keyToDelete) == 0)
            {
                this.DeleteKeyFromTreeNode(TreeNode, keyToDelete, i);
                return;
            }

            // delete key from subtree
            if (!TreeNode.IsLeaf)
            {
                this.DeleteKeyFromSubtree(TreeNode, keyToDelete, i);
            }
        }

        /// <summary>
        /// Helper method that deletes a key from a subtree.
        /// </summary>
        /// <param name="parentTreeNode">Parent TreeNode used to start search for the key.</param>
        /// <param name="keyToDelete">Key to be deleted.</param>
        /// <param name="subtreeIndexInTreeNode">Index of subtree TreeNode in the parent TreeNode.</param>
        private void DeleteKeyFromSubtree(TreeNode<TK, TP> parentTreeNode, TK keyToDelete, int subtreeIndexInTreeNode)
        {
            TreeNode<TK, TP> childTreeNode = parentTreeNode.Children[subtreeIndexInTreeNode];

            // TreeNode has reached min # of entries, and removing any from it will break the btree property,
            // so this block makes sure that the "child" has at least "degree" # of TreeNodes by moving an 
            // entry from a sibling TreeNode or merging TreeNodes
            if (childTreeNode.HasReachedMinEntries)
            {
                int leftIndex = subtreeIndexInTreeNode - 1;
                TreeNode<TK, TP> leftSibling = subtreeIndexInTreeNode > 0 ? parentTreeNode.Children[leftIndex] : null;

                int rightIndex = subtreeIndexInTreeNode + 1;
                TreeNode<TK, TP> rightSibling = subtreeIndexInTreeNode < parentTreeNode.Children.Count - 1
                                                ? parentTreeNode.Children[rightIndex]
                                                : null;
                
                if (leftSibling != null && leftSibling.Entries.Count > this.Degree - 1)
                {
                    // left sibling has a TreeNode to spare, so this moves one TreeNode from left sibling 
                    // into parent's TreeNode and one TreeNode from parent into this current TreeNode ("child")
                    childTreeNode.Entries.Insert(0, parentTreeNode.Entries[subtreeIndexInTreeNode]);
                    parentTreeNode.Entries[subtreeIndexInTreeNode] = leftSibling.Entries.Last();
                    leftSibling.Entries.RemoveAt(leftSibling.Entries.Count - 1);

                    if (!leftSibling.IsLeaf)
                    {
                        childTreeNode.Children.Insert(0, leftSibling.Children.Last());
                        leftSibling.Children.RemoveAt(leftSibling.Children.Count - 1);
                    }
                }
                else if (rightSibling != null && rightSibling.Entries.Count > this.Degree - 1)
                {
                    // right sibling has a TreeNode to spare, so this moves one TreeNode from right sibling 
                    // into parent's TreeNode and one TreeNode from parent into this current TreeNode ("child")
                    childTreeNode.Entries.Add(parentTreeNode.Entries[subtreeIndexInTreeNode]);
                    parentTreeNode.Entries[subtreeIndexInTreeNode] = rightSibling.Entries.First();
                    rightSibling.Entries.RemoveAt(0);

                    if (!rightSibling.IsLeaf)
                    {
                        childTreeNode.Children.Add(rightSibling.Children.First());
                        rightSibling.Children.RemoveAt(0);
                    }
                }
                else
                {
                    // this block merges either left or right sibling into the current TreeNode "child"
                    if (leftSibling != null)
                    {
                        childTreeNode.Entries.Insert(0, parentTreeNode.Entries[subtreeIndexInTreeNode]);
                        var oldEntries = childTreeNode.Entries;
                        childTreeNode.Entries = leftSibling.Entries;
                        childTreeNode.Entries.AddRange(oldEntries);
                        if (!leftSibling.IsLeaf)
                        {
                            var oldChildren = childTreeNode.Children;
                            childTreeNode.Children = leftSibling.Children;
                            childTreeNode.Children.AddRange(oldChildren);
                        }

                        parentTreeNode.Children.RemoveAt(leftIndex);
                        parentTreeNode.Entries.RemoveAt(subtreeIndexInTreeNode);
                    }
                    else
                    {
                        Debug.Assert(rightSibling != null, "TreeNode should have at least one sibling");
                        childTreeNode.Entries.Add(parentTreeNode.Entries[subtreeIndexInTreeNode]);
                        childTreeNode.Entries.AddRange(rightSibling.Entries);
                        if (!rightSibling.IsLeaf)
                        {
                            childTreeNode.Children.AddRange(rightSibling.Children);
                        }

                        parentTreeNode.Children.RemoveAt(rightIndex);
                        parentTreeNode.Entries.RemoveAt(subtreeIndexInTreeNode);
                    }
                }
            }

            // at this point, we know that "child" has at least "degree" TreeNodes, so we can
            // move on - this guarantees that if any TreeNode needs to be removed from it to
            // guarantee BTree's property, we will be fine with that
            this.DeleteInternal(childTreeNode, keyToDelete);
        }
        
        /// <summary>
        /// Helper method that deletes key from a TreeNode that contains it, be this
        /// TreeNode a leaf TreeNode or an internal TreeNode.
        /// </summary>
        /// <param name="TreeNode">TreeNode that contains the key.</param>
        /// <param name="keyToDelete">Key to be deleted.</param>
        /// <param name="keyIndexInTreeNode">Index of key within the TreeNode.</param>
        private void DeleteKeyFromTreeNode(TreeNode<TK, TP> TreeNode, TK keyToDelete, int keyIndexInTreeNode)
        {
            // if leaf, just remove it from the list of entries (we're guaranteed to have
            // at least "degree" # of entries, to BTree property is maintained
            if (TreeNode.IsLeaf)
            {
                TreeNode.Entries.RemoveAt(keyIndexInTreeNode);
                return;
            }

            TreeNode<TK, TP> predecessorChild = TreeNode.Children[keyIndexInTreeNode];
            if (predecessorChild.Entries.Count >= this.Degree)
            {
                Entry<TK, TP> predecessor = this.DeletePredecessor(predecessorChild);
                TreeNode.Entries[keyIndexInTreeNode] = predecessor;
            }
            else
            {
                TreeNode<TK, TP> successorChild = TreeNode.Children[keyIndexInTreeNode + 1];
                if (successorChild.Entries.Count >= this.Degree)
                {
                    Entry<TK, TP> successor = this.DeleteSuccessor(predecessorChild);
                    TreeNode.Entries[keyIndexInTreeNode] = successor;
                }
                else
                {
                    predecessorChild.Entries.Add(TreeNode.Entries[keyIndexInTreeNode]);
                    predecessorChild.Entries.AddRange(successorChild.Entries);
                    predecessorChild.Children.AddRange(successorChild.Children);

                    TreeNode.Entries.RemoveAt(keyIndexInTreeNode);
                    TreeNode.Children.RemoveAt(keyIndexInTreeNode + 1);

                    this.DeleteInternal(predecessorChild, keyToDelete);
                }
            }
        }

        /// <summary>
        /// Helper method that deletes a predecessor key (i.e. rightmost key) for a given TreeNode.
        /// </summary>
        /// <param name="TreeNode">TreeNode for which the predecessor will be deleted.</param>
        /// <returns>Predecessor entry that got deleted.</returns>
        private Entry<TK, TP> DeletePredecessor(TreeNode<TK, TP> TreeNode)
        {
            if (TreeNode.IsLeaf)
            {
                var result = TreeNode.Entries[TreeNode.Entries.Count - 1];
                TreeNode.Entries.RemoveAt(TreeNode.Entries.Count - 1);
                return result;
            }

            return this.DeletePredecessor(TreeNode.Children.Last());
        }

        /// <summary>
        /// Helper method that deletes a successor key (i.e. leftmost key) for a given TreeNode.
        /// </summary>
        /// <param name="TreeNode">TreeNode for which the successor will be deleted.</param>
        /// <returns>Successor entry that got deleted.</returns>
        private Entry<TK, TP> DeleteSuccessor(TreeNode<TK, TP> TreeNode)
        {
            if (TreeNode.IsLeaf)
            {
                var result = TreeNode.Entries[0];
                TreeNode.Entries.RemoveAt(0);
                return result;
            }

            return this.DeletePredecessor(TreeNode.Children.First());
        }

        /// <summary>
        /// Helper method that search for a key in a given BTree.
        /// </summary>
        /// <param name="TreeNode">TreeNode used to start the search.</param>
        /// <param name="key">Key to be searched.</param>
        /// <returns>Entry object with key information if found, null otherwise.</returns>
        private Entry<TK, TP> SearchInternal(TreeNode<TK, TP> TreeNode, TK key)
        {
            int i = TreeNode.Entries.TakeWhile(entry => key.CompareTo(entry.Key) > 0).Count();

            if (i < TreeNode.Entries.Count && TreeNode.Entries[i].Key.CompareTo(key) == 0)
            {
                return TreeNode.Entries[i];
            }

            return TreeNode.IsLeaf ? null : this.SearchInternal(TreeNode.Children[i], key);
        }

        /// <summary>
        /// Helper method that splits a full TreeNode into two TreeNodes.
        /// </summary>
        /// <param name="parentTreeNode">Parent TreeNode that contains TreeNode to be split.</param>
        /// <param name="TreeNodeToBeSplitIndex">Index of the TreeNode to be split within parent.</param>
        /// <param name="TreeNodeToBeSplit">TreeNode to be split.</param>
        private void SplitChild(TreeNode<TK, TP> parentTreeNode, int TreeNodeToBeSplitIndex, TreeNode<TK, TP> TreeNodeToBeSplit)
        {
            var newTreeNode = new TreeNode<TK, TP>(this.Degree);

            parentTreeNode.Entries.Insert(TreeNodeToBeSplitIndex, TreeNodeToBeSplit.Entries[this.Degree - 1]);
            parentTreeNode.Children.Insert(TreeNodeToBeSplitIndex + 1, newTreeNode);

            newTreeNode.Entries.AddRange(TreeNodeToBeSplit.Entries.GetRange(this.Degree, this.Degree - 1));
            
            // remove also Entries[this.Degree - 1], which is the one to move up to the parent
            TreeNodeToBeSplit.Entries.RemoveRange(this.Degree - 1, this.Degree);

            if (!TreeNodeToBeSplit.IsLeaf)
            {
                newTreeNode.Children.AddRange(TreeNodeToBeSplit.Children.GetRange(this.Degree, this.Degree));
                TreeNodeToBeSplit.Children.RemoveRange(this.Degree, this.Degree);
            }
        }

        private void InsertNonFull(TreeNode<TK, TP> TreeNode, TK newKey, TP newPointer)
        {
            int positionToInsert = TreeNode.Entries.TakeWhile(entry => newKey.CompareTo(entry.Key) >= 0).Count();

            // leaf TreeNode
            if (TreeNode.IsLeaf)
            {
                TreeNode.Entries.Insert(positionToInsert, new Entry<TK, TP>() { Key = newKey, Pointer = newPointer });
                return;
            }

            // non-leaf
            TreeNode<TK, TP> child = TreeNode.Children[positionToInsert];
            if (child.HasReachedMaxEntries)
            {
                this.SplitChild(TreeNode, positionToInsert, child);
                if (newKey.CompareTo(TreeNode.Entries[positionToInsert].Key) > 0)
                {
                    positionToInsert++;
                }
            }

            this.InsertNonFull(TreeNode.Children[positionToInsert], newKey, newPointer);
        }
    }
}
