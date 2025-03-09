using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using Npgsql;

namespace Db1
{
    public partial class AddMaterialForm : Form
    {
        public AddMaterialForm()
        {
            InitializeComponent();
            LoadProducts();
            LoadPartners();
        }

        private void LoadProducts()
        {
            try
            {
                using (var conn = new NpgsqlConnection(MainForm.ConnectionString))
                {
                    conn.Open();
                    var cmd = new NpgsqlCommand("SELECT Наименование_продукции FROM Продукты", conn);
                    var reader = cmd.ExecuteReader();

                    cmbProduct.Items.Clear();
                    while (reader.Read())
                    {
                        cmbProduct.Items.Add(reader["Наименование_продукции"].ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки продукции: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadPartners()
        {
            try
            {
                using (var conn = new NpgsqlConnection(MainForm.ConnectionString))
                {
                    conn.Open();
                    var cmd = new NpgsqlCommand("SELECT Наименование FROM Партнеры", conn);
                    var reader = cmd.ExecuteReader();

                    cmbPartner.Items.Clear();
                    while (reader.Read())
                    {
                        cmbPartner.Items.Add(reader["Наименование"].ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки партнеров: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnAddMaterial_Click(object sender, EventArgs e)
        {
            if (cmbProduct.SelectedIndex == -1 || cmbPartner.SelectedIndex == -1 || numQuantity.Value <= 0)
            {
                MessageBox.Show("Заполните все поля корректно!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (var conn = new NpgsqlConnection(MainForm.ConnectionString))
                {
                    conn.Open();
                    var cmd = new NpgsqlCommand("INSERT INTO Материалы (Продукция_ID, Партнер_ID, Количество) VALUES (@productId, @partnerId, @quantity)", conn);

                    // Получаем ID выбранной продукции
                    var productIdCmd = new NpgsqlCommand("SELECT ID FROM Продукты WHERE Наименование_продукции = @name", conn);
                    productIdCmd.Parameters.AddWithValue("name", cmbProduct.SelectedItem.ToString());
                    var productId = Convert.ToInt32(productIdCmd.ExecuteScalar());

                    // Получаем ID выбранного партнера
                    var partnerIdCmd = new NpgsqlCommand("SELECT ID FROM Партнеры WHERE Наименование = @name", conn);
                    partnerIdCmd.Parameters.AddWithValue("name", cmbPartner.SelectedItem.ToString());
                    var partnerId = Convert.ToInt32(partnerIdCmd.ExecuteScalar());

                    cmd.Parameters.AddWithValue("productId", productId);
                    cmd.Parameters.AddWithValue("partnerId", partnerId);
                    cmd.Parameters.AddWithValue("quantity", (int)numQuantity.Value);

                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Материал успешно добавлен!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка добавления материала: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}