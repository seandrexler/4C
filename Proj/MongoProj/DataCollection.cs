using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Driver;
using MongoDB.Bson;
using MongoDB.Driver.Builders;

namespace MongoProj
{
    public class DataCollection
    {
        private MongoCollection<BsonDocument> collection;
            
        public DataCollection(MongoDatabase db)
        {
            collection = db.GetCollection<BsonDocument>("care_center");
        }

        public BsonDocument UpdateAgency(string name, string streetAddress, string city, string state, int zipCode, bool addComplaint = false)
        {
            BsonDocument document;

            if (collection == null)
                return null;

            try
            {
                //var query = Query.And(Query.EQ("name", name), Query.EQ("zipCode",zipCode));
                var query = Query.And(Query.EQ("name", name));
                document = collection.Find(query).First();
//                document = collection.FindAs(query) ?? new BsonDocument();
                document["name"] = name;
                document["address"] = streetAddress;
                document["city"] = city;
                document["state"] = state;
                document["zipcode"] = zipCode;

                if(!addComplaint)
                    collection.Save(document);
            }
            catch (Exception)
            {
                return null;
            }

            return document;
        }

        public bool CreateComplaint(string name, string streetAddress, string city, string state, int zipCode, string reporterName, string issue, string description, int severity, DateTime date,
                                     string contactRef)
        {
            var document = UpdateAgency(name, streetAddress, city, state, zipCode, true);
            var complaints = new BsonArray();
            if (document.Contains("complaints"))
                complaints = document["complaints"] as BsonArray;
            var complaint = new BsonDocument()
                {
                    {"reporter",reporterName},
                    {"issue",issue},
                    {"description",description},
                    {"severity",severity},
                    {"incidentDate",date},
                    {"contactRef",contactRef}
                };
            complaints.Add(complaint);
            document["complaints"] = complaints;
            collection.Save(document);

            return true;
        }

        public void GetComplaint()
        {
        }
    }
}
