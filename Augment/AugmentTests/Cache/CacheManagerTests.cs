using System;
using Augment.Cache;
using AutoMoq;
using FizzWare.NBuilder;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Augment.Tests.Cache
{
    [TestClass]
    public class CacheManagerTests
    {
        #region Helpers/Test Initializers

        private class User
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public DateTime Date { get; set; }
        }

        private AutoMoqer Mocker { get; set; }
        private Mock<ICacheProvider> MockProvider { get; set; }
        private CacheManager SubjectUnderTest { get; set; }

        private string AnyKey { get { return It.IsAny<string>(); } }

        [TestInitialize]
        public void TestInitialize()
        {
            Mocker = new AutoMoqer();

            SubjectUnderTest = Mocker.Create<CacheManager>();

            MockProvider = Mocker.GetMock<ICacheProvider>();
        }

        #endregion

        [TestMethod]
        public void CacheManger_Should_Call_Loader()
        {
            //  arrange
            var user = Builder<User>.CreateNew().Build();

            MockProvider.Setup(x => x.Get(AnyKey)).Returns(null as User);

            var called = false;

            //  act
            var actual = SubjectUnderTest.Cache<User>(() => { called = true; return user; }).CachedObject;

            //  assert
            Assert.IsTrue(called);

            Assert.IsNotNull(actual);

            Assert.AreSame(user, actual);

            MockProvider.VerifyAll();
        }

        [TestMethod]
        public void CacheManger_Should_Not_Call_Loader()
        {
            //  arrange
            var user = Builder<User>.CreateNew().Build();

            MockProvider.Setup(x => x.Get(AnyKey)).Returns(user);

            var called = false;

            //  act
            var actual = SubjectUnderTest.Cache<User>(() => { called = true; return user; }).CachedObject;

            //  assert
            Assert.IsFalse(called);

            Assert.IsNotNull(actual);

            Assert.AreSame(user, actual);

            MockProvider.VerifyAll();
        }

        [TestMethod]
        public void CacheManger_Should_Find_Object()
        {
            //  arrange
            var user = Builder<User>.CreateNew().Build();

            MockProvider.Setup(x => x.Get(AnyKey)).Returns(user);

            //  act
            var actual = SubjectUnderTest.Find<User>().CachedObject;

            //  assert
            Assert.IsNotNull(actual);

            Assert.AreSame(user, actual);

            MockProvider.VerifyAll();
        }

        [TestMethod]
        public void CacheManger_Should_Not_Find_Object()
        {
            //  arrange
            MockProvider.Setup(x => x.Get(AnyKey)).Returns(null as User);

            //  act
            var actual = SubjectUnderTest.Find<User>().CachedObject;

            //  assert
            Assert.IsNull(actual);

            MockProvider.VerifyAll();
        }

        [TestMethod]
        public void CacheManger_Should_Remove_Found_Object()
        {
            //  arrange
            var user = Builder<User>.CreateNew().Build();

            MockProvider.Setup(x => x.Remove(AnyKey)).Returns(user);

            //  act
            var actual = SubjectUnderTest.Find<User>().Remove;

            //  assert
            Assert.IsNotNull(actual);

            Assert.AreSame(user, actual);

            MockProvider.VerifyAll();
        }

        [TestMethod]
        public void CacheManger_Should_Remove_Not_Found_Object()
        {
            //  arrange
            MockProvider.Setup(x => x.Remove(AnyKey)).Returns(null as User);

            //  act
            var actual = SubjectUnderTest.Find<User>().Remove;

            //  assert
            Assert.IsNull(actual);

            MockProvider.VerifyAll();
        }
    }
}
