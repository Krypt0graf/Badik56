using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Badik56.Models;
using Badik56.Models.Entitys;
using Microsoft.EntityFrameworkCore;
using Badik56.Models.PageObjects;
using System.Net;
using System.Net.Http;
using Microsoft.AspNetCore.Http.Features;

namespace Badik56.Controllers
{
    public class HomeController : Controller
    {
        private FrontContext db { get; set; }
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            db = new FrontContext(new DbContextOptions<FrontContext>());
            _logger = logger;
        }
        private IndexModel GetModelIndex()
        {
            IndexModel indexModel = new IndexModel();
            var contact = db.Contacts.OrderByDescending(x => x).FirstOrDefault();
            indexModel.Contacts = contact == null ? new Models.Objects.Contact() : contact;
            indexModel.Products = db.Products/*.Where(o => o.Category.Name == "Рекомендуемые")*/.Include(i => i.Image).OrderByDescending(x => x).ToList();
            indexModel.Baners = db.Baners.ToList();
            indexModel.Categories = db.Categories.ToList();
            return indexModel;
        }
        private ContactModel GetModelContact()
        {
            ContactModel contactModel = new ContactModel();
            var contact = db.Contacts.OrderByDescending(x => x).FirstOrDefault();
            contactModel.Contacts = contact == null ? new Models.Objects.Contact() : contact;
            contactModel.Categories = db.Categories.ToList();
            return contactModel;
        }
        private MarketModel GetModelMarket(List<Models.Objects.Product> product, string selectCoty)
        {
            MarketModel marketModel = new MarketModel();
            var contact = db.Contacts.OrderByDescending(x => x).FirstOrDefault();
            marketModel.Contacts = contact == null ? new Models.Objects.Contact() : contact;
            marketModel.Categories = db.Categories.ToList();
            marketModel.Products = product;
            marketModel.SelectedCategory = selectCoty;
            return marketModel;
        }
        private PartnerModel GetModelPartner()
        {
            PartnerModel partnerModel = new PartnerModel();
            partnerModel.Categories = db.Categories.ToList();
            var contact = db.Contacts.OrderByDescending(x => x).FirstOrDefault();
            partnerModel.Contacts = contact == null ? new Models.Objects.Contact() : contact;
            return partnerModel;
        }

        #region GETMETHODE
        public IActionResult Index()
        {
            db.Database.EnsureCreated();
            //TODO: Отладить функцию котроля IP
            //Models.Objects.IP IP = new Models.Objects.IP()
            //{
            //    Address = System.Text.Encoding.UTF8.GetString(HttpContext.Connection.RemoteIpAddress.GetAddressBytes())
            //};
            //var ipDb = db.IPs.Where(i => i.Address == IP.Address).FirstOrDefault();
            //db.Users.Add(new Models.Objects.User()
            //{
            //    DateCreate = DateTime.Now,
            //    IP = ipDb != null && IP.Address == ipDb.Address ? ipDb.Address : IP.Address
            //});
            //db.SaveChanges();
            IndexModel indexModel = GetModelIndex();
            return View(indexModel);
        }
        public IActionResult Contact()
        {
            ContactModel contactModel = GetModelContact();
            return View("Contact", contactModel);
        }
        public IActionResult Market(string id)
        {
            List<Models.Objects.Product> product;
            if (id == string.Empty || id == null)
            {
                product = db.Products.Include(i => i.Image).ToList();
            }
            else
            {
                product = db.Products.Where(p => p.Category.Url == id).Include(i => i.Image).ToList();
            }
            product.SelectMany(i => i.Image);
            var selectCategory = db.Categories.Where(c => c.Url == id).Select(s => s.Name).FirstOrDefault();
            MarketModel marketModel = GetModelMarket(product, selectCategory);
            return View("Market", marketModel);
        }
        public IActionResult Partner()
        {
            var partner = GetModelPartner();
            return View("Partner", partner);
        }
        public IActionResult About()
        {
            var about = db.Abouts.ToList();
            Models.Objects.About a = new Models.Objects.About();
            if (about.Count == 0)
            {
                about = new List<Models.Objects.About>();
            }
            else
            {
                a = about.OrderByDescending(o => o.Id).FirstOrDefault();
            }
            var cont = db.Contacts.ToList();
            Models.Objects.Contact c = new Models.Objects.Contact();
            if (cont.Count != 0)
            {
                c = cont.OrderByDescending(c => c.Id).First();
            }
            var aboutModel = new AboutModel()
            {
                Abouts = a,
                Categories = db.Categories.ToList(),
                Contacts = c
            };
            return View("About", aboutModel);
        }
        public IActionResult Product(int id)
        {
            var product = db.Products.Where(x => x.Id == id).Include(c => c.Image).FirstOrDefault();
            var productModel = new ProductModel();
            productModel.Product = product;
            productModel.Categories = db.Categories.ToList();
            var contact = db.Contacts.OrderByDescending(x => x).FirstOrDefault();
            productModel.Contacts = contact == null ? new Models.Objects.Contact() : contact;
            return View("Product", productModel);
        }

