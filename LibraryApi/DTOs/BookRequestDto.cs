namespace LibraryApi.DTOs
{
    public class BookRequestDto
    {
        public string KitapAd { get; set; }
        public int YazarID { get; set; }
        public int TurID { get; set; }
        public int SayfaSayisi { get; set; }
        public string BasimYili { get; set; }
        public int StokAdeti { get; set; }
        public string RafNo { get; set; }
    }
}