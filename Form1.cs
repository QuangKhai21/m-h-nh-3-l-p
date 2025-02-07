﻿using buoi5;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace buoi5
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

      
      


        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            using (StudentContextDB context = new StudentContextDB())
            {
                // Kiểm tra các trường nhập liệu
                if (string.IsNullOrWhiteSpace(txtStudentID.Text) || string.IsNullOrWhiteSpace(txtFullName.Text) || string.IsNullOrWhiteSpace(txtAverageScore.Text))
                {
                    MessageBox.Show("Vui lòng nhập đầy đủ thông tin!");
                    return;
                }

                if (txtStudentID.Text.Length != 10)
                {
                    MessageBox.Show("Mã số sinh viên phải có 10 kí tự!");
                    return;
                }

                if (!double.TryParse(txtAverageScore.Text, out double averageScore))
                {
                    MessageBox.Show("Điểm trung bình không hợp lệ!");
                    return;
                }

                // Kiểm tra nếu mã sinh viên đã tồn tại
                var existingStudent = context.Students.FirstOrDefault(s => s.StudentID == txtStudentID.Text);
                if (existingStudent == null)
                {
                    // Tạo một đối tượng sinh viên mới
                    Student newStudent = new Student()
                    {
                        StudentID = txtStudentID.Text,
                        FullName = txtFullName.Text,
                        FacultyID = (int)cmbFaculty.SelectedValue,
                        AverageScore = averageScore
                    };

                    // Thêm sinh viên mới vào cơ sở dữ liệu
                    context.Students.Add(newStudent);
                    context.SaveChanges();  // Lưu lại thay đổi vào cơ sở dữ liệu

                    // Làm mới dữ liệu trong DataGridView
                    LoadStudentData();

                    // Thông báo thành công
                    MessageBox.Show("Thêm mới dữ liệu thành công!");

                    // Xóa các trường nhập liệu sau khi thêm
                    ClearInputFields();
                }
                else
                {
                    MessageBox.Show("Mã sinh viên đã tồn tại. Vui lòng nhập mã khác.");
                }
            }
        }
        private void ClearInputFields()
        {
            txtStudentID.Clear();
            txtFullName.Clear();
            txtAverageScore.Clear();
            cmbFaculty.SelectedIndex = -1;
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            using (StudentContextDB context = new StudentContextDB())
            {
                if (dataGridView1.CurrentRow != null)
                {
                    // Lấy StudentID của sinh viên đang chọn
                    string oldStudentID = dataGridView1.CurrentRow.Cells["StudentID"].Value.ToString();

                    // Tìm sinh viên trong cơ sở dữ liệu theo StudentID cũ
                    Student dbUpdate = context.Students.FirstOrDefault(p => p.StudentID == oldStudentID);

                    if (dbUpdate != null)
                    {
                        try
                        {
                            // Kiểm tra xem dữ liệu nhập vào có hợp lệ không
                            if (string.IsNullOrWhiteSpace(txtStudentID.Text))
                            {
                                MessageBox.Show("Vui lòng nhập mã số sinh viên mới.");
                                return;
                            }

                            if (string.IsNullOrWhiteSpace(txtFullName.Text))
                            {
                                MessageBox.Show("Vui lòng nhập tên sinh viên.");
                                return;
                            }

                            if (cmbFaculty.SelectedValue == null)
                            {
                                MessageBox.Show("Vui lòng chọn khoa.");
                                return;
                            }

                            if (!double.TryParse(txtAverageScore.Text, out double averageScore))
                            {
                                MessageBox.Show("Điểm trung bình không hợp lệ.");
                                return;
                            }

                            // Nếu StudentID mới khác với StudentID cũ, kiểm tra xem mã mới đã tồn tại chưa
                            if (txtStudentID.Text != oldStudentID)
                            {
                                var existingStudent = context.Students.FirstOrDefault(s => s.StudentID == txtStudentID.Text);
                                if (existingStudent != null)
                                {
                                    MessageBox.Show("Mã số sinh viên mới đã tồn tại. Vui lòng chọn mã khác.");
                                    return;
                                }

                                // Xóa sinh viên cũ
                                context.Students.Remove(dbUpdate);

                                // Tạo sinh viên mới với thông tin được cập nhật
                                Student newStudent = new Student()
                                {
                                    StudentID = txtStudentID.Text,
                                    FullName = txtFullName.Text,
                                    FacultyID = (int)cmbFaculty.SelectedValue,  // Lấy FacultyID từ ComboBox
                                    AverageScore = averageScore
                                };

                                // Thêm sinh viên mới vào cơ sở dữ liệu
                                context.Students.Add(newStudent);
                            }
                            else
                            {
                                // Nếu StudentID không thay đổi, chỉ cập nhật các thông tin khác
                                dbUpdate.FullName = txtFullName.Text;
                                dbUpdate.FacultyID = (int)cmbFaculty.SelectedValue;
                                dbUpdate.AverageScore = averageScore;
                            }

                            // Lưu thay đổi vào cơ sở dữ liệu
                            context.SaveChanges();

                            // Làm mới lại DataGridView để hiển thị thông tin mới cập nhật
                            LoadStudentData();

                            // Xóa các trường nhập liệu sau khi cập nhật
                            ClearInputFields();

                            MessageBox.Show("Cập nhật sinh viên thành công!");
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Lỗi khi cập nhật sinh viên: " + ex.Message);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Không tìm thấy sinh viên để cập nhật.");
                    }
                }
                else
                {
                    MessageBox.Show("Vui lòng chọn sinh viên cần sửa.");
                }
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            using (StudentContextDB context = new StudentContextDB())
            {
                if (dataGridView1.CurrentRow != null)
                {
                    string selectedStudentID = dataGridView1.CurrentRow.Cells["StudentID"].Value.ToString();

                    // Kiểm tra sinh viên có tồn tại không
                    Student dbDelete = context.Students.FirstOrDefault(p => p.StudentID == selectedStudentID);
                    if (dbDelete != null)
                    {
                        // Hiển thị cảnh báo YES/NO
                        var confirmResult = MessageBox.Show("Bạn có chắc chắn muốn xóa sinh viên này?", "Xác nhận xóa", MessageBoxButtons.YesNo);
                        if (confirmResult == DialogResult.Yes)
                        {
                            context.Students.Remove(dbDelete);
                            context.SaveChanges(); // Lưu thay đổi sau khi xóa

                            // Làm mới lại DataGridView để hiển thị thông tin sau khi xóa
                            LoadStudentData();

                            // Thông báo thành công
                            MessageBox.Show("Xóa sinh viên thành công!");

                            // Xóa các trường nhập liệu sau khi xóa
                            ClearInputFields();
                        }
                    }
                    else
                    {
                        MessageBox.Show("Không tìm thấy MSSV cần xóa!");
                    }
                }
            }
        }

      

        private void btnOut_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                ConfigureDataGridView();

                using (StudentContextDB context = new StudentContextDB())
                {
                    // Get the list of faculties
                    List<Faculty> listFaculties = context.Faculties.ToList();

                    // Fill the Faculty ComboBox
                    FillFacultyComboBox(listFaculties);

                    // Load student data into DataGridView
                    LoadStudentData();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

      

     
        private void FillFacultyComboBox(List<Faculty> listFacultys)
        {


            cmbFaculty.DataSource = listFacultys;
            cmbFaculty.DisplayMember = "FacultyName"; // Display name
            this.cmbFaculty.ValueMember = "FacultyID";     // Value member
        }
        private void LoadStudentData()
        {
            using (StudentContextDB context = new StudentContextDB())
            {
                // Include Faculty when retrieving students
                //List<Student> listStudents = context.Students.Include(string=).ToList();

                // Bind the data to DataGridView
                //BindGrid(listStudents);
            }
        }

        private void BindGrid(List<Student> listStudents)
        {

            dataGridView1.AutoGenerateColumns = false;
            dataGridView1.DataSource = null;
            dataGridView1.DataSource = listStudents;
        }
        private void ConfigureDataGridView()
        {
            
            // Xóa các cột cũ
            dataGridView1.Columns.Clear();

            // Cột StudentID
            DataGridViewTextBoxColumn colStudentID = new DataGridViewTextBoxColumn();
            colStudentID.Name = "StudentID";
            colStudentID.HeaderText = "MSSV";
            colStudentID.DataPropertyName = "StudentID";
            dataGridView1.Columns.Add(colStudentID);

            // Cột FullName
            DataGridViewTextBoxColumn colFullName = new DataGridViewTextBoxColumn();
            colFullName.Name = "FullName";
            colFullName.HeaderText = "Họ Tên";
            colFullName.DataPropertyName = "FullName";
            dataGridView1.Columns.Add(colFullName);

            // Cột Faculty (hiển thị FacultyName)
            DataGridViewTextBoxColumn colFaculty = new DataGridViewTextBoxColumn();
            colFaculty.Name = "FacultyName";
            colFaculty.HeaderText = "Khoa";
            colFaculty.DataPropertyName = "Faculty.FacultyName";
            dataGridView1.Columns.Add(colFaculty);

            // Cột AverageScore
            DataGridViewTextBoxColumn colAverageScore = new DataGridViewTextBoxColumn();
            colAverageScore.Name = "AverageScore";
            colAverageScore.HeaderText = "Điểm TB";
            colAverageScore.DataPropertyName = "AverageScore";
            dataGridView1.Columns.Add(colAverageScore);
        }

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow != null)
            {
                txtStudentID.Text = dataGridView1.CurrentRow.Cells["StudentID"].Value.ToString();
                txtFullName.Text = dataGridView1.CurrentRow.Cells["FullName"].Value.ToString();
                txtAverageScore.Text = dataGridView1.CurrentRow.Cells["AverageScore"].Value.ToString();

                var selectedStudent = (Student)dataGridView1.CurrentRow.DataBoundItem;
                if (selectedStudent.Faculty != null)
                {
                    cmbFaculty.SelectedValue = selectedStudent.FacultyID;
                }
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
    
}
    



   

      
        
     