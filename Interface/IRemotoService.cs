using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Threading.Tasks;
using ServerBlockChain.Entities;

namespace ServerBlockChain.Interface
{
    public interface IRemotoService
    {
        void CreateRemoto();

        void StopRemoto();
    }
}