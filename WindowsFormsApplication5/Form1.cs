using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Data.SqlClient;

//using Access = Microsoft.Office.Interop.Access;

namespace WindowsFormsApplication5
{
    public partial class Form1 : Form
    {
        private DataSet ds = new DataSet();
        public string openFilePath { get; set; }
        public string saveFilePath { get; set; }
        public Form1()
        {
            InitializeComponent();
        }

        private string OpenFile() 
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            string path="";
            DialogResult result = openFileDialog1.ShowDialog(); // Show the dialog.
            if (result == DialogResult.OK) // Test result.
            {
                path = openFileDialog1.FileName;

            }
            return path;
        }

        private bool CopyFile(string fileToCopy, string newLocation) 
        {
            if (System.IO.File.Exists(fileToCopy))
            {
                if (fileToCopy == newLocation) 
                {
                    lbl_Message.ForeColor = Color.Red;
                    lbl_Message.Text = "You select your past file, pls choose another file."; 
                }
                System.IO.File.Copy(fileToCopy, newLocation, true);
                return true;
            }
            else
            {
                return false;
            }        
        }



       

        private void btn_browse_Click(object sender, EventArgs e)
        {
            openFilePath = OpenFile();
            textBox1.Text =openFilePath ;
            lbl_Message.ForeColor = Color.Red;

            if (!File.Exists(openFilePath))
            {
                lbl_Message.Text = "File path is incorrect!";
            }
            else
            {
                ds = AccessDBLoader.LoadFromFile(openFilePath);
                if (ds == null)
                {
                    lbl_Message.Text = "Can't open your Database!";
                }
                else
                {
                    if (ds.Tables.Count == 0)
                    {
                        lbl_Message.Text = "Your Database is empty,\n please select an other file.";
                    }
                    else
                    {
                        dataGridView1.DataSource = ds.Tables[0];
                    }
                }
            }
        }

        private void btn_run_Click(object sender, EventArgs e)
        {
            

            lbl_Message.ForeColor = Color.Red;
            lbl_Message.Text = "";

            ds = AccessDBLoader.LoadFromFile(openFilePath);
            ds = SQLDBLoader.RunQuery(textBox3.Text, ds);

            if (ds == null)
            {
                lbl_Message.Text = "Query doesn't run correctly!";
            }
            else
            {

                lbl_Message.ForeColor = Color.Blue;
                lbl_Message.Text = "Loading...";

                saveFileDialog1.ShowDialog();
                CopyFile(openFilePath, saveFilePath);

                if (!AccessDBLoader.UploadToFile(saveFilePath, ds))
                {
                    lbl_Message.ForeColor = Color.Red;
                    lbl_Message.Text = "Records of Access template not matched by query! \n OR \n Access template isn't empty!";
                }
                else
                {
                    if (lbl_Message.Text == "Loading...")
                    {
                        lbl_Message.ForeColor = Color.Green;
                        lbl_Message.Text = "Program run successfully";
                    }
                }
            }
        }

        private void saveFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            saveFilePath = saveFileDialog1.FileName;
            File.Create(saveFilePath).Dispose();
        }





        //void RunAccessModule(string str)
        //{
        //    try
        //    {
        //        Access.Application oAccess = new Access.Application();
        //        oAccess.OpenCurrentDatabase(str, false);
        //        query = oAccess.Run(textBox3.Text.ToString()).ToString();
        //        textBox2.Text = query;
        //        oAccess.CloseCurrentDatabase();
        //    }
        //    catch (Exception ex)
        //    {
        //        // ex.Message;
        //    }

        //}

     
    }
}
