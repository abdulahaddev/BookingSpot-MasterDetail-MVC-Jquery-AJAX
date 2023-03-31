using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MVC_EV_FINAL.ViewModels;
using MVC_EV_FINAL.Models;
using Newtonsoft.Json;
using System.Data.Entity;

namespace MVC_EV_FINAL.Controllers
{
    public class MasterDetailController : Controller
    {
        BookingDbContext db = new BookingDbContext();
        // GET: MasterDetail
        public ActionResult Index()
        {
            ViewBag.spots = new SelectList(db.Spots.ToList(), "SpotId", "SpotName");
            var data = db.Clients.Include(c => c.bookingEntries.Select(b => b.Spot)).OrderByDescending(x => x.ClientId).ToList();
            return View(data);
        }
        public ActionResult AddNewSpot(int? id)
        {
            ViewBag.spots = new SelectList(db.Spots.ToList(), "SpotId", "SpotName", (id != null) ? id.ToString() : "");
            return PartialView("_addNewSpot");
        }
        [HttpPost]
        public JsonResult Create(string postedClient, string ClientId, HttpPostedFileBase file, int[] spot)
        {
            if (ClientId == "")
            {
                Client client = JsonConvert.DeserializeObject<Client>(postedClient);
                string filePath;

                if (file != null)
                {
                    filePath = Path.Combine("/Images/", Guid.NewGuid().ToString() + Path.GetExtension(file.FileName));
                    file.SaveAs(Server.MapPath(filePath));
                    client.Picture = filePath;
                }
                if (file == null)
                {
                    client.Picture = "";
                }

                foreach (var item in spot)
                {
                    BookingEntry bookingEntry = new BookingEntry()
                    {
                        ClientId = client.ClientId,
                        Client = client,
                        SpotId = item
                    };
                    db.BookingEntries.Add(bookingEntry);
                }
                db.SaveChanges();
            }

            if (ClientId != "") // For existed and modify
            {
                string filePath;

                Client client = JsonConvert.DeserializeObject<Client>(postedClient);
                client.ClientId = int.Parse(ClientId);

                if (file != null)
                {
                    filePath = Path.Combine("/Images/", Guid.NewGuid().ToString() + Path.GetExtension(file.FileName));
                    file.SaveAs(Server.MapPath(filePath));
                    client.Picture = filePath;
                }
                if (file == null)
                {
                    client.Picture = "";
                }

                var existsSpot = db.BookingEntries.Where(x => x.ClientId == client.ClientId).ToList();

                foreach (var bookingEntry in existsSpot)
                {
                    db.BookingEntries.Remove(bookingEntry);
                }

                foreach (var item in spot)
                {
                    BookingEntry bookingEntry = new BookingEntry()
                    {
                        ClientId = client.ClientId,
                        SpotId = item
                    };
                    db.BookingEntries.Add(bookingEntry);
                }
                Client dbClient = db.Clients.Find(client.ClientId);
                dbClient.ClientName = client.ClientName;
                dbClient.BirthDate = client.BirthDate;
                dbClient.Age = client.Age;
                dbClient.Picture = client.Picture;
                dbClient.MaritalStatus = client.MaritalStatus;

                db.Entry(dbClient).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
            
            return Json("ok", JsonRequestBehavior.AllowGet);
        }
      
        public JsonResult Delete(int id)
        {
            var client = db.Clients.Find(id);
            var existsSpot = db.BookingEntries.Where(x => x.ClientId == id).ToList();

            foreach (var bookingEntry in existsSpot)
            {
                db.BookingEntries.Remove(bookingEntry);
            }
            db.Entry(client).State = EntityState.Deleted;
            db.SaveChanges();
            return Json("", JsonRequestBehavior.AllowGet);
        }

    }
}
