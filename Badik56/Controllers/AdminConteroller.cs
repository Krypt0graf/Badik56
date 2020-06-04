using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Badik56.Models.Entitys;
using Badik56.Models.Objects;

namespace Badik56.Controllers
{
    public class AdminController : Controller
    {
        private readonly ILogger<AdminController> _logger;
        private BackContext db { get; set; }

        public AdminController(ILogger<AdminController> logger)
        {
            db = new BackContext(new DbContextOptions<BackContext>());
            _logger = logger;
        }
        private bool Auth(string login = "", string password = "")
        {
            if (login == "" && password == "")
            {
                login = HttpContext.Session.GetString("login");
                password = HttpContext.Session.GetString("password");
            }
            var admin = db.Admins.Where(u => u.Login == login && u.Password == password).ToList();
            if (admin.Count != 0)
            {
                HttpContext.Session.SetString("login", login);
                HttpContext.Session.SetString("password", password);
                return true;
            }
            else
            {
                return false;
            }
        }
        private Models.Objects.Contact GetContact()
        {
            var contacts = db.Contacts.ToList().LastOrDefault();
            if (contacts == null)
            {
                contacts = new Models.Objects.Contact();
            }

            return contacts;
        }
        private List<Models.Objects.Baner> GetBaners()
        {
            List<Models.Objects.Baner> baners = db.Baners.ToList();
            if (baners.Count != 0)
            {
                baners.OrderByDescending(o => o.Id);
            }

            return baners;
        }

        public IActionResult Index()
        {
            if (Auth())
            {
                return View("Index");
            }
            else
            {
                return View("Login");
            }
        }

        public IActionResult Requests(string type)
        {
            List<Models.Objects.Request> reqs = new List<Models.Objects.Request>();
            if (type == "" || type == null)
            {
                reqs = db.Requests.Include(i => i.User).ToList();
            }
            else
            {
                reqs = db.Requests.Where(t => t.Theme == type).Include(i => i.User).ToList();
            }
            if (reqs != null)
            {
                reqs.OrderByDescending(i => i.Id);
            }

            return View("Requests", reqs);
        }
        public IActionResult Contacts()
        {

            if (Auth())
            {
                Models.Objects.Contact contacts = GetContact();
                return View("Contacts", contacts);
            }
            else
            {
                return View("Login");
            }
        }

        public IActionResult Baners()
        {
            if (Auth())
            {
                List<Models.Objects.Baner> baners = GetBaners();
                return View("Baners", baners);
            }
            else
            {
                return View("Login");
            }
        }
        public IActionResult Categories()
        {
            if (Auth())
            {
                List<Models.Objects.Category> category = db.Categories.ToList();
                return View("Categories", category);
            }

            else
            {
                return View("Login");
            }
        }

        public IActionResult Products(string category)
        {
            Models.PageObjects.MarketModel marketModel = GetProducts(category);
            return View("Products", marketModel);
        }

        private Models.PageObjects.MarketModel GetProducts(string category)
        {
            Models.PageObjects.MarketModel marketModel = new Models.PageObjects.MarketModel();
            marketModel.Categories = db.Categories.ToList();
            if (category != "" && category != null)
            {
                marketModel.Products = db.Products.Where(p => p.Category.Name == category).OrderByDescending(b => b.Id).Include(a => a.Image).ToList();
            }
            else
            {
                marketModel.Products = db.Products.OrderByDescending(b => b.Id).Include(a => a.Image).ToList();
            }

            return marketModel;
        }
        public IActionResult Partners()
        {
            if (Auth())
            {
                var partners = db.Partners.ToList();
                return View("Partners", partners);
            }
            else
            {
                return View("Login");
            }
        }
        public IActionResult Login()
        {
            return View("Login");
        }

