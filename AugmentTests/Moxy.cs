using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Moq;

namespace Augment.Tests
{
    public class Moxy
    {
        #region Members

        private Dictionary<Type, object> _mocks = new Dictionary<Type, object>();

        #endregion

        #region Moq.Mock Methods

        public Mock<T> GetMock<T>() where T : class
        {
            object mock = null;

            if (!_mocks.TryGetValue(typeof(T), out mock))
            {
                mock = new Mock<T>();

                _mocks.Add(typeof(T), mock);
            }

            return (Mock<T>)mock;
        }

        public void VerifyAll()
        {
            foreach (var item in _mocks)
            {
                Mock mock = item.Value as Mock;

                if (mock != null)
                {
                    mock.VerifyAll();
                }
            }
        }

        #endregion

        #region Instance Methods

        public void SetInstance<T>(object instance) where T : class
        {
            _mocks.Add(typeof(T), instance);
        }

        public T CreateInstance<T>() where T : class
        {
            ConstructorInfo[] constructors = typeof(T).GetConstructors();

            if (constructors.Length > 1)
            {
                string msg = GetType().Name + " only supports one Constructor per instance Type.";

                throw new ArgumentNullException(msg);
            }

            ConstructorInfo constructor = constructors.Single();

            List<object> parameters = new List<object>();

            foreach (ParameterInfo pi in constructor.GetParameters())
            {
                object obj = GetParameterObject(pi.ParameterType);

                parameters.Add(obj);
            }

            return (T)constructor.Invoke(parameters.ToArray());
        }

        private object GetParameterObject(Type type)
        {
            object obj = null;

            if (_mocks.TryGetValue(type, out obj))
            {
                Mock mocked = obj as Mock;

                if (mocked == null)
                {
                    return obj;
                }

                return mocked.Object;
            }

            Mock mock = (Mock)MakeMock(type);

            _mocks.Add(type, mock);

            return mock.Object;
        }

        private object MakeMock(Type interfaceType)
        {
            Type generic = typeof(Mock<>);

            Type[] genericArgTypes = new[] { interfaceType };

            Type mockType = generic.MakeGenericType(genericArgTypes);

            Type[] argTypes = new[] { typeof(MockBehavior) };

            object[] argValues = new[] { (object)MockBehavior.Strict };

            ConstructorInfo constructor = mockType.GetConstructor(argTypes);

            return constructor.Invoke(argValues);
        }

        #endregion
    }
}
