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



        #endregion
        Object classname = testclass;
       
        //Teacher
        String firstname;
        String lastname;
        String shortname;

        //Klasse
        String name;
        String longname;
        int did;

        classname cn = new classname();

        #region Methods
       
        private Subject makeSubjectObject()
        {

            return Subject;
        }

        #endregion
        cn.getTeacher(int id){
        firstname = cn.GetValue(firstname);
        lastname = cn.GetValue(lastname);
        shortname = cn.GetValue(shortname);
    }
        cn.getKlasse(int id){
        name = cn.GetValue(name);
        longname = cn.GetValue(longname);
    
    }
        cn.getSubject(int id){
    }
        cn.getDepartment(int id){
    }
        cn.getHoliday(int id){
    }
        cn.getSchoolyear(int id){
    }
        cn.getTimeTableElement(int id){
    }
}
}