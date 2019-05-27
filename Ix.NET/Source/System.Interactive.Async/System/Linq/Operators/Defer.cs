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
        public static IAsyncEnumerable<TSource> Defer<TSource>(Func<IAsyncEnumerable<TSource>> factory)
        {
            if (factory == null)
                throw Error.ArgumentNull(nameof(factory));

#if HAS_ASYNC_ENUMERABLE_CANCELLATION
            return Core(factory);

            static async IAsyncEnumerable<TSource> Core(Func<IAsyncEnumerable<TSource>> factory, [System.Runtime.CompilerServices.EnumeratorCancellation]CancellationToken cancellationToken = default)
#else
            return AsyncEnumerable.Create(Core);

            async IAsyncEnumerator<TSource> Core(CancellationToken cancellationToken)
#endif
            {
                await foreach (var item in factory().WithCancellation(cancellationToken).ConfigureAwait(false))
                {
                    yield return item;
                }
            }
        }

        public static IAsyncEnumerable<TSource> Defer<TSource>(Func<Task<IAsyncEnumerable<TSource>>> factory)
        {
            if (factory == null)
                throw Error.ArgumentNull(nameof(factory));

#if HAS_ASYNC_ENUMERABLE_CANCELLATION
            return Core(factory);

            static async IAsyncEnumerable<TSource> Core(Func<Task<IAsyncEnumerable<TSource>>> factory, [System.Runtime.CompilerServices.EnumeratorCancellation]CancellationToken cancellationToken = default)
#else
            return AsyncEnumerable.Create(Core);

            async IAsyncEnumerator<TSource> Core(CancellationToken cancellationToken)
#endif
            {
                await foreach (var item in (await factory().ConfigureAwait(false)).WithCancellation(cancellationToken).ConfigureAwait(false))
                {
                    yield return item;
                }
            }
        }

#if !NO_DEEP_CANCELLATION
        public static IAsyncEnumerable<TSource> Defer<TSource>(Func<CancellationToken, Task<IAsyncEnumerable<TSource>>> factory)
        {
            if (factory == null)
                throw Error.ArgumentNull(nameof(factory));

#if HAS_ASYNC_ENUMERABLE_CANCELLATION
            return Core(factory);

            static async IAsyncEnumerable<TSource> Core(Func<CancellationToken, Task<IAsyncEnumerable<TSource>>> factory, [System.Runtime.CompilerServices.EnumeratorCancellation]CancellationToken cancellationToken = default)
#else
            return AsyncEnumerable.Create(Core);

            async IAsyncEnumerator<TSource> Core(CancellationToken cancellationToken)
#endif
            {
                await foreach (var item in (await factory(cancellationToken).ConfigureAwait(false)).WithCancellation(cancellationToken).ConfigureAwait(false))
                {
                    yield return item;
                }
            }
        }
#endif
    }
}
