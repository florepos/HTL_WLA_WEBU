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
using System.Diagnostics;


namespace DoDownload
{
    //! DoDownload::RequestState
    /*! State of the Download */
    public class RequestState
    {
        // This class stores the State of the request.
        const int BUFFER_SIZE = 1024;/*!< Field containing Size of the Buffer */
        public StringBuilder requestData;/*!< An Object to crate a String */
        public byte[] BufferRead;/*!< Buffer to Download the Project */
        public HttpWebRequest request;/*!< The Data to send to the Server */
        public HttpWebResponse response;/*!< The Data to get from the Server */
        public Stream streamResponse;/*!< The Stream of the Data in the Server */
        public RequestState()
        {
            BufferRead = new byte[BUFFER_SIZE];
            requestData = new StringBuilder("");
            request = null;
            streamResponse = null;
        }
    }

    //! DoDownload::HttpWebRequest_BeginGetResponse
    /*! Class to load  */
    class HttpWebRequest_BeginGetResponse
    {
        public static ManualResetEvent allDone = new ManualResetEvent(false);/*!< Contains the State of the Download */
        const int BUFFER_SIZE = 1024;/*!< The Buffer Size of the DowloadBuffer */
        const int DefaultTimeout = 2 * 60 * 1000;/*!< The timeouf when the connection fails */

        //! TimeoutCallback
        /*! Abort the request if the timer fires.*/
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

