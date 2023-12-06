using Microsoft.AspNetCore.Mvc;
using RestSharp;
using System.Diagnostics;
using Trendyol_Api.Models;

namespace Trendyol_Api.Controllers
{

    public class Items
    {
        public string barcode { get; set; }
        public int quantity { get; set; }
    }
    public class Updated
    {

        public List<Items> items { get; set; }

    }
    public class HomeController : Controller
    {
        public IConfiguration _config;
        public HomeController(IConfiguration config)
        {

            _config = config;

        }

        public IActionResult Index()
        {   
            return View("Update");
        }
        [HttpPost]
        public IActionResult StokGuncelle(UpdateViewModel model)
        {
            var aut=_config["Autorization"];
            var supp = _config["SuplierId"];
            RestSharp.RestClient cli = new RestSharp.RestClient("https://api.trendyol.com/sapigw/suppliers/"+supp+"/products/price-and-inventory");
            RestRequest request = new RestRequest();
            request.Method = Method.Post;
            request.AddHeader("Authorization", "Basic "+aut+"");

            request.AddHeader("Connection", "keep-alive");
            request.AddHeader("User-Agent", ""+supp+"- TrendyolSoft");
            request.AddHeader("Content-Type", "application/json");

            Updated up = new Updated();

            up.items = new List<Items>();
            up.items.Add(new Items() { barcode = model.Barcode,  quantity=model.Quantity });
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(up);

            //string json = @"{
            //     ""items"": [
            //                    {
            //                     ""barcode"": ""4820083908927"",
            //                     ""quantity"": 11
            //                    }
            //                ]
            //                }";


            request.AddJsonBody(json);

            var response = cli.Post(request);


            UpdateViewModel returnModel = new UpdateViewModel();
            returnModel.IsOk = (response.ResponseStatus == ResponseStatus.Completed);
            return View("Update", model);



        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}