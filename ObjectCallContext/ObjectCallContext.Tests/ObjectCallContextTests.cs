using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ObjectCallContext.Tests
{
    [TestClass]
    public class ObjectCallContextTests
    {
        [TestMethod]
        public void WorksWithThreadPoolThreads()
        {
            //Arrange
            string key = "test_key";
            object obj = new object();
            bool done = false;

            //Act
            object actual = null;

            ObjectCallContext.TrySetData(key, obj);
            ThreadPool.QueueUserWorkItem(state =>
            {
                ObjectCallContext.TryGetData(key, out actual);
                done = true;
            });

            //Assert
            while (!done) { }
            Assert.AreEqual(obj, actual);
        }


        [TestMethod]
        public void RemovesFromObjectContext()
        {
            //Arrange
            string key = "test_key";
            object obj = new object();

            //Act
            object actual;

            ObjectCallContext.TrySetData(key, obj);
            ObjectCallContext.TryRemove(key, out actual);

            object another;
            ObjectCallContext.TryGetData(key, out another);

            //Assert
            Assert.AreEqual(obj, actual);
            Assert.IsNull(another);
        }

        [TestMethod]
        public async Task WotksWithTasks()
        {
            //Arrange
            string key = "test_key";
            object obj = new object();

            //Act
            object actual = null;

            ObjectCallContext.TrySetData(key, obj);
            await Task.Run(() =>
            {
                ObjectCallContext.TryGetData(key, out actual);
            });
            
            //Assert
            Assert.AreEqual(obj, actual);
        }

        [TestMethod]
        public void ClearsAll()
        {
            //Arrange
            string key = "test_key";
            object obj = new object();

            //Act
            object actual;

            ObjectCallContext.TrySetData(key, obj);
            ObjectCallContext.ClearAll();
            ObjectCallContext.TryGetData(key, out actual);

            //Assert
            Assert.IsNull(actual);
        }
    }
}
