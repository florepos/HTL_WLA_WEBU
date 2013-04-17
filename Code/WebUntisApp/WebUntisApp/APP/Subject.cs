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
    /// This Class represents a Subject with one or more Teachers, with on ore
    /// more Schoolclasses, one Schoolhour long and with all information about
    /// Tests, Supplierungen and Class eliminations.
    /// </summary>
    public class Subject
    {
        #region Variables & Constructor

        private String strSubName;
        private String strSubNameShort;
        private String strRoom;
        private String[] strTeacherName;
        private String[] strTeacherNameShort;
        private String[] strClass;
        private Boolean bolTest;
        private Boolean bolSup;
        private Boolean bolElimination;

        /// <summary>
        /// Subject Constructor
        /// </summary>
        /// <param name="strSubName">Subject Name</param>
        /// <param name="strSubNameShort">Subject abbreviation</param>
        /// <param name="strRoom">The Room where the Subject will take place</param>
        /// <param name="strTeacherName">The Subject's Teacher Name</param>
        /// <param name="strTeacherNameShort">The abbreviation of the Teacher</param>
        /// <param name="strClass">All Schoolclasses wich join the Subject</param>
        /// <param name="bolTest">If there is a test in this lesson</param>
        /// <param name="bolSup">If there is a replacement Subject</param>
        /// <param name="bolElimination">If the subject is eliminated</param>
        public Subject( String strSubName,
                        String strSubNameShort,
                        String strRoom,
                        String[] strTeacherName,
                        String[] strTeacherNameShort,
                        String[] strClass,
                        Boolean bolTest,
                        Boolean bolSup,
                        Boolean bolElimination
                      )
        {
            this.strSubName = strSubName;
            this.strSubNameShort = strSubNameShort;
            this.strRoom = strRoom;
            this.strTeacherName = strTeacherName;
            this.strTeacherNameShort = strTeacherNameShort;
            this.strClass = strClass;
            this.bolTest = bolTest;
            this.bolSup = bolSup;
            this.bolElimination = bolElimination;
        }

        #endregion

        #region Getters & Setters

        public String getStrSubName()
        {
            return this.strSubName;
        }

        public String getStrSubNameShort()
        {
            return this.strSubNameShort;
        }

        public String getStrRoom()
        {
            return this.strRoom;
        }

        public String[] getStrTeacherName()
        {
            return this.strTeacherName;
        }

        public String[] getStrTeacherNameShort()
        {
            return this.strTeacherNameShort;
        }

        public String[] getStrClass()
        {
            return this.strClass;
        }

        public Boolean getBolTest()
        {
            return this.bolTest;
        }

        public Boolean getBolSup()
        {
            return this.bolSup;
        }

        public Boolean getBolElimination()
        {
            return this.bolElimination;
        }

        public void setStrSubName(String name)
        {
            this.strSubName = name;
        }

        public void setStrSubNameShort(String name)
        {
            this.strSubNameShort = name;
        }

        public void setStrRoom(String room)
        {
            this.strRoom = room;
        }

        public void setStrTeacherName(String name, int i)
        {
            this.strTeacherName[i] = name;
        }

        public void setStrTeacherNameShort(String name, int i)
        {
            this.strTeacherNameShort[i] = name;
        }

        public void setStrClass(String name, int i)
        {
            this.strClass[i] = name;
        }

        public void setBolTest(bool test)
        {
            this.bolTest = test;
        }

        public void setBolSup(bool sup)
        {
            this.bolSup = sup;
        }

        public void setBolElimination(bool elimination)
        {
            this.bolElimination = elimination;
        }

        #endregion

    }
}
