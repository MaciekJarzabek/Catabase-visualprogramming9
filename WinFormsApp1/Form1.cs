using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;
using Microsoft.VisualBasic.ApplicationServices;

namespace WinFormsApp1
{
    public partial class Form1 : Form
    {
        private readonly string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=""C:\Users\mjarz\source\repos\WinFormsApp1\WinFormsApp1\Database1.mdf"";Integrated Security=True";
        private readonly DatabaseManager dbManager;

        public Form1()
        {
            InitializeComponent();
            dbManager = new DatabaseManager(connectionString);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!ValidateInputs()) return;
            var user = GetUserFromInputs();
            dbManager.WriteData(user);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var users = dbManager.ReadData();
            UpdateListBox(users);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Czy na pewno chcesz wyczyœciæ wszystkie dane z bazy?", "Potwierdzenie", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                dbManager.ClearData();
                MessageBox.Show("Baza danych zosta³a wyczyszczona.");
            }
        }

        private bool ValidateInputs()
        {
            foreach (Control control in Controls)
            {
                if (control is TextBox textBox && string.IsNullOrWhiteSpace(textBox.Text))
                {
                    MessageBox.Show($"{textBox.Tag} nie mo¿e byæ puste.");
                    return false;
                }
            }
            if (!DateTime.TryParseExact(textBox2.Text, "dd.MM.yyyy", null, System.Globalization.DateTimeStyles.None, out _) || !DateTime.TryParseExact(textBox14.Text, "dd.MM.yyyy", null, System.Globalization.DateTimeStyles.None, out _))
            {
                MessageBox.Show("Data musi byæ w formacie DD.MM.YYYY.");
                return false;
            }
            if (!int.TryParse(textBox7.Text, out _))
            {
                MessageBox.Show("Punkty musz¹ byæ liczb¹ ca³kowit¹.");
                return false;
            }
            return true;
        }

        private User GetUserFromInputs()
        {
            return new User
            {
                nr_albumu = textBox1.Text,
                data1 = DateTime.ParseExact(textBox2.Text, "dd.MM.yyyy", null),
                imie = textBox3.Text,
                sem = textBox4.Text,
                kierunek = textBox5.Text,
                przedmiot = textBox6.Text,
                punkty = int.Parse(textBox7.Text),
                prowadzacy = textBox8.Text,
                uzasadnienie = textBox9.Text,
                podpis1 = textBox10.Text,
                komisja1 = textBox11.Text,
                komisja2 = textBox12.Text,
                komisja3 = textBox13.Text,
                data2 = DateTime.ParseExact(textBox14.Text, "dd.MM.yyyy", null),
                podpis2 = textBox15.Text
            };
        }

        private void UpdateListBox(List<User> users)
        {
            listBox1.Items.Clear();
            if (users.Count == 0)
            {
                MessageBox.Show("Nie znaleziono ¿adnych danych do odczytu.");
                return;
            }
            foreach (var user in users)
            {
                listBox1.Items.Add("Nr albumu: " + user.nr_albumu);
                listBox1.Items.Add("Data 1: " + user.data1.ToString("dd.MM.yyyy"));
                listBox1.Items.Add("Imiê: " + user.imie);
                listBox1.Items.Add("Semestr: " + user.sem);
                listBox1.Items.Add("Kierunek: " + user.kierunek);
                listBox1.Items.Add("Przedmiot: " + user.przedmiot);
                listBox1.Items.Add("Punkty: " + user.punkty);
                listBox1.Items.Add("Prowadz¹cy: " + user.prowadzacy);
                listBox1.Items.Add("Uzasadnienie: " + user.uzasadnienie);
                listBox1.Items.Add("Podpis 1: " + user.podpis1);
                listBox1.Items.Add("Komisja 1: " + user.komisja1);
                listBox1.Items.Add("Komisja 2: " + user.komisja2);
                listBox1.Items.Add("Komisja 3: " + user.komisja3);
                listBox1.Items.Add("Data 2: " + user.data2.ToString("dd.MM.yyyy"));
                listBox1.Items.Add("Podpis 2: " + user.podpis2);
                listBox1.Items.Add("*************************************");
                listBox1.Items.Add(""); 
            }
        }
    }

    public class DatabaseManager
    {
        private readonly string connectionString;

        public DatabaseManager(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public List<User> ReadData()
        {
            var userList = new List<User>();
            using (var connection = new SqlConnection(connectionString))
            using (var command = new SqlCommand("SELECT * FROM [dbo].[Table]", connection))
            {
                try
                {
                    connection.Open();
                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        var user = new User();
                        for (var i = 0; i < reader.FieldCount; i++)
                        {
                            var propName = reader.GetName(i);
                            var propValue = reader.GetValue(i);
                            typeof(User).GetProperty(propName)?.SetValue(user, propValue);
                        }
                        userList.Add(user);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error reading data: " + ex.Message);
                }
            }
            return userList;
        }

        public void WriteData(User user)
        {
            using (var connection = new SqlConnection(connectionString))
            using (var command = new SqlCommand("INSERT INTO [dbo].[Table] VALUES (@nr_albumu, @data1, @imie, @sem, @kierunek, @przedmiot, @punkty, @prowadzacy, @uzasadnienie, @podpis1, @komisja1, @komisja2, @komisja3, @data2, @podpis2)", connection))
            {
                command.Parameters.AddWithValue("@nr_albumu", user.nr_albumu);
                command.Parameters.AddWithValue("@data1", user.data1);
                command.Parameters.AddWithValue("@imie", user.imie);
                command.Parameters.AddWithValue("@sem", user.sem);
                command.Parameters.AddWithValue("@kierunek", user.kierunek);
                command.Parameters.AddWithValue("@przedmiot", user.przedmiot);
                command.Parameters.AddWithValue("@punkty", user.punkty);
                command.Parameters.AddWithValue("@prowadzacy", user.prowadzacy);
                command.Parameters.AddWithValue("@uzasadnienie", user.uzasadnienie);
                command.Parameters.AddWithValue("@podpis1", user.podpis1);
                command.Parameters.AddWithValue("@komisja1", user.komisja1);
                command.Parameters.AddWithValue("@komisja2", user.komisja2);
                command.Parameters.AddWithValue("@komisja3", user.komisja3);
                command.Parameters.AddWithValue("@data2", user.data2);
                command.Parameters.AddWithValue("@podpis2", user.podpis2);
                try
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                    MessageBox.Show("Dane zapisane pomyœlnie.");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error writing data " + ex.Message);
                }
            }
        }

        public void ClearData()
        {
            using (var connection = new SqlConnection(connectionString))
            using (var command = new SqlCommand("DELETE FROM [dbo].[Table]", connection))
            {
                try
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error clearing data: " + ex.Message);
                }
            }
        }
    }

    public class User
    {
        public string nr_albumu { get; set; }
        public DateTime data1 { get; set; }
        public string imie { get; set; }
        public string sem { get; set; }
        public string kierunek { get; set; }
        public string przedmiot { get; set; }
        public int punkty { get; set; }
        public string prowadzacy { get; set; }
        public string uzasadnienie { get; set; }
        public string podpis1 { get; set; }
        public string komisja1 { get; set; }
        public string komisja2 { get; set; }
        public string komisja3 { get; set; }
        public DateTime data2 { get; set; }
        public string podpis2 { get; set; }
    }
}
