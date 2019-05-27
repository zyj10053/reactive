// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the Apache 2.0 License.
// See the LICENSE file in the project root for more information. 

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace System.Linq
{
    public static partial class AsyncEnumerableEx
    {
        public static IAsyncEnumerable<TSource> Concat<TSource>(this IAsyncEnumerable<IAsyncEnumerable<TSource>> sources)
        {
            if (sources == null)
                throw Error.ArgumentNull(nameof(sources));

#if HAS_ASYNC_ENUMERABLE_CANCELLATION
            return Core(sources);

            static async IAsyncEnumerable<TSource> Core(IAsyncEnumerable<IAsyncEnumerable<TSource>> sources, [System.Runtime.CompilerServices.EnumeratorCancellation]CancellationToken cancellationToken = default)
#else
            return AsyncEnumerable.Create(Core);

            async IAsyncEnumerator<TSource> Core(CancellationToken cancellationToken)
#endif
            {
                await foreach (var source in sources.WithCancellation(cancellationToken).ConfigureAwait(false))
                {
                    await foreach (var item in source.WithCancellation(cancellationToken).ConfigureAwait(false))
                    {
                        yield return item;
                    }
                }
            }
        }

        public static IAsyncEnumerable<TSource> Concat<TSource>(this IEnumerable<IAsyncEnumerable<TSource>> sources)
        {
            if (sources == null)
                throw Error.ArgumentNull(nameof(sources));

#if HAS_ASYNC_ENUMERABLE_CANCELLATION
            return Core(sources);

            static async IAsyncEnumerable<TSource> Core(IEnumerable<IAsyncEnumerable<TSource>> sources, [System.Runtime.CompilerServices.EnumeratorCancellation]CancellationToken cancellationToken = default)
#else
            return AsyncEnumerable.Create(Core);

            async IAsyncEnumerator<TSource> Core(CancellationToken cancellationToken)
#endif
            {
                foreach (var source in sources)
                {
                    await foreach (var item in source.WithCancellation(cancellationToken).ConfigureAwait(false))
                    {
                        yield return item;
                    }
                }
            }
        }

        public static IAsyncEnumerable<TSource> Concat<TSource>(params IAsyncEnumerable<TSource>[] sources)
        {
            if (sources == null)
                throw Error.ArgumentNull(nameof(sources));

#if HAS_ASYNC_ENUMERABLE_CANCELLATION
            return Core(sources);

            static async IAsyncEnumerable<TSource> Core(IAsyncEnumerable<TSource>[] sources, [System.Runtime.CompilerServices.EnumeratorCancellation]CancellationToken cancellationToken = default)
#else
            return AsyncEnumerable.Create(Core);

            async IAsyncEnumerator<TSource> Core(CancellationToken cancellationToken)
#endif
            {
                foreach (var source in sources)
                {
                    await foreach (var item in source.WithCancellation(cancellationToken).ConfigureAwait(false))
                    {
                        yield return item;
                    }
                }
            }
        }
    }
}
