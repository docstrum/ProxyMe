using InstaSharp.Models.Responses;
using InstaSharp.Sample.Mvc.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.Web.Mvc;

namespace InstaSharp.Sample.Mvc.Controllers
{
    public class HomeController : Controller
    {
        static string clientId = "f14134ed24754b658b616e1ce855d350";
        static string clientSecret = "1262e676faeb4eb0a3b42928c4fc3147";
        //static string redirectUri = "http://localhost:5969/Home/OAuth";
        static string redirectUri = "http://www.proxme.net/Home/OAuth";

        InstagramConfig config = new InstagramConfig(clientId, clientSecret, redirectUri, "");

        public ActionResult Index()
        {
            var oAuthResponse = Session["InstaSharp.AuthInfo"] as OAuthResponse;
            if (oAuthResponse == null)
            {
                return RedirectToAction("Login");
            }
            else
            {
                return RedirectToAction("NearMe");
            }
            return View(oAuthResponse.User);
        }

        public ActionResult Login()
        {
            var scopes = new List<OAuth.Scope>();
            scopes.Add(InstaSharp.OAuth.Scope.Likes);
            scopes.Add(InstaSharp.OAuth.Scope.Comments);
            var link = InstaSharp.OAuth.AuthLink(config.OAuthUri + "authorize", config.ClientId, config.RedirectUri, scopes, InstaSharp.OAuth.ResponseType.Code);
            return Redirect(link);
        }

        public async Task<ActionResult> MyFeed()
        {
            var oAuthResponse = Session["InstaSharp.AuthInfo"] as OAuthResponse;
            if (oAuthResponse == null)
            {
                return RedirectToAction("Login");
            }
            var users = new Endpoints.Users(config, oAuthResponse);
            var feed = await users.Feed(null, null, null);
            var posts = new List<WallElement>();
            foreach (var post in feed.Data)
            {
                //var distance = CalculateDistance(latitude, post.Location.Latitude, longitude, post.Location.Longitude);
                posts.Add(new WallElement()
                {
                    CreatedTime = post.CreatedTime.ToLocalTime(),
                    FullName = post.User.FullName,
                    Id = post.Id,
                    Location = (post.Location != null ? post.Location.Name : null),
                    LocationId = (post.Location != null ? post.Location.Id : 0),
                    Longitude = 0, //post.Location.Longitude,
                    Latitude = 0, //post.Location.Latitude,
                    ProfilePictureUrl = post.User.ProfilePicture,
                    Distance = 0, //distance,
                    StandardResolutionUrl = post.Images.StandardResolution.Url,
                    LowResoltionUrl = post.Images.LowResolution.Url,
                    ThumbnailUrl = post.Images.Thumbnail.Url,
                    Username = post.User.Username,
                    VideoUrl = (post.Videos != null? post.Videos.StandardResolution.Url: "")
                });
            }
            posts = posts.OrderBy(x => x.Distance).ToList();
            return View(posts);
        }

        public async Task<ActionResult> MyPosts()
        {
            var oAuthResponse = Session["InstaSharp.AuthInfo"] as OAuthResponse;
            if (oAuthResponse == null)
            {
                return RedirectToAction("Login");
            }
            var users = new Endpoints.Users(config, oAuthResponse);
            var feed = await users.RecentSelf();
            var posts = new List<WallElement>();
            foreach (var post in feed.Data)
            {
                //var distance = CalculateDistance(latitude, post.Location.Latitude, longitude, post.Location.Longitude);
                posts.Add(new WallElement()
                {
                    CreatedTime = post.CreatedTime.ToLocalTime(),
                    FullName = post.User.FullName,
                    Id = post.Id,
                    Location = (post.Location != null ? post.Location.Name : null),
                    LocationId = (post.Location != null ? post.Location.Id : 0),
                    Longitude = 0, //post.Location.Longitude,
                    Latitude = 0, //post.Location.Latitude,
                    ProfilePictureUrl = post.User.ProfilePicture,
                    Distance = 0, //distance,
                    StandardResolutionUrl = post.Images.StandardResolution.Url,
                    LowResoltionUrl = post.Images.LowResolution.Url,
                    ThumbnailUrl = post.Images.Thumbnail.Url,
                    Username = post.User.Username,
                    VideoUrl = (post.Videos != null ? post.Videos.StandardResolution.Url : "")
                });
            }
            posts = posts.OrderBy(x => x.Distance).ToList();
            return View(posts);
        }

        public async Task<ActionResult> NearMe()
        {
            var oAuthResponse = Session["InstaSharp.AuthInfo"] as OAuthResponse;

            if (oAuthResponse == null)
            {
                return RedirectToAction("Login");
            }
            var locations = new Endpoints.Media(config, oAuthResponse);
            var geo = new Endpoints.Geographies(config, oAuthResponse);
            var start = System.DateTime.Now - System.TimeSpan.FromDays(1);
            var end = System.DateTime.Now;
            var posts = new List<WallElement>();
            return View(posts);
        }

