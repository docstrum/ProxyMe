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

        InstagramConfig config = new InstagramConfig(clientId, clientSecret, redirectUri, "");

        // GET: Home
        public ActionResult Index()
        {
            var oAuthResponse = Session["InstaSharp.AuthInfo"] as OAuthResponse;

            if (oAuthResponse == null)
            {
                return RedirectToAction("Login");
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

            var users = new Endpoints.Users(config, oAuthResponse);
            var locations = new Endpoints.Media(config, oAuthResponse);

           
            var locFeed = await locations.Search(41.285765, -81.8548987, 5000, System.DateTime.Now - System.TimeSpan.FromDays(1), System.DateTime.Now);
            var feed = await users.Feed(null, null, null);


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