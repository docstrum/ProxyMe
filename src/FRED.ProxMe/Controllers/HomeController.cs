﻿using InstaSharp.Models.Responses;
using FRED.Proxme.Mvc.Models;
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
using InstaSharp;

namespace FRED.Proxme.Mvc.Controllers
{
    public class HomeController : Controller
    {
        static string clientId = "f14134ed24754b658b616e1ce855d350";
        static string clientSecret = "1262e676faeb4eb0a3b42928c4fc3147";
        static string redirectUri = "http://localhost:5969/Home/OAuth";
        //static string redirectUri = "http://www.proxme.net/Home/OAuth";

        InstagramConfig config = new InstagramConfig(clientId, clientSecret, redirectUri, "");

        public ActionResult Index()
        {
            var oAuthResponse = Session["FRED.AuthInfo"] as OAuthResponse;
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
            var oAuthResponse = Session["FRED.AuthInfo"] as OAuthResponse;
            if (oAuthResponse == null)
            {
                return RedirectToAction("Login");
            }
            var users = new InstaSharp.Endpoints.Users(config, oAuthResponse);
            var feed = await users.Feed(null, null, null);
            var posts = new List<WallElement>();
            foreach (var post in feed.Data)
            {
                posts.Add(InstsharpMedia2WallElement(post, 0, 0));
            }
            posts = posts.OrderBy(x => x.Distance).ToList();
            return View(posts);
        }

        public async Task<ActionResult> MyPosts()
        {
            var oAuthResponse = Session["FRED.AuthInfo"] as OAuthResponse;
            if (oAuthResponse == null)
            {
                return RedirectToAction("Login");
            }
            var users = new InstaSharp.Endpoints.Users(config, oAuthResponse);
            var feed = await users.RecentSelf();
            var posts = new List<WallElement>();
            foreach (var post in feed.Data)
            {
                posts.Add(InstsharpMedia2WallElement(post, 0, 0));
            }
            posts = posts.OrderBy(x => x.Distance).ToList();
            return View(posts);
        }

        public async Task<ActionResult> NearMe()
        {
            var oAuthResponse = Session["FRED.AuthInfo"] as OAuthResponse;

            if (oAuthResponse == null)
            {
                return RedirectToAction("Login");
            }
            var locations = new InstaSharp.Endpoints.Media(config, oAuthResponse);
            var geo = new InstaSharp.Endpoints.Geographies(config, oAuthResponse);
            var start = DateTime.Now - TimeSpan.FromDays(1);
            var end = DateTime.Now;
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
            var oAuthResponse = Session["FRED.AuthInfo"] as OAuthResponse;
            if (oAuthResponse == null)
            {
                return RedirectToAction("Login");
            }
            var locations = new InstaSharp.Endpoints.Media(config, oAuthResponse);
            var geo = new InstaSharp.Endpoints.Geographies(config, oAuthResponse);
            var start = DateTime.Now - TimeSpan.FromDays(1);
            var end = DateTime.Now;
            var locFeed = await locations.Search(latitude, longitude, 5000, start, end);
            var posts = new List<WallElement>();
            foreach (var post in locFeed.Data)
            {
                posts.Add(InstsharpMedia2WallElement(post, latitude, longitude));
            }
            posts = posts.OrderBy(x => x.Distance).ToList();
            return View(posts);
        }

        public async Task<ActionResult> WhoIsNear()
        {
            var oAuthResponse = Session["FRED.AuthInfo"] as OAuthResponse;
            if (oAuthResponse == null)
            {
                return RedirectToAction("Login");
            }
            var locations = new InstaSharp.Endpoints.Media(config, oAuthResponse);
            var geo = new InstaSharp.Endpoints.Geographies(config, oAuthResponse);
            var start = DateTime.Now - TimeSpan.FromDays(1);
            var end = DateTime.Now;
            var posts = new List<WallElement>();
            return View(posts);
        }

