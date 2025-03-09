using System;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Npgsql;

namespace Db1
{
    public partial class MainForm : Form
    {
        public static string ConnectionString = "Host=195.46.187.72;Username=postgres;Password=1337;Database=RuslanFinish";

        public MainForm()
        {
            InitializeComponent();
            LoadPartners();
            LoadRefreshButtonImage(); // Загружаем изображение для кнопки "Обновить"
        }

        private void LoadRefreshButtonImage()
        {
            try
            {
                // Путь к иконке (используем путь с вашего скриншота)
                string iconPath = @"D:\Tex\Ty\mark\61224.jpg";

                // Проверяем, существует ли файл по указанному пути
                if (!File.Exists(iconPath))
                {
                    MessageBox.Show("Файл иконки не найден по указанному пути.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Загружаем изображение из указанного пути
                var image = Image.FromFile(iconPath);

                // Масштабируем изображение до 24x24 пикселей
                var resizedImage = new Bitmap(image, new Size(24, 24));

                // Устанавливаем изображение на кнопку
                this.btnRefresh.Image = resizedImage;
                this.btnRefresh.ImageAlign = ContentAlignment.MiddleLeft; // Выравниваем иконку слева
                this.btnRefresh.TextImageRelation = TextImageRelation.ImageBeforeText; // Текст после иконки
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки изображения: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadPartners()
        {
            try
            {
                flowLayoutPanel.Controls.Clear(); // Очищаем контейнер для карточек

                using (var conn = new NpgsqlConnection(ConnectionString))
                {
                    conn.Open();
                    var query = @"
                        SELECT 
                            p.*,
                            COALESCE(SUM(pr.Количество), 0) as TotalSales,
                            CASE
                                WHEN COALESCE(SUM(pr.Количество), 0) >= 300000 THEN 15
                                WHEN COALESCE(SUM(pr.Количество), 0) >= 50000 THEN 10
                                WHEN COALESCE(SUM(pr.Количество), 0) >= 10000 THEN 5
                                ELSE 0
                            END as Скидка
                        FROM Партнеры p
                        LEFT JOIN Продажи pr ON p.ID = pr.Партнер_ID
                        GROUP BY p.ID";

                    using (var cmd = new NpgsqlCommand(query, conn))
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            // Пропускаем некорректные записи
                            if (!ValidatePartner(reader)) continue;

                            // Создаем карточку партнера
                            var card = new GroupBox
                            {
                                Width = 300,
                                Height = 200, // Увеличиваем высоту для отображения материалов
                                Margin = new Padding(10),
                                Tag = reader["ID"]
                            };

                            // Заголовок с типом, названием и скидкой
                            var lblTitle = new Label
                            {
                                Text = $"{reader["Тип_партнера"]} | {reader["Наименование"]} {reader["Скидка"]}%",
                                Font = new Font("Arial", 10, FontStyle.Bold),
                                Location = new Point(10, 10),
                                AutoSize = true
                            };

                            // Информация
                            var lblInfo = new Label
                            {
                                Text = $"Директор: {reader["Директор"]}\n" +
                                       $"Телефон: {FormatPhone(reader["Телефон"].ToString())}\n" +
                                       $"Рейтинг: {reader["Рейтинг"]}",
                                Location = new Point(10, 35),
                                AutoSize = true
                            };

                            // Кнопка "Удалить"
                            var btnDelete = new Button
                            {
                                Text = "Удалить",
                                Location = new Point(10, 120),
                                Size = new Size(100, 30),
                                Tag = reader["ID"] // Сохраняем ID партнера в кнопке
                            };
                            btnDelete.Click += BtnDelete_Click; // Обработчик нажатия

                            // Добавляем информацию о материалах
                            var materialsText = new System.Text.StringBuilder("Материалы:\n");
                            using (var materialsConn = new NpgsqlConnection(ConnectionString))
                            {
                                materialsConn.Open();
                                var materialsQuery = @"
                                    SELECT m.Количество, pr.Наименование_продукции 
                                    FROM Материалы m
                                    JOIN Продукты pr ON m.Продукция_ID = pr.ID
                                    WHERE m.Партнер_ID = @partnerId";

                                using (var materialsCmd = new NpgsqlCommand(materialsQuery, materialsConn))
                                {
                                    materialsCmd.Parameters.AddWithValue("partnerId", reader["ID"]);
                                    using (var materialsReader = materialsCmd.ExecuteReader())
                                    {
                                        while (materialsReader.Read())
                                        {
                                            materialsText.AppendLine($"{materialsReader["Наименование_продукции"]}: {materialsReader["Количество"]}");
                                        }
                                    }
                                }
                            }

                            var lblMaterials = new Label
                            {
                                Text = materialsText.ToString(),
                                Location = new Point(10, 80),
                                AutoSize = true
                            };
                            card.Controls.Add(lblMaterials);

                            // Добавляем обработчик клика для выбора карточки
                            card.Click += (s, e) => SelectCard(card);

                            card.Controls.Add(lblTitle);
                            card.Controls.Add(lblInfo);
                            card.Controls.Add(btnDelete);
                            flowLayoutPanel.Controls.Add(card);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Вы уверены, что хотите удалить партнера?", "Подтверждение", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                var btnDelete = sender as Button;
                if (btnDelete?.Tag == null) return;

                int partnerId = (int)btnDelete.Tag;

                try
                {
                    using (var conn = new NpgsqlConnection(ConnectionString))
                    {
                        conn.Open();
                        var cmd = new NpgsqlCommand("DELETE FROM Партнеры WHERE ID = @id", conn);
                        cmd.Parameters.AddWithValue("id", partnerId);
                        cmd.ExecuteNonQuery();
                    }

                    // Удаляем карточку из интерфейса
                    var card = btnDelete.Parent as GroupBox;
                    if (card != null)
                    {
                        flowLayoutPanel.Controls.Remove(card);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка удаления: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private bool ValidatePartner(IDataRecord partner)
        {
            // Проверка ИНН (только цифры)
            return Regex.IsMatch(partner["ИНН"].ToString(), @"^\d+$");
        }

        private string FormatPhone(string phone)
        {
            // Форматирование +7 XXX XXX XX XX (без использования range)
            var cleanPhone = new string(phone.Where(char.IsDigit).ToArray());
            if (long.TryParse(cleanPhone, out var num))
            {
                string phoneStr = num.ToString();
                // Проверяем, что длина строки достаточна для форматирования
                if (phoneStr.Length >= 11)
                {
                    return $"+7 {phoneStr.Substring(1, 3)} {phoneStr.Substring(4, 3)} {phoneStr.Substring(7, 2)} {phoneStr.Substring(9, 2)}";
                }
                else
                {
                    // Если номер короче, возвращаем его в исходном виде
                    return phone;
                }
            }
            return phone;
        }

        private GroupBox _selectedCard = null;

        private void SelectCard(GroupBox card)
        {
            if (_selectedCard != null)
            {
                _selectedCard.BackColor = SystemColors.Control; // Сбрасываем цвет предыдущей выбранной карточки
            }
            _selectedCard = card;
            _selectedCard.BackColor = Color.LightBlue; // Подсвечиваем выбранную карточку
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            var form = new PartnerEditForm();
            if (form.ShowDialog() == DialogResult.OK)
                LoadPartners();
        }

        private void BtnEdit_Click(object sender, EventArgs e)
        {
            if (_selectedCard == null) return;
            int id = (int)_selectedCard.Tag;
            var form = new PartnerEditForm(id);
            if (form.ShowDialog() == DialogResult.OK)
                LoadPartners();
        }

        private void BtnSalesHistory_Click(object sender, EventArgs e)
        {
            if (_selectedCard == null) return;
            int id = (int)_selectedCard.Tag;
            new SalesHistoryForm(id).ShowDialog();
        }

        private void BtnMaterialCalc_Click(object sender, EventArgs e)
        {
            new MaterialCalcForm().ShowDialog();
        }

        private void BtnAddMaterial_Click(object sender, EventArgs e)
        {
            var form = new AddMaterialForm();
            form.ShowDialog();
        }

        private void BtnRefresh_Click(object sender, EventArgs e)
        {
            LoadPartners(); // Обновляем данные
        }
    }
}