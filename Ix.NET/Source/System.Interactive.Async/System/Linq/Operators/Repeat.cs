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
        public static IAsyncEnumerable<TResult> Repeat<TResult>(TResult element)
        {
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
#if HAS_ASYNC_ENUMERABLE_CANCELLATION
            return Core(element);

            static async IAsyncEnumerable<TResult> Core(TResult element, [System.Runtime.CompilerServices.EnumeratorCancellation]CancellationToken cancellationToken = default)
#else
            return AsyncEnumerable.Create(Core);

            async IAsyncEnumerator<TResult> Core(CancellationToken cancellationToken)
#endif
            {
                while (true)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    yield return element;
                }
            }
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        }

        public static IAsyncEnumerable<TSource> Repeat<TSource>(this IAsyncEnumerable<TSource> source)
        {
            if (source == null)
                throw Error.ArgumentNull(nameof(source));

#if HAS_ASYNC_ENUMERABLE_CANCELLATION
            return Core(source);

            static async IAsyncEnumerable<TSource> Core(IAsyncEnumerable<TSource> source, [System.Runtime.CompilerServices.EnumeratorCancellation]CancellationToken cancellationToken = default)
#else
            return AsyncEnumerable.Create(Core);

            async IAsyncEnumerator<TSource> Core(CancellationToken cancellationToken)
#endif
            {
                while (true)
                {
                    await foreach (var item in source.WithCancellation(cancellationToken).ConfigureAwait(false))
                    {
                        yield return item;
                    }
                }
            }
        }

        public static IAsyncEnumerable<TSource> Repeat<TSource>(this IAsyncEnumerable<TSource> source, int count)
        {
            if (source == null)
                throw Error.ArgumentNull(nameof(source));
            if (count < 0)
                throw Error.ArgumentOutOfRange(nameof(count));

#if HAS_ASYNC_ENUMERABLE_CANCELLATION
            return Core(source, count);

            static async IAsyncEnumerable<TSource> Core(IAsyncEnumerable<TSource> source, int count, [System.Runtime.CompilerServices.EnumeratorCancellation]CancellationToken cancellationToken = default)
#else
            return AsyncEnumerable.Create(Core);

            async IAsyncEnumerator<TSource> Core(CancellationToken cancellationToken)
#endif
            {
                for (var i = 0; i < count; i++)
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
