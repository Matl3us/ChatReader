using Microsoft.AspNetCore.Mvc;

namespace ChatReader.Controllers;

public class HomeController : Controller
{
    public string Index()
    {
        return "Welcome to chat reader app";
    }
}