using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using System.Diagnostics;
using DoDownload;



namespace TestForWebUntis
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Konstruktor

        private HttpWebRequest_BeginGetResponse dod;

        public MainPage()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            textBoxRequest.Text = "hello";
            //dod = new HttpWebRequest_BeginGetResponse();
            //dod.start();
            WebuntisAPI.WebUntisAPI API = new WebuntisAPI.WebUntisAPI(new Uri("http://www.htldornbirn.at/"),"HTL Dornbirn","User","Passwort");
                Debug.WriteLine( API.getTeacher(1).ToString());
            //System.Diagnostics.Debug.WriteLine("Yay!");
        }
    }
}
