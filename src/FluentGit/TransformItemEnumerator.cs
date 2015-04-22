using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FluentGit
{
    public class TransformItemEnumerator<TFromType, ToType> : IEnumerator<ToType>
        where TFromType : class
        where ToType : class
    {

        private IEnumerator<TFromType> _inputEnumerator;
        private Func<TFromType, ToType> _transformFunction;

        public TransformItemEnumerator(IEnumerator<TFromType> inputEnumerator, Func<TFromType, ToType> transformItem)
        {
            _inputEnumerator = inputEnumerator;
            _transformFunction = transformItem;
        }

        public object Current
        {
            get { return _transformFunction(_inputEnumerator.Current); }
        }

        public bool MoveNext()
        {
            return _inputEnumerator.MoveNext();
        }

        public void Reset()
        {
            _inputEnumerator.Reset();
        }

        ToType IEnumerator<ToType>.Current
        {
            get
            {
                var currentItem = _transformFunction(_inputEnumerator.Current);
                return currentItem as ToType;
            }
        }

        public void Dispose()
        {
            _inputEnumerator.Dispose();
        }
    }
}
