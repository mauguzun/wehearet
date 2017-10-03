using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenQA.Selenium.Chrome;
using System.Threading;
using OpenQA.Selenium.Interactions;
using System.IO;
using OpenQA.Selenium;
using System.Collections.ObjectModel;

namespace WeHearIt
{
    public partial class Form1 : Form
    {
        private  int pause = 5000;
        private ChromeDriver _driver;
        private Links _links;

        List<string> _url;
        private string filename = "links.txt";
        ExistReaderWriter ex;
        public Form1()
        {
            InitializeComponent();
            
            _links = new Links();
            ex = new ExistReaderWriter();
           
        }

        private void loginToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this._url.Count ==0 )
            {
                this._url = new List<string>();
            }
            
        }




        private void followToolStripMenuItem_Click_1(object sender, EventArgs e)
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



        private void _MakeLogin(ChromeDriver _driver)
        {
           

            try
            {
                _driver.Url = _links.DefaultUrl;
                _driver.FindElementById("user_email_or_username").SendKeys("lovedrugs@inbox.lv");
                _driver.FindElementById("user_password_login").SendKeys("trance12");
              //  Thread.Sleep(300);
                _driver.FindElementById("user_password_login").SendKeys(OpenQA.Selenium.Keys.Enter);
                this.con.Text += $"{ex.GetCount().ToString()} pinned ,and {_url.Count().ToString()}";

            }
            catch { }
            
        }

        private void likeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var driver = this._GetDriver();
            this._MakeLogin(driver);
            Task.Run(() => _LikeMethod(driver));

        }
        private ChromeDriver _GetDriver ()
        {
            ChromeOptions option = new ChromeOptions();
         //  option.AddArgument("--headless");
         //  option.AddArgument("--no-startup-window");
            var driver = new ChromeDriver(option);
            
            return driver;

        }

        private void _LikeMethod(ChromeDriver driver)
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
                        }
                        catch { }

                    }


                }

            }
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
          
            var driver = this._GetDriver();
            driver.Manage().Timeouts().ImplicitWait = new TimeSpan(0,0,45);
            this._MakeLogin(driver);
            Task.Run(() => MakePost(driver));
        }

        private  void SaveLinks()
        {
            File.WriteAllLines(this.filename, this._url);
        }

        private  void MakePost(ChromeDriver driver)
        {
            
            while(true)
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
                    if(divs.Count() ==0)
                    {
                        _RemoveBadUrl(url);
                        throw new Exception();
                    }
                        
                    

                    bool clicked = false;
                    foreach(var div in divs)
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

                    if(!clicked)
                    {
                        _RemoveBadUrl(url);
                        throw new Exception();
                    }


                   // Thread.Sleep(pause);

                    var lastButton = driver.FindElementByCssSelector("input.btn.bg-primary.btn-wide");
                    lastButton.Click();
                    ex.Save();
//
                 //   Thread.Sleep(pause/2);
                    this._url.Remove(url);
                    this.SaveLinks();
                }
                catch
                {

                }
            }
           
        }

        private void _RemoveBadUrl(string url)
        {
            this._url.Remove(url);
            this.SaveLinks();
            return;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
               this._url =    File.ReadAllLines(this.filename).ToList();
            }
            catch(Exception ex)
            {
                con.Text = ex.Message;
            }
        }
    }
}
