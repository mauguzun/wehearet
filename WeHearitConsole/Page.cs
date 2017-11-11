using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.PhantomJS;
using OpenQA.Selenium.Remote;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WeHearIt;


namespace WeHearitConsole
{
    class Page
    {
        private int pause = 5000;
        private ChromeDriver _driver;
        private PageImortantUrl _links;

        private int ValueAfterRead;

        private int _liked = 0;
        ObservableCollection<string> _url;
        private string filename = "links.txt";
        ExistReaderWriter ex;
        public Page()
        {
         

            _links = new PageImortantUrl();
            ex = new ExistReaderWriter();


            try
            {

                this._url = new ObservableCollection<string>(File.ReadAllLines(this.filename).ToList());
                this._url.CollectionChanged += _url_CollectionChanged;
                ValueAfterRead = this._url.Count();

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);

            }

            this._Kill();

        }

        internal void Like()
        {
            var driver = this._GetDriver();
            this._MakeLogin(driver);
            Task.Run(() => _LikeMethod(driver));
        }

        private void followToolStripMenuItem_Click_1()
        {
            for (int i = 200; i < 493; i++)
            {
                _driver.Url = $"http://weheartit.com/search/users?query=ola&page={i.ToString()}";

                var followButtons = _driver.FindElementsByClassName("js-follow-button");
                if (followButtons.Count() > 0)
                {
                    foreach (var follow in followButtons)
                    {
                        try
                        {
                            follow.Click();

                        }
                        catch { }

                    }


                }

            }
        }

        public void Post()
        {
            var driver = this._GetDriver();
            driver.Manage().Timeouts().ImplicitWait = new TimeSpan(0, 0, 45);
            this._MakeLogin(driver);
            Task.Run(() => MakePost(driver));

        }


        private void _MakeLogin(RemoteWebDriver _driver)
        {


            try
            {
                _driver.Url = _links.DefaultUrl;
                _driver.FindElementById("user_email_or_username").SendKeys("lovedrugs@inbox.lv");
                _driver.FindElementById("user_password_login").SendKeys("trance12");
                _driver.FindElementById("user_password_login").SendKeys(OpenQA.Selenium.Keys.Enter);
                Console.WriteLine($"{ex.GetCount().ToString()} done ," +
                    $"and {_url.Count().ToString()}");
                                 
                
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                _driver.GetScreenshot().SaveAsFile("error.png", ScreenshotImageFormat.Png);
            }

        }

        private void likeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var driver = this._GetDriver();
            this._MakeLogin(driver);
            Task.Run(() => _LikeMethod(driver));

        }
        private RemoteWebDriver _GetDriver()
        {
            //var serviceJs = PhantomJSDriverService.CreateDefaultService();
            //serviceJs.HideCommandPromptWindow = true;
            //return new PhantomJSDriver(serviceJs);

            var sercive = ChromeDriverService.CreateDefaultService();
            sercive.HideCommandPromptWindow = true;


            return new ChromeDriver(sercive);



        }

        private void _LikeMethod(RemoteWebDriver driver)
        {
            for (int i = 2; i < 493; i++)
            {
                driver.Url = $"http://weheartit.com/search/entries?utf8=✓&ac=0&query=fashion&page={i.ToString()}";

                var followButtons = driver.FindElementsByClassName("icon-heart");
                if (followButtons.Count() > 0)
                {
                    foreach (var follow in followButtons)
                    {
                        try
                        {
                            follow.Click();
                            _liked++;
                            Console.WriteLine($"liked {_liked}");
                        }
                        catch (Exception ex)
                        {


                            Console.WriteLine($"...................... {ex.Message}");
                        }

                    }


                }

            }
        }

       

        private void SaveLinks()
        {
            Console.WriteLine($"{this._url.Count()} pinned from {this.ValueAfterRead} ");
            File.WriteAllLines(this.filename, this._url);
        }

        private void MakePost(RemoteWebDriver driver)
        {

            while (true)
            {
                try
                {
                    driver.Url = _links.MakePost;
                    driver.Navigate().Refresh();
                    //Thread.Sleep(pause);

                    string url = this._url.FirstOrDefault();
                    var input = driver.FindElement(By.Name("upload_url"));
                    input.SendKeys(url);


                    var button = driver.FindElementByCssSelector("div.upload-url-button.btn.btn-block.bg-primary.upload-input");
                    button.Click();
                    Thread.Sleep(pause);

                    var divs = driver.FindElements(By.ClassName("upload-image"));
                    if (divs.Count() == 0)
                    {
                        _RemoveBadUrl(url);
                        throw new Exception();
                    }



                    bool clicked = false;
                    foreach (var div in divs)
                    {
                        var img = div.FindElement(By.CssSelector("img"));
                        string imgurl = img.GetAttribute("src");
                        if (!ex.HaveThis(imgurl))
                        {
                            div.Click();
                            ex.Add(imgurl, false);
                            clicked = true;
                            break;
                        }
                    }

                    if (!clicked)
                    {
                        _RemoveBadUrl(url);
                        throw new Exception();
                    }


                    // Thread.Sleep(pause);

                    var lastButton = driver.FindElementByCssSelector("input.btn.bg-primary.btn-wide");
                    lastButton.Click();
                    ex.Save();
                    //
                    Thread.Sleep(pause / 2);
                    this._url.Remove(url);
                    this.SaveLinks();



                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    driver.GetScreenshot().SaveAsFile("error.png", ScreenshotImageFormat.Png);
                }
            }

        }

        private void _RemoveBadUrl(string url)
        {
            this._url.Remove(url);
            this.SaveLinks();
            return;
        }


        private void _url_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            Console.WriteLine($"{this._url.Count()}p /{_liked.ToString()} l");
           
        }

        private void _Kill()
        {

            var proccess = Process.GetProcesses();
            foreach (Process pr in proccess)
            {
                if (pr.ProcessName.ToLower().Contains("chromedriver"))
                {
                   
                    Console.WriteLine(pr.ProcessName);
                    pr.Kill();
                }

            }

        }

    }
}
