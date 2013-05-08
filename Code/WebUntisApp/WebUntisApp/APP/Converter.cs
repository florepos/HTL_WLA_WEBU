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

namespace WebUntisAPP
{
    /// <summary>
    /// This Class transfers the Data from the API-Team Classes to the Design-Team Pages so that
    /// the Design Team must not do any formatting work. They just have to use our methods and go
    /// on with the delivered data!
    /// 
    /// --The App-Team
    /// </summary>
    public class Converter
    {
        #region Variables

        //Variablen für API Team
        private static Uri URL;
        private static String school;
        private static String user;
        private static String password;
        WebUntisAPI wAPI = new WebUntisAPI(URL, school, user, password);

        //Teacher
        private String strTeacherFirstname;
        private String strTeacherLastname;
        private String strTeacherShortname;

        //Klasse
        private String strKlasseName;
        private String strKlasseLongname;

        //Fach
        private String[] strSubjectName;
        private String[] strSubjectLongname;

        //Raum
        private String room;

        #endregion

        public Converter(Uri uriURL, String schule, String benutzer, String passwort)
        {
            URL = uriURL;
            school = schule;
            user = benutzer;
            password = passwort;
        }

        #region Methods
       
        private Subject makeSubjectObject()
        {
            return null;
        }

        private void makeTimetable(int weeknumber)
        {

        }

        public Subject[][][] getWeekTimetable(int weeknumber)
        {
            return null;
        }

        private void setVariables()
        {
            Types.TimeTableElement timeTabelElement = wAPI.getTimeTableElement(intTimeTableElementId);
            int[] intKlasseId = timeTabelElement.classids;
            int[] intSubjectId = timeTabelElement.subjectids;
            int[] intRoomId = timeTabelElement.roomids;
           
            //foreach (int i in intKlasseId)
            //{
            //    Types.Klasse klasse = wAPI.getKlasse(intKlasseId[i]); <-- Methode gibt noch nicht!
            //    strKlasseName = ...
            //    strKlasseLongname = ...
            //}
            
            //Es werden alle Fächer des TimeTableElements in die Variablen gespeichert!
            foreach (int i in intSubjectId)
            {
                Types.Subject subject = wAPI.getSubject(intSubjectId[i]);
                strSubjectLongname[i] = subject.longname;
                strSubjectName[i] = subject.name;
            }

            //Es werden alle Räume des TimeTableElements in die Variablen gespiechert!
            foreach (int i in intRoomId)
            {
                Types.Room room = wAPI.getRoom(intRoomId[i]);

                
            }
            
            
            //Types.Department department = wAPI.getDepartment(intDepartmentId);
            //Types.Holiday holiday = wAPI.getHoliday(intHolidayId);
            //Types.Schoolyear schoolyear = wAPI.getSchoolyear(i);
            
            
            
        }

        #endregion
        

}

