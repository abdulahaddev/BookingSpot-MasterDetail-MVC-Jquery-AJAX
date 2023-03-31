using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVC_EV_FINAL.ViewModels
{
    public class ClientVM
    {
		public string ClientName { get; set; }
		public DateTime BirthDate { get; set; }
		public int Age { get; set; }
		public HttpPostedFileBase Picture { get; set; }
		public bool MaritalStatus { get; set; }
	}
}