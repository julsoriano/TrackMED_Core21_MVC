// N O T   U S E D 

namespace TrackMED.Services
{
    public static class Util
    {
        public static string getRootUri()
        {
            // For IIS Express, use localhost:5000 
            // var uri = "http://localhost:5000/";
            // Get the root URI from Web.config
            var uri = Configuration.PropertyServiceURI;
            
            return uri;
        }

        public static string getServiceUri(string srv)
        {
            return getRootUri() + "api/" + srv;
            //return "api/" + srv;
        }
    }

    public static class Configuration
    {
        private static string _uri = null;

        public static string PropertyServiceURI
        {
            get
            {
                if (!string.IsNullOrEmpty(_uri))
                    return _uri;

                _uri = getKeyVal("TrackMEDApi");
                if (string.IsNullOrEmpty(_uri))
                    return "https://trackmedapi.azurewebsites.net/";
                else
                    return _uri;
            }
        }
        
        public static string getKeyVal(string key)
        {
            return "TrackMEDApi";
        }
        
    }
}