using InstaSharp;
using InstaSharp.Models.Responses;

namespace Endpoints
{
    internal class Media
    {
        private InstagramConfig config;
        private OAuthResponse oAuthResponse;

        public Media(InstagramConfig config, OAuthResponse oAuthResponse)
        {
            this.config = config;
            this.oAuthResponse = oAuthResponse;
        }
    }
}