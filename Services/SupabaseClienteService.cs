using Supabase;

namespace Sabires_Backend_Application.Services
{
    public class SupabaseClienteService
    {
        public Client Client { get; }

        public SupabaseClienteService(IConfiguration Configuration)
        {
            var url = Configuration["Supabase:Url"];
            var key = Configuration["Supabase:PublishableKey"];

            if (string.IsNullOrEmpty(url))
                throw new Exception("Supabase URL não configurada.");


            if (string.IsNullOrEmpty(key))
                throw new Exception("Supabase Publishable Key não configurada.");

            var options = new SupabaseOptions
            {
                AutoConnectRealtime = true
            };

            var supabase = new Client(url, key, options);
        }
    }
}