        [HttpGet]
        public ActionResult BalancePartner(string code, string password)
        {
            var balance = db.Partners.Where(o => o.Code == code && o.Password == password).Select(x => x.Balance).FirstOrDefault();
            return Json(balance);
        }
        [HttpPost]
        public ActionResult OutOfMoneyPartners(string code, string card, string password)
        {
            var User = db.Partners.Where(x => x.Code == code && x.Password == password).FirstOrDefault();
            if (User != null)
            {
                Models.Objects.Request request = new Models.Objects.Request();
                request.Message = User.Balance.ToString() + "\n" + card;
                request.Theme = "Запрос на вывод средств";
                request.User = User;
                db.Requests.Add(request);
                db.SaveChanges();
            }

            var indexModel = GetModelIndex();

            return View("Index", indexModel);
        }
        #endregion

        #region POSTMETHODE
        [HttpPost]
        public IActionResult Contact(string name, string contact, string theme, string text)
        {
            Models.Objects.Request request = new Models.Objects.Request()
            {
                Message = text,
                Theme = theme
            };
            Models.Objects.User user = new Models.Objects.User();
            if (db.Users.Any(u => u.Email == contact))
            {
                user = db.Users.Where(u => u.Email == contact).FirstOrDefault();
            }
            else
            {
                user = new Models.Objects.User() { Email = contact };
            }
            user.IP = HttpContext.Features.Get<IHttpConnectionFeature>()?.RemoteIpAddress?.ToString();
            user.Name = name;
            request.User = user;
            var telegram = new Telegram("1035927104:AAG-NfYKKy0uVEET4YSm-Cymrxl7v3I0lpo");
            telegram.SendMessage("Пользователь: " + contact, "478950049");
            telegram.SendMessage("Тема письма: " + theme, "478950049");
            telegram.SendMessage("Текст письма: " + text, "478950049");

            telegram.SendMessage("Пользователь: " + contact, "516270089");
            telegram.SendMessage("Тема письма: " + theme, "516270089");
            telegram.SendMessage("Текст письма: " + text, "516270089");
            db.Requests.Add(request);
            db.SaveChanges();
            ContactModel contactModel = GetModelContact();
            return View("Contact", contactModel);
        }
        [HttpPost]
        public IActionResult Subscribe(string email)
        {
            if (!db.Users.Any(u => u.Email == email))
            {
                db.Users.Add(new Models.Objects.User()
                {
                    IP = System.Text.Encoding.UTF8.GetString(HttpContext.Connection.RemoteIpAddress.GetAddressBytes()),
                    Email = email,
                    DateCreate = DateTime.Now
                });
                db.SaveChanges();
            }
            IndexModel indexModel = GetModelIndex();
            return View("Index", indexModel);
        }
        #endregion

        //public IActionResult Privacy()
        //{
        //    return View();
        //}

        //[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        //public IActionResult Error()
        //{
        //    return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        //}
        [HttpGet]
        public string SendRequest(string code, string amount, string card, string bank, string fio)
        {
            var d = DateTime.Now;
            string message = "---Badik56---\r\n" +
                             "[Заявка на вывод начислений]\r\n" + 
                             "Код партнера: " + code + "\r\n" +
                             "Сумма: " + amount + "₽\r\n" +
                             "Банк: " + bank + "\r\n" +
                             "Карта: " + card + "\r\n" +
                             "ФИО: " + fio;
            VK.SendMessage("random_id=" + d.ToString("yyMMddHHmmss") +
                          "&v=" + "5.92" + 
                          "&user_ids=" + "198672105, 9084514" +
                          "&message=" + message +
                          "&access_token=" + "c07179b5ffae3b7433a84cc3ea3240e3be05c207fbc6eed4dbcb299a0c4a614199bd8ad999afdadaa9471");
            return "Ваша заявка на вывод средств принята, ожидайте поступления на указанные реквизиты";
        }
    }
    public static class VK
    {
        public static string SendMessage(string data)
        {
            using (WebClient w = new WebClient())
            {
                w.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
               string result =  w.UploadString("https://api.vk.com/method/messages.send", data);
            }
                return "1";
        }
    }
    public class Telegram
    {
        private string Token { get; set; }
        private string Api { get; set; } = "https://api.telegram.org/bot";
        private string MethodeSendMessage { get; set; } = "/sendMessage";

        public Telegram(string token)
        {
            Token = token;
        }
        
        public void SendMessage(string text, string chatId)
        {
            //c07179b5ffae3b7433a84cc3ea3240e3be05c207fbc6eed4dbcb299a0c4a614199bd8ad999afdadaa9471
            //string url = Api + Token + MethodeSendMessage;
            //Dictionary<string, string> param = new Dictionary<string, string>();
            //param["chat_id"] = chatId;
            //param["text"] = text;
            //string queryText = url + "?" + BuildQueryData(param);
            using (var webClient = new WebClient())
            {
                // Выполняем запрос по адресу и получаем ответ в виде строки
                
                var response = webClient.DownloadString("https://afcstudio.ru/core/telegram.php?token=" + Token + "&text=" + text + "&chatid=" + chatId);
            }
        }
    }
}
