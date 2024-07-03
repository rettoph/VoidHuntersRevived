using Moq;
using System.Linq.Expressions;

namespace VoidHuntersRevived.Tests.Common
{
    public class MockBuilder<T>
        where T : class
    {
        private readonly Mock<T> _instance = new Mock<T>();

        public static MockBuilder<T> Create()
        {
            MockBuilder<T> builder = new MockBuilder<T>();

            return builder;
        }

        public MockBuilder<T> Setup<TResult>(Expression<Func<T, TResult>> expression, TResult result)
        {
            _instance.Setup(expression).Returns(result);

            return this;
        }

        public MockBuilder<T> Setup<TResult>(Expression<Func<T, TResult>> expression, Func<TResult> result)
        {
            _instance.Setup(expression).Returns(result);

            return this;
        }

        public MockBuilder<T> Setup<TResult, T1>(Expression<Func<T, TResult>> expression, Func<T1, TResult> result)
        {
            _instance.Setup(expression).Returns(result);

            return this;
        }

        public MockBuilder<T> Setup<TResult, T1, T2>(Expression<Func<T, TResult>> expression, Func<T1, T2, TResult> result)
        {
            _instance.Setup(expression).Returns(result);

            return this;
        }

        public MockBuilder<T> Setup<TResult, T1, T2, T3>(Expression<Func<T, TResult>> expression, Func<T1, T2, T3, TResult> result)
        {
            _instance.Setup(expression).Returns(result);

            return this;
        }

        public MockBuilder<T> Setup<TResult, T1, T2, T3, T4>(Expression<Func<T, TResult>> expression, Func<T1, T2, T3, T4, TResult> result)
        {
            _instance.Setup(expression).Returns(result);

            return this;
        }

        public Mock<T> Build()
        {
            return this._instance;
        }
    }
}