        #region REST CONTROL
        [HttpPost]
        public IActionResult Products(string name, double price, string about, List<IFormFile> images, string categori, string count)
        {
            var now = DateTime.Now.ToString("yyyyMMddHHmmss");


            if (Auth())
            {
                List<Image> imgs = new List<Image>();

                if (images != null && images.Count() != 0)
                {
                    int i = 0;
                    foreach (var file in images)
                    {
                        var w = file.FileName.Split('.').Last();
                        var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/products", now + "_" + i.ToString() + "." + w);
                        using (var stream = new FileStream(path, FileMode.Create))
                        {
                            file.CopyTo(stream);
                        }
                        

                        imgs.Add(new Image() { Name = now + "_" + i.ToString() + "." + w});
                        i++;
                    }
                }
                var ca = db.Categories.Where(o => o.Name == categori).FirstOrDefault();

                db.Products.Add(new Models.Objects.Product()
                {
                    Name = name,
                    Category = ca,
                    Description = about,
                    Image = imgs,
                    Price = price,
                    Count = Convert.ToInt32(count)
                });
                db.SaveChanges();
                Models.PageObjects.MarketModel marketModel = GetProducts("");
                return View("Products", marketModel);
            }
            else
            {
                return View("Login");
            }

        }
        [HttpPost]
        public IActionResult Partners(string code, string login, string password, double balance)
        {
            if (Auth())
            {
                db.Partners.Add(new Models.Objects.Partner()
                {
                    Balance = balance,
                    Code = code,
                    Login = login,
                    Password = password
                });
                db.SaveChanges();
                var partners = db.Partners.ToList();
                return View("Partners", partners);
            }
            else
            {
                return View("Login");
            }
        }
        [HttpPost]
        public IActionResult DeletePartners(string code, string login, string password, double balance)
        {
            if (Auth())
            {
                var partd = db.Partners.Where(o => o.Code == code && o.Login == login && o.Password == password && o.Balance == balance).FirstOrDefault();
                db.Partners.Remove(partd);
                db.SaveChanges();
                var partners = db.Partners.ToList();
                return View("Partners", partners);
            }
            else
            {
                return View("Login");
            }
        }
        [HttpPost]
        public IActionResult DeletePhoto(string Id, string Id_photo)
        {
            if (Auth())
            {
                var product = db.Products.Where(o => o.Id == Convert.ToInt32(Id)).Include(f => f.Image).FirstOrDefault();
                var image = product.Image.Where(i => i.Id == Convert.ToInt32(Id_photo)).FirstOrDefault();
                product.Image.Remove(image);
                if (System.IO.File.Exists("wwwroot/images/products/" + image.Name)) System.IO.File.Delete("wwwroot/images/products/" + image.Name);
                db.SaveChanges();
                Models.PageObjects.MarketModel marketModel = GetProducts("");
                return View("Products", marketModel);
            }
            else 
            {
                return View("Login");
            }

            
        }
        [HttpPost]
        public IActionResult EditProducts(string Id, string name, string about, string price, List<IFormFile> images, string categori, string action, string count)
        {
            var now = DateTime.Now.ToString("yyyyMMddHHmmss");
            if (Auth())
            {
                if (action == "delete")
                {
                    var partd = db.Products.Where(o => o.Id == Convert.ToInt32(Id)).Include(f => f.Image).FirstOrDefault();
                    if (System.IO.File.Exists("wwwroot/images/products/" + partd.Name)) System.IO.File.Delete("wwwroot/images/products/" + partd.Name);
                    db.Products.Remove(partd);
                    db.SaveChanges();
                }
                else if (action == "edit")
                {
                    var partd = db.Products.Where(o => o.Id == Convert.ToInt32(Id)).Include(x => x.Image).FirstOrDefault();
                    var ca = db.Categories.Where(o => o.Name == categori).FirstOrDefault();
                    partd.Price = Convert.ToDouble(price.Replace('.', ','));
                    partd.Name = name;
                    partd.Description = about;
                    partd.Category = ca;
                    partd.Count = Convert.ToInt32(count);
                    if (images != null && images.Count() != 0)
                    {
                        int i = 0;
                        foreach (var file in images)
                        {
                            var w = file.FileName.Split('.').Last();
                            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/products", now + "_" + i.ToString() + "." + w);
                            using (var stream = new FileStream(path, FileMode.Create))
                            {
                                file.CopyTo(stream);
                            }
                            partd.Image.Add(new Image() { Name = now + "_" + i.ToString() + "." + w });
                            i++;
                        }
                    }
                    db.SaveChanges();
                }
                Models.PageObjects.MarketModel marketModel = GetProducts("");
                return View("Products", marketModel);
            }
            else
            {
                return View("Login");
            }
        }
        [HttpPost]
        public IActionResult Categories(string name, string url)
        {
            if (Auth())
            {

                db.Categories.Add(new Models.Objects.Category() { Name = name, Url = url, CountProducts = 0, LastEdit = DateTime.Now });
                db.SaveChanges();
                List<Models.Objects.Category> category = db.Categories.ToList();

                return View("Categories", category);
            }
            else
            {
                return View("Login");
            }
        }
        [HttpPost]
        public IActionResult DeleteCategories(string name, string url)
        {
            if (Auth())
            {
                var categoryd = db.Categories.Where(c => c.Name == name && c.Url == url).FirstOrDefault();
                db.Categories.Remove(categoryd);
                db.SaveChanges();
                List<Models.Objects.Category> category = db.Categories.ToList();
                return View("Categories", category);
            }
            else
            {
                return View("Login");
            }
        }
        [HttpPost]
        public IActionResult Baners(IFormFile file)
        {
            if (file != null && file.Length != 0)
            {
                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", file.FileName);
                using (var stream = new FileStream(path, FileMode.Create))
                {
                    file.CopyTo(stream);
                }
            }
            db.Baners.Add(new Models.Objects.Baner() { URL = file.FileName });
            db.SaveChanges();
            List<Models.Objects.Baner> baners = GetBaners();
            return View("Baners", baners);
        }
        [HttpPost]
        public IActionResult DeleteBaners(string url)
        {
            var baner = db.Baners.Where(b => b.URL == url).FirstOrDefault();
            db.Baners.Remove(baner);
            db.SaveChanges();
            List<Models.Objects.Baner> baners = GetBaners();
            return View("Baners", baners);
        }
        [HttpPost]
        public IActionResult Contacts(string EmailInfo, string EmailMarketing, string PhoneInfo, string PhoneMarketing, string VKLink, string OKLink, string FacebookLink, string InstagramLink, string Address)
        {
            db.Contacts.Add(new Models.Objects.Contact()
            {
                EmailInfo = EmailInfo,
                EmailMarketing = EmailMarketing,
                PhoneInfo = PhoneInfo,
                PhoneMarketing = PhoneMarketing,
                VKLink = VKLink,
                OKLink = OKLink,
                FacebookLink = FacebookLink,
                InstagramLink = InstagramLink,
                Address = Address,
                LastDateEdit = DateTime.Now
            });
            db.SaveChanges();

            Models.Objects.Contact contacts = GetContact();
            if (Auth())
            {
                return View("Contacts", contacts);
            }
            else
            {
                return View("Login");
            }
        }
        [HttpPost]
        public IActionResult Login(string login, string password)
        {
            if (Auth(login, password))
            {
                return View("Index");
            }
            else
            {
                return View("Login");
            }
        }
        #endregion
    }
}