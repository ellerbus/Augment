using System;
using System.Collections.Generic;
using FizzWare.NBuilder;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Augment.Tests.Helpers
{
    [TestClass]
    public class MultiKeyCollectionTests
    {
        #region Helpers/Test Initializers

        private class User
        {
            public int Id { get; set; }
            public string WindowsId { get; set; }
            public string Name { get; set; }
            public DateTime Date { get; set; }
        }

        private class UserCollection : MultiKeyCollection<User, int, string>
        {
            public UserCollection(IEnumerable<User> users)
            {
                foreach (var u in users)
                {
                    Add(u);
                }
            }

            protected override int GetPrimaryKey(User item)
            {
                return item.Id;
            }

            protected override string GetUniqueKey(User item)
            {
                return item.WindowsId;
            }
        }

        #endregion

        #region Methods

        [TestMethod]
        public void MultiKeyCollection_Should_IndexByPrimaryKey()
        {
            var users = Builder<User>.CreateListOfSize(3).Build();

            var u1 = users[0];
            var u2 = users[1];
            var u3 = users[2];

            var sut = new UserCollection(users);

            Assert.IsTrue(sut.ContainsPrimaryKey(u1.Id));
            Assert.IsTrue(sut.ContainsPrimaryKey(u2.Id));
            Assert.IsTrue(sut.ContainsPrimaryKey(u3.Id));
            Assert.IsFalse(sut.ContainsPrimaryKey(u3.Id + 99));

            Assert.AreEqual(u1, sut.GetByPrimaryKey(u1.Id));
            Assert.AreEqual(u2, sut.GetByPrimaryKey(u2.Id));
            Assert.AreEqual(u3, sut.GetByPrimaryKey(u3.Id));
        }

        [TestMethod]
        public void MultiKeyCollection_Should_IndexByUniqueKey()
        {
            var users = Builder<User>.CreateListOfSize(3).Build();

            var u1 = users[0];
            var u2 = users[1];
            var u3 = users[2];

            var sut = new UserCollection(users);

            Assert.IsTrue(sut.ContainsUniqueKey(u1.WindowsId));
            Assert.IsTrue(sut.ContainsUniqueKey(u2.WindowsId));
            Assert.IsTrue(sut.ContainsUniqueKey(u3.WindowsId));
            Assert.IsFalse(sut.ContainsUniqueKey(u3.WindowsId + "X"));

            Assert.AreEqual(u1, sut.GetByUniqueKey(u1.WindowsId));
            Assert.AreEqual(u2, sut.GetByUniqueKey(u2.WindowsId));
            Assert.AreEqual(u3, sut.GetByUniqueKey(u3.WindowsId));
        }

        [TestMethod]
        public void MultiKeyCollection_Should_RemoveAtIndex()
        {
            var users = Builder<User>.CreateListOfSize(3).Build();

            var u2 = users[1];

            var sut = new UserCollection(users);

            sut.RemoveAt(1);

            Assert.IsFalse(sut.ContainsPrimaryKey(u2.Id));

            Assert.IsFalse(sut.ContainsUniqueKey(u2.WindowsId));
        }

        [TestMethod]
        public void MultiKeyCollection_Should_RemoveItem()
        {
            var users = Builder<User>.CreateListOfSize(3).Build();

            var u2 = users[1];

            var sut = new UserCollection(users);

            sut.Remove(u2);

            Assert.IsFalse(sut.ContainsPrimaryKey(u2.Id));

            Assert.IsFalse(sut.ContainsUniqueKey(u2.WindowsId));
        }

        #endregion
    }
}
