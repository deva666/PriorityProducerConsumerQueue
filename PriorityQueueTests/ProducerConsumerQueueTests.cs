using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MarkoDevcic;
using System.Threading;

namespace PriorityQueueTests
{
    [TestClass]
    public class ProducerConsumerQueueTests
    {
        [TestMethod]
        public void TestException()
        {
            var queue = new ProducerConsumerQueue();
            var task = queue.Enqueue(() =>
            {
                Thread.Sleep(500);
                throw new Exception("You failed");

            });
            try
            {
                task.Wait();
            }
            catch (Exception e)
            {
                Assert.IsNotNull(e);
                Assert.IsTrue(e.InnerException.Message == "You failed");
            }
        }

        [TestMethod]
        public void TestTaskCompletion()
        {
            var queue = new ProducerConsumerQueue();
            var task = queue.Enqueue(() =>
            {
                Thread.Sleep(500);
            });
            task.Wait();     
       
            Assert.IsFalse(task.IsFaulted);
            Assert.IsTrue(task.IsCompleted);
            Assert.IsTrue(task.Status == System.Threading.Tasks.TaskStatus.RanToCompletion);
        }
    }
}
