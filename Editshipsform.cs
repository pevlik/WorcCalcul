using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Windows.Forms;

namespace EffortCalculator
{
    public class EditShipsForm : Form
    {
        private DataGridView shipsDataGridView;
        private Button saveButton;
        private Button deleteButton; // Кнопка удаления
        private const string DataFilePath = "shipsData.json"; // путь к файлу данных

        public EditShipsForm()
        {
            InitializeComponent(); // вызов инициализации
            LoadData(); // Загрузка данных при открытии формы
        }

        private void InitializeComponent()
        {
            this.shipsDataGridView = new DataGridView();
            this.saveButton = new Button();
            this.deleteButton = new Button(); // Инициализация кнопки удаления
            this.SuspendLayout();

            // shipsDataGridView
            this.shipsDataGridView.AllowUserToAddRows = true;
            this.shipsDataGridView.AllowUserToDeleteRows = true;
            this.shipsDataGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect; // Включаем возможность выбора нескольких строк
            this.shipsDataGridView.MultiSelect = true; // Разрешаем выбор нескольких строк
            this.shipsDataGridView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.shipsDataGridView.Columns.AddRange(new DataGridViewColumn[] {
                new DataGridViewTextBoxColumn { Name = "Code", HeaderText = "Code", DataPropertyName = "Code", Width = 50 },
                new DataGridViewTextBoxColumn { Name = "Name", HeaderText = "Name", DataPropertyName = "Name", Width = 450},
                new DataGridViewTextBoxColumn { Name = "FormulaLow", HeaderText = "Formula Low", DataPropertyName = "FormulaLow", Width = 200},
                new DataGridViewTextBoxColumn { Name = "FormulaHigh", HeaderText = "Formula High", DataPropertyName = "FormulaHigh" , Width = 200},
                new DataGridViewTextBoxColumn { Name = "MaxDisplacement", HeaderText = "Max Displacement", DataPropertyName = "MaxDisplacement" , Width = 100} // Новая колонка
            });
            this.shipsDataGridView.Dock = DockStyle.Top;
            this.shipsDataGridView.Location = new System.Drawing.Point(0, 0);
            this.shipsDataGridView.Name = "shipsDataGridView";
            this.shipsDataGridView.Size = new System.Drawing.Size(484, 400);
            this.shipsDataGridView.TabIndex = 0;

            // saveButton
            this.saveButton.Text = "Сохранить";
            this.saveButton.Dock = DockStyle.Bottom;
            this.saveButton.Click += SaveButton_Click;

            // deleteButton
            this.deleteButton.Text = "Удалить выбранные";
            this.deleteButton.Dock = DockStyle.Bottom;
            this.deleteButton.Click += DeleteButton_Click; // Привязка события

            // EditShipsForm
            this.ClientSize = new System.Drawing.Size(1200, 400); // Увеличиваем размер, чтобы разместить кнопки (Ширина, Высота)
            this.Controls.Add(this.shipsDataGridView);
            this.Controls.Add(this.deleteButton); // Добавляем кнопку удаления
            this.Controls.Add(this.saveButton);
            this.Name = "EditShipsForm";
            this.Text = "Edit Ships";
            this.ResumeLayout(false);
        }

        private void DeleteButton_Click(object sender, EventArgs e)
        {
            // Удаляем выбранные строки из DataGridView
            foreach (DataGridViewRow row in shipsDataGridView.SelectedRows)
            {
                if (!row.IsNewRow)
                {
                    shipsDataGridView.Rows.Remove(row);
                }
            }
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            SaveData();
        }

        private void LoadData()
        {
            if (File.Exists(DataFilePath))
            {
                string jsonData = File.ReadAllText(DataFilePath);
                var ships = JsonSerializer.Deserialize<List<ShipType>>(jsonData);
                if (ships != null)
                {
                    foreach (var ship in ships)
                    {
                        shipsDataGridView.Rows.Add(ship.Code, ship.Name, ship.FormulaLow, ship.FormulaHigh, ship.MaxDisplacement);
                    }
                }
            }
        }

        private void SaveData()
        {
            var ships = new List<ShipType>();
            foreach (DataGridViewRow row in shipsDataGridView.Rows)
            {
                if (row.IsNewRow) continue;

                ships.Add(new ShipType
                {
                    Code = row.Cells["Code"].Value?.ToString(),
                    Name = row.Cells["Name"].Value?.ToString(),
                    FormulaLow = row.Cells["FormulaLow"].Value?.ToString(),
                    FormulaHigh = row.Cells["FormulaHigh"].Value?.ToString(),
                    MaxDisplacement = double.TryParse(row.Cells["MaxDisplacement"].Value?.ToString(), out double displacement) ? displacement : 0 // Обработка числа
                });
            }

            string jsonData = JsonSerializer.Serialize(ships, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(DataFilePath, jsonData);
            MessageBox.Show("Изменения сохранены!", "Сохранение данных", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
