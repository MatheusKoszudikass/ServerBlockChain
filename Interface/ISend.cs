using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Security;
using System.Threading.Tasks;

namespace ServerBlockChain.Interface;

public interface ISend<T>
{
    Task SendAsync(T data, CancellationToken cts = default);
    Task SendAsync(List<T> data, CancellationToken cts = default);
}