        //! start()
        /*! Starts a Webrequest */
        public void start()
        {

            try
            {
                // Create a HttpWebrequest object to the desired URL. 
                HttpWebRequest myHttpWebRequest = (HttpWebRequest)WebRequest.Create("http://www.contoso.com");
                
                /*! set the HttpWebRequest variables*/
              //  myHttpWebRequest.Method = "POST";
                //myHttpWebRequest.ContentType = "application/json";

               // Stream stream = myHttpWebRequest.BeginGetRequestStream();

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
        //! RespCallback
        /*! Callbach when the Data arrives  */
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
        //! ReadCallBack
        /*! Reads and compute the Callback result */
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
        //! WebuntisAPI::Types::Room
        /* The Structure to save information about a room */
        public struct Room
        {
            public int id;/*!< Field containing id */
            public string name;/*!< Field containing the name of the room. important: the first character is 'R'*/
            public string longname;/*!< Field containg the long name of the room. important: the first character isn't 'R'*/
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
            /* Request Timegrid
             * Request {"id":"ID","method":"getTimegridUnits","params":{},"jsonrpc":"2.0"}
             * {"jsonrpc":"2.0","id":"ID","result":[{"day":2,"timeUnits":[{"name":"0","startTime":755,"endTime":845},{"name":"0","startTime":845,"endTime":935},{"name":"0","startTime":950,"endTime":1040},{"name":"0","startTime":1040,"endTime":1130},{"name":"0","startTime":1130,"endTime":1220},{"name":"0","startTime":1220,"endTime":1310},{"name":"0","startTime":1310,"endTime":1400},{"name":"0","startTime":1400,"endTime":1450},{"name":"0","startTime":1500,"endTime":1550},{"name":"0","startTime":1550,"endTime":1640},{"name":"0","startTime":1640,"endTime":1730}]},{"day":3,"timeUnits":[{"name":"0","startTime":755,"endTime":845},{"name":"0","startTime":845,"endTime":935},{"name":"0","startTime":950,"endTime":1040},{"name":"0","startTime":1040,"endTime":1130},{"name":"0","startTime":1130,"endTime":1220},{"name":"0","startTime":1220,"endTime":1310},{"name":"0","startTime":1310,"endTime":1400},{"name":"0","startTime":1400,"endTime":1450},{"name":"0","startTime":1500,"endTime":1550},{"name":"0","startTime":1550,"endTime":1640},{"name":"0","startTime":1640,"endTime":1730}]},{"day":4,"timeUnits":[{"name":"0","startTime":755,"endTime":845},{"name":"0","startTime":845,"endTime":935},{"name":"0","startTime":950,"endTime":1040},{"name":"0","startTime":1040,"endTime":1130},{"name":"0","startTime":1130,"endTime":1220},{"name":"0","startTime":1220,"endTime":1310},{"name":"0","startTime":1310,"endTime":1400},{"name":"0","startTime":1400,"endTime":1450},{"name":"0","startTime":1500,"endTime":1550},{"name":"0","startTime":1550,"endTime":1640},{"name":"0","startTime":1640,"endTime":1730}]},{"day":5,"timeUnits":[{"name":"0","startTime":755,"endTime":845},{"name":"0","startTime":845,"endTime":935},{"name":"0","startTime":950,"endTime":1040},{"name":"0","startTime":1040,"endTime":1130},{"name":"0","startTime":1130,"endTime":1220},{"name":"0","startTime":1220,"endTime":1310},{"name":"0","startTime":1310,"endTime":1400},{"name":"0","startTime":1400,"endTime":1450},{"name":"0","startTime":1500,"endTime":1550},{"name":"0","startTime":1550,"endTime":1640},{"name":"0","startTime":1640,"endTime":1730}]},{"day":6,"timeUnits":[{"name":"0","startTime":755,"endTime":845},{"name":"0","startTime":845,"endTime":935},{"name":"0","startTime":950,"endTime":1040},{"name":"0","startTime":1040,"endTime":1130},{"name":"0","startTime":1130,"endTime":1220},{"name":"0","startTime":1220,"endTime":1310},{"name":"0","startTime":1310,"endTime":1400},{"name":"0","startTime":1400,"endTime":1450},{"name":"0","startTime":1500,"endTime":1550},{"name":"0","startTime":1550,"endTime":1640},{"name":"0","startTime":1640,"endTime":1730}]},{"day":7,"timeUnits":[{"name":"0","startTime":755,"endTime":845},{"name":"0","startTime":845,"endTime":935},{"name":"0","startTime":950,"endTime":1040},{"name":"0","startTime":1040,"endTime":1130},{"name":"0","startTime":1130,"endTime":1220}]}]}
             * /
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
        private List<Types.Klasse> klassen;
        private List<Types.Subject> subjects;
        private List<Types.Department> departments;
        private List<Types.Holiday> holidays;
        private List<Types.Schoolyear> schoolyears;
        private List<Types.TimeTableElement> timeTableElements;
        private List<Types.Room> rooms;

        WebUntisConnector connection;
        public WebUntisAPI(Uri URL, String Schule, String Benutzer, String Passwort)
        {
            connection = new WebUntisConnector(URL, Schule, Benutzer, Passwort);
        }
        private void loadTeacherList(String data)
        {
            //data = "[{\"id\":1,\"name\":\"Bach\",\"foreName\":\"Ingeborg\",\"longName\":\"Bachmann\",\"foreColor\":\"000000\",\"backColor\":\"000000\"},{\"id\":2,\"name\":\"Foss\",\"foreName\":\"Dian\",\"longName\":\"Fossey\",\"foreColor\":\"000000\",\"backColor\":\"000000\"}]";
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
        public List<Types.Teacher> getTeachers()
        {
            return teachers;
        }

        /*! loadKlassenList*/
        private void loadKlassenList(String data)
        {
            //data = "[{\"id\":71,\"name\":\"1A\",\"longName\":\"Klasse1A\",\"foreColor\":\"000000\",\"backColor\":\"000000\",did:2},{\"id\":72,\"name\":\"1B\",\"longName\":\"Klasse1B\",\"foreColor\":\"000000\",\"backColor\":\"000000\"}]";
            klassen = new List<Types.Klasse>();
            data = data.Substring(2, data.Length - 4);
            String[] klasseobjects = Regex.Split(data, "\\},\\{");
            foreach (String klassestring in klasseobjects)
            {
                String[] namevalues = klassestring.Split(',');
                Types.Klasse newklasse = new Types.Klasse();
                foreach (String namevalue in namevalues)
                {
                    String[] set = namevalue.Split(':');
                    if (set[0] == "\"id\"")
                    {
                        newklasse.id = Convert.ToInt32(set[1]);
                    }
                    else if (set[0] == "\"name\"")
                    {
                        newklasse.name = set[1].Substring(1, set[1].Length - 2);
                    }
                    else if (set[0] == "\"longName\"")
                    {
                        newklasse.longname = set[1].Substring(1, set[1].Length - 2);
                    }
                    else if (set[0] == "\"did\"")
                    {
                        newklasse.did = Convert.ToInt32( set[1].Substring(1, set[1].Length - 2));
                    }
                }
                klassen.Add(newklasse);
            }
        }
        /*! getKlasse*/
        public Types.Klasse getKlasse(int id)
        {
            loadKlassenList("");
            foreach (Types.Klasse klasse in klassen)
            {
                if (klasse.id == id)
                {
                    return klasse;
                }
            }
            return new Types.Klasse();
        }
        /*! getKlassen*/
        public List<Types.Klasse> getKlassen()
        {
            return klassen;
        }
		
		private void loadSubjectList(String data)
        {
            //data = "[{\"id\":1,\"name\":\"RK\",\"longName\":\"Kath.Religion\",\"foreColor\":\"000000\",\"backColor\":\"000000\"},{\"id\":2,\"name\":\"RE\",\"longName\":\"Evang.Religion\",\"foreColor\":\"000000\",\"backColor\":\"000000\"}]";
            subjects = new List<Types.Subject>();
            data = data.Substring(2, data.Length - 4);
            String[] subjectObject = Regex.Split(data, "\\},\\{");

            foreach (String subjectString in subjectObject)
            {
                String[] namevalues = subjectString.Split(',');
                Types.Subject newsubject = new Types.Subject();

                foreach (String namevalue in namevalues)
                {
                    String[] set = namevalue.Split(':');
                    if (set[0] == "\"id\"")
                    {
                        newsubject.id = Convert.ToInt32(set[1]);
                    }
                    else if( set[0] == "\"name\"")
                    {
                        newsubject.name = set[1].Substring( 1, set[1].Length-2);
                    }
                    else if( set[0] == "\"longName\"")
                    {
                        newsubject.longname = set[1].Substring( 1, set[1].Length-2);
                    }
                }
                subjects.Add( newsubject);
             }
        }
        //! getSubject
        /*!  */
        public Types.Subject getSubject(int id)
        {
            loadSubjectList("");
            foreach (Types.Subject subject in subjects)
            {
                if (subject.id == id)
                {
                    return subject;
                }
            }
            return new Types.Subject();
        }
        public List<Types.Subject> getSubjects()
        {
            return subjects;
        }

        private void loadDepartmentList(String data)
        {
            //data = "[{\"id\":1,\"name\":\"A1\",\"longName\":\"AAA1\"},{\"id\":2,\"name\":\"A2\",\"longName\":\"AAA2\"}]";
            departments = new List<Types.Department>();
            data = data.Substring(2, data.Length - 4);
            String[] departmentObject = Regex.Split(data, "\\},\\{");

            foreach (String departmentString in departmentObject)
            {
                String[] namevalues = departmentString.Split(',');
                Types.Department newDepartment = new Types.Department();

                foreach (String namevalue in namevalues)
                {
                    String[] set = namevalue.Split(':');
                    if (set[0] == "\"id\"")
                    {
                        newDepartment.id = Convert.ToInt32(set[1]);
                    }
                    else if( set[0] == "\"name\"")
                    {
                        newDepartment.name = set[1].Substring( 1, set[1].Length-2);
                    }
                    else if( set[0] == "\"longName\"")
                    {
                        newDepartment.longname = set[1].Substring( 1, set[1].Length-2);
                    }
                }
                departments.Add( newDepartment);
             }

        }
        public Types.Department getDepartment(int id)
        {
            loadDepartmentList("");
            foreach (Types.Department department in departments)
            {
                if (department.id == id)
                {
                    return department;
                }
            }
            return new Types.Department();
        }
        public List<Types.Department> getDepartments()
        {
            return departments;
        }

        private void loadHolidayList(String data)
        {
            //data = "[{\"id\":1,\"name\":\"Natio\",\"longName\":\"Nationalfeiertag\",\"startDate\":20101026,\"endDate\":20101026},{\"id\":2,\"name\":\"Allerheiligen\",\"longName\":\"Allerheiligen\",\"startDate\":20101101,\"endDate\":20101101}]";
            holidays = new List<Types.Holiday>();
            data = data.Substring(2, data.Length - 4);
            String[] holidayObject = Regex.Split(data, "\\},\\{");

            foreach (String holidayString in holidayObject)
            {
                String[] namevalues = holidayString.Split(',');
                Types.Holiday newHoliday = new Types.Holiday();

                foreach (String namevalue in namevalues)
                {
                    String[] set = namevalue.Split(':');
                    if (set[0] == "\"id\"")
                    {
                        newHoliday.id = Convert.ToInt32(set[1]);
                    }
                    else if( set[0] == "\"name\"")
                    {
                        newHoliday.name = set[1].Substring( 1, set[1].Length-2);
                    }
                    else if( set[0] == "\"longName\"")
                    {
                        newHoliday.longname = set[1].Substring( 1, set[1].Length-2);
                    }
                    else if (set[0] == "\"startDate\"")
                    {
                        newHoliday.startTime = new DateTime( Convert.ToInt32(set[1].Substring(0, 4)), Convert.ToInt32( set[1].Substring(4, 2)), Convert.ToInt32( set[1].Substring(6, 2)));
                    }
                    else if (set[0] == "\"endDate\"")
                    {
                        newHoliday.endTime = new DateTime(Convert.ToInt32(set[1].Substring(0, 4)), Convert.ToInt32(set[1].Substring(4, 2)), Convert.ToInt32(set[1].Substring(6, 2)));
                    }
                }
                holidays.Add( newHoliday);
             }
        }
        public Types.Holiday getHoliday(int id)
        {
            loadHolidayList("");
            foreach (Types.Holiday holiday in holidays)
            {
                if (holiday.id == id)
                {
                    return holiday;
                }
            }
            return new Types.Holiday();
        }
        public List<Types.Holiday> getHolidays()
        {
            return holidays;
        }

        private void loadSchoolyearList(String data)
        {
            //data = "[{\"id\":10,\"name\":\"2010/2011\",\"startDate\":20100830,\"endDate\":20110731},{\"id\":11,\"name\":\"2011/2012\",\"startDate\":20110905,\"endDate\":20120729}]";
            schoolyears = new List<Types.Schoolyear>();
            data = data.Substring(2, data.Length - 4);
            String[] schoolyearObject = Regex.Split(data, "\\},\\{");

            foreach (String schoolyearString in schoolyearObject)
            {
                String[] namevalues = schoolyearString.Split(',');
                Types.Schoolyear newSchoolyear = new Types.Schoolyear();

                foreach (String namevalue in namevalues)
                {
                    String[] set = namevalue.Split(':');
                    if (set[0] == "\"id\"")
                    {
                        newSchoolyear.id = Convert.ToInt32(set[1]);
                    }
                    else if (set[0] == "\"name\"")
                    {
                        newSchoolyear.name = set[1].Substring(1, set[1].Length - 2);
                    }
                    else if (set[0] == "\"startDate\"")
                    {
                        newSchoolyear.startTime = new DateTime(Convert.ToInt32(set[1].Substring(0, 4)), Convert.ToInt32(set[1].Substring(4, 2)), Convert.ToInt32(set[1].Substring(6, 2)));
                    }
                    else if (set[0] == "\"endDate\"")
                    {
                        newSchoolyear.endTime = new DateTime(Convert.ToInt32(set[1].Substring(0, 4)), Convert.ToInt32(set[1].Substring(4, 2)), Convert.ToInt32(set[1].Substring(6, 2)));
                    }
                }
                schoolyears.Add(newSchoolyear);
            }
        }
        public Types.Schoolyear getSchoolyear(int id)
        {
            loadSchoolyearList("");
            foreach (Types.Schoolyear schoolyear in schoolyears)
            {
                if (schoolyear.id == id)
                {
                    return schoolyear;
                }
            }
            return new Types.Schoolyear();
        }
        public List<Types.Schoolyear> getSchoolyears()
        {
            return schoolyears;
        }

        private void loadTimeTableElementList(String data)
        {
            //data = "[{\"id\":125043,\"date\":20110117,\"startTime\":800,\"endTime\":850,\"kl\":[{\"id\":71,\"id\":5}],\"te\":[{\"id\":23}],\"su\":[{\"id\":13}],\"ro\":[{\"id\":1}]},{\"id\":125127,\"date\":20120117,\"startTime\":1055,\"endTime\":1145,\"kl\":[{\"id\":22}],\"te\":[{\"id\":41}],\"su\":[{\"id\":19}],\"ro\":[{\"id\":31}]}]";
            //data = "[{\"id\":125043,\"date\":20110117,\"startTime\":800,\"endTime\":850,\"kl\":[{\"id\":71}],\"te\":[{\"id\":23}],\"su\":[{\"id\":13}],\"ro\":[{\"id\":1}]}]";

            timeTableElements = new List<Types.TimeTableElement>();
            data = data.Substring(2, data.Length - 4);
            String[] timeTableElementObject = Regex.Split(data, "\\},\\{");
            char[] dataCharArr;
            TimeTableId timeTableInfo;
            
            foreach (String timeTableElementString in timeTableElementObject)
            {
                String[] namevalues = timeTableElementString.Split(',');
                Types.TimeTableElement newTimeTableElement = new Types.TimeTableElement();
                dataCharArr = data.ToCharArray();

                for( int s = 0; s < namevalues.Length; s++)
                {
                    String[] set = namevalues[s].Split(':');

                    if (set[0] == "\"id\"")
                    {
                        newTimeTableElement.id = Convert.ToInt32( Regex.Replace( set[1], "}]", ""));
                    }
                    else if (set[0] == "\"date\"")
                    {
                        newTimeTableElement.date_lesson = new DateTime(Convert.ToInt32(set[1].Substring(0, 4)), Convert.ToInt32(set[1].Substring(4, 2)), Convert.ToInt32(set[1].Substring(6, 2)));
                    }
                    else if (set[0] == "\"startTime\"")
                    {
                        Int32 startHour;
                        Int32 startMinute;

                        if (set[1].Length == 3)
                        {
                            startHour = Convert.ToInt32( 0 + set[1].Substring( 0, 1));
                            startMinute = Convert.ToInt32( 0 + set[1].Substring( 1, 2));
                        }
                        else
                        {
                            startHour = Convert.ToInt32( 0 + set[1].Substring( 0, 2));
                            startMinute = Convert.ToInt32( 0 + set[1].Substring( 2, 2));
                        }
                        newTimeTableElement.startTime = new DateTime(newTimeTableElement.date_lesson.Year, newTimeTableElement.date_lesson.Month, newTimeTableElement.date_lesson.Day, startHour, startMinute, 0);
                    }
                    else if (set[0] == "\"endTime\"")
                    {
                        Int32 endHour;
                        Int32 endMinute;

                         if (set[1].Length == 3)
                        {
                            endHour = Convert.ToInt32( 0 + set[1].Substring( 0, 1));
                            endMinute = Convert.ToInt32( 0 + set[1].Substring( 1, 2));
                        }
                        else
                        {
                            endHour = Convert.ToInt32( 0 + set[1].Substring( 0, 2));
                            endMinute = Convert.ToInt32( 0 + set[1].Substring( 2, 2));
                        }
                        newTimeTableElement.endTime = new DateTime( newTimeTableElement.date_lesson.Year, newTimeTableElement.date_lesson.Month, newTimeTableElement.date_lesson.Day, endHour, endMinute, 0);
                    }
                    else if (set[0] == "\"kl\"")
                    {
                        // set klassen ids
                        timeTableInfo = getTimeTableId("\"kl\"", s, data, dataCharArr);
                        newTimeTableElement.classids = timeTableInfo.id;
                        s = timeTableInfo.s;
                    }
                    else if (set[0] == "\"su\"")
                    {
                        // set subject ids
                        timeTableInfo = getTimeTableId("\"su\"", s, data, dataCharArr);
                        newTimeTableElement.subjectids = timeTableInfo.id;
                        s = timeTableInfo.s;
                    }
                    else if (set[0] == "\"ro\"")
                    {
                        // set room ids
                        timeTableInfo = getTimeTableId("\"ro\"", s, data, dataCharArr);
                        newTimeTableElement.roomids = timeTableInfo.id;
                        s = timeTableInfo.s;
                    }
                }

                try
                {
                    data = data.Remove(0, timeTableElementString.Length + 4);
                }
                catch (ArgumentOutOfRangeException)
                {
                }

                timeTableElements.Add( newTimeTableElement);
            }
        }

        private struct TimeTableId
        {
            public int[] id;
            public int s;
        }

        private TimeTableId getTimeTableId(String idString, int s, String data, char[] dataCharArr)
        {
            int start = 0;
            int i = 0;
            int countOpen = 1;
            int countObjects = 1;
            int countQuote = 0;
            int n = 0;
            String data1;
            String[] klasseIdObject;

            TimeTableId timeTableId = new TimeTableId();

            start = data.IndexOf( idString + ":[{");//"\"kl\":[{"
            countObjects = 0;

            if (start != -1)
            {
                countOpen = 0;
                            
                for (i = start++; i < dataCharArr.Length; i++)
                {
                    if ((dataCharArr[i] == '\"') && (countQuote == 0))
                    {
                        countObjects++;
                        countQuote++;
                    }
                    else if ((dataCharArr[i] == '\"') && (countQuote > 0))
                    {
                        countQuote--;
                    }
                    else if (dataCharArr[i] == '{')
                    {
                        countOpen++;
                    }
                    else if (dataCharArr[i] == '}')
                    {
                        countOpen--;
                        if (countOpen == 0)
                            break;
                    }
                }
            }
            if ( start == -1)
            {
                timeTableId.id = new int[1];
            }
            else
            {
                data1 = data.Substring(start + 2, i - start);

                data1 = data1.Remove(data1.IndexOf(']')-1);//+1
                klasseIdObject = Regex.Split(data1, "\\}\\,\\{");
                timeTableId.id = new int[countObjects-1];

                String[] klasseIdNameValues = klasseIdObject[0].Split(',');
                n = 0;

                foreach (String klasseIdString in klasseIdNameValues)
                {
                    if (n > 0)
                        s++;

                    String[] setKlassenId = klasseIdString.Split(':');
                    try
                    {
                        timeTableId.id[n] = Convert.ToInt32(setKlassenId[1]);
                    }
                    catch( Exception)
                    {
                        timeTableId.id[n] = Convert.ToInt32(setKlassenId[2]);
                    }
                    n++;
                }
            }
            timeTableId.s = s;
            return timeTableId;
        }
        //! WebuntisAPI::getTimeTableElement
        /*! Get The Timetable Element from Buffer */
        public Types.TimeTableElement getTimeTableElement(int id)
        {
            loadTimeTableElementList("");
            foreach (Types.TimeTableElement timeTableElement in timeTableElements)
            {
                if (timeTableElement.id == id)
                {
                    return timeTableElement;
                }
            }
            return new Types.TimeTableElement();
        }
        public List<Types.TimeTableElement> getTimeTableElements()
        {
            return timeTableElements;
        }
        private void loadRoomList(String data)
        {
            data = "[{\"id\":1,\"name\":\"R1A\",\"longName\":\"1A\",\"foreColor\":\"000000\",\"backColor\":\"000000\"},{\"id\":2,\"name\":\"R1B\",\"longName\":\"1B\",\"foreColor\":\"000000\",\"backColor\":\"000000\"}]}";
            rooms = new List<Types.Room>();
            data = data.Substring(2, data.Length - 4);
            String[] roomobjects = Regex.Split(data, "\\},\\{");
            foreach (String roomstring in roomobjects)
            {
                String[] namevalues = roomstring.Split(',');
                Types.Room newroom = new Types.Room();
                foreach (String namevalue in namevalues)
                {
                    String[] set = namevalue.Split(':');
                    if (set[0] == "\"id\"")
                    {
                        newroom.id = Convert.ToInt32(set[1]);
                    }
                    else if (set[0] == "\"name\"")
                    {
                        newroom.name = set[1].Substring(1, set[1].Length - 2);
                    }
                    else if (set[0] == "\"longName\"")
                    {
                        newroom.longname = set[1].Substring(1, set[1].Length - 2);
                    }
                }
                rooms.Add(newroom);
            }
        }
        //! WebuntisAPI::getRoom
        /*! Get the Room Element from Buffer */
        public Types.Room getRoom(int id)
        {
            loadRoomList("");
            foreach (Types.Room room in rooms)
            {
                if (room.id == id)
                {
                    return room;
                }
            }
            return new Types.Room();
        }
        public List<Types.Room> getRooms()
        {
            return rooms;
        }
    }
}
//! Download
/*! Contains Funcions to Download */
namespace Download
{
    //! Download::RequestState
    /*! Holds The State of the Dowload */
    public class RequestState
    {
        // This class stores the State of the request.
        const int BUFFER_SIZE = 1024;/*!< Buffer variable for 1024 bytes (in utf8 it ist 1024 chars) */
        public StringBuilder requestData;/*!< Builds a String like java string builder class */
        public byte[] BufferRead;/*!< This is the Download Buffer itself */
        public HttpWebRequest request;/*!< Object for the Request */
        public HttpWebResponse response;/*!< Here is the Answer */
        public Stream streamResponse;/*!< Response Stream */
        //! Initialisise Object
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
        public static ManualResetEvent allDone = new ManualResetEvent(false); /*!< Information if done */
        const int BUFFER_SIZE = 1024;/*!< Buffer Size with 1024 chars */
        const int DefaultTimeout = 2 * 60 * 1000;/*!< Timeout after 2 minutes. */

        // Abort the request if the timer fires.
        //! HttpWebRequest_BeginGetResponse::TimeoutCallback
        /*! Stop loading after Timeout */
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
        //! 
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
        //! WebuntisAPI::HttpWebRequest_BeginGetResponse.RespCallback
        /*! Working with the Result of the Request - Callback function */
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
