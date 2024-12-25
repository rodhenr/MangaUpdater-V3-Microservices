using Microsoft.AspNetCore.Mvc;
using RabbitMQ.Client;

namespace MangaUpdater.Services.Fetcher.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BaseController : Controller
{ }