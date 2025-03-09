using System;
using System.Windows.Forms;
using Npgsql;

namespace Db1
{
    public partial class MaterialCalcForm : Form
    {
        private ComboBox cmbProductType;
        private ComboBox cmbMaterialType;
        private NumericUpDown numQuantity;
        private TextBox txtParam1;
        private TextBox txtParam2;
        private TextBox txtResult;
        private Button btnCalculate;

        public MaterialCalcForm()
        {
            InitializeComponent();
            LoadProductTypes();
            LoadMaterialTypes();
        }

        private void LoadProductTypes()
        {
            try
            {
                using (var conn = new NpgsqlConnection(MainForm.ConnectionString))
                {
                    conn.Open();
                    var cmd = new NpgsqlCommand("SELECT Тип_продукции FROM Типы_продукции", conn);
                    var reader = cmd.ExecuteReader();

                    cmbProductType.Items.Clear();
                    while (reader.Read())
                    {
                        cmbProductType.Items.Add(reader["Тип_продукции"].ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки типов продукции: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadMaterialTypes()
        {
            try
            {
                using (var conn = new NpgsqlConnection(MainForm.ConnectionString))
                {
                    conn.Open();
                    var cmd = new NpgsqlCommand("SELECT Тип_материала FROM Типы_материалов", conn);
                    var reader = cmd.ExecuteReader();

                    cmbMaterialType.Items.Clear();
                    while (reader.Read())
                    {
                        cmbMaterialType.Items.Add(reader["Тип_материала"].ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки типов материалов: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnCalculate_Click(object sender, EventArgs e)
        {
            if (!ValidateForm()) return;

            try
            {
                decimal param1 = decimal.Parse(txtParam1.Text);
                decimal param2 = decimal.Parse(txtParam2.Text);
                int quantity = (int)numQuantity.Value;

                decimal coefficient = GetProductCoefficient(cmbProductType.Text);
                decimal defect = GetMaterialDefect(cmbMaterialType.Text);

                decimal materialPerUnit = param1 * param2 * coefficient;
                decimal totalMaterial = materialPerUnit * quantity * (1 + defect);

                txtResult.Text = Math.Ceiling(totalMaterial).ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка расчета: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool ValidateForm()
        {
            if (cmbProductType.SelectedIndex == -1)
            {
                MessageBox.Show("Выберите тип продукции!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (cmbMaterialType.SelectedIndex == -1)
            {
                MessageBox.Show("Выберите тип материала!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (!decimal.TryParse(txtParam1.Text, out decimal p1) || p1 <= 0)
            {
                MessageBox.Show("Некорректный параметр 1!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (!decimal.TryParse(txtParam2.Text, out decimal p2) || p2 <= 0)
            {
                MessageBox.Show("Некорректный параметр 2!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }

        private decimal GetProductCoefficient(string productType)
        {
            using (var conn = new NpgsqlConnection(MainForm.ConnectionString))
            {
                conn.Open();
                var cmd = new NpgsqlCommand("SELECT Коэффициент FROM Типы_продукции WHERE Тип_продукции = @type", conn);
                cmd.Parameters.AddWithValue("type", productType);
                return Convert.ToDecimal(cmd.ExecuteScalar());
            }
        }

        private decimal GetMaterialDefect(string materialType)
        {
            using (var conn = new NpgsqlConnection(MainForm.ConnectionString))
            {
                conn.Open();
                var cmd = new NpgsqlCommand("SELECT Процент_брака FROM Типы_материалов WHERE Тип_материала = @type", conn);
                cmd.Parameters.AddWithValue("type", materialType);
                return Convert.ToDecimal(cmd.ExecuteScalar());
            }
        }
    }
}