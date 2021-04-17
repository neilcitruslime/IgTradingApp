using IgTrading.Ig.Models;

namespace IgTrading.Ig
{
    public class IgConfirms
    {
        public string GetConfirms(IgSessionModel igSession, string dealReference)
        {
            string action = $"/confirms/{dealReference}";

            IgHttpClient igHttpClient = new IgHttpClient();

            return igHttpClient.Get(igSession, action, 1); ;
        }

    }
}