        [HttpPost]
        public async Task<ActionResult> WhoIsNear2(double latitude, double longitude)
        {
            ModelState.Clear();
            var oAuthResponse = Session["FRED.AuthInfo"] as OAuthResponse;
            if (oAuthResponse == null)
            {
                return RedirectToAction("Login");
            }
            var locations = new InstaSharp.Endpoints.Media(config, oAuthResponse);
            var geo = new InstaSharp.Endpoints.Geographies(config, oAuthResponse);
            var start = DateTime.Now - TimeSpan.FromDays(1);
            var end = DateTime.Now;
            var locFeed = await locations.Search(latitude, longitude, 5000, start, end);
            var posts = new List<WallElement>();
            foreach (var post in locFeed.Data)
            {
                posts.Add(InstsharpMedia2WallElement(post, latitude, longitude));
            }
            posts = posts.OrderBy(x => x.Distance).ToList();
            var compare = new WallElementComparer();
            return View(posts.Distinct(compare));
        }

        public async Task<ActionResult> UserFeed(string usercode)
        {
            var oAuthResponse = Session["FRED.AuthInfo"] as OAuthResponse;
            if (oAuthResponse == null)
            {
                return RedirectToAction("Login");
            }
            var users = new InstaSharp.Endpoints.Users(config, oAuthResponse);
            var feed = await users.Recent(usercode);
            var posts = new List<WallElement>();
            foreach (var post in feed.Data)
            {
                posts.Add(InstsharpMedia2WallElement(post, 0, 0));
            }
            posts = posts.OrderBy(x => x.Distance).ToList();
            return View(posts);
        }

        public async Task<ActionResult> FullInfo()
        {
            var oAuthResponse = Session["FRED.AuthInfo"] as OAuthResponse;

            if (oAuthResponse == null)
            {
                return RedirectToAction("Login");
            }
            var locations = new Endpoints.Media(config, oAuthResponse);
            var geo = new InstaSharp.Endpoints.Geographies(config, oAuthResponse);
            var start = DateTime.Now - TimeSpan.FromDays(1);
            var end = DateTime.Now;
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
            var oAuthResponse = Session["FRED.AuthInfo"] as OAuthResponse;
            if (oAuthResponse == null)
            {
                return RedirectToAction("Login");
            }
            var InstagramId = oAuthResponse.User.Id.ToString();
            var locations = new InstaSharp.Endpoints.Media(config, oAuthResponse);
            var geo = new InstaSharp.Endpoints.Geographies(config, oAuthResponse);
            var start = DateTime.Now - TimeSpan.FromDays(1);
            var end = DateTime.Now;
            var locFeed = await locations.Search(latitude, longitude, 5000, start, end);
            var posts = new List<WallElement>();
            foreach (var post in locFeed.Data)
            {
                posts.Add(InstsharpMedia2WallElement(post, latitude, longitude));
            }
            posts = posts.OrderBy(x => x.Distance).ToList();
            //var temp = CreateWall(posts, oAuthResponse.User.Id.ToString());
            return View(posts);
        }

        public async Task<ActionResult> OAuth(string code)
        {
            var auth = new OAuth(config);
            var oauthResponse = await auth.RequestToken(code);
            Session.Add("FRED.AuthInfo", oauthResponse);
            return RedirectToAction("Index");
        }

