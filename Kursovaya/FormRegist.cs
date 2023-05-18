using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Security.Cryptography;

namespace Kursovaya
{
    public partial class FormRegist : Form
    {
        public FormRegist()
        {
            InitializeComponent();
            StartPosition = FormStartPosition.CenterScreen;
            textBoxLogin.MaxLength = 20;
            textBoxPassword.MaxLength = 20;
            textBoxPasswordConfirmation.MaxLength = 20;
        }

        private void выходToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult res = MessageBox.Show("Вы уверены, что хотите выйти?", "Телефонный справочник", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (res == DialogResult.Yes)
            {
                Application.Exit();
            }
        }

        private void buttonRegist_Click(object sender, EventArgs e)
        {
            textBoxLogin.Text = textBoxLogin.Text.Trim(); // убираем пробелы из начала и конца
            textBoxPassword.Text = textBoxPassword.Text.Trim();
            textBoxPasswordConfirmation.Text = textBoxPasswordConfirmation.Text.Trim();
            
            if (new Database("Data Source=dataBase.db;Version=3;").Exists(textBoxLogin.Text) == false)
                if (textBoxLogin.Text != "" && textBoxPassword.Text != "" && textBoxPasswordConfirmation.Text != "")
                    if (textBoxPassword.Text.Length >= 8 && textBoxLogin.Text.Length >= 3)
                        if (textBoxPassword.Text == textBoxPasswordConfirmation.Text)
                        {
                            new Database("Data Source=dataBase.db;Version=3;").createUser(textBoxLogin.Text, textBoxPassword.Text);
                            this.DialogResult = DialogResult.OK;
                            MessageBox.Show("Вы успешно зарегистрировались", "Телефонный справочник", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                            MessageBox.Show("Пароли не совпадают. Попробуйте снова", "Телефонный справочник", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    else
                        MessageBox.Show("Введенный пароль или логин не соответствует требованиям", "Телефонный справочник", MessageBoxButtons.OK, MessageBoxIcon.Error);
                else
                    MessageBox.Show("Заполните все поля", "Телефонный справочник", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            else
                MessageBox.Show("Аккаунт с таким логином уже существует. Попробуйте придумать другой", "Телефонный справочник", MessageBoxButtons.OK, MessageBoxIcon.Warning);

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e) //чекбокс Показать пароль
        {
            if (checkBox1.Checked)
            {
                textBoxPassword.UseSystemPasswordChar = false;
            }
            else
            {
                textBoxPassword.UseSystemPasswordChar = true;
            }
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox2.Checked)
            {
                textBoxPasswordConfirmation.UseSystemPasswordChar = false;
            }
            else
            {
                textBoxPasswordConfirmation.UseSystemPasswordChar = true;
            }
        }

        private void информацияToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Данное приложение позволяет найти номер телефона интересующего абонента. Для поиска необходимо ввести имя и фамилию целиком.", "Возможности приложения", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
