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
        public static IAsyncEnumerable<TSource> IgnoreElements<TSource>(this IAsyncEnumerable<TSource> source)
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
                await foreach (var _ in source.WithCancellation(cancellationToken).ConfigureAwait(false))
                {
                }

                yield break;
            }
        }
    }
}
