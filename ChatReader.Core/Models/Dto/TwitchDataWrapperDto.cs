namespace ChatReader.Core.Models.Dto
{
    public class TwitchDataWrapperDto<T>
    {
        public List<T> data { get; set; } = [];

        public bool IsEmpty() => data.Count == 0;

        public T? GetFirst() => data.FirstOrDefault();

        public List<T> GetAll() => data;
    }
}
