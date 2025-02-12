using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServerBlockChain.Entiites
{
    public class PoolConection(string Pool, uint Port)
    {
        private string Pool { get; set; } = Pool;
        private uint Port { get; set; } = Port;

        private string connectionString { get; set; } = string.Empty;


        public string GetPool()
        {
            return this.Pool;
        }

        public uint GetPort()
        {
            return this.Port;
        }

        public void AddConection()
        {
           this.connectionString = this.Pool + ":" + this.Port;
        }

        public string GetConnectionString()
        {
            return this.connectionString;
        }
        
    }
}