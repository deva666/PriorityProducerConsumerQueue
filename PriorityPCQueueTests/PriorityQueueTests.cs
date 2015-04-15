using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MarkoDevcic;

namespace PriorityQueueTests
{
    [TestClass]
    public class PriorityQueueTests
    {
        private const int ARRAY_SIZE = 500000;

        [TestMethod]
        public void InsertTest()
        {
            var range = new int[ARRAY_SIZE];
            var random = new Random();
            var queue = new PriorityQueue<int>();
            for (int i = 0; i < ARRAY_SIZE; i++)
            {
                var number = random.Next(int.MaxValue);
                range[i] = number;
                queue.Insert(number);
            }

            TestSort(range, queue);
        }


        [TestMethod]
        public void ConstructorIEnumerableTest()
        {
            var range = new int[ARRAY_SIZE];
            var random = new Random();
            for (int i = 0; i < ARRAY_SIZE; i++)
            {
                var number = random.Next(int.MaxValue);
                range[i] = number;
            }

            var queue = new PriorityQueue<int>(range);

            TestSort(range, queue);
        }

        [TestMethod]
        public void InsertRangeTest()
        {
            var range = new int[ARRAY_SIZE];
            var random = new Random();
            for (int i = 0; i < ARRAY_SIZE; i++)
            {
                var number = random.Next(int.MaxValue);
                range[i] = number;
            }

            var queue = new PriorityQueue<int>();
            queue.InsertRange(range);

            TestSort(range, queue);
        }

        private static void TestSort(int[] range, PriorityQueue<int> queue)
        {
            Array.Sort(range);
            for (int i = range.Length - 1; i >= 0; i--)
            {
                var max = queue.ExtractTopItem();
                Assert.AreEqual(range[i], max);
            }
            Assert.AreEqual(queue.Size, 0);
        }
    }
}
