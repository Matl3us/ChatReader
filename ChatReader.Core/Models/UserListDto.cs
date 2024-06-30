namespace ChatReader.Core.Models
{
    public class UserListDto
    {
        public class UserDto
        {
            public string id { get; set; }
            public string login { get; set; }
            public string display_name { get; set; }
        }

        public List<UserDto> data { get; set; } = [];
    }
}
