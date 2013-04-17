/*
using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using System.Threading.Tasks;

using System.Net;
using System.IO;
using System.Threading;
using System.Text.RegularExpressions;


namespace DoDownload
{
    public class RequestState
    {
        // This class stores the State of the request.
        const int BUFFER_SIZE = 1024;
        public StringBuilder requestData;
        public byte[] BufferRead;
        public HttpWebRequest request;
        public HttpWebResponse response;
        public Stream streamResponse;
        public RequestState()
        {
            BufferRead = new byte[BUFFER_SIZE];
            requestData = new StringBuilder("");
            request = null;
            streamResponse = null;
        }
    }

    class HttpWebRequest_BeginGetResponse
    {
        public static ManualResetEvent allDone = new ManualResetEvent(false);
        const int BUFFER_SIZE = 1024;
        const int DefaultTimeout = 2 * 60 * 1000;

        // Abort the request if the timer fires.
        private static void TimeoutCallback(object state, bool timedOut)
        {
            if (timedOut)
            {
                HttpWebRequest request = state as HttpWebRequest;
                if (request != null)
                {
                    request.Abort();
                }
            }
        }

        //public static void Main()
        public void start()
        {

            try
            {
                // Create a HttpWebrequest object to the desired URL. 
                HttpWebRequest myHttpWebRequest = (HttpWebRequest)WebRequest.Create("http://www.contoso.com");


                /**
                * If you are behind a firewall and you do not have your browser proxy setup
                * you need to use the following proxy creation code.

                    // Create a proxy object.
                    WebProxy myProxy = new WebProxy();

                    // Associate a new Uri object to the _wProxy object, using the proxy address
                    // selected by the user.
                    myProxy.Address = new Uri("http://myproxy");


                    // Finally, initialize the Web request object proxy property with the _wProxy
                    // object.
                    myHttpWebRequest.Proxy=myProxy;
                ***/

                // Create an instance of the RequestState and assign the previous myHttpWebRequest
                // object to its request field.  
                RequestState myRequestState = new RequestState();
                myRequestState.request = myHttpWebRequest;


                // Start the asynchronous request.
                IAsyncResult result =
                (IAsyncResult)myHttpWebRequest.BeginGetResponse(new AsyncCallback(RespCallback), myRequestState);

                // this line implements the timeout, if there is a timeout, the callback fires and the request becomes aborted
                ThreadPool.RegisterWaitForSingleObject(result.AsyncWaitHandle, new WaitOrTimerCallback(TimeoutCallback), myHttpWebRequest, DefaultTimeout, true);

                // The response came in the allowed time. The work processing will happen in the 
                // callback function.
                allDone.WaitOne();

                // Release the HttpWebResponse resource.
                myRequestState.response.Close();
            }
            catch (WebException e)
            {
                System.Diagnostics.Debug.WriteLine("\nMain Exception raised!");
                System.Diagnostics.Debug.WriteLine("\nMessage:{0}", e.Message);
                System.Diagnostics.Debug.WriteLine("\nStatus:{0}", e.Status);
                System.Diagnostics.Debug.WriteLine("Press any key to continue..........");
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("\nMain Exception raised!");
                // System.Diagnostics.Debug.WriteLine("Source :{0} " , e.Source);
                System.Diagnostics.Debug.WriteLine("Message :{0} ", e.Message);
                System.Diagnostics.Debug.WriteLine("Press any key to continue..........");
                //System.Diagnostics.Debug.Read();
            }
        }
        private static void RespCallback(IAsyncResult asynchronousResult)
        {
            try
            {
                // State of request is asynchronous.
                RequestState myRequestState = (RequestState)asynchronousResult.AsyncState;
                HttpWebRequest myHttpWebRequest = myRequestState.request;
                myRequestState.response = (HttpWebResponse)myHttpWebRequest.EndGetResponse(asynchronousResult);

                // Read the response into a Stream object.
                Stream responseStream = myRequestState.response.GetResponseStream();
                myRequestState.streamResponse = responseStream;

                // Begin the Reading of the contents of the HTML page and print it to the System.Diagnostics.Debug.
                IAsyncResult asynchronousInputRead = responseStream.BeginRead(myRequestState.BufferRead, 0, BUFFER_SIZE, new AsyncCallback(ReadCallBack), myRequestState);
                return;
            }
            catch (WebException e)
            {
                System.Diagnostics.Debug.WriteLine("\nRespCallback Exception raised!");
                System.Diagnostics.Debug.WriteLine("\nMessage:{0}", e.Message);
                System.Diagnostics.Debug.WriteLine("\nStatus:{0}", e.Status);
            }
            allDone.Set();
        }
        private static void ReadCallBack(IAsyncResult asyncResult)
        {
            try
            {

                RequestState myRequestState = (RequestState)asyncResult.AsyncState;
                Stream responseStream = myRequestState.streamResponse;
                int read = responseStream.EndRead(asyncResult);
                // Read the HTML page and then print it to the System.Diagnostics.Debug.
                if (read > 0)
                {
                    // use UTF8 instead of ASCII
                    myRequestState.requestData.Append(Encoding.UTF8.GetString(myRequestState.BufferRead, 0, read));
                    //myRequestState.requestData.Append(Encoding.ASCII.GetString(myRequestState.BufferRead, 0, read));
                    IAsyncResult asynchronousResult = responseStream.BeginRead(myRequestState.BufferRead, 0, BUFFER_SIZE, new AsyncCallback(ReadCallBack), myRequestState);
                    return;
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("\nThe contents of the Html page are : ");
                    if (myRequestState.requestData.Length > 1)
                    {
                        string stringContent;
                        stringContent = myRequestState.requestData.ToString();
                        System.Diagnostics.Debug.WriteLine(stringContent);
                    }
                    System.Diagnostics.Debug.WriteLine("Press any key to continue..........");
                   // System.Diagnostics.Debug.ReadLine();

                    responseStream.Close();
                }

            }
            catch (WebException e)
            {
                System.Diagnostics.Debug.WriteLine("\nReadCallBack Exception raised!");
                System.Diagnostics.Debug.WriteLine("\nMessage:{0}", e.Message);
                System.Diagnostics.Debug.WriteLine("\nStatus:{0}", e.Status);
            }
            allDone.Set();

        }
        /*static void Main(string[] args)
        {
        }*/
    }
}

