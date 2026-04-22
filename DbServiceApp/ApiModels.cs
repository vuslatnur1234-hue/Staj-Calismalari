using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DbServiceApp
{
    public class GoogleBooksResponse
    {
        [JsonPropertyName("items")]
        public List<BookItem> Items { get; set; }
    }

    public class BookItem
    {
        [JsonPropertyName("volumeInfo")]
        public VolumeInfo VolumeInfo { get; set; }
    }

    public class VolumeInfo
    {
        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("authors")]
        public List<string> Authors { get; set; }

        [JsonPropertyName("categories")]
        public List<string> Categories { get; set; }

        [JsonPropertyName("pageCount")]
        public int? PageCount { get; set; }

        [JsonPropertyName("publishedDate")]
        public string PublishedDate { get; set; }
    }
}