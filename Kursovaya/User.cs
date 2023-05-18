using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace Kursovaya
{
    class User
    {
        private string login;
        private string password;
        //Database database = new Database();

        public User(string login, string password /*string passconf = null*/)
        {
            this.login = login;
            this.password = password;
            //this.passconf = passconf;
        }

        public string Login { get => login; }
        public string Password { get => password; }

        public bool IsCorrect()
        {
            Database database = new Database("Data Source = dataBase.db; Version = 3; ");

            if (database.checkUser(this))
                return true;

            return false;
        }
    }
}