//! WebuntisAPI Nampespace
/*! This is the Main Namespace of the WebUntis API. It contains the Namespace of the Types and classes for communication width WebUntis */
namespace WebuntisAPI
{
    //! WebuntisAPI::Types Namespase
    /*! This contains the structure-variables for x-Change with the APP (Coding) */
    namespace Types
    {
        //! WebuntisAPI::Types::Teacher
        /*! The Structure to save information about a teacher */
        public struct Teacher
        {
            public int id;
            public String firstname;/*!< Field containing firstname */
            public String lastname;/*!< Field containing lastname */
            public String shortname;/*!< Field containing the shortname e.g. LED */
        }
        //! WebuntisAPI::Types::Klasse
        /*! The Structure to save information about a Schoolclass in german because class makes problems */
        public struct Klasse
        {
            public int id;/*!< Field containing id */
            public String name;/*!< Field containing short classname */
            public String longname;/*!< Field containing classname */
            public int did;
        }
        //! WebuntisAPI::Types::Subject
        /*! The Structure to save information about a teacher */
        public struct Subject
        {
            public int id;/*!< Field containing id */
            public String name;/*!< Field containing short subject e.g. D */
            public String longname;/*!< Field containing subject e.g. Deutsch */
        }
        //! WebuntisAPI::Types::Department
        /*! The Structure to save information about a Department - really not important but API gives it */
        public struct Department
        {
            public int id;/*!< Field containing id */
            public String name;/*!< Field containing short name of department */
            public String longname;/*!< Field containing long name of department */
        }
        //! WebuntisAPI::Types::Holiday
        /*! The Structure to save information about a holidays */
        public struct Holiday
        {
            public int id;/*!< Field containing id */
            public String name;/*!< Field containing name of holiday */
            public String longname;/*!< Field containing long name of holiday */
            public DateTime startTime;/*!< Field containing starttime of holiday */
            public DateTime endTime;/*!< Field containing endtime of holiday */
        }
        //! WebuntisAPI::Types::Schoolyear
        /*! The Structure to save information about a schoolyear, but I think this is NOT important */
        public struct Schoolyear
        {
            public int id;/*!< Field containing id */
            public String name;/*!< Field containing name of shoolyear */
            public DateTime startTime;/*!< Field containing first day of schoolyear */
            public DateTime endTime;/*!< Field containing last day of shoolyear */
        }
        //! WebuntisAPI::Types::TimeTableElement
        /*! The Structure to save information about an element which is a cell in a table */
        public struct TimeTableElement
        {
            public int id;/*!< Field containing id */
            public DateTime date_lesson;/*!< Field containing date of the lesson */
            public DateTime startTime;/*!< Field containing start of the hour */
            public DateTime endTime;/*!< Field containing end of the hour */
            public int[] classids;/*!< Field containing array of ids of classes */
            public int[] subjectids;/*!< Field containing array of ids of subjects */
            public int[] roomids;/*!< Field containing ids of rooms */
        }
    }
    public class WebUntisConnector
    {
        private HttpWebRequest wr;/*!< Field containing Webrequest to connect to WebUntis */
        private String User;/*!< Field containing Username of WebUntis */
        private String Password;/*!< Field containing Password for user */
        private String SessionID;/*!< Field containing Sessionid for Connection */
        private Uri connection;/*!< Field containing Connectionstring */
        private String schule;/*!< Field containing School name */
        public WebUntisConnector(Uri connection, String Schule, String User, String Password)
        {
            //{"id":"ID","method":"authenticate","params":{"user":"ANDROID","password":"PASSWORD", "client":"CLIENT"},"jsonrpc":"2.0"}
            this.User = User;
            this.Password = Password;
            this.connection = connection;
            this.schule = Schule;
            /*wr = WebRequest.CreateHttp(this.connection);
            wr.Method = "POST";
            string postData = "This is a test that posts this string to a Web server.";
            byte[] byteArray = Encoding.UTF8.GetBytes(postData);
            wr.ContentType = "application/x-www-form-urlencoded";
            wr.ContentLength = byteArray.Length;
            Stream dataStream = wr.BeginGetRequestStream();
            wr.*/

        }
        public String runJSONRequest(String Method, String Params)
        {
            return null;
        }
        public void doLogin()
        {
            //params":{"user":"ANDROID","password":"PASSWORD", "client":"CLIENT"}

        }
    }
    public class WebUntisAPI
    {
        public const int KEY_TIMETABLE_Klasse = 1;
        public const int KEY_TIMETABLE_Teacher = 2;
        public const int KEY_TIMETABLE_Subject = 3;
        public const int KEY_TIMETABLE_Room = 4;
        public const int KEY_TIMETABLE_Student = 5;
        private List<Types.Teacher> teachers;
        private List<Types.Subject> subjects;
        WebUntisConnector connection;
        public WebUntisAPI(Uri URL, String Schule, String Benutzer, String Passwort)
        {
            connection = new WebUntisConnector(URL, Schule, Benutzer, Passwort);
        }
        private void loadTeacherList(String data)
        {
            teachers = new List<Types.Teacher>();
            data = data.Substring(2, data.Length - 4);
            String[] teacherobjects = Regex.Split(data, "\\},\\{");
            foreach (String teacherstring in teacherobjects)
            {
                String[] namevalues = teacherstring.Split(',');
                Types.Teacher newteacher = new Types.Teacher();
                foreach (String namevalue in namevalues)
                {
                    String[] set = namevalue.Split(':');
                    if (set[0] == "\"id\"")
                    {
                        newteacher.id = Convert.ToInt32(set[1]);
                    }
                    else if (set[0] == "\"name\"")
                    {
                        newteacher.shortname = set[1].Substring(1, set[1].Length - 2);
                    }
                    else if (set[0] == "\"foreName\"")
                    {
                        newteacher.firstname = set[1].Substring(1, set[1].Length - 2);
                    }
                    else if (set[0] == "\"longName\"")
                    {
                        newteacher.lastname = set[1].Substring(1, set[1].Length - 2);
                    }
                }
                teachers.Add(newteacher);
            }
        }
        private void loadSubjects(String data = "[]")
        {
            data = "[{\"id\":1,\"name\":\"RK\",\"longName\":\"Kath.Religion\",\"foreColor\":\"000000\",\"backColor\":\"000000\"},{\"id\":2,\"name\":\"RE\",\"longName\":\"Evang.Religion\",\"foreColor\":\"000000\",\"backColor\":\"000000\"}]";
            data = data.Substring(2, data.Length - 2);
            String[] subjectsjsonobjects = Regex.Split(data,"\\},\\{");
            subjects = new List<Types.Subject>();
            foreach (String subjectobject in subjectsjsonobjects)
            {
                String[] namevalues = subjectobject.Split(',');
                Types.Subject newsubject = new Types.Subject();
                foreach (String namevalue in namevalues)
                {
                    String[] set = namevalue.Split(':');
                    if (set[0] == "\"id\"")
                    {
                        newsubject.id = Convert.ToInt32(set[1]);
                    }
                    else if (set[0] == "\"name\"")
                    {
                        newsubject.name = set[1].Substring(1, set[1].Length - 2);
                    }
                    else if (set[0] == "\"longName\"")
                    {
                        newsubject.longname = set[1].Substring(1, set[1].Length - 2);
                    }
                }
                subjects.Add(newsubject);
            }
        }
        public Types.Teacher getTeacher(int id)
        {
            loadTeacherList("");
            foreach (Types.Teacher lehrer in teachers)
            {
                if (lehrer.id == id)
                {
                    return lehrer;
                }
            }
            return new Types.Teacher();
        }
        List<Types.Teacher> getTeachers()
        {
            return teachers;
        }
        public Types.Subject getSubject(int id)
        {

        }
    }
}
namespace Download
{
    public class RequestState
    {
        // This class stores the State of the request.
        const int BUFFER_SIZE = 1024;
        public StringBuilder requestData;
        public byte[] BufferRead;
        public HttpWebRequest request;
        public HttpWebResponse response;
        public Stream streamResponse;
        public RequestState()
        {
            BufferRead = new byte[BUFFER_SIZE];
            requestData = new StringBuilder("");
            request = null;
            streamResponse = null;
        }
    }

