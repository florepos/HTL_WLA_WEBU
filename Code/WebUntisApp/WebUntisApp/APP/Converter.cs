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

namespace AppTestSolution.otherClasses
{
    /// <summary>
    /// This Class transfers the Data from the API-Team Classes to the Design-Team Pages so that
    /// the Design Team must not do any formatting work. They just have to use our methods and go
    /// on with the delivered data!
    /// 
    /// --The App-Team
    /// </summary>
    public class AppClass
    {
        #region Variables

        private String strSubName;

        #endregion

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


        #endregion

        Object classname = classname;
       
        //Teacher
        private String strFirstname;
        private String lastname;
        private String shortname;

        //Klasse
        private String name;
        private String longname;
        private int did;

        //für FabiansClass muss später die Klasse des API Teams eingefügt werden!!!
        Types.Teacher teacher = WebuntisAPI.Types.

        cn.getTeacher(int id)
        {
            strFirstname = cn.GetValue(firstname);
            lastname = cn.GetValue(lastname);
            shortname = cn.GetValue(shortname);
        }
        cn.getKlasse(int id){
        name = cn.GetValue(name);
        longname = cn.GetValue(longname);
    
        }
        
        cn.getAllTeachers(){}
        

}

