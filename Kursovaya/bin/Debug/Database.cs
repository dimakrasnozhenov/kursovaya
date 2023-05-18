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
            catch
            {
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

            conn.Open();
            if (conn.State == ConnectionState.Open)
            {
                var pass = GetMD5Hash(password); //при регистрации пароль подается в БД сразу в хэшированном виде
                SQLiteCommand cmd = conn.CreateCommand();
                cmd.CommandText = string.Format("INSERT INTO users (login, password)"
                + "VALUES ('{0}', '{1}')",
                //+"WHERE NOT EXISTS (SELECT login FROM users WHERE login = '{0}')", //(НЕ РАБОТАЕТ) будет создвать юзера только если такого логина уже нет в БД
                login, pass);
                cmd.ExecuteNonQuery();
                return true;
            }
            return true;
            conn.Close();
            conn.Dispose();
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
                    cmd.CommandText = string.Format("SELECT COUNT(login)"
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
                MessageBox.Show(ex.Message, "Ошибка авторизации", MessageBoxButtons.OK, MessageBoxIcon.Information); //выведем сообщение о возникшем исключении
                return false;
            }
            finally
            {
                conn.Close();
                conn.Dispose();
            }
            return false;
        }
        //public bool alreadyExists(User user) // проверка на наличие в БД записи с таким же логином (В РАБОТЕ)
        //{
        //    SQLiteConnection conn = new SQLiteConnection(dataSource);
        //    try
        //    {
        //        conn.Open();
        //        if (conn.State == ConnectionState.Open)
        //        {
        //            //SQLiteCommand cmd = conn.CreateCommand();
        //            //cmd.CommandText = string.Format("SELECT COUNT(login)"
        //            //+ "FROM users "
        //            //+ "where login = '{0}' AND "
        //            //+ "password = '{1}'",
        //            //user.Login, user.Password);
        //            //var usersCount = Convert.ToInt32(cmd.ExecuteScalar());
        //            //return usersCount > 0;

        //            SQLiteCommand cmd = conn.CreateCommand();
        //            cmd.CommandText = string.Format("SELECT COUNT(*)"
        //                + "FROM users "
        //                + "where login = '{0}'",
        //                user.Login);
        //            var usersCount = Convert.ToInt32(cmd.ExecuteScalar());
        //            if (usersCount > 0)
        //            {
        //                MessageBox.Show("Аккаунт с таким логином уже существует. Попробуйте придумать другой", "Провал");
        //            }

        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return false;
        //    }
        //    finally
        //    {
        //        conn.Dispose();
        //    }
        //    return false;
        //}
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
                MessageBox.Show(ex.Message, "Ошибка регистрации", MessageBoxButtons.OK, MessageBoxIcon.Information); //выведем сообщение о возникшем исключении
            }
            finally
            {
                conn.Dispose();
            }
            return false;
        }
        public string findUser(string name, string surname)
        {
            string noUser = "Человека с таким именем нет в справочнике";
            SQLiteConnection conn = new SQLiteConnection(dataSource);
            try
            {
                conn.Open();
                SQLiteCommand cmd = conn.CreateCommand();
                cmd.CommandText = string.Format("SELECT number"
                    + "FROM phonebook "
                    + "WHERE name = '{0}' AND "
                    + "surname = '{1}'",
                    name, surname);
                var number = Convert.ToString(cmd.ExecuteScalar());
                return number;
                conn.Close();

                //    new SQLiteCommand("SELECT number FROM phonebook WHERE name = '{0}' AND surname = '{1}';", user conn);
                //SQLiteDataReader reader = cmd.ExecuteReader();
                //foreach (DbDataRecord record in reader)
                //{

                //}
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "При поиске произошла ошибка", MessageBoxButtons.OK, MessageBoxIcon.Information); //выведем сообщение о возникшем исключении
                return noUser;
            }
            finally
            {
                conn.Dispose();
            }
        }
    }
}
