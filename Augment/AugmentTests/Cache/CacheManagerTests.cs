using System;
using System.Collections.Generic;
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
        private object AnyObject { get { return It.IsAny<object>(); } }
        private TimeSpan AnyTimeSpan { get { return It.IsAny<TimeSpan>(); } }
        private CacheExpiration AnyCacheExpiration { get { return It.IsAny<CacheExpiration>(); } }
        private CachePriority AnyCachePriority { get { return It.IsAny<CachePriority>(); } }

        private string UserKey { get { return "Augment.Tests.Cache.CacheManagerTests+User;"; } }
        private string UserListKey { get { return "Augment.Tests.Cache.CacheManagerTests+User;Enumerable;"; } }

        [TestInitialize]
        public void TestInitialize()
        {
            Mocker = new AutoMoqer();

            SubjectUnderTest = Mocker.Create<CacheManager>();

            MockProvider = Mocker.GetMock<ICacheProvider>();
        }

        #endregion

        #region Basics

        [TestMethod]
        public void CacheManger_Should_Use_Absolute_Expiration_Normal_Priority()
        {
            //  arrange
            var user = Builder<User>.CreateNew().Build();

            MockProvider.Setup(x => x.Get(AnyKey)).Returns(null as User);

            MockProvider.Setup(x => x.Add(AnyKey, AnyObject, AnyTimeSpan, CacheExpiration.Absolute, CachePriority.Normal));

            //  act
            var actual = SubjectUnderTest.Cache<User>(() => user)
                .Expires(20.Minutes(), CacheExpiration.Absolute)
                .CachedObject;

            //  assert
            Assert.IsNotNull(actual);

            Assert.AreSame(user, actual);

            MockProvider.VerifyAll();
        }

        [TestMethod]
        public void CacheManger_Should_Use_Absolute_Expiration_High_Priority()
        {
            //  arrange
            var user = Builder<User>.CreateNew().Build();

            MockProvider.Setup(x => x.Get(AnyKey)).Returns(null as User);

            MockProvider.Setup(x => x.Add(AnyKey, AnyObject, AnyTimeSpan, CacheExpiration.Absolute, CachePriority.High));

            //  act
            var actual = SubjectUnderTest
                .Cache<User>(() => user)
                .Expires(20.Minutes(), CacheExpiration.Absolute)
                .Priority(CachePriority.High)
                .CachedObject;

            //  assert
            Assert.IsNotNull(actual);

            Assert.AreSame(user, actual);

            MockProvider.VerifyAll();
        }

        [TestMethod]
        public void CacheManger_Should_Use_Sliding_Expiration_Normal_Priority()
        {
            //  arrange
            var user = Builder<User>.CreateNew().Build();

            MockProvider.Setup(x => x.Get(AnyKey)).Returns(null as User);

            MockProvider.Setup(x => x.Add(AnyKey, AnyObject, AnyTimeSpan, CacheExpiration.Sliding, CachePriority.Normal));

            //  act
            var actual = SubjectUnderTest
                .Cache<User>(() => user)
                .Expires(20.Minutes(), CacheExpiration.Sliding)
                .CachedObject;

            //  assert
            Assert.IsNotNull(actual);

            Assert.AreSame(user, actual);

            MockProvider.VerifyAll();
        }

        [TestMethod]
        public void CacheManger_Should_Use_Sliding_Expiration_High_Priority()
        {
            //  arrange
            var user = Builder<User>.CreateNew().Build();

            MockProvider.Setup(x => x.Get(AnyKey)).Returns(null as User);

            MockProvider.Setup(x => x.Add(AnyKey, AnyObject, AnyTimeSpan, CacheExpiration.Sliding, CachePriority.High));

            //  act
            var actual = SubjectUnderTest
                .Cache<User>(() => user)
                .Expires(20.Minutes(), CacheExpiration.Sliding)
                .Priority(CachePriority.High)
                .CachedObject;

            //  assert
            Assert.IsNotNull(actual);

            Assert.AreSame(user, actual);

            MockProvider.VerifyAll();
        }

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
        public void CacheManger_Should_Find_Object_By_Key()
        {
            //  arrange
            var user = Builder<User>.CreateNew().Build();

            MockProvider.Setup(x => x.Get(UserKey + "123;")).Returns(user);

            //  act
            var actual = SubjectUnderTest.Find<User>().By(123).CachedObject;

            //  assert
            Assert.IsNotNull(actual);

            Assert.AreSame(user, actual);

            MockProvider.VerifyAll();
        }

        [TestMethod]
        public void CacheManger_Should_Find_Remove_Object_By_Key()
        {
            //  arrange
            var user = Builder<User>.CreateNew().Build();

            MockProvider.Setup(x => x.Remove(UserKey + "123;")).Returns(user);

            //  act
            var actual = SubjectUnderTest.Find<User>().By(123).Remove();

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
            var actual = SubjectUnderTest.Find<User>().Remove();

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
            var actual = SubjectUnderTest.Find<User>().Remove();

            //  assert
            Assert.IsNull(actual);

            MockProvider.VerifyAll();
        }

        #endregion

        #region More Advanced Convention Testing

        [TestMethod]
        public void CacheManger_Should_Cache_Item_With_BaseKey()
        {
            //  arrange
            var user = Builder<User>.CreateNew().Build();

            MockProvider.Setup(x => x.Add(UserKey, user, AnyTimeSpan, AnyCacheExpiration, AnyCachePriority));

            //  act
            var actual = SubjectUnderTest.Cache(() => user).CachedObject;

            //  assert

            MockProvider.VerifyAll();
        }

        [TestMethod]
        public void CacheManger_Should_Cache_Item_With_BaseKey_Plus_Single_Key()
        {
            //  arrange
            var user = Builder<User>.CreateNew().Build();

            MockProvider.Setup(x => x.Add(UserKey + "123;", user, AnyTimeSpan, AnyCacheExpiration, AnyCachePriority));

            //  act
            var actual = SubjectUnderTest.Cache(() => user).By(123).CachedObject;

            //  assert

            MockProvider.VerifyAll();
        }

        [TestMethod]
        public void CacheManger_Should_Cache_List_With_BaseKey()
        {
            //  arrange
            var users = Builder<User>.CreateListOfSize(10).Build();

            MockProvider.Setup(x => x.Add(UserListKey, users, AnyTimeSpan, AnyCacheExpiration, AnyCachePriority));

            //  act
            var actual = SubjectUnderTest.Cache(() => users).CachedObject;

            //  assert

            MockProvider.VerifyAll();
        }

        [TestMethod]
        public void CacheManger_Should_Cache_List_With_BaseKey_Plus_Multiple_Keys()
        {
            //  arrange
            var users = Builder<User>.CreateListOfSize(10).Build();

            MockProvider.Setup(x => x.Add(UserListKey + "123,456;", users, AnyTimeSpan, AnyCacheExpiration, AnyCachePriority));

            //  act
            var actual = SubjectUnderTest.Cache(() => users).By(123, 456).CachedObject;

            //  assert

            MockProvider.VerifyAll();
        }

        [TestMethod]
        public void CacheManger_Should_RemoveAll_User()
        {
            //  arrange

            MockProvider.Setup(x => x.GetAllKeys()).Returns(new[] {
                UserKey,
                UserListKey
            });

            MockProvider.Setup(x => x.Remove(UserKey)).Returns(null as User);
            MockProvider.Setup(x => x.Remove(UserListKey)).Returns(null as User);

            //  act
            SubjectUnderTest.RemoveAll<User>();

            //  assert

            MockProvider.VerifyAll();
        }

        [TestMethod]
        public void CacheManger_Should_RemoveAll_User_List()
        {
            //  arrange

            MockProvider.Setup(x => x.GetAllKeys()).Returns(new[] {
                UserKey,
                UserListKey
            });

            MockProvider.Setup(x => x.Remove(UserListKey)).Returns(null as User);

            //  act
            SubjectUnderTest.RemoveAll<IList<User>>();

            //  assert

            MockProvider.VerifyAll();
        }

        #endregion
    }
}
