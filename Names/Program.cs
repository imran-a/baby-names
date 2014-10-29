using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using System.Web;

namespace Names
{
    class Program
    {

        public static Database db { get; set; }

        static void Main(string[] args)
        {

            db = new Database();

            var sites = new List<NameSite>()
            {
                new NameSite("http://www.nordicnames.de", 
                             "http://www.nordicnames.de/w/index.php?title=Category:Finnish_Male_Names&pagefrom=A", 
                              "//*/div[@id='mw-pages']//ul//a", 
                              "//*/div[@id='mw-pages']//a[contains(text(), 'next')]",
                              "f")
                              
                              ,

                new NameSite("http://babynames.net", 
                             "http://babynames.net/boy/finnish?page=1", 
                              "//*/ul[@class='names-results']//span[@class='result-name']", 
                              "//*/section[@class='pagination']//a[contains(text(), 'Next')]",
                              "f")

                              ,

                new NameSite("http://www.pakistan.web.pk/",
                             "http://www.pakistan.web.pk/names/gender/baby-boy-names/?gender=Baby+Boy+Names",
                             "//*/ol[@class='discussionListItems']//li//a", 
                             "//*/div[@class='pageNavLinkGroup']//a[contains(text(), 'Next')]", 
                             "p")
            };

            foreach (var site in sites)
            {
                DoPage(site.Root, site.StartPage, site.NameSelector, site.NextSelector, site.Origin);
            }

            Console.ReadLine();
        }

        private static void ParseSite(NameSite site)
        {
            var page = new HtmlWeb().Load(site.StartPage);

            // scrape first page
            foreach (var name in page.DocumentNode.SelectNodes(site.NameSelector))
            {
                Console.WriteLine(name);
            }

            // get next link
            var links = page.DocumentNode.SelectNodes(site.NextSelector);
            if (links.Any())
            {
                var link = links.First().Attributes["href"].Value;
            }




        }

        public static void DoPage(string root, string url, string namexp, string nextxp, string type)
        {
            url = HttpUtility.HtmlDecode(url);

            var page = new HtmlWeb().Load(url);

            foreach (var name in page.DocumentNode.SelectNodes(namexp))
            {
                if (!name.InnerText.Contains(" "))
                {
                    if (type == "f")
                    {
                        var f = new FinnishName() { Name = name.InnerText };
                        db.FinnishNames.Add(f);
                    }
                    else
                    {
                        var p = new PakistanName() { Name = name.InnerText };
                        db.PakistaniNames.Add(p);
                    }

                    Console.WriteLine(name.InnerText);
                }
            }

            db.SaveChanges();

            // get next link
            var links = page.DocumentNode.SelectNodes(nextxp);
            if (links != null && links.Any())
            {
                var link = links.First().Attributes["href"].Value;

                if (!link.Contains("javascript"))
                    DoPage(root, root + link, namexp, nextxp, type);
            }
        }
        
    }

    public class NameSite
    {
        public NameSite(string root, string startPage, string nameSelector, string nextSelector, string origin)
        {
            Root = root;
            StartPage = startPage;
            NameSelector = nameSelector;
            NextSelector = nextSelector;
            Origin = origin;
        }

        public string Root { get; set; }
        public string StartPage { get; set; }
        public string NameSelector { get; set; }
        public string NextSelector { get; set; }
        public string Origin { get; set; }
    }
}
