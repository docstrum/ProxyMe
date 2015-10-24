using InstaSharp.Models.Responses;
using InstaSharp.Sample.Mvc.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.Web.Mvc;
using System.Drawing;
using System;
using System.Net;
using System.IO;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

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
            //return View(oAuthResponse.User);
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
                posts.Add(new WallElement()
                {
                    CreatedTime = post.CreatedTime.ToLocalTime(),
                    FullName = post.User.FullName,
                    Id = post.Id,
                    Location = (post.Location != null ? post.Location.Name : null),
                    LocationId = (post.Location != null ? post.Location.Id : 0),
                    Longitude = (post.Location != null ? post.Location.Longitude : 0),
                    Latitude = (post.Location != null ? post.Location.Latitude : 0),
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
            var start = System.DateTime.Now - TimeSpan.FromDays(1);
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
            var start = DateTime.Now - TimeSpan.FromDays(1);
            var end = DateTime.Now;
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
            var start = DateTime.Now - TimeSpan.FromDays(1);
            var end = DateTime.Now;
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
            var start = DateTime.Now - TimeSpan.FromDays(1);
            var end = DateTime.Now;
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

        public async Task<ActionResult> UserFeed(string usercode)
        {
            var oAuthResponse = Session["InstaSharp.AuthInfo"] as OAuthResponse;
            if (oAuthResponse == null)
            {
                return RedirectToAction("Login");
            }
            var users = new Endpoints.Users(config, oAuthResponse);
            var feed = await users.Recent(usercode);
            var posts = new List<WallElement>();
            foreach (var post in feed.Data)
            {
                //var distance = (post.Location != null ? CalculateDistance(latitude, post.Location.Latitude, longitude, post.Location.Longitude) : 0);
                posts.Add(new WallElement()
                {
                    CreatedTime = post.CreatedTime.ToLocalTime(),
                    FullName = post.User.FullName,
                    Id = post.Id,
                    Location = (post.Location != null ? post.Location.Name : null),
                    LocationId = (post.Location != null ? post.Location.Id : 0),
                    Longitude = (post.Location != null ? post.Location.Longitude : 0),
                    Latitude = (post.Location != null ? post.Location.Latitude : 0),
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

        public async Task<ActionResult> FullInfo()
        {
            var oAuthResponse = Session["InstaSharp.AuthInfo"] as OAuthResponse;

            if (oAuthResponse == null)
            {
                return RedirectToAction("Login");
            }
            var locations = new Endpoints.Media(config, oAuthResponse);
            var geo = new Endpoints.Geographies(config, oAuthResponse);
            var start = System.DateTime.Now - TimeSpan.FromDays(1);
            var end = System.DateTime.Now;
            var posts = new List<WallElement>();
            return View(posts);
        }

        [HttpPost]
        public async Task<ActionResult> FullInfo2(double latitude, double longitude)
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
            var InstagramId = oAuthResponse.User.Id.ToString();
            var locations = new Endpoints.Media(config, oAuthResponse);
            var geo = new Endpoints.Geographies(config, oAuthResponse);
            var start = DateTime.Now - TimeSpan.FromDays(1);
            var end = DateTime.Now;
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
                    Username = post.User.Username,
                    TempId = InstagramId + ".jpg"
                });
            }
            posts = posts.OrderBy(x => x.Distance).ToList();
            //var temp = CreateWall(posts, oAuthResponse.User.Id.ToString());
            return View(posts);
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
            var lat1 = Lat1 * Math.PI / 180.0;
            var lon1 = Lon1 * Math.PI / 180.0;
            var lat2 = Lat2 * Math.PI / 180.0;
            var lon2 = Lon2 * Math.PI / 180.0;
            var dlat = lat2 - lat1;
            var dlon = lon2 - lon1;
            var a = Math.Pow(Math.Sin(dlat / 2), 2) + Math.Cos(lat1) * Math.Cos(lat2) * Math.Pow(Math.Sin(dlon / 2), 2);
            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            var d = 3961 * c; //use 6373 for km
            var distance = Math.Round(d * 100) / 100;
            return distance;
        }

        private int imageWidth = 240, imageHeight = 240, maxWidth = 2000, maxHeight = 2000, borderSize = 1;


        private List<WallElement> CreateWall(List<WallElement> model, string userName)
        {
            //context.Response.ContentType = "image/jpg";
            int rows = maxHeight / (imageHeight + 2 * borderSize);
            int cols = maxWidth / (imageWidth + 2 * borderSize);
            int total = rows * cols;
            maxWidth = cols * (imageWidth + 2 * borderSize);
            maxHeight = rows * (imageHeight + 2 * borderSize);
            using (Bitmap img = new Bitmap(maxWidth, maxHeight))
            {
                List<String> listImg = GetProfileImages(model, total);
                //In case no sufficient images, repeat images 
                total = listImg.Count;
                int row, col, x = 0, y = 0;
                using (Graphics g = Graphics.FromImage(img))
                {
                    using (Pen pen = new Pen(Color.White, borderSize))
                    {
                        for (row = 0; row < rows; row++)
                        {
                            for (col = 0; col < cols; col++)
                            {
                                x = col * (imageWidth + borderSize * 2);
                                y = row * (imageHeight + borderSize * 2);
                                WebRequest req = WebRequest.Create(listImg[(row * cols + col) % total]);
                                WebResponse response = req.GetResponse();
                                Stream stream = response.GetResponseStream();
                                Image webImg = Image.FromStream(stream);
                                stream.Close();
                                g.DrawImage(webImg, x + borderSize, y + borderSize, imageWidth, imageHeight);
                                //pen.Alignment = System.Drawing.Drawing2D.PenAlignment.Inset;
                                g.DrawRectangle(pen, x, y, imageWidth + 2 * borderSize - 1, imageHeight + 2 * borderSize - 1);

                            }
                        }
                    }
                    //Gradient effect
                    if (cols > 3)
                    {
                        // Create back box brush
                        Rectangle rect = new Rectangle(0, 0, imageWidth, maxHeight);
                        //left side gradient
                        using (LinearGradientBrush lgBrush = new LinearGradientBrush(rect, Color.White, Color.Transparent, LinearGradientMode.Horizontal))
                        {
                            g.FillRectangle(lgBrush, rect);
                        }
                        //Right side gradient
                        rect = new Rectangle(maxWidth - imageWidth, 0, imageWidth, maxHeight);
                        using (LinearGradientBrush lgBrush = new LinearGradientBrush(rect, Color.Transparent, Color.White, LinearGradientMode.Horizontal))
                        {
                            g.FillRectangle(lgBrush, rect);
                        }
                    }
                    //Gradient effect
                    if (rows > 3)
                    {
                        // Create back box brush
                        Rectangle rect = new Rectangle(0, 0, maxWidth, imageHeight);
                        //top gradient
                        using (LinearGradientBrush lgBrush = new LinearGradientBrush(rect, Color.White, Color.Transparent, LinearGradientMode.Vertical))
                        {
                            g.FillRectangle(lgBrush, rect);
                        }
                        //bottom gradient
                        rect = new Rectangle(0, maxHeight - imageHeight, maxWidth, imageHeight);
                        using (LinearGradientBrush lgBrush = new LinearGradientBrush(rect, Color.Transparent, Color.White, LinearGradientMode.Vertical))
                        {
                            g.FillRectangle(lgBrush, rect);
                        }
                    }
                }
                img.Save(Server.MapPath("~/Images/temp/" + userName + ".jpg"), ImageFormat.Jpeg);
            }
            return model;
        }

        private List<String> GetProfileImages(List<WallElement> record, int max)
        {
            List<String> listImg;
            listImg = record
                .OrderBy(x => Guid.NewGuid())
                .Take(max)
                .Select(x => new { x.StandardResolutionUrl }).ToList()
                .Select(x => x.StandardResolutionUrl).ToList();
            return listImg;

        }
    }
}