        public WallElement InstsharpMedia2WallElement(InstaSharp.Models.Media post, double latitude, double longitude)
        {
            var wall = new WallElement
            {
                CreatedTime = post.CreatedTime.ToLocalTime(),
                FullName = post.User.FullName,
                Id = post.Id,
                Location = (post.Location != null ? post.Location.Name : null),
                LocationId = (post.Location != null ? post.Location.Id : 0),
                Longitude = (post.Location != null ? post.Location.Longitude : 0),
                Latitude = (post.Location != null ? post.Location.Latitude : 0),
                ProfilePictureUrl = post.User.ProfilePicture,
                Distance = 0,
                StandardResolutionUrl = post.Images.StandardResolution.Url,
                PhotoHeight = post.Images.StandardResolution.Height,
                PhotoWidth = post.Images.StandardResolution.Width,
                LowResoltionUrl = post.Images.LowResolution.Url,
                ThumbnailUrl = post.Images.Thumbnail.Url,
                Username = post.User.Username,
                VideoUrl = (post.Videos != null ? post.Videos.StandardResolution.Url : ""),
                VideoHeight = (post.Videos != null ? post.Videos.StandardResolution.Height : 0),
                VideoWidth = (post.Videos != null ? post.Videos.StandardResolution.Width : 0)
            };
            var distance = (post.Location != null && latitude !=0 ? CalculateDistance(latitude, post.Location.Latitude, longitude, post.Location.Longitude) : 0);
            if (distance > 0)
            {
                wall.Distance = distance;
                wall.Bearing = (post.Location != null && latitude != 0 ? CalculateBearing(latitude, post.Location.Latitude, longitude, post.Location.Longitude) : 0);
                if (wall.Bearing >= 337.5 || wall.Bearing <= 22.5)
                    wall.Direction = "N";
                else if (wall.Bearing > 22.5 && wall.Bearing < 67.5)
                    wall.Direction = "NE";
                else if (wall.Bearing >= 67.5 && wall.Bearing <= 112.5)
                    wall.Direction = "E";
                else if (wall.Bearing > 112.5 && wall.Bearing < 157.5)
                    wall.Direction = "SE";
                else if (wall.Bearing >= 157.5 && wall.Bearing <= 202.5)
                    wall.Direction = "S";
                else if (wall.Bearing > 202.5 && wall.Bearing < 247.5)
                    wall.Direction = "SW";
                else if (wall.Bearing >= 247.5 && wall.Bearing <= 292.5)
                    wall.Direction = "W";
                else if (wall.Bearing > 292.5 && wall.Bearing < 337.5)
                    wall.Direction = "NE";
            }
            return wall;
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

        public double CalculateBearing(double Lat1, double Lat2, double Lon1, double Lon2)
        {
            var lat1 = Lat1 * Math.PI / 180.0;
            var lat2 = Lat2 * Math.PI / 180.0;
            var a = lat1;
            var b = lat2;
            var c = (Lon2 - Lon1) * Math.PI / 180.0;   
            var y = Math.Sin(c) * Math.Cos(b);
            var x = Math.Cos(a) * Math.Sin(b) - Math.Sin(a) * Math.Cos(b) * Math.Cos(c);
            var θ = Math.Atan2(y, x);
            var bearing = ((θ * 180) / Math.PI) % 360;
            return Math.Abs(bearing);
        }

        private int imageWidth = 428, imageHeight = 428, maxWidth = 3000, maxHeight = 3000, borderSize = 1;


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
                        Rectangle rect = new Rectangle(0, 0, imageWidth/2, maxHeight);
                        //left side gradient
                        using (LinearGradientBrush lgBrush = new LinearGradientBrush(rect, Color.White, Color.Transparent, LinearGradientMode.Horizontal))
                        {
                            g.FillRectangle(lgBrush, rect);
                        }
                        //Right side gradient
                        rect = new Rectangle(maxWidth - (imageWidth/2), 0, imageWidth/2, maxHeight);
                        using (LinearGradientBrush lgBrush = new LinearGradientBrush(rect, Color.Transparent, Color.White, LinearGradientMode.Horizontal))
                        {
                            g.FillRectangle(lgBrush, rect);
                        }
                    }
                    //Gradient effect
                    if (rows > 3)
                    {
                        // Create back box brush
                        Rectangle rect = new Rectangle(0, 0, maxWidth, imageHeight/2);
                        //top gradient
                        using (LinearGradientBrush lgBrush = new LinearGradientBrush(rect, Color.White, Color.Transparent, LinearGradientMode.Vertical))
                        {
                            g.FillRectangle(lgBrush, rect);
                        }
                        //bottom gradient
                        rect = new Rectangle(0, maxHeight - (imageHeight/2), maxWidth, imageHeight/2);
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