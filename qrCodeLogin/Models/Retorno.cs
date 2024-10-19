namespace qrCodeLogin.Models
{
    public class Retorno<T>
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public int? Code{ get; set; }
        public List<T> Data { get; set; } = new List<T>();
    }
}
