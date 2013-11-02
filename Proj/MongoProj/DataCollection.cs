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

        public BsonDocument UpdateAgency(string name, string streetAddress, string city, string state, string zipCode, bool addComplaint = false)
        {
            BsonDocument document;

            if (collection == null)
                return null;

            try
            {
                var query = Query.And(Query.EQ("name", name), Query.EQ("zipCode", zipCode));
                document = collection.Find(query).FirstOrDefault();
                if (document == null)
                    document = new BsonDocument();
                //                document = collection.FindAs(query) ?? new BsonDocument();
                document["name"] = name;
                document["address"] = streetAddress;
                document["city"] = city;
                document["state"] = state;
                document["zipcode"] = zipCode;

                if (!addComplaint)
                    collection.Save(document);
            }
            catch (Exception)
            {
                return null;
            }

            return document;
        }

        public bool CreateComplaint(string name, string address, string city, string state, string zip, string reportedBy, string issue, string description, int severity, DateTime date, string contactInfo)
        {
            var doc = new BsonDocument()
                {
                    {"name", name},
                    {"address", address},
                    {"city", city},
                    {"state", state},
                    {"zipcode", zip},
                    {"reporter", reportedBy},
                    {"issue", issue},
                    {"description", description},
                    {"severity", severity},
                    {"incidentDate", date},
                    {"contactRef", contactInfo},
                };
            return CreateComplaint(doc);
        }

        public bool CreateComplaint(BsonDocument bsonDocument)
        {
            var TimeStampUTC = DateTime.UtcNow;
            var document = UpdateAgency(bsonDocument["name"].ToString(), bsonDocument["address"].ToString(), bsonDocument["city"].ToString(), bsonDocument["state"].ToString(), bsonDocument["zipcode"].ToString(), true);
            var complaints = new BsonArray();
            if (document.Contains("complaints"))
                complaints = document["complaints"] as BsonArray;
            var complaint = new BsonDocument()
                {
                    {"reporter",bsonDocument["reporter"]},
                    {"issue",bsonDocument["issue"]},
                    {"description",bsonDocument["description"]},
                    {"severity",bsonDocument["severity"]},
                    {"incidentDate",bsonDocument["incidentDate"]},
                    {"reportDate", TimeStampUTC},
                    {"contactRef",bsonDocument["contactRef"]}
                };
            complaints.Add(complaint);
            document["complaints"] = complaints;
            collection.Save(document);

            return true;
        }

        public BsonDocument[] GetItemsByField(string fieldName, string value)
        {
            var query = Query.And(Query.EQ(fieldName, value));
            var doc = collection.Find(query).ToArray();
            return doc;
        }

        public void getAllDocs()
        {
            var docs = collection.FindAll();
            foreach (BsonDocument doc in docs)
            {
                BsonArray cDocs = doc["complaints"] as BsonArray;
                foreach (var cDoc in cDocs)
                    exportAsOne(doc, cDoc.ToBsonDocument());
            }
        }

        public BsonDocument exportAsOne(BsonDocument doc, BsonDocument complaint)
        {
            var reportDate = complaint.Contains("reportDate") ? complaint["reportDate"] : null;
            var result = new BsonDocument()
                {
                    {"businessName", doc["name"]},
                    {"streetAddress", doc["address"]},
                    {"city", doc["city"]},
                    {"state", doc["state"]},
                    {"zipCode", doc["zipcode"]},
                    {"reporter", complaint["reporter"]},
                    {"issue", complaint["issue"]},
                    {"description", complaint["description"]},
                    {"severity", complaint["severity"]},
                    {"incidentDate", complaint["incidentDate"]},
                    {"dateReported", reportDate},
                    {"contactInfo", complaint["contactRef"]}
                };
            Console.WriteLine(result.ToString());
            return result;
        }
    }
}
