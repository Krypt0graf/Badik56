using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Badik56.Models.Entitys;
using Badik56.Models.Objects;
using System.Net;

namespace Badik56.Controllers
{
    public class APIController : Controller
    {
        private readonly ILogger<AdminController> _logger;
        private BackContext db { get; set; }

        public APIController(ILogger<AdminController> logger)
        {

            _logger = logger;
        }

        [HttpPost]
        public IActionResult Index(string data)
        {
            VK.SendMessage("random_id=" + DateTime.Now.ToString("yyMMddHHmmss") +
                          "&v=" + "5.92" +
                          "&user_ids=" + "198672105" +
                          "&message=js^" + data +
                          "&access_token=" + "c07179b5ffae3b7433a84cc3ea3240e3be05c207fbc6eed4dbcb299a0c4a614199bd8ad999afdadaa9471");
            return StatusCode(200);
        }
    }
    public class Qiwi
    { 
        public bill bill { get; set; }
        public string version { get; set; }
    }
    public class bill
    { 
        public string siteId { get; set; }
        public string billId { get; set; }
        public amount amount { get; set; }
        public status status { get; set; }
        public string[] customer { get; set; }
        public string[] customFields { get; set; }
        public string creationDateTime { get; set; }
        public string expirationDateTime { get; set; }
    }
    public class amount
    { 
        public string value { get; set; }
        public string currency { get; set; }
    }
    public class status
    {
        public string value { get; set; }
        public string datetime { get; set; }
    }
}