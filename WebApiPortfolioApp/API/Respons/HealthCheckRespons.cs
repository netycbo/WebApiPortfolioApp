namespace WebApiPortfolioApp.API.Respons
{
    public class HealthCheckRespons
    {
        public string ApiHealthStatus { get; set; }
        public string DatabaseHealthStatus { get; set; }
        public string ApiDescription { get; set; }
        public string DatabaseDescription { get; set; }
    }
}
