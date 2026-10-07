using Supabase;

namespace Sabires_Backend_Application.Services
{
    public class SupabaseClienteService
    {
        public Client Client { get; }
        public Client AdminClient { get; }

        public SupabaseClienteService(IConfiguration Configuration)
        {
            var url = Configuration["Supabase:Url"];
            var key = Configuration["Supabase:PublishableKey"];
            var secretKey = Configuration["Supabase:SecretKey"];

            if (string.IsNullOrEmpty(url))
                throw new Exception("Supabase URL não configurada.");

            if (string.IsNullOrEmpty(key))
                throw new Exception("Supabase Publishable Key não configurada.");

            if (string.IsNullOrEmpty(secretKey))
                throw new Exception("Supabase Secret Key não configurada.");

            var options = new SupabaseOptions
            {
                AutoConnectRealtime = true
            };

            Client = new Client(url, key, options);
            AdminClient = new Client(url, secretKey, options);
        }
    }
}
