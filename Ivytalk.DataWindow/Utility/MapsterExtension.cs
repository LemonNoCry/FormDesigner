using System;
using System.Collections.Generic;
using Mapster;

namespace Ivytalk.DataWindow.Utility
{
    public static class MapsterExtension
    {
        public static T MapsterCopyTo<T>(this object source, T target)
        {
            if (source == null)
                throw new ArgumentNullException();

            return source.Adapt(target);
        }

        public static TTarget MapsterCopyTo<TSource, TTarget>(this TSource source, TTarget target)
        {
            if (source == null)
                throw new ArgumentNullException();

            return source.Adapt(target);
        }

        public static TTarget MapsterCopyTo<TTarget>(this object source)
        {
            if (source == null)
                throw new ArgumentNullException();

            return source.Adapt<TTarget>();
        }

        public static List<TTarget> MapsterCopyTo<TSource, TTarget>(this List<TSource> source, List<TTarget> target)
        {
            if (source == null)
                throw new ArgumentNullException();

            return source.Adapt(target);
        }
    }
}