using System;
using System.ComponentModel;
using System.Data;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Npgsql;

namespace Db1
{
    public partial class PartnerEditForm : Form
    {
        private int? _partnerId;
        private ErrorProvider _errorProvider = new ErrorProvider();

        public PartnerEditForm(int? partnerId = null)
        {
            _partnerId = partnerId;
            InitializeComponent();
            InitializeValidation();
            if (_partnerId.HasValue) LoadPartnerData();
        }

        private void InitializeValidation()
        {
            txtEmail.Validating += (s, e) => ValidateField(txtEmail, @"^[^@]+@[^@]+\.[^@]+$", "Некорректный email", e);
            txtPhone.Validating += (s, e) => ValidateField(txtPhone, @"^\+?\d[\d\s-]{7,}$", "Некорректный телефон", e);
            txtINN.Validating += (s, e) => ValidateField(txtINN, @"^\d{10,12}$", "ИНН должен содержать 10-12 цифр", e);
        }

        private void ValidateField(Control control, string pattern, string errorMessage, CancelEventArgs e)
        {
            if (!Regex.IsMatch(control.Text, pattern))
            {
                _errorProvider.SetError(control, errorMessage);
                e.Cancel = true;
            }
            else
            {
                _errorProvider.SetError(control, "");
            }
        }

        private void LoadPartnerData()
        {
            try
            {
                using (var conn = new NpgsqlConnection(MainForm.ConnectionString))
                {
                    conn.Open();
                    var cmd = new NpgsqlCommand("SELECT * FROM Партнеры WHERE ID = @id", conn);
                    cmd.Parameters.AddWithValue("id", _partnerId);

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            cmbType.SelectedItem = reader["Тип_партнера"].ToString();
                            txtName.Text = reader["Наименование"].ToString();
                            txtDirector.Text = reader["Директор"].ToString();
                            txtPhone.Text = reader["Телефон"].ToString();
                            txtEmail.Text = reader["Электронная_почта"].ToString();
                            txtAddress.Text = reader["Адрес"].ToString();
                            txtINN.Text = reader["ИНН"].ToString();
                            numRating.Value = Convert.ToDecimal(reader["Рейтинг"]);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (!ValidateChildren() || !IsFormValid()) return;

            try
            {
                using (var conn = new NpgsqlConnection(MainForm.ConnectionString))
                {
                    conn.Open();

                    // Проверяем, существует ли партнер с таким ИНН
                    if (!_partnerId.HasValue && PartnerExists(txtINN.Text.Trim(), conn))
                    {
                        MessageBox.Show("Партнер с таким ИНН уже существует!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    var cmd = _partnerId.HasValue ? CreateUpdateCommand(conn) : CreateInsertCommand(conn);
                    cmd.ExecuteNonQuery();
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
            }
            catch (Npgsql.PostgresException ex) when (ex.SqlState == "23505")
            {
                MessageBox.Show("Партнер с таким ИНН уже существует!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool PartnerExists(string inn, NpgsqlConnection conn)
        {
            var cmd = new NpgsqlCommand("SELECT COUNT(*) FROM Партнеры WHERE ИНН = @inn", conn);
            cmd.Parameters.AddWithValue("inn", inn);
            var count = Convert.ToInt32(cmd.ExecuteScalar());
            return count > 0;
        }

        private bool IsFormValid()
        {
            if (cmbType.SelectedIndex == -1)
            {
                MessageBox.Show("Выберите тип партнера!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            return true;
        }

        private NpgsqlCommand CreateInsertCommand(NpgsqlConnection conn)
        {
            const string query = @"
                INSERT INTO Партнеры 
                (Тип_партнера, Наименование, Директор, Телефон, Электронная_почта, Адрес, ИНН, Рейтинг)
                VALUES (@type, @name, @director, @phone, @email, @address, @inn, @rating)";
            return CreateCommand(query, conn);
        }

        private NpgsqlCommand CreateUpdateCommand(NpgsqlConnection conn)
        {
            const string query = @"
                UPDATE Партнеры SET 
                    Тип_партнера = @type,
                    Наименование = @name,
                    Директор = @director,
                    Телефон = @phone,
                    Электронная_почта = @email,
                    Адрес = @address,
                    ИНН = @inn,
                    Рейтинг = @rating
                WHERE ID = @id";
            var cmd = CreateCommand(query, conn);
            cmd.Parameters.AddWithValue("id", _partnerId);
            return cmd;
        }

        private NpgsqlCommand CreateCommand(string query, NpgsqlConnection conn)
        {
            var cmd = new NpgsqlCommand(query, conn);
            cmd.Parameters.AddWithValue("type", cmbType.SelectedItem);
            cmd.Parameters.AddWithValue("name", txtName.Text.Trim());
            cmd.Parameters.AddWithValue("director", txtDirector.Text.Trim());
            cmd.Parameters.AddWithValue("phone", txtPhone.Text.Trim());
            cmd.Parameters.AddWithValue("email", txtEmail.Text.Trim());
            cmd.Parameters.AddWithValue("address", txtAddress.Text.Trim());
            cmd.Parameters.AddWithValue("inn", txtINN.Text.Trim());
            cmd.Parameters.AddWithValue("rating", (int)numRating.Value);
            return cmd;
        }
    }
}