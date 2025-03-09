using System;
using System.Data;
using System.Windows.Forms;
using Npgsql;

namespace Db1
{
    public partial class SalesHistoryForm : Form
    {
        private int _partnerId;

        public SalesHistoryForm(int partnerId)
        {
            _partnerId = partnerId;
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            try
            {
                using (var conn = new NpgsqlConnection(MainForm.ConnectionString))
                {
                    conn.Open();
                    var query = @"
                        SELECT 
                            p.Наименование_продукции AS Продукция,
                            pr.Количество,
                            pr.Дата_продажи AS Дата,
                            (pr.Количество * p.Минимальная_стоимость_партнера) AS Сумма
                        FROM Продажи pr
                        JOIN Продукты p ON pr.Продукт_ID = p.ID
                        WHERE pr.Партнер_ID = @id";

                    var cmd = new NpgsqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("id", _partnerId);

                    var dt = new DataTable();
                    new NpgsqlDataAdapter(cmd).Fill(dt);
                    dataGridView.DataSource = dt;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}