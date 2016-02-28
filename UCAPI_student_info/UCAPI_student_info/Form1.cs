using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MongoDB.Driver;
using MongoDB.Bson;
using System.IO;
using UCAPI_student_info.Classes;

namespace UCAPI_student_info
{
    public partial class Form1 : Form
    {
        List<db_document> users;
        db_document user;

        public Form1()
        {
            InitializeComponent();

            /********************************************************************************
             ********************************************************************************                        
             *****                                                                      *****
             *****                      CODE FROM WEBSITE:                              *****
             *****                                                                      *****
             *****  https://msdn.microsoft.com/en-us/library/3s8ys666(v=vs.110).aspx    *****
             *****                                                                      *****
             *****                                                                      *****
             ********************************************************************************
             ********************************************************************************/
            this.browser.Navigate("http://www.google.com");
            SetupDataGridView();
        }

        private async void Form1_Load(object sender, EventArgs e)
        {
            var dbfunc = new db_functions();
            await dbfunc.setup_users();
            users = dbfunc.users;
            PopulateDataGridView(users);
            foreach(db_document u in users)
            {
                //set global user to u
                user = u;

                //canopy
                this.browser.Navigate("https://canopy.uc.edu/webapps/login/?action=relogin");
                browser.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(canopy_info);
            }
        }

        private void SetupDataGridView()
        {
            this.Controls.Add(dataGrid);

            //set amount of columns on datagrid
            dataGrid.ColumnCount = 3;

            //set style to header
            dataGrid.ColumnHeadersDefaultCellStyle.BackColor = Color.Navy;
            dataGrid.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dataGrid.ColumnHeadersDefaultCellStyle.Font =
                new Font(dataGrid.Font, FontStyle.Bold);

            //set column names
            dataGrid.Columns[0].Name = "ID";
            dataGrid.Columns[1].Name = "username";
            dataGrid.Columns[2].Name = "password";
            dataGrid.SelectionMode =
                DataGridViewSelectionMode.FullRowSelect;
            dataGrid.MultiSelect = false;
        }

        private void PopulateDataGridView(List<db_document> users)
        {
            var lou = new List<string[]>();
            foreach (db_document u in users)
            {
                var lov = new List<string>();
                var id = "";
                var username = "";
                var password = "";
                foreach(db_values v in u.variables)
                {
                    switch (v.name)
                    {
                        case "username":
                            username = v.value;
                            break;
                        case "password":
                            password = v.value;
                            break;
                        case "_id":
                            id = v.value;
                            break;
                        default:
                            break;
                    }
                }
                lou.Add(new string[] { id, username, password });
            }
            foreach(string[] v in lou)
            {
                dataGrid.Rows.Add(v);
            }
            dataGrid.Columns[0].DisplayIndex = 0;
            dataGrid.Columns[1].DisplayIndex = 1;
            dataGrid.Columns[2].DisplayIndex = 2;
        }

        private void canopy_info(object sender, WebBrowserDocumentCompletedEventArgs e)
        {

            //login to canopy
            if (e.Url.OriginalString.Equals("https://canopy.uc.edu/webapps/login/?action=relogin"))
            {
                var txtUsername = browser.Document.GetElementById("user_id");
                var txtPassword = browser.Document.GetElementById("password");
                var btnLogin = browser.Document.GetElementById("entry-login");
                
                txtPassword.InnerText = user.password;
                txtUsername.InnerText = user.username;

                btnLogin.InvokeMember("Click");
            }

            //get information from canopy
            if(e.Url.OriginalString.Equals(""))
            {

                // ALL <a> TAGS BELOW ARE CLASSES
                /*
                <div id="_3_1termCourses__175_1" style="">
                    <h4 class="u_indent" id="anonymous_element_16">Courses where you are: Student</h4> 
                    <ul class="portletList-img courseListing coursefakeclass u_indent">
                        <li>
                            <img alt="" src="/images/ci/icons/bookopen_li.gif" width="12" height="12">
                            <a href=" /webapps/blackboard/execute/launcher?type=Course&amp;id=_166334_1&amp;url=" target="_top">16SS_IT2021003: (16SS) HUMAN COMP INTERACT (003)</a>
                        </li>
                        <li>
                            <img alt="" src="/images/ci/icons/bookopen_li.gif" width="12" height="12">
                            <a href=" /webapps/blackboard/execute/launcher?type=Course&amp;id=_161272_1&amp;url=" target="_top">16SS_IT4020001: (16SS) MANAGEMENT IN IT (001)</a>
                        </li>
                        <li>
                            <img alt="" src="/images/ci/icons/bookopen_li.gif" width="12" height="12">
                            <a href=" /webapps/blackboard/execute/launcher?type=Course&amp;id=_161273_1&amp;url=" target="_top">16SS_IT5002001: (16SS) SR PROJ MGT II (001)</a>
                        </li>
                        <li>
                            <img alt="" src="/images/ci/icons/bookopen_li.gif" width="12" height="12">
                            <a href=" /webapps/blackboard/execute/launcher?type=Course&amp;id=_161276_1&amp;url=" target="_top">16SS_IT5042001: (16SS) SFT APP DEV PRAC II (001)</a>
                        </li>
                    </ul>
                </div>
                */
            }

        }
    }
}
