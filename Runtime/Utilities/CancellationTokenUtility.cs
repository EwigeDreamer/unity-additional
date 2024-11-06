using System;
using System.Threading;
using ED.Additional.Collections;

namespace ED.Extensions.System
{
    public static class CancellationTokenUtility
    {
        public static IDisposable Combine(out CancellationToken resultToken, params CancellationToken[] cancellationTokens)
        {
            var cts = new CancellationTokenSource();
            var disposables = new DisposableCollection();
            foreach (var token in cancellationTokens)
                disposables.Add(token.Register(cts.Cancel));
            disposables.Add(cts);
            resultToken = cts.Token;
            return disposables;
        }
    }
}