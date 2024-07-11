namespace ChatReader.Core.Models.Dto
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
