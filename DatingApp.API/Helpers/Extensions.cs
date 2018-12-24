using Microsoft.AspNetCore.Http;

namespace DatingApp.API.Helpers {
    public static class Extensions {
        // extension method per aggiungere gli header CORS nella response in caso di errore
        public static void AddApplicationError(this HttpResponse response, string message) {
            response.Headers.Add("Application-Error", message);
            response.Headers.Add("Access-Control-Expose-Headers", "Application-Error");
            response.Headers.Add("Access-Control-Allow-Origin", "*");
        }
    }
}