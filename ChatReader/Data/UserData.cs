using ChatReader.Dto;

namespace ChatReader.Data;

public class UserData
{
    private UserAuthDto? _user;

    public void SaveUser(UserAuthDto user)
    {
        _user = user;
    }

    public UserAuthDto? GetUser()
    {
        return _user;
    }
}