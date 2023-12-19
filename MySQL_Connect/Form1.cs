using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace MySQL_Connect
{
    public partial class Form1 : Form
    {
        private MySqlConnection connection;
        private string server;
        private string database;
        private string user;
        private string password;
        private string port;
        private string connectionString;
        private string sslM;
        public Form1()
        {
            InitializeComponent();

            server = "localhost";
            database = "testdb";
            user = "root";
            password = "";
            port = "3306";
            sslM = "none";
            connectionString = String.Format("server={0}; port={1}; userid={2}; password={3}; database={4}; SslMode={5}",
                                     server, port, user, password, database, sslM);

            connection = new MySqlConnection(connectionString);
        }
        private void LoadtoListview()
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString)) 
            {
                try
                {
                    connection.Open();

                    string query = "SELECT * FROM phonebook";
                    using (MySqlCommand cmd = new MySqlCommand(query, connection))
                    {
                        using (MySqlDataReader dataReader = cmd.ExecuteReader())
                        {
                            listView1.Items.Clear();

                            while (dataReader.Read())
                            {
                                ListViewItem item = new ListViewItem(dataReader["id"].ToString());
                                item.SubItems.Add(dataReader["Name"].ToString());
                                item.SubItems.Add(dataReader["Mobile"].ToString());
                                listView1.Items.Add(item);
                            }
                        }
                    }
                }
                catch (Exception ex) 
                {
                    MessageBox.Show("เกิดข้อผิดพลาด: " + ex.Message);
                }
                
            }
        }

        public  bool conexion()
        {
            try
            {
                connection.Open();
                MessageBox.Show("Successful Connection", "Connection Data Base", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return true;
            }
            catch (MySqlException ex)
            {
                MessageBox.Show(ex.Message + connectionString);
                return false;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            listView1.Columns.Add("Id", 50);
            listView1.Columns.Add("Name", 120);
            listView1.Columns.Add("Mobile", 70);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            conexion();
            LoadtoListview();

            string query = "SELECT * FROM phonebook";
            MySqlCommand cmd = new MySqlCommand(query, connection);
            MySqlDataReader dataReader = cmd.ExecuteReader();
            StringBuilder Bulabel = new StringBuilder();
            
            while (dataReader.Read())
            {
                Bulabel.Append("id:" + dataReader.GetString(0) + " Name:" + dataReader.GetString(1) + " Mobile:" + dataReader.GetString(2) + "\n");
                label2.Text = Bulabel.ToString();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string id = textBox1.Text;
            string Name = textBox2.Text;
            string Mobile = textBox3.Text;

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string query = "INSERT INTO phonebook (Id, Name , Mobile) VALUES (@id , @Name , @Mobile)";
                    using (MySqlCommand cmd = new MySqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@id", id);
                        cmd.Parameters.AddWithValue("@Name", Name);
                        cmd.Parameters.AddWithValue("@Mobile", Mobile);
                        cmd.ExecuteNonQuery();
                        MessageBox.Show("ข้อมูลเพิ่มลงในฐานข้อมูลแล้ว");

                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("เกิดข้อผิดพลาด: " + ex.Message);
                }
            }

        }

        private void button3_Click(object sender, EventArgs e)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    string query = "SELECT * FROM phonebook";
                    using (MySqlCommand cmd = new MySqlCommand(query, connection))
                    {
                        MySqlDataReader dataReader = cmd.ExecuteReader();
                        StringBuilder Bulabel = new StringBuilder();

                        listView1.Items.Clear();

                        while (dataReader.Read())
                        {
                            Bulabel.Append("id:" + dataReader.GetString(0) + "Name:" + dataReader.GetString(1) + "Mobile:" + dataReader.GetString(2) + "\n");
                            label2.Text = Bulabel.ToString();

                            ListViewItem item = new ListViewItem(dataReader.GetString(0));
                            item.SubItems.Add(dataReader.GetString(1));
                            item.SubItems.Add(dataReader.GetString(2));
                            listView1.Items.Add(item); 


                        }


                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("เกิดข้อผิดพลาด: " + ex.Message);
                }

            }
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(listView1.SelectedItems.Count > 0)
            {
                textBox1.Text = listView1.SelectedItems[0].SubItems[0].Text;
                textBox2.Text = listView1.SelectedItems[0].SubItems[1].Text;
                textBox3.Text = listView1.SelectedItems[0].SubItems[2].Text;
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                string id = listView1.SelectedItems[0].SubItems[0].Text; // Assuming ID is in the first column (index 0)
                string newName = textBox2.Text; // Assuming the TextBox for Name is textBox2
                string newMobile = textBox3.Text; // Assuming the TextBox for Mobile is textBox3

                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    try
                    {
                        connection.Open();

                        string query = "UPDATE phonebook SET Name=@Name, Mobile=@Mobile WHERE id=@Id";
                        using (MySqlCommand cmd = new MySqlCommand(query, connection))
                        {
                            cmd.Parameters.AddWithValue("@Name", newName);
                            cmd.Parameters.AddWithValue("@Mobile", newMobile);
                            cmd.Parameters.AddWithValue("@Id", id);

                            int rowsAffected = cmd.ExecuteNonQuery();

                            if (rowsAffected > 0)
                            {
                                MessageBox.Show("Record updated successfully.");
                                LoadtoListview(); // Refresh the ListView after update
                            }
                            else
                            {
                                MessageBox.Show("No records updated.");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error updating record: " + ex.Message);
                    }
                }
            }
            else
            {
                MessageBox.Show("Please select a record to update.");
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                string id = listView1.SelectedItems[0].SubItems[0].Text; // Assuming ID is in the first column (index 0)

                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    try
                    {
                        connection.Open();

                        string query = "DELETE FROM phonebook WHERE id=@Id";
                        using (MySqlCommand cmd = new MySqlCommand(query, connection))
                        {
                            cmd.Parameters.AddWithValue("@Id", id);

                            int rowsAffected = cmd.ExecuteNonQuery();

                            if (rowsAffected > 0)
                            {
                                MessageBox.Show("Record deleted successfully.");
                                LoadtoListview(); // Refresh the ListView after delete
                            }
                            else
                            {
                                MessageBox.Show("No records deleted.");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error deleting record: " + ex.Message);
                    }
                }
            }
            else
            {
                MessageBox.Show("Please select a record to delete.");
            }
        }
    }
}
