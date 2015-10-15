using InstaSharp.Models.Responses;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace InstaSharp.Sample.Mvc.Controllers
{
    public class HomeController : Controller
    {
        static string clientId = "f14134ed24754b658b616e1ce855d350";
        static string clientSecret = "1262e676faeb4eb0a3b42928c4fc3147";
        static string redirectUri = "http://localhost:5969/Home/OAuth";
        //static string redirectUri = "http://www.proxme.net/Home/OAuth";

        InstagramConfig config = new InstagramConfig(clientId, clientSecret, redirectUri, "");

        // GET: Home
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
            
            return View(feed.Data);
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

            return View(feed.Data);
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

            var locFeed = await locations.Search(49,-81, 5000, start, end); //41.285765, -81.8548987

            //var goFeed = await geo.Recent()
            ModelState.Clear();
            return View(locFeed.Data);
        }

        [HttpPost]
        public async Task<ActionResult> NearMe2(double latitude, double longitude)
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

            //var goFeed = await geo.Recent()
            

            return View(locFeed.Data);
        }


        public async Task<ActionResult> OAuth(string code)
        {
            // add this code to the auth object
            var auth = new OAuth(config);

            // now we have to call back to instagram and include the code they gave us
            // along with our client secret
            var oauthResponse = await auth.RequestToken(code);

            // both the client secret and the token are considered sensitive data, so we won't be
            // sending them back to the browser. we'll only store them temporarily.  If a user's session times
            // out, they will have to click on the authenticate button again - sorry bout yer luck.
            Session.Add("InstaSharp.AuthInfo", oauthResponse);

            // all done, lets redirect to the home controller which will send some initial data to the app
            return RedirectToAction("Index");
        }
    }
}