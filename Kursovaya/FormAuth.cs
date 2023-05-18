using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Button;

namespace Kursovaya
{
    public partial class FormAuth : Form
    {
        public FormAuth()
        {
            InitializeComponent();
            StartPosition = FormStartPosition.CenterScreen;
            textBoxLogin.MaxLength = 20;
            textBoxPassword.MaxLength = 20;
        }

        private void buttonAuth_Click(object sender, EventArgs e)
        {
            textBoxLogin.Text = textBoxLogin.Text.Trim(); // убираем пробелы из начала и конца
            textBoxPassword.Text = textBoxPassword.Text.Trim();
            if (userAuthSucceess())
                this.DialogResult = DialogResult.OK;
            else
                return;
        }
        private bool userAuthSucceess()
        {
            if (incorrectFiledsOnForm())
            {
                MessageBox.Show("Заполните все поля", "Телефонный справочник", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (hasUser(textBoxLogin.Text, textBoxPassword.Text))
                return true;
            else
            {
                MessageBox.Show("Введенный логин или пароль неправильный. Помните, что логин и пароль регистрозависимые.", "Телефонный справочник", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }
        private bool incorrectFiledsOnForm()
        {
            if (isUnCorrectField(textBoxLogin.Text) || isUnCorrectField(textBoxPassword.Text))
                return true;


            return false;
        }
        private bool isUnCorrectField(string field)
        {
            if (String.IsNullOrWhiteSpace(field))
                return true;
            return false;
        }
        private bool hasUser(string login, string password)
        {
            User user = new User(login, password);

            return user.IsCorrect();
        }

        private void помощьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Данное приложение позволяет найти номер телефона интересующего абонента. Для поиска необходимо ввести имя и фамилию целиком.", "Возможности приложения", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void выходToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult res = MessageBox.Show("Вы уверены, что хотите выйти?", "Телефонный справочник", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (res == DialogResult.Yes)
            {
                Application.Exit();
            }

        }

        private void linkLabelRegistration_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            DialogResult Reg = new FormRegist().ShowDialog();
            textBoxLogin.Text = "";
            textBoxPassword.Text = "";
        }

        private void checkBoxShowAuth_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxShowAuth.Checked)
            {
                textBoxPassword.UseSystemPasswordChar = false;
            }
            else
            {
                textBoxPassword.UseSystemPasswordChar = true;
            }
        }
    }
}
