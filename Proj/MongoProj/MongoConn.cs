using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace MongoProj
{
    public class MongoConn
    {
        private MongoClient client;
        private MongoServer server;
        public MongoDatabase database
        {
            get { return _database; }
        }
        private MongoDatabase _database;


        public MongoConn(string connectionString, string dbName)
        {
            try
            {
                client = new MongoClient(connectionString);
                server = client.GetServer();
                _database = server.GetDatabase(dbName);
            }
            catch (Exception)
            {
            }
        }

        ~MongoConn()
        {
            server.Disconnect();
        }
    }
}