    class HttpWebRequest_BeginGetResponse
    {
        public static ManualResetEvent allDone = new ManualResetEvent(false);
        const int BUFFER_SIZE = 1024;
        const int DefaultTimeout = 2 * 60 * 1000;

        // Abort the request if the timer fires.
        private static void TimeoutCallback(object state, bool timedOut)
        {
            if (timedOut)
            {
                HttpWebRequest request = state as HttpWebRequest;
                if (request != null)
                {
                    request.Abort();
                }
            }
        }

        void FakeMain()
        {
            try
            {
                // Create a HttpWebrequest object to the desired URL. 
                HttpWebRequest myHttpWebRequest = (HttpWebRequest)WebRequest.Create("http://www.contoso.com");
                /**
                * If you are behind a firewall and you do not have your browser proxy setup
                * you need to use the following proxy creation code.

                // Create a proxy object.
                WebProxy myProxy = new WebProxy();

                // Associate a new Uri object to the _wProxy object, using the proxy address
                // selected by the user.
                myProxy.Address = new Uri("http://myproxy");


                // Finally, initialize the Web request object proxy property with the _wProxy
                // object.
                myHttpWebRequest.Proxy=myProxy;
                ***/

                // Create an instance of the RequestState and assign the previous myHttpWebRequest
                // object to its request field.  
                RequestState myRequestState = new RequestState();
                myRequestState.request = myHttpWebRequest;


                // Start the asynchronous request.
                IAsyncResult result =
                (IAsyncResult)myHttpWebRequest.BeginGetResponse(new AsyncCallback(RespCallback), myRequestState);

                // this line implements the timeout, if there is a timeout, the callback fires and the request becomes aborted
                ThreadPool.RegisterWaitForSingleObject(result.AsyncWaitHandle, new WaitOrTimerCallback(TimeoutCallback), myHttpWebRequest, DefaultTimeout, true);

                // The response came in the allowed time. The work processing will happen in the 
                // callback function.
                allDone.WaitOne();

                // Release the HttpWebResponse resource.
                myRequestState.response.Close();
            }
            catch (WebException e)
            {
                /*Console.WriteLine("\nMain Exception raised!");
                Console.WriteLine("\nMessage:{0}", e.Message);
                Console.WriteLine("\nStatus:{0}", e.Status);
                Console.WriteLine("Press any key to continue..........");*/
            }
            catch (Exception e)
            {
                /*Console.WriteLine("\nMain Exception raised!");
                Console.WriteLine("Source :{0} ", e.Source);
                Console.WriteLine("Message :{0} ", e.Message);
                Console.WriteLine("Press any key to continue..........");
                Console.Read();*/
            }
        }
        private static void RespCallback(IAsyncResult asynchronousResult)
        {
            try
            {
                // State of request is asynchronous.
                RequestState myRequestState = (RequestState)asynchronousResult.AsyncState;
                HttpWebRequest myHttpWebRequest = myRequestState.request;
                myRequestState.response = (HttpWebResponse)myHttpWebRequest.EndGetResponse(asynchronousResult);

                // Read the response into a Stream object.
                Stream responseStream = myRequestState.response.GetResponseStream();
                myRequestState.streamResponse = responseStream;

                // Begin the Reading of the contents of the HTML page and print it to the console.
                IAsyncResult asynchronousInputRead = responseStream.BeginRead(myRequestState.BufferRead, 0, BUFFER_SIZE, new AsyncCallback(ReadCallBack), myRequestState);
                return;
            }
            catch (WebException e)
            {
                /*Console.WriteLine("\nRespCallback Exception raised!");
                Console.WriteLine("\nMessage:{0}", e.Message);
                Console.WriteLine("\nStatus:{0}", e.Status);*/
            }
            allDone.Set();
        }
        private static void ReadCallBack(IAsyncResult asyncResult)
        {
            try
            {
                RequestState myRequestState = (RequestState)asyncResult.AsyncState;
                Stream responseStream = myRequestState.streamResponse;
                int read = responseStream.EndRead(asyncResult);
                // Read the HTML page and then print it to the console.
                if (read > 0)
                {
                    myRequestState.requestData.Append(Encoding.UTF8.GetString(myRequestState.BufferRead, 0, read));
                    IAsyncResult asynchronousResult = responseStream.BeginRead(myRequestState.BufferRead, 0, BUFFER_SIZE, new AsyncCallback(ReadCallBack), myRequestState);
                    return;
                }
                else
                {
                    //Console.WriteLine("\nThe contents of the Html page are : ");
                    if (myRequestState.requestData.Length > 1)
                    {
                        string stringContent;
                        stringContent = myRequestState.requestData.ToString();
                        Console.WriteLine(stringContent);
                    }
                    /*Console.WriteLine("Press any key to continue..........");
                    Console.ReadLine();
                    responseStream.Close();*/
                }
            }
            catch (WebException e)
            {
                /*Console.WriteLine("\nReadCallBack Exception raised!");
                Console.WriteLine("\nMessage:{0}", e.Message);
                Console.WriteLine("\nStatus:{0}", e.Status);*/
            }
            allDone.Set();
        }
        /*static void Main(string[] args)
        {
        }*/
    }
}
