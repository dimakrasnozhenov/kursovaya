using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Data.Common;
using System.Windows.Forms;
using System.Security.Cryptography;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace Kursovaya
{
    class Database
    {
        private readonly string dataSource;
        public Database(string dataSource)
        {
            this.dataSource = dataSource;
        }
        public bool InitializeDatabase()
        {
            SQLiteConnection conn = new SQLiteConnection(dataSource);
            try
            {
                conn.Open();
                if (conn.State == ConnectionState.Open)
                {
                    SQLiteCommand cmd = conn.CreateCommand();
                    string sql_command = "DROP TABLE IF EXISTS users;"
                    + "CREATE TABLE users("
                    + "id INTEGER PRIMARY KEY AUTOINCREMENT, "
                    + "login TEXT, "
                    + "password TEXT, "
                    + "role TEXT);" +
                    "" +
                    "";
                    cmd.CommandText = sql_command;
                    cmd.ExecuteNonQuery();
                }
                conn.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка инициализации БД", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            finally
            {
                conn.Dispose();
            }
            return true;
        }
        private static string GetMD5Hash(string text) //для хранения пароля в хэшированном виде
        {
            using (var hashAlg = MD5.Create())
            {
                byte[] hash = hashAlg.ComputeHash(Encoding.UTF8.GetBytes(text));
                var builder = new StringBuilder(hash.Length * 2);
                for (int i = 0; i < hash.Length; i++)
                {
                    builder.Append(hash[i].ToString("X2"));
                }
                return builder.ToString();
            }
        }
        public bool createUser(string login, string password) //регистрация
        {
            SQLiteConnection conn = new SQLiteConnection(dataSource);

            try
            {
                conn.Open();
                if (conn.State == ConnectionState.Open)
                {
                    var pass = GetMD5Hash(password); //при регистрации пароль подается в БД сразу в хэшированном виде
                    SQLiteCommand cmd = conn.CreateCommand();
                    cmd.CommandText = string.Format("INSERT INTO users (login, password)"
                    + "VALUES ('{0}', '{1}')",
                    login, pass);
                    cmd.ExecuteNonQuery();
                }
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка регистрации", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            finally
            {
                conn.Dispose();
            }
        }
        public bool checkUser(User user) //проверка логина и пароля при авторизации
        {
            SQLiteConnection conn = new SQLiteConnection(dataSource);
            try
            {
                conn.Open();
                if (conn.State == ConnectionState.Open)
                {
                    var pass = GetMD5Hash(user.Password); //при авторизации пароль сразу подается в БД в хэшированном виде и сравнивается с хранящимся там
                    SQLiteCommand cmd = conn.CreateCommand();
                    cmd.CommandText = string.Format("SELECT COUNT(login) "
                    + "FROM users "
                    + "where login = '{0}' AND "
                    + "password = '{1}'",
                    user.Login, pass);
                    var usersCount = Convert.ToInt32(cmd.ExecuteScalar());
                    return usersCount > 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка авторизации", MessageBoxButtons.OK, MessageBoxIcon.Error); //выведем сообщение о возникшем исключении
                return false;
            }
            finally
            {
                conn.Close();
                conn.Dispose();
            }
            return false;
        }
        public bool Exists(string login)
        {
            SQLiteConnection conn = new SQLiteConnection(dataSource);
            try
            {
                conn.Open();
                SQLiteCommand cmd = new SQLiteCommand("SELECT * FROM users;", conn);
                SQLiteDataReader reader = cmd.ExecuteReader();
                string tempLogin;
                foreach (DbDataRecord record in reader)
                {
                    tempLogin = record["login"].ToString();
                    if (login == tempLogin)
                    {
                        reader.Close();
                        conn.Close();
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка регистрации", MessageBoxButtons.OK, MessageBoxIcon.Error); //выведем сообщение о возникшем исключении
            }
            finally
            {
                conn.Dispose();
            }
            return false;
        }
        public bool Sort(string name, string surname) // сортирует номера по алфавиту их владельцев (фамилия, имя)
        {
            SQLiteConnection conn = new SQLiteConnection(dataSource);
            try
            {
                conn.Open();
                SQLiteCommand cmd = conn.CreateCommand();
                cmd.CommandText = string.Format("SELECT number "
                    + "FROM phonebook "
                    + "ORDER BY surname, name");
                var number = Convert.ToString(cmd.ExecuteScalar());

                /**SQLiteDataReader phonebook = cmd.ExecuteReader();

                foreach (DbDataRecord phoneLine in phonebook)
                {
                    string phone = phoneLine["number"].ToString();
                }
                conn.Close();
                return true;**/

                ////using (var reader = cmd.ExecuteReader())
                ////{
                ////    var list = new List<object[]>();
                ////    while (reader.Read())
                ////    {
                ////        var arr = new object[1];
                ////        arr[0] = Convert.ToString(reader[0]);
                ////        list.Add(arr);
                ////    }
                ////    list.ToArray();
                ////}
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "При сортировке произошла ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error); //выведем сообщение о возникшем исключении
                return false;
            }
            finally
            {
                conn.Dispose();
            }
        }

        public string findUser(string name, string surname)
        {
            string noUser = "Человека с таким именем нет в справочнике";
            SQLiteConnection conn = new SQLiteConnection(dataSource);
            try
            {
                conn.Open();
                SQLiteCommand cmd = conn.CreateCommand();
                cmd.CommandText = string.Format("SELECT number "
                    + "FROM phonebook "
                    + "WHERE name = '{0}' AND "
                    + "surname = '{1}'",
                    name, surname);
                var number = Convert.ToString(cmd.ExecuteScalar());
                if (number != "")
                {
                    MessageBox.Show($"Номер пользователя {name} {surname}:\n\t{number}", "Телефонный справочник", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show($"Номер пользователя {name} {surname} не найден в справочнике:(", "Телефонный справочник", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                return number;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "При поиске произошла ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error); //выведем сообщение о возникшем исключении
                return noUser;
            }
            finally
            {
                conn.Dispose();
            }
        }
        public string[] NametoArray()
        {
            SQLiteConnection conn = new SQLiteConnection(dataSource);
            conn.Open();
            SQLiteCommand cmd = conn.CreateCommand();
            cmd.CommandText = string.Format("SELECT * "
                + "FROM phonebook "
                + "ORDER BY surname, name");

            SQLiteDataReader phonebook = cmd.ExecuteReader();
            int counter = 0;
            foreach (DbDataRecord phoneLine in phonebook)
            {
                counter = counter + 1;

            }
            conn.Close();

            conn.Open();
            cmd = conn.CreateCommand();
            cmd.CommandText = string.Format("SELECT * "
                + "FROM phonebook "
                + "ORDER BY surname, name");

            phonebook = cmd.ExecuteReader();
            string[] array = new string[counter];
            int point = 0;
            foreach (DbDataRecord phoneLine in phonebook)
            {
                string phone = phoneLine["surname"].ToString() + " " + phoneLine["name"].ToString();
                array[point] = phone;
                point = point + 1;
            }
            conn.Close();
            return array;
        }

        public string ReturnNumber(int index)
        {
            SQLiteConnection conn = new SQLiteConnection(dataSource);
            conn.Open();
            SQLiteCommand cmd = conn.CreateCommand();
            cmd.CommandText = string.Format("SELECT number "
                + "FROM phonebook "
                + "ORDER BY surname, name");

            SQLiteDataReader phonebook = cmd.ExecuteReader();
            int counter = 0;
            foreach (DbDataRecord phoneLine in phonebook)
            {
                if (counter == index)
                {
                    string number = phoneLine["number"].ToString();
                    conn.Close();
                    return number;
                }
                counter = counter + 1;

            }
            conn.Close();
            return "";
        }
    }
}
