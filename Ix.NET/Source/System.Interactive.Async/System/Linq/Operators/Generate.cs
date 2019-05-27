// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the Apache 2.0 License.
// See the LICENSE file in the project root for more information. 

using System.Collections.Generic;
using System.Threading;

namespace System.Linq
{
    public static partial class AsyncEnumerableEx
    {
        // REVIEW: Add async variant?

        public static IAsyncEnumerable<TResult> Generate<TState, TResult>(TState initialState, Func<TState, bool> condition, Func<TState, TState> iterate, Func<TState, TResult> resultSelector)
        {
            if (condition == null)
                throw Error.ArgumentNull(nameof(condition));
            if (iterate == null)
                throw Error.ArgumentNull(nameof(iterate));
            if (resultSelector == null)
                throw Error.ArgumentNull(nameof(resultSelector));

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
#if HAS_ASYNC_ENUMERABLE_CANCELLATION
            return Core(initialState, condition, iterate, resultSelector);

            static async IAsyncEnumerable<TResult> Core(TState initialState, Func<TState, bool> condition, Func<TState, TState> iterate, Func<TState, TResult> resultSelector, [System.Runtime.CompilerServices.EnumeratorCancellation]CancellationToken cancellationToken = default)
#else
            return AsyncEnumerable.Create(Core);

            async IAsyncEnumerator<TResult> Core(CancellationToken cancellationToken)
#endif
            {
                for (var state = initialState; condition(state); state = iterate(state))
                {
                    // REVIEW: Check for cancellation?

                    yield return resultSelector(state);
                }
            }
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        }
    }
}
