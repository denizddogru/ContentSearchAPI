using Microsoft.AspNetCore.Mvc;

namespace ContentSearchAPI.Controllers;

[ApiController]
[Route("mock")]
public class MockProvidersController : ControllerBase
{
    /// <summary>
    /// Mock JSON provider endpoint for testing
    /// </summary>
    [HttpGet("provider1/content")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult GetJsonContent()
    {
        var mockData = new[]
        {
            new
            {
                title = "Introduction to .NET 8",
                description = "Learn the new features in .NET 8",
                type = "Video",
                url = "https://example.com/video1",
                views = (int?)15000,
                likes = (int?)1200,
                readingTime = (int?)null,
                reactions = (int?)null,
                createdDate = DateTime.UtcNow.AddDays(-5)
            },
            new
            {
                title = "Advanced C# Patterns",
                description = "Deep dive into design patterns",
                type = "Text",
                url = "https://example.com/article1",
                views = (int?)null,
                likes = (int?)null,
                readingTime = (int?)15,
                reactions = (int?)450,
                createdDate = DateTime.UtcNow.AddDays(-10)
            }
        };

        return Ok(mockData);
    }

    /// <summary>
    /// Mock XML provider endpoint for testing
    /// </summary>
    [HttpGet("provider2/data")]
    [Produces("application/xml")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult GetXmlContent()
    {
        var xml = @"<?xml version=""1.0"" encoding=""UTF-8""?>
<items>
    <item>
        <title>Microservices Architecture</title>
        <description>Building scalable microservices</description>
        <type>Video</type>
        <url>https://example.com/video2</url>
        <views>25000</views>
        <likes>2100</likes>
        <createdDate>" + DateTime.UtcNow.AddDays(-3).ToString("o") + @"</createdDate>
    </item>
    <item>
        <title>Clean Code Principles</title>
        <description>Writing maintainable code</description>
        <type>Text</type>
        <url>https://example.com/article2</url>
        <readingTime>20</readingTime>
        <reactions>680</reactions>
        <createdDate>" + DateTime.UtcNow.AddDays(-7).ToString("o") + @"</createdDate>
    </item>
</items>";

        return Content(xml, "application/xml");
    }
}
