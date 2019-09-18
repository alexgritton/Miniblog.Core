using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Miniblog.Core.Services;
using WebEssentials.AspNetCore.Pwa;

namespace Miniblog.Core.Controllers
{
    public class RobotsController : Controller
    {
        private readonly IBlogService _blog;
        private readonly IOptionsSnapshot<BlogSettings> _settings;
        private readonly WebManifest _manifest;

        public RobotsController(IBlogService blog, IOptionsSnapshot<BlogSettings> settings, WebManifest manifest)
        {
            _blog = blog;
            _settings = settings;
            _manifest = manifest;
        }

        [Route("/robots.txt")]
        [OutputCache(Profile = "default")]
        public string RobotsTxt()
        {
            string host = Request.Scheme + "://" + Request.Host;
            var sb = new StringBuilder();
            sb.AppendLine("User-agent: *");
            sb.AppendLine("Disallow:");
            sb.AppendLine($"sitemap: {host}/sitemap.xml");

            return sb.ToString();
        }

        [Route("/sitemap.xml")]
        public async Task SitemapXml()
        {
            string host = Request.Scheme + "://" + Request.Host;

            Response.ContentType = "application/xml";

            using (var xml = XmlWriter.Create(Response.Body, new XmlWriterSettings { Indent = true }))
            {
                xml.WriteStartDocument();
                xml.WriteStartElement("urlset", "http://www.sitemaps.org/schemas/sitemap/0.9");

                var posts = await _blog.GetPosts(int.MaxValue);

                foreach (Models.Post post in posts)
                {
                    var lastMod = new[] { post.PubDate, post.LastModified };

                    xml.WriteStartElement("url");
                    xml.WriteElementString("loc", host + post.GetLink());
                    xml.WriteElementString("lastmod", lastMod.Max().ToString("yyyy-MM-ddThh:mmzzz"));
                    xml.WriteEndElement();
                }

                xml.WriteEndElement();
            }
        }

        [Route("/rsd.xml")]
        public void RsdXml()
        {
            string host = Request.Scheme + "://" + Request.Host;

            Response.ContentType = "application/xml";
            Response.Headers["cache-control"] = "no-cache, no-store, must-revalidate";

            using (var xml = XmlWriter.Create(Response.Body, new XmlWriterSettings { Indent = true }))
            {
                xml.WriteStartDocument();
                xml.WriteStartElement("rsd");
                xml.WriteAttributeString("version", "1.0");

                xml.WriteStartElement("service");

                xml.WriteElementString("enginename", "Miniblog.Core");
                xml.WriteElementString("enginelink", "http://github.com/madskristensen/Miniblog.Core/");
                xml.WriteElementString("homepagelink", host);

                xml.WriteStartElement("apis");
                xml.WriteStartElement("api");
                xml.WriteAttributeString("name", "MetaWeblog");
                xml.WriteAttributeString("preferred", "true");
                xml.WriteAttributeString("apilink", host + "/metaweblog");
                xml.WriteAttributeString("blogid", "1");

                xml.WriteEndElement(); // api
                xml.WriteEndElement(); // apis
                xml.WriteEndElement(); // service
                xml.WriteEndElement(); // rsd
            }
        }

        [Route("/feed/{type}")]
        public async Task Rss(string type)
        {
            Response.ContentType = "application/xml";
            string host = Request.Scheme + "://" + Request.Host;

            using (XmlWriter xmlWriter = XmlWriter.Create(Response.Body, new XmlWriterSettings() { Async = true, Indent = true, Encoding = new UTF8Encoding(false) }))
            {
                var posts = await _blog.GetPosts(10);
                // var writer = await GetWriter(type, xmlWriter, posts.Max(p => p.PubDate));
                
                List<SyndicationItem> items = new List<SyndicationItem>();
                foreach (Models.Post post in posts)
                {
                    var item = new SyndicationItem(post.Title, new TextSyndicationContent(post.Content, TextSyndicationContentKind.Html), new Uri(host+post.GetLink()), host+post.GetLink(), post.LastModified);
                    item.PublishDate = post.PubDate;

                    foreach (string category in post.Categories)
                    {                        
                        item.Categories.Add(new SyndicationCategory(category));
                    }

                    
                    item.Contributors.Add(new SyndicationPerson("test@example.com", "Alex Gritton", host));
                    item.Links.Add(new SyndicationLink(new Uri(item.Id)));

                    items.Add(item);
                    // await writer.Write(item);
                }
                var feed = new SyndicationFeed(_manifest.Name, _manifest.Description, new Uri(host), "Miniblog.Core", posts.Max(p=>p.PubDate), items);
                if(type.Equals("rss", StringComparison.OrdinalIgnoreCase)){
                    var formatter = new Rss20FeedFormatter(feed, false);
                    formatter.WriteTo(xmlWriter);
                    
                }
                else{
                    var formatter = new Atom10FeedFormatter(feed);
                    formatter.WriteTo(xmlWriter);
                }
                
            }
        }

        // private async Task<SyndicationFeedFormatter> GetWriter(string type, XmlWriter xmlWriter, DateTime updated)
        // {
        //     string host = Request.Scheme + "://" + Request.Host + "/";

        //     if (type.Equals("rss", StringComparison.OrdinalIgnoreCase))
        //     {
        //         var rss = new Rss20FeedFormatter();
        //         await rss.WriteTitle(_manifest.Name);
        //         await rss.WriteDescription(_manifest.Description);
        //         await rss.WriteGenerator("Miniblog.Core");
        //         await rss.WriteValue("link", host);
        //         return rss;
        //     }

        //     var atom = new Atom10FeedFormatter(xmlWriter);
        //     await atom.WriteTitle(_manifest.Name);
        //     await atom.WriteId(host);
        //     await atom.WriteSubtitle(_manifest.Description);
        //     await atom.WriteGenerator("Miniblog.Core", "https://github.com/madskristensen/Miniblog.Core", "1.0");
        //     await atom.WriteValue("updated", updated.ToString("yyyy-MM-ddTHH:mm:ssZ"));
        //     return atom;
        // }
    }
}
