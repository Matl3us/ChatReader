namespace ChatReader.Core.Models.Dto
{
    public class BadgeSetDto
    {
        public class BadgeDto
        {
            public string id { get; set; }
            public string image_url_1x { get; set; }
            public string image_url_2x { get; set; }
            public string image_url_4x { get; set; }
            public string title { get; set; }
            public string description { get; set; }
            public string click_action { get; set; }
            public string click_url { get; set; }
        }

        public string set_id { get; set; }
        public List<BadgeDto> versions { get; set; } = [];
    }
}
