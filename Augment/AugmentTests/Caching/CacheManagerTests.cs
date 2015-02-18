using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Augment.Caching;
using AutoMoq;
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

        private AutoMoqer Mocker { get; set; }
        private Mock<ICacheProvider> MockProvider { get; set; }
        private CacheManager SubjectUnderTest { get; set; }

        private string AnyKey { get { return It.IsAny<string>(); } }
        private object AnyObject { get { return It.IsAny<object>(); } }
        private TimeSpan AnyTimeSpan { get { return It.IsAny<TimeSpan>(); } }
        private CacheExpiration AnyCacheExpiration { get { return It.IsAny<CacheExpiration>(); } }
        private CachePriority AnyCachePriority { get { return It.IsAny<CachePriority>(); } }

        [TestInitialize]
        public void TestInitialize()
        {
            Mocker = new AutoMoqer();

            SubjectUnderTest = Mocker.Create<CacheManager>();

            MockProvider = Mocker.GetMock<ICacheProvider>();
        }

        private string CreateKey(bool isEnumerable, params object[] keys)
        {
            return CreateKey(typeof(User), isEnumerable, keys);
        }

        private string CreateKey(Type type, bool isEnumerable, params object[] keys)
        {
            StringBuilder key = new StringBuilder(type.FullName + ";")
                .Append(keys.Select(x => x.ToString()).Join(","))
                .Append(";")
                .AppendIf(isEnumerable, typeof(IEnumerable<>).Name);

            return key.ToString();
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
                .DurationOf(20.Minutes())
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
                .DurationOf(20.Minutes(), cachePriority: CachePriority.High)
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
                .DurationOf(20.Minutes(), CacheExpiration.Sliding)
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
                .DurationOf(20.Minutes(), CacheExpiration.Sliding, CachePriority.High)
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

            MockProvider.Setup(x => x.Get(CreateKey(false, 123))).Returns(user);

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

            MockProvider.Setup(x => x.Remove(CreateKey(false, 123))).Returns(user);

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

            MockProvider.Setup(x => x.Get(CreateKey(false))).Returns(null as User);

            MockProvider.Setup(x => x.Add(CreateKey(false), user, AnyTimeSpan, AnyCacheExpiration, AnyCachePriority));

            //  act
            var actual = SubjectUnderTest.Cache(() => user).CachedObject;

            //  assert

            MockProvider.VerifyAll();
        }

        [TestMethod]
        public void CacheManger_Should_Cache_Item_With_BaseKey_Using_ValueTypes()
        {
            //  arrange
            var idList = Builder<int>.CreateListOfSize(10).Build();

            string key = CreateKey(typeof(IList<int>), true);

            MockProvider.Setup(x => x.Get(key)).Returns(null as IList<int>);

            MockProvider.Setup(x => x.Add(key, idList, AnyTimeSpan, AnyCacheExpiration, AnyCachePriority));

            //  act
            var actual = SubjectUnderTest.Cache(() => idList).CachedObject;

            //  assert

            MockProvider.VerifyAll();
        }

        [TestMethod]
        public void CacheManger_Should_Cache_Item_With_BaseKey_Plus_Single_Key()
        {
            //  arrange
            var user = Builder<User>.CreateNew().Build();

            var key = CreateKey(false, 123);

            MockProvider.Setup(x => x.Get(key)).Returns(null as User);

            MockProvider.Setup(x => x.Add(key, user, AnyTimeSpan, AnyCacheExpiration, AnyCachePriority));

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

            MockProvider.Setup(x => x.Get(CreateKey(true))).Returns(null as IList<User>);

            MockProvider.Setup(x => x.Add(CreateKey(true), users, AnyTimeSpan, AnyCacheExpiration, AnyCachePriority));

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

            var key = CreateKey(true, 123, 456);

            MockProvider.Setup(x => x.Get(key)).Returns(null as IList<User>);

            MockProvider.Setup(x => x.Add(key, users, AnyTimeSpan, AnyCacheExpiration, AnyCachePriority));

            //  act
            var actual = SubjectUnderTest.Cache(() => users).By(123, 456).CachedObject;

            //  assert

            System.Diagnostics.Debug.WriteLine(actual.Count);

            MockProvider.VerifyAll();
        }

        [TestMethod]
        public void CacheManger_Should_RemoveAll_User()
        {
            //  arrange

            MockProvider.Setup(x => x.GetAllKeys()).Returns(new[] {
                CreateKey(false), CreateKey(true)
            });

            MockProvider.Setup(x => x.Remove(CreateKey(false))).Returns(null as User);
            MockProvider.Setup(x => x.Remove(CreateKey(true))).Returns(null as User);

            //  act
            SubjectUnderTest.Find<User>().RemoveAll();

            //  assert

            MockProvider.VerifyAll();
        }

        [TestMethod]
        public void CacheManger_Should_RemoveAll_User_List()
        {
            //  arrange

            MockProvider.Setup(x => x.GetAllKeys()).Returns(new[] {
                CreateKey(false), CreateKey(true)
            });

            MockProvider.Setup(x => x.Remove(CreateKey(true))).Returns(null as User);

            //  act
            SubjectUnderTest.Find<IList<User>>().RemoveAll();

            //  assert

            MockProvider.VerifyAll();
        }

        [TestMethod]
        public void CacheManger_Should_RemoveAll_User_Collection()
        {
            //  arrange

            MockProvider.Setup(x => x.GetAllKeys()).Returns(new[] {
                CreateKey(false), CreateKey(true)
            });

            MockProvider.Setup(x => x.Remove(CreateKey(true))).Returns(null as User);

            //  act
            SubjectUnderTest.Find<IList<User>>().RemoveAll();

            //  assert

            MockProvider.VerifyAll();
        }

        [TestMethod]
        public void CacheManger_Should_RemoveAll_User_By()
        {
            //  arrange

            MockProvider.Setup(x => x.GetAllKeys()).Returns(new[] {
                CreateKey(false), CreateKey(true),
                CreateKey(false, 123), CreateKey(false, 456)                
            });

            MockProvider.Setup(x => x.Remove(CreateKey(false, 123))).Returns(null as User);

            //  act
            SubjectUnderTest.Find<User>().By(123).RemoveAll();

            //  assert

            MockProvider.VerifyAll();
        }

        [TestMethod]
        public void CacheManger_Should_RemoveAll_User_ByWildcard()
        {
            //  arrange

            MockProvider.Setup(x => x.GetAllKeys()).Returns(new[] {
                CreateKey(false), CreateKey(true),
                CreateKey(false, 123,456), CreateKey(false, 123,789)
            });

            MockProvider.Setup(x => x.Remove(CreateKey(false, 123, 456))).Returns(null as User);
            MockProvider.Setup(x => x.Remove(CreateKey(false, 123, 789))).Returns(null as User);

            //  act
            SubjectUnderTest.Find<User>().By(123, "*").RemoveAll();

            //  assert

            MockProvider.VerifyAll();
        }

        #endregion
    }
}
