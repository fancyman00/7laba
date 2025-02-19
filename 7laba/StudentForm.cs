using Laba7;
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
using RadioButton = System.Windows.Forms.RadioButton;

namespace Laba7
{
    // Основная форма для работы со студентами
    public partial class StudentForm : Form
    {
        // Статическое свойство для хранения объекта студента
        public static Student student { get; set; }

        // Флаг, который показывает, были ли изменения в данных
        private bool changed = false;

        // Свойство для получения информации об изменении данных
        public bool Changed { get { return changed; } }

        // Флаг, который указывает, редактируем ли мы существующего студента или добавляем нового
        private bool is_edit;

        // Конструктор для создания формы для нового студента (не для редактирования)
        public StudentForm()
        {
            InitializeComponent();
            StudentForm.student = new Student(); // Создаем нового студента(новый объект класса студента)
            this.is_edit = false; // Устанавливаем флаг добавления
        }

        // Конструктор для создания формы для редактирования уже существующего студента
        public StudentForm(Student student)
        {
            InitializeComponent();
            StudentForm.student = student; // Используем переданного студента
            this.is_edit = true; // Устанавливаем флаг редактирования
        }

        // Обработчик события для проверки ввода в текстовое поле (только цифры)
        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            char number = e.KeyChar;
            if (number != 8 && !char.IsDigit(number)) // Разрешаем только цифры и клавишу Backspace
                e.Handled = true;
        }

        // Обработчик события загрузки формы
        private void StudentForm_Load(object sender, EventArgs e)
        {
            academPerfomanceGrid.Rows.Clear(); // Очищаем таблицу успеваемости
            if (is_edit) // Если форма для редактирования
            {
                OpenForEditing(); // Загружаем данные студента для редактирования
            }
        }

        // Метод для открытия данных студента для редактирования
        private void OpenForEditing()
        {
            textBoxFIO.Text = student.FIO; // Заполняем ФИО студента
            textBoxGroup.Text = student.Group.ToString(); // Заполняем номер группы
            switch (student.Course) // Устанавливаем курс студента в соответствующий radioButton
            {
                case 1:
                    radioButton1.Checked = true;
                    break;
                case 2:
                    radioButton2.Checked = true;
                    break;
                case 3:
                    radioButton3.Checked = true;
                    break;
                case 4:
                    radioButton4.Checked = true;
                    break;
            }

            if (student.EducationForm == Student.EEducationForm.Budget)
                radioButtonBudget.Checked = true;
            else
                radioButtonContract.Checked = true;

            // Заполняем таблицу академической успеваемости предметами и оценками
            int examNum = student.Course * Student.CountExams * 2;
            for (int i = 0; i < examNum; i++)
            {
                academPerfomanceGrid.Rows[i].Cells[2].Value = student.Exams[i].Subject; // Название предмета
                academPerfomanceGrid.Rows[i].Cells[3].Value = student.Exams[i].Mark.ToString(); // Оценка
            }
        }

        // Метод для сохранения данных студента
        private bool SaveStudent()
        {
            if (textBoxFIO.Text == "")
            {
                MessageBox.Show("Введите ФИО студента", "Ошибка в ФИО студента", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            student.FIO = textBoxFIO.Text;
            var group = textBoxGroup.Text;
            if (group == "" || group[0] == '0') 
            {
                MessageBox.Show("Номер группы является положительным числом", "Ошибка в номере группы", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            student.Group = byte.Parse(group); 

            if (radioButton1.Checked)
                student.Course = 1;
            else if (radioButton2.Checked)
                student.Course = 2;
            else if (radioButton3.Checked)
                student.Course = 3;
            else
                student.Course = 4;

            if (radioButtonBudget.Checked)
                student.EducationForm = Student.EEducationForm.Budget;
            else
                student.EducationForm = Student.EEducationForm.Contract;


            int examNum = student.Course * Student.CountExams * 2;
            student.Exams = new Exam[examNum]; 

            for (var i = 0; i < examNum; ++i)
            {
                student.Exams[i] = new Exam(); // Создаем новый объект экзамена
                if (academPerfomanceGrid.Rows[i].Cells[2].Value == null)
                {
                    MessageBox.Show($"Введите название экзамена в ячейке {i}:{2}", "Ошибка в заполнении экзаменов", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
                string s = academPerfomanceGrid.Rows[i].Cells[2].Value.ToString(); // Получаем название предмета
                if (s == String.Empty) // Проверяем, что название предмета не пустое
                {
                    MessageBox.Show("Введите названия предметов", "Ошибка в DataGridView", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
                student.Exams[i].Subject = s; // Сохраняем название предмета

                s = academPerfomanceGrid.Rows[i].Cells[3].Value.ToString(); // Получаем оценку
                if (s == String.Empty || !byte.TryParse(s, out byte mark) || mark < 2 || mark > 5) // Проверяем, что оценка корректная
                {
                    MessageBox.Show("Оценки должны быть целым числом в диапазоне [2; 5]", "Ошибка в DataGridView", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
                student.Exams[i].Mark = mark; // Сохраняем оценку
            }
            return true; // Возвращаем true, если все прошло успешно
        }

        // Обработчик события изменения выбранного радио-кнопки для курса
        private void radioButton_CheckedChanged(object sender, EventArgs e)
        {
            int course = Convert.ToInt32(((RadioButton)sender).Text); // Получаем выбранный курс
            int examNum = course * Student.CountExams * 2; // Рассчитываем количество экзаменов для выбранного курса
            for (var i = academPerfomanceGrid.Rows.Count; i > examNum; --i) // Очищаем таблицу успеваемости
            {
                DataGridViewRow row = academPerfomanceGrid.Rows[i-1];
                academPerfomanceGrid.Rows.Remove(row); 
            }

            // Добавляем строки в таблицу, если их недостаточно
            for (var i = academPerfomanceGrid.Rows.Count; i < examNum; ++i)
            {
                academPerfomanceGrid.Rows.Add(); // Добавляем новую строку
                if (i % 10 == 0) // Если строка является первой в блоке, задаем номер блока
                    academPerfomanceGrid.Rows[i].Cells[0].Value = (i / 10 + 1).ToString();
                if (i % 5 == 0) // Если строка является первой в подблоке, задаем номер подблока
                    academPerfomanceGrid.Rows[i].Cells[1].Value = (i / 5 + 1).ToString();
            }
        }

        // Обработчик кнопки "Отмена" - закрытие формы без сохранения
        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.Close(); // Закрываем форму
        }

        // Обработчик кнопки "Сохранить" - сохраняем данные и закрываем форму
        private void saveButton_Click(object sender, EventArgs e)
        {
            if (SaveStudent()) // Пытаемся сохранить данные студента
            {
                changed = true; // Если сохранение прошло успешно, устанавливаем флаг изменений
                this.Close(); // Закрываем форму
            }
        }
    }
}
