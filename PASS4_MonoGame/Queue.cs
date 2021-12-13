//Author Name: Elad Perlman
//Project Name: PASS4_MonoGame
//File Name: MainGame.cs
//Date Created: Dec, 15th, 2020
//Date Modified: Jan, 27th, 2021
//Description: Implementation of a generic queue
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PASS4_MonoGame
{
    class Queue<T>
    {
        //Maintain the collection of Items
        private List<T> queue = new List<T>();

        public Queue()
        {
        }

        //Pre: Item is not null
        //Post: None
        //Description: Add newItem to the back of the queue
        public void Enqueue(T newItem)
        {
            queue.Add(newItem);
        }

        //Pre: None
        //Post: Returns the front element of the queue
        //Description: returns and removes the element at the front of the queue, null if
        //             none exists
        public T Dequeue()
        {
            //Maintain the front of the queue for return
            T result = default(T);

            //Only remove an Item if possible
            if (queue.Count > 0)
            {
                result = queue[0];
                queue.RemoveAt(0);
            }

            return result;
        }

        //Pre: None
        //Post: Returns the front element of the queue
        //Description: returns the element at the front of the queue, null if non exists
        public T Peek()
        {
            //Maintain the front of the queue for return
            T result = default;

            //Only retrieve the Item if possible
            if (queue.Count > 0)
            {
                result = queue[0];
            }

            return result;
        }
        
        //Pre: N/A
        //Post: N/A
        //Description: Clears queue
        public void Clear()
        {
            queue.Clear();
        }

        //Pre: None
        //Post: Returns the current size of the queue
        //Description: Returns the current number of elements in the queue
        public int Size()
        {
            return queue.Count;
        }

        //Pre: None
        //Post: Returns true if the size of the queue is 0, false otherwise
        //Description: Compare the size of the queue against 0 to determine its empty status
        public bool IsEmpty()
        {
            return queue.Count == 0;
        }
    }
}

