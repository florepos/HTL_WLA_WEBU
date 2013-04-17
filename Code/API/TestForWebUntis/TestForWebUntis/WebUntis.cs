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
        private List<Types.Klasse> klassen;
        private List<Types.Subject> subjects;
        private List<Types.Department> departments;
        private List<Types.Holiday> holidays;
        private List<Types.Schoolyear> schoolyears;
        private List<Types.TimeTableElement> timeTableElements;

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
       /* private void loadSubjects(String data = "[]")
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

        }*/
		
		private void loadSubjectList(String data)
        {
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
                    else if( set[0] == "\"longNname\"")
                    {
                        newsubject.longname = set[1].Substring( 1, set[1].Length-2);
                    }
                }
                subjects.Add( newsubject);
             }
        }
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
        List<Types.Subject> getSubjects()
        {
            return subjects;
        }

        private void loadDepartmentList(String data)
        {
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
        List<Types.Department> getDepartments()
        {
            return departments;
        }

        private void loadHolidayList(String data)
        {
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
        List<Types.Holiday> getHolidays()
        {
            return holidays;
        }

        private void loadSchoolyearList(String data)
        {
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
            loadHolidayList("");
            foreach (Types.Schoolyear schoolyear in schoolyears)
            {
                if (schoolyear.id == id)
                {
                    return schoolyear;
                }
            }
            return new Types.Schoolyear();
        }
        List<Types.Schoolyear> getSchoolyears()
        {
            return schoolyears;
        }

        private void loadTimeTableElementList(String data)
        {
            timeTableElements = new List<Types.TimeTableElement>();
            data = data.Substring(2, data.Length - 4);
            String[] timeTableElementObject = Regex.Split(data, "\\},\\{");
            char[] dataCharArr = data.ToCharArray();
            int start = 0;
            int i = 0;
            int countOpen = 1;
            int countObjects = 1;
            int n = 0;

            foreach (String timeTableElementString in timeTableElementObject)
            {
                String[] namevalues = timeTableElementString.Split(',');
                Types.TimeTableElement newTimeTableElement = new Types.TimeTableElement();

                foreach (String namevalue in namevalues)
                {
                    String[] set = namevalue.Split(':');
                    if (set[0] == "\"id\"")
                    {
                        newTimeTableElement.id = Convert.ToInt32(set[1]);
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
                    else if (set[0] == "kl")
                    {
                        // set klassen ids

                        start = data.IndexOf("kl{[");
                        countObjects = 0;

                        if (start != -1)
                        {
                            countOpen = 1;

                            start +=  4;

                            for (i = start++; dataCharArr[i] != '\0'; i++)
                            {
                                if (dataCharArr[i] == ',')
                                    countObjects++;
                                else if (dataCharArr[i] == '{')
                                    countOpen++;
                                else if (dataCharArr[i] == '}')
                                {
                                    countOpen--;
                                    if (countOpen == 0)
                                        break;
                                }
                            }
                        }
                        if (countObjects == 0)
                        {
                            newTimeTableElement.classids = new int[1];
                        }
                        else
                        {
                            data = data.Substring( start, i-1);
                            String[] klasseIdObject = Regex.Split( data, "\\}\\,\\{");
                            newTimeTableElement.classids = new int[countObjects];

                            foreach (String klasseIdString in klasseIdObject)
                            {
                                String[] klasseIdNameValues = klasseIdString.Split(',');

                                foreach (String klasseIdNameValue in klasseIdNameValues)
                                {
                                    String[] setKlassenId = klasseIdNameValue.Split(':');
                                    n = 0;

                                    foreach (String setKlasseId in setKlassenId)
                                    {
                                        if (setKlasseId == "\"id\"")
                                            continue;
                                        else
                                        {
                                            newTimeTableElement.classids[n] = Convert.ToInt32( setKlasseId);
                                            n++;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else if (set[0] == "su")
                    {
                        // set subject ids

                        start = data.IndexOf("su{[");
                        countObjects = 0;

                        if (start != -1)
                        {
                            countOpen = 1;

                            start += 4;

                            for (i = start++; dataCharArr[i] != '\0'; i++)
                            {
                                if (dataCharArr[i] == ',')
                                    countObjects++;
                                else if (dataCharArr[i] == '{')
                                    countOpen++;
                                else if (dataCharArr[i] == '}')
                                {
                                    countOpen--;
                                    if (countOpen == 0)
                                        break;
                                }
                            }
                        }
                        if (countObjects == 0)
                        {
                            newTimeTableElement.subjectids = new int[1];
                        }
                        else
                        {
                            data = data.Substring(start, i - 1);
                            String[] subjectIdObject = Regex.Split(data, "\\}\\,\\{");
                            newTimeTableElement.subjectids = new int[countObjects];

                            foreach (String subjectIdString in subjectIdObject)
                            {
                                String[] subjectIdNameValues = subjectIdString.Split(',');

                                foreach (String subjectIdNameValue in subjectIdNameValues)
                                {
                                    String[] setSubjectIds = subjectIdNameValue.Split(':');
                                    n = 0;

                                    foreach (String setSubjectId in setSubjectIds)
                                    {
                                        if (setSubjectId == "\"id\"")
                                            continue;
                                        else
                                        {
                                            newTimeTableElement.subjectids[n] = Convert.ToInt32(setSubjectId);
                                            n++;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else if (set[0] == "ro")
                    {
                        // set room ids

                        start = data.IndexOf("ro{[");
                        countObjects = 0;

                        if (start != -1)
                        {
                            countOpen = 1;

                            start += 4;

                            for (i = start++; dataCharArr[i] != '\0'; i++)
                            {
                                if (dataCharArr[i] == ',')
                                    countObjects++;
                                else if (dataCharArr[i] == '{')
                                    countOpen++;
                                else if (dataCharArr[i] == '}')
                                {
                                    countOpen--;
                                    if (countOpen == 0)
                                        break;
                                }
                            }
                        }
                        if (countObjects == 0)
                        {
                            newTimeTableElement.roomids = new int[1];
                        }
                        else
                        {
                            data = data.Substring(start, i - 1);
                            String[] roomIdObject = Regex.Split(data, "\\}\\,\\{");
                            newTimeTableElement.roomids = new int[countObjects];

                            foreach (String roomIdString in roomIdObject)
                            {
                                String[] roomIdNameValues = roomIdString.Split(',');

                                foreach (String roomIdNameValue in roomIdNameValues)
                                {
                                    String[] setRoomIds = roomIdNameValue.Split(':');
                                    n = 0;

                                    foreach (String setRoomId in setRoomIds)
                                    {
                                        if (setRoomId == "\"id\"")
                                            continue;
                                        else
                                        {
                                            newTimeTableElement.roomids[n] = Convert.ToInt32(setRoomId);
                                            n++;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                timeTableElements.Add( newTimeTableElement);
            }
        }
        public Types.TimeTableElement getTimeTableElement(int id)
        {
            loadHolidayList("");
            foreach (Types.TimeTableElement timeTableElement in timeTableElements)
            {
                if (timeTableElement.id == id)
                {
                    return timeTableElement;
                }
            }
            return new Types.TimeTableElement();
        }
        List<Types.TimeTableElement> getTimeTableElements()
        {
            return timeTableElements;
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
