﻿using System;
using System.Collections.Generic;
using Augment.Caching;
using FizzWare.NBuilder;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Augment.Tests.Caching
{
    [TestClass]
    public class CacheManagerTests
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
            protected override int GetPrimaryKey(User item)
            {
                return item.Id;
            }

            protected override string GetUniqueKey(User item)
            {
                return item.WindowsId;
            }
        }

        private Moxy Moxy { get; set; }
        private Mock<ICacheProvider> MockProvider { get; set; }
        private CacheManager SubjectUnderTest { get; set; }

        private string AnyKey { get { return It.IsAny<string>(); } }
        private object AnyObject { get { return It.IsAny<object>(); } }
        private TimeSpan AnyTimeSpan { get { return It.IsAny<TimeSpan>(); } }
        private CacheExpiration AnyCacheExpiration { get { return It.IsAny<CacheExpiration>(); } }
        private CachePriority AnyCachePriority { get { return It.IsAny<CachePriority>(); } }

        private string UserKey { get { return typeof(User).FullName + ";"; } }
        private string UserEnumerableKey { get { return UserKey + "Enumerable;"; } }

        [TestInitialize]
        public void TestInitialize()
        {
            Moxy = new Moxy();

            SubjectUnderTest = Moxy.CreateInstance<CacheManager>();

            MockProvider = Moxy.GetMock<ICacheProvider>();
        }

        #endregion

        #region Basics

        [TestMethod]
        public void CacheManger_Should_Use_Absolute_Expiration_Normal_Priority()
        {
            //  arrange
            var user = Builder<User>.CreateNew().Build();

            MockProvider
                .Setup(x => x.Get(AnyKey))
                .Returns(null as User);

            MockProvider
                .Setup(x => x.Add(AnyKey, AnyObject, AnyTimeSpan, CacheExpiration.Absolute, CachePriority.Normal));

            //  act
            var actual = SubjectUnderTest.Cache<User>(() => user)
                .DurationOf(20.Minutes())
                .CachedObject;

            //  assert
            Assert.IsNotNull(actual);

            Assert.AreSame(user, actual);

            Moxy.VerifyAll();
        }

        [TestMethod]
        public void CacheManger_Should_Use_Absolute_Expiration_High_Priority()
        {
            //  arrange
            var user = Builder<User>.CreateNew().Build();

            MockProvider
                .Setup(x => x.Get(AnyKey))
                .Returns(null as User);

            MockProvider
                .Setup(x => x.Add(AnyKey, AnyObject, AnyTimeSpan, CacheExpiration.Absolute, CachePriority.High));

            //  act
            var actual = SubjectUnderTest
                .Cache<User>(() => user)
                .DurationOf(20.Minutes(), cachePriority: CachePriority.High)
                .CachedObject;

            //  assert
            Assert.IsNotNull(actual);

            Assert.AreSame(user, actual);

            Moxy.VerifyAll();
        }

        [TestMethod]
        public void CacheManger_Should_Use_Sliding_Expiration_Normal_Priority()
        {
            //  arrange
            var user = Builder<User>.CreateNew().Build();

            MockProvider
                .Setup(x => x.Get(AnyKey))
                .Returns(null as User);

            MockProvider
                .Setup(x => x.Add(AnyKey, AnyObject, AnyTimeSpan, CacheExpiration.Sliding, CachePriority.Normal));

            //  act
            var actual = SubjectUnderTest
                .Cache<User>(() => user)
                .DurationOf(20.Minutes(), CacheExpiration.Sliding)
                .CachedObject;

            //  assert
            Assert.IsNotNull(actual);

            Assert.AreSame(user, actual);

            Moxy.VerifyAll();
        }

        [TestMethod]
        public void CacheManger_Should_Use_Sliding_Expiration_High_Priority()
        {
            //  arrange
            var user = Builder<User>.CreateNew().Build();

            MockProvider
                .Setup(x => x.Get(AnyKey))
                .Returns(null as User);

            MockProvider
                .Setup(x => x.Add(AnyKey, AnyObject, AnyTimeSpan, CacheExpiration.Sliding, CachePriority.High));

            //  act
            var actual = SubjectUnderTest
                .Cache<User>(() => user)
                .DurationOf(20.Minutes(), CacheExpiration.Sliding, CachePriority.High)
                .CachedObject;

            //  assert
            Assert.IsNotNull(actual);

            Assert.AreSame(user, actual);

            Moxy.VerifyAll();
        }

        [TestMethod]
        public void CacheManger_Should_Call_Loader()
        {
            //  arrange
            var user = Builder<User>.CreateNew().Build();

            MockProvider
                .Setup(x => x.Get(AnyKey))
                .Returns(null as User);

            MockProvider
                .Setup(x => x.Add(AnyKey, user, 20.Minutes(), CacheExpiration.Absolute, CachePriority.Default));

            var called = false;

            //  act
            var actual = SubjectUnderTest.Cache<User>(() => { called = true; return user; }).CachedObject;

            //  assert
            Assert.IsTrue(called);

            Assert.IsNotNull(actual);

            Assert.AreSame(user, actual);

            Moxy.VerifyAll();
        }

        [TestMethod]
        public void CacheManger_Should_Not_Call_Loader()
        {
            //  arrange
            var user = Builder<User>.CreateNew().Build();

            MockProvider
                .Setup(x => x.Get(AnyKey))
                .Returns(user);

            var called = false;

            //  act
            var actual = SubjectUnderTest.Cache<User>(() => { called = true; return user; }).CachedObject;

            //  assert
            Assert.IsFalse(called);

            Assert.IsNotNull(actual);

            Assert.AreSame(user, actual);

            Moxy.VerifyAll();
        }

        [TestMethod]
        public void CacheManger_Should_Find_Object()
        {
            //  arrange
            var user = Builder<User>.CreateNew().Build();

            MockProvider
                .Setup(x => x.Get(AnyKey))
                .Returns(user);

            //  act
            var actual = SubjectUnderTest.Find<User>().CachedObject;

            //  assert
            Assert.IsNotNull(actual);

            Assert.AreSame(user, actual);

            Moxy.VerifyAll();
        }

        [TestMethod]
        public void CacheManger_Should_Find_Object_By_Key()
        {
            //  arrange
            var user = Builder<User>.CreateNew().Build();

            MockProvider
                .Setup(x => x.Get(UserKey + "123;"))
                .Returns(user);

            //  act
            var actual = SubjectUnderTest.Find<User>().By(123).CachedObject;

            //  assert
            Assert.IsNotNull(actual);

            Assert.AreSame(user, actual);

            Moxy.VerifyAll();
        }

        [TestMethod]
        public void CacheManger_Should_Find_Remove_Object_By_Key()
        {
            //  arrange
            var user = Builder<User>.CreateNew().Build();

            MockProvider
                .Setup(x => x.Remove(UserKey + "123;"))
                .Returns(user);

            //  act
            var actual = SubjectUnderTest.Find<User>().By(123).Remove();

            //  assert
            Assert.IsNotNull(actual);

            Assert.AreSame(user, actual);

            Moxy.VerifyAll();
        }

        [TestMethod]
        public void CacheManger_Should_Not_Find_Object()
        {
            //  arrange
            MockProvider
                .Setup(x => x.Get(AnyKey))
                .Returns(null as User);

            //  act
            var actual = SubjectUnderTest.Find<User>().CachedObject;

            //  assert
            Assert.IsNull(actual);

            Moxy.VerifyAll();
        }

        [TestMethod]
        public void CacheManger_Should_Remove_Found_Object()
        {
            //  arrange
            var user = Builder<User>.CreateNew().Build();

            MockProvider
                .Setup(x => x.Remove(AnyKey))
                .Returns(user);

            //  act
            var actual = SubjectUnderTest.Find<User>().Remove();

            //  assert
            Assert.IsNotNull(actual);

            Assert.AreSame(user, actual);

            Moxy.VerifyAll();
        }

        [TestMethod]
        public void CacheManger_Should_Remove_Not_Found_Object()
        {
            //  arrange
            MockProvider
                .Setup(x => x.Remove(AnyKey))
                .Returns(null as User);

            //  act
            var actual = SubjectUnderTest.Find<User>().Remove();

            //  assert
            Assert.IsNull(actual);

            Moxy.VerifyAll();
        }

        #endregion

        #region More Advanced Convention Testing

        [TestMethod]
        public void CacheManger_Should_Cache_Item_With_BaseKey()
        {
            //  arrange
            var user = Builder<User>.CreateNew().Build();

            MockProvider
                .Setup(x => x.Get(UserKey))
                .Returns(null as User);

            MockProvider
                .Setup(x => x.Add(UserKey, user, AnyTimeSpan, AnyCacheExpiration, AnyCachePriority));

            //  act
            var actual = SubjectUnderTest.Cache(() => user).CachedObject;

            //  assert

            Moxy.VerifyAll();
        }

        [TestMethod]
        public void CacheManger_Should_Cache_Item_With_BaseKey_Using_ValueTypes()
        {
            //  arrange
            var list = Builder<int>.CreateListOfSize(10).Build();

            var key = "System.Int32;Enumerable;";

            MockProvider
                .Setup(x => x.Get(key))
                .Returns(null as IList<int>);

            MockProvider
                .Setup(x => x.Add(key, list, AnyTimeSpan, AnyCacheExpiration, AnyCachePriority));

            //  act
            var actual = SubjectUnderTest.Cache(() => list).CachedObject;

            //  assert

            Moxy.VerifyAll();
        }

        [TestMethod]
        public void CacheManger_Should_Cache_Item_With_BaseKey_Plus_Single_Key()
        {
            //  arrange
            var user = Builder<User>.CreateNew().Build();

            var key = UserKey + "123;";

            MockProvider
                .Setup(x => x.Get(key))
                .Returns(null as User);

            MockProvider
                .Setup(x => x.Add(key, user, AnyTimeSpan, AnyCacheExpiration, AnyCachePriority));

            //  act
            var actual = SubjectUnderTest.Cache(() => user).By(123).CachedObject;

            //  assert

            Moxy.VerifyAll();
        }

        [TestMethod]
        public void CacheManger_Should_Cache_List_With_BaseKey()
        {
            //  arrange
            var users = Builder<User>.CreateListOfSize(10).Build();

            MockProvider
                .Setup(x => x.Get(UserEnumerableKey))
                .Returns(null as IList<User>);

            MockProvider
                .Setup(x => x.Add(UserEnumerableKey, users, AnyTimeSpan, AnyCacheExpiration, AnyCachePriority));

            //  act
            var actual = SubjectUnderTest.Cache(() => users).CachedObject;

            //  assert

            Moxy.VerifyAll();
        }

        [TestMethod]
        public void CacheManger_Should_Cache_List_With_BaseKey_Plus_Multiple_Keys()
        {
            //  arrange
            var users = Builder<User>.CreateListOfSize(10).Build();

            var key = UserEnumerableKey + "123,456;";

            MockProvider
                .Setup(x => x.Get(key))
                .Returns(null as IList<User>);

            MockProvider
                .Setup(x => x.Add(key, users, AnyTimeSpan, AnyCacheExpiration, AnyCachePriority));

            //  act
            var actual = SubjectUnderTest.Cache(() => users).By(123, 456).CachedObject;

            //  assert

            System.Diagnostics.Debug.WriteLine(actual.Count);

            Moxy.VerifyAll();
        }

        [TestMethod]
        public void CacheManger_Should_RemoveAll_User()
        {
            //  arrange

            MockProvider
                .Setup(x => x.GetAllKeys())
                .Returns(new[] {
                UserKey, UserEnumerableKey
            });

            MockProvider
                .Setup(x => x.Remove(UserKey))
                .Returns(null as User);
            MockProvider
                .Setup(x => x.Remove(UserEnumerableKey))
                .Returns(null as User);

            //  act
            SubjectUnderTest.Find<User>().RemoveAll();

            //  assert

            Moxy.VerifyAll();
        }

        [TestMethod]
        public void CacheManger_Should_RemoveAll_User_List()
        {
            //  arrange

            MockProvider
                .Setup(x => x.GetAllKeys())
                .Returns(new[] {
                UserKey, UserEnumerableKey
            });

            MockProvider
                .Setup(x => x.Remove(UserEnumerableKey))
                .Returns(null as User);

            //  act
            SubjectUnderTest.Find<IList<User>>().RemoveAll();

            //  assert

            Moxy.VerifyAll();
        }

        [TestMethod]
        public void CacheManger_Should_RemoveAll_User_Collection()
        {
            //  arrange

            MockProvider
                .Setup(x => x.GetAllKeys())
                .Returns(new[] {
                UserKey, UserEnumerableKey
            });

            MockProvider
                .Setup(x => x.Remove(UserEnumerableKey))
                .Returns(null as User);

            //  act
            SubjectUnderTest.Find<IList<User>>().RemoveAll();

            //  assert

            Moxy.VerifyAll();
        }

        [TestMethod]
        public void CacheManger_Should_RemoveAll_User_By()
        {
            //  arrange
            var keys = new[] { UserKey, UserEnumerableKey, UserKey + "123;", UserKey + "456;" };

            MockProvider
                .Setup(x => x.GetAllKeys())
                .Returns(keys);

            MockProvider
                .Setup(x => x.Remove(UserKey + "123;"))
                .Returns(null as User);

            //  act
            SubjectUnderTest.Find<User>().By(123).RemoveAll();

            //  assert

            Moxy.VerifyAll();
        }

        [TestMethod]
        public void CacheManger_Should_RemoveAll_User_ByWildcard()
        {
            //  arrange
            var keys = new[] { UserKey, UserEnumerableKey, UserKey + "123,456;", UserKey + "123,789;" };

            MockProvider
                .Setup(x => x.GetAllKeys())
                .Returns(keys);

            MockProvider
                .Setup(x => x.Remove(UserKey + "123,456;"))
                .Returns(null as User);
            MockProvider
                .Setup(x => x.Remove(UserKey + "123,789;"))
                .Returns(null as User);

            //  act
            SubjectUnderTest.Find<User>().By(123, "*").RemoveAll();

            //  assert

            Moxy.VerifyAll();
        }

        #endregion
    }
}
