using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FluentGit
{

    //public class TransformingEnumerable<TFromType, ToType> : IFluentEnumerable<ToType>
    //    where TFromType : class
    //    where ToType : class
    //{
    //    private IEnumerator<TFromType> _enumerator;
    //    private TransformItemEnumerator<TFromType, ToType> _transformingEnumerator; 

    //    public TransformingEnumerable(IEnumerator<TFromType> sourceEnumerator)
    //    {
    //        _enumerator = sourceEnumerator;
            
    //        _transformingEnumerator = new TransformItemEnumerator<TFromType,ToType>(sourceEnumerator, f=> ()f)
    //    }

    //    public IEnumerator<T> GetEnumerator()
    //    {
    //        return _enumerator;
    //    }

    //    IEnumerator IEnumerable.GetEnumerator()
    //    {
    //        return _enumerator;
    //    }
    //}

    public class FluentEnumerable<T> : IFluentEnumerable<T>
    {
        private IEnumerator<T> _enumerator;

        public FluentEnumerable(IEnumerator<T> enumerator)
        {
            _enumerator = enumerator;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _enumerator;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _enumerator;
        }
    }
}
