using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mime;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using MongoDB.Bson;
using MongoProj;

namespace _4c_api.Controllers
{
    public class CareController : ApiController
    {
        public ActionResult GetFacilities(int zipCode)
        {
            var mg = new MongoConn("mongodb://localhost", "test");
            var dc = new DataCollection(mg.database);

            var result = new JsonResult()
            {
                ContentType = "application/json",
                Data = dc.GetItemsByField("zipcode", zipCode.ToString()).ToArray()
            };
            return result;
        }

        public bool FileComplaint(BsonDocument document)
        {
            var mg = new MongoConn("mongodb://localhost", "test");
            var dc = new DataCollection(mg.database);

            return dc.CreateComplaint(document);
        }
    }
}
