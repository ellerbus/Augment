using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Augment.Tests.Helpers
{
    [TestClass]
    public class ReflectionHelperTests
    {
        public class Parent { public int ID { get; set; } }
        public class Child { public Parent Parent { get; set; } public int ID { get; set; } }

        [TestMethod]
        public void ReflectionHelper_HasProperty()
        {
            var p = new Parent { ID = 99 };

            Assert.IsTrue(ReflectionHelper.HasProperty(p, "ID"));
            Assert.IsFalse(ReflectionHelper.HasProperty(p, "IDx"));
        }

        [TestMethod]
        public void ReflectionHelper_GetProperty_Should_Succeed()
        {
            var p = new Parent { ID = 99 };

            Assert.AreEqual(99, ReflectionHelper.GetValueOfProperty(p, "ID"));
        }

        [TestMethod, ExpectedException(typeof(ReflectionHelperException))]
        public void ReflectionHelper_GetProperty_Should_Fail()
        {
            var p = new Parent { ID = 99 };

            Assert.AreEqual(99, ReflectionHelper.GetValueOfProperty(p, "IDx"));
        }

        [TestMethod]
        public void ReflectionHelper_GetPropertyPath_Should_Succeed()
        {
            var c = new Child { Parent = new Parent { ID = 99 }, ID = 999 };

            Assert.AreEqual(c.Parent.ID, ReflectionHelper.GetValueOfPropertyPath(c, "Parent.ID"));
        }

        [TestMethod, ExpectedException(typeof(ReflectionHelperException))]
        public void ReflectionHelper_GetPropertyPath_Should_Fail()
        {
            var c = new Child { Parent = new Parent { ID = 99 }, ID = 999 };

            Assert.AreEqual(c.Parent.ID, ReflectionHelper.GetValueOfPropertyPath(c, "Parent.IDx"));
        }

        [TestMethod]
        public void ReflectionHelper_SetProperty_Should_Succeed()
        {
            var p = new Parent { ID = 99 };

            ReflectionHelper.SetValueOfProperty(p, "ID", 100);

            Assert.AreEqual(100, p.ID);
        }

        [TestMethod, ExpectedException(typeof(ReflectionHelperException))]
        public void ReflectionHelper_SetProperty_Should_Fail()
        {
            var p = new Parent { ID = 99 };

            ReflectionHelper.SetValueOfProperty(p, "IDx", 100);

            Assert.AreEqual(100, p.ID);
        }

        [TestMethod]
        public void ReflectionHelper_SetPropertyPath_Should_Succeed()
        {
            var c = new Child { Parent = new Parent { ID = 99 }, ID = 999 };

            ReflectionHelper.SetValueOfPropertyPath(c, "Parent.ID", 100);

            Assert.AreEqual(100, c.Parent.ID);
        }

        [TestMethod, ExpectedException(typeof(ReflectionHelperException))]
        public void ReflectionHelper_SetPropertyPath_Should_Fail()
        {
            var c = new Child { Parent = new Parent { ID = 99 }, ID = 999 };

            ReflectionHelper.SetValueOfPropertyPath(c, "Parent.IDx", 100);
        }
    }
}