        [HttpPost]
        public async Task<ActionResult> NearMe2(double latitude, double longitude)
        {
            if (latitude == 0)
            {
                return View();
            }
            ModelState.Clear();
            var oAuthResponse = Session["InstaSharp.AuthInfo"] as OAuthResponse;
            if (oAuthResponse == null)
            {
                return RedirectToAction("Login");
            }
            var locations = new Endpoints.Media(config, oAuthResponse);
            var geo = new Endpoints.Geographies(config, oAuthResponse);
            var start = System.DateTime.Now - System.TimeSpan.FromDays(1);
            var end = System.DateTime.Now;
            var locFeed = await locations.Search(latitude, longitude, 5000, start, end);
            var posts = new List<WallElement>();
            foreach (var post in locFeed.Data)
            {
                var distance = CalculateDistance(latitude, post.Location.Latitude, longitude, post.Location.Longitude);
                posts.Add(new WallElement()
                {
                    CreatedTime = post.CreatedTime.ToLocalTime(),
                    FullName = post.User.FullName,
                    Id = post.Id,
                    Location = post.Location.Name,
                    LocationId = post.Location.Id,
                    Longitude = post.Location.Longitude,
                    Latitude = post.Location.Latitude,
                    ProfilePictureUrl = post.User.ProfilePicture,
                    Distance = distance,
                    StandardResolutionUrl = post.Images.StandardResolution.Url,
                    LowResoltionUrl = post.Images.LowResolution.Url,
                    ThumbnailUrl = post.Images.Thumbnail.Url,
                    Username = post.User.Username
                });
            }
            posts = posts.OrderBy(x => x.Distance).ToList();
            return View(posts);
        }

        public async Task<ActionResult> WhoIsNear()
        {
            var oAuthResponse = Session["InstaSharp.AuthInfo"] as OAuthResponse;
            if (oAuthResponse == null)
            {
                return RedirectToAction("Login");
            }
            var locations = new Endpoints.Media(config, oAuthResponse);
            var geo = new Endpoints.Geographies(config, oAuthResponse);
            var start = System.DateTime.Now - System.TimeSpan.FromDays(1);
            var end = System.DateTime.Now;
            var posts = new List<WallElement>();
            return View(posts);
        }

        [HttpPost]
        public async Task<ActionResult> WhoIsNear2(double latitude, double longitude)
        {
            ModelState.Clear();
            var oAuthResponse = Session["InstaSharp.AuthInfo"] as OAuthResponse;
            if (oAuthResponse == null)
            {
                return RedirectToAction("Login");
            }
            var locations = new Endpoints.Media(config, oAuthResponse);
            var geo = new Endpoints.Geographies(config, oAuthResponse);
            var start = System.DateTime.Now - System.TimeSpan.FromDays(1);
            var end = System.DateTime.Now;
            var locFeed = await locations.Search(latitude, longitude, 5000, start, end);
            var posts = new List<WallElement>();
            foreach (var post in locFeed.Data)
            {
                var distance = CalculateDistance(latitude, post.Location.Latitude, longitude, post.Location.Longitude);
                posts.Add(new WallElement()
                {
                    CreatedTime = post.CreatedTime.ToLocalTime(),
                    FullName = post.User.FullName,
                    Id = post.Id,
                    Location = post.Location.Name,
                    LocationId = post.Location.Id,
                    Longitude = post.Location.Longitude,
                    Latitude = post.Location.Latitude,
                    ProfilePictureUrl = post.User.ProfilePicture,
                    Distance = distance,
                    StandardResolutionUrl = post.Images.StandardResolution.Url,
                    LowResoltionUrl = post.Images.LowResolution.Url,
                    ThumbnailUrl = post.Images.Thumbnail.Url,
                    Username = post.User.Username
                });
            }
            posts = posts.OrderBy(x => x.Distance).ToList();
            var compare = new WallElementComparer();
            return View(posts.Distinct(compare));
        }

        public async Task<ActionResult> OAuth(string code)
        {
            var auth = new OAuth(config);
            var oauthResponse = await auth.RequestToken(code);
            Session.Add("InstaSharp.AuthInfo", oauthResponse);
            return RedirectToAction("Index");
        }

        public double CalculateDistance(double Lat1, double Lat2, double Lon1, double Lon2)
        {
            var lat1 = Lat1 * System.Math.PI / 180.0;
            var lon1 = Lon1 * System.Math.PI / 180.0;
            var lat2 = Lat2 * System.Math.PI / 180.0;
            var lon2 = Lon2 * System.Math.PI / 180.0;
            var dlat = lat2 - lat1;
            var dlon = lon2 - lon1;
            var a = System.Math.Pow(System.Math.Sin(dlat / 2), 2) + System.Math.Cos(lat1) * System.Math.Cos(lat2) * System.Math.Pow(System.Math.Sin(dlon / 2), 2);
            var c = 2 * System.Math.Atan2(System.Math.Sqrt(a), System.Math.Sqrt(1 - a));
            var d = 3961 * c; //use 6373 for km
            var distance = System.Math.Round(d * 100) / 100;
            return distance; 
        }
    }
}