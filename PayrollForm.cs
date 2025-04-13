using System;
using System.IO;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Text.Json;
using System.Linq;

namespace Project_KTMH
{
    public partial class PayrollForm : Form
    {
        private List<(Employee, Payroll)> employees = EmployeeList.emp;
        private List<Payroll> payrolls;
        private string payrollFilePath = "payroll.json"; // Đổi tên file lưu trữ thành JSON
        private DataGridView dgvPayrolls;
        private ComboBox cmbEmployees;

        private TextBox txtBaseSalary;
        private TextBox txtAttendanceDay;
        private Button btnUpdatePayroll;
        private Button btnDeletePayroll;
        private Label label1;
        private Label label2;
        private Button button1;
        private Button btnAddPayroll;


        public PayrollForm()
        {
            InitializeComponent();
            LoadData();
            InitializeEmployeeComboBox(); // Gọi phương thức này để khởi tạo ComboBox
            LoadPayrolls();
            cmbEmployees.SelectedIndexChanged += CmbEmployees_SelectedIndexChanged;
        }

        // Phương thức tải dữ liệu từ file
        public void LoadData()
        {
            List<Employee> employeesList = LoadEmployeesFromFile("employee.json");
            List<Payroll> payrollsList = LoadPayrollsFromFile("payroll.json");

            // Sử dụng JSON cho employees
            payrolls = LoadPayrollsFromFile(payrollFilePath);

            employees = new List<(Employee, Payroll)>();

            foreach (Employee employee in employeesList)
            {
                Payroll current_p = null;
                foreach (Payroll payroll in payrolls)
                {
                    if (payroll.PayrollID == employee.EmployeeID1)
                    {
                        current_p = payroll;
                        break;
                    }
                }
                employees.Add((employee, current_p));
            }
        }

        public void SaveEmployeesToFile(string filePath)
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            List<Employee> employeesList = new List<Employee>();

            foreach ((Employee, Payroll) employeeTuple in employees)
            {
                employeesList.Add(employeeTuple.Item1);
            }

            string json = JsonSerializer.Serialize(employeesList, options);
            File.WriteAllText(filePath, json);
        }

        // Phương thức tải danh sách nhân viên từ file JSON
        public List<Employee> LoadEmployeesFromFile(string filePath)
        {
            if (!File.Exists(filePath))
            {
                MessageBox.Show($"File {filePath} không tồn tại.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return new List<Employee>();
            }

            try
            {
                string json = File.ReadAllText(filePath);
                return JsonSerializer.Deserialize<List<Employee>>(json) ?? new List<Employee>();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải dữ liệu từ {filePath}: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return new List<Employee>();
            }
        }
        public List<Payroll> LoadPayrollsFromFile(string filePath)
        {
            if (!File.Exists(filePath))
            {
                MessageBox.Show($"File {filePath} không tồn tại.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return new List<Payroll>();
            }

            try
            {
                string json = File.ReadAllText(filePath);
                return JsonSerializer.Deserialize<List<Payroll>>(json) ?? new List<Payroll>();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải dữ liệu từ {filePath}: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return new List<Payroll>();
            }
        }

        // Phương thức lưu danh sách lương vào file JSON
        public void SavePayrollsToFile()
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            string json = JsonSerializer.Serialize(payrolls, options);
            File.WriteAllText(payrollFilePath, json);
        }
        // Phương thức tải danh sách nhân viên vào ComboBox
        public void LoadEmployees()
        {
            List<Employee> employeesList = new List<Employee>();
            foreach ((Employee, Payroll) employee in employees)
            {
                employeesList.Add(employee.Item1);
            }

            cmbEmployees.DataSource = employeesList;
            cmbEmployees.DisplayMember = "Name";
            cmbEmployees.ValueMember = "EmployeeID1";
            cmbEmployees.SelectedIndex = -1;
        }
        public void LoadPayrolls()
        {
            dgvPayrolls.DataSource = null;
            dgvPayrolls.DataSource = payrolls;
        }
        private void btnAddPayroll_Click(object sender, EventArgs e)
        {
            if (cmbEmployees.SelectedValue == null)
            {
                MessageBox.Show("Vui lòng chọn một nhân viên!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string employeeID = cmbEmployees.SelectedValue.ToString();

            Employee selectedEmployee = null;
            Payroll existingPayroll = null;
            foreach ((Employee emp, Payroll pay) in EmployeeList.emp)
            {
                if (emp.EmployeeID1 == employeeID)
                {
                    selectedEmployee = emp;
                    existingPayroll = pay;
                    break;
                }
            }

            if (existingPayroll != null)
            {
                MessageBox.Show("Nhân viên đã có bảng lương. Vui lòng cập nhật bảng lương nếu cần.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
        
            if (selectedEmployee == null)
            {
                MessageBox.Show("Không tìm thấy nhân viên.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            decimal baseSalary = 300000;
            int attendanceDays = selectedEmployee.Attendances.Count; 

            string departmentID = selectedEmployee.DepartmentID1;
            string employeeName = selectedEmployee.Name;
            double salaryCoefficient = 1.0; 
            double salaryCoefficientDepartment = 1.2; 
            double salaryCoefficientPosition = 1.3; 

           
            Payroll newPayroll = new Payroll(
                payrollID: employeeID,
                departmentID: departmentID,
                employeeID: employeeID,
                employeeName: employeeName,
                salaryCoefficient: salaryCoefficient,
                salaryCoefficientDepartment: salaryCoefficientDepartment,
                salaryCoefficientPosition: salaryCoefficientPosition,
                baseSalary: baseSalary
            );
          
            payrolls.Add(newPayroll);
            SavePayrollsToFile();        
            LoadPayrolls();
            MessageBox.Show("Thêm bảng lương thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);

            ClearInputs();
        }
        private void btnUpdatePayroll_Click(object sender, EventArgs e)
        {
            if (dgvPayrolls.CurrentRow != null)
            {
                Payroll selectedPayroll = (Payroll)dgvPayrolls.CurrentRow.DataBoundItem;
                //selectedPayroll.BaseSalary = decimal.Parse(txtBaseSalary.Text);
                selectedPayroll.AttendanceDay = int.Parse(txtAttendanceDay.Text);
                selectedPayroll.CalculateTotalSalary();
                SavePayrollsToFile();
                LoadPayrolls(); // Tải lại dữ liệu bảng lương
            }
            ClearInputs();
        }

        private void btnDeletePayroll_Click(object sender, EventArgs e)
        {
            if (dgvPayrolls.CurrentRow != null)
            {
                Payroll selectedPayroll = (Payroll)dgvPayrolls.CurrentRow.DataBoundItem;
                payrolls.Remove(selectedPayroll);
                SavePayrollsToFile();
                LoadPayrolls(); // Tải lại dữ liệu bảng lương
            }
            ClearInputs();
        }

        // Sự kiện khi thay đổi lựa chọn trong DataGridView
        private void dgvPayrolls_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvPayrolls.CurrentRow != null)
            {
                Payroll selectedPayroll = dgvPayrolls.CurrentRow.DataBoundItem as Payroll;
                if (selectedPayroll != null)
                {
                    txtBaseSalary.Text = selectedPayroll.BaseSalary.ToString();
                    txtAttendanceDay.Text = selectedPayroll.AttendanceDay.ToString();
                }
            }
        }
        private void InitializeEmployeeComboBox()
        {

            List<Employee> employees = new List<Employee>();


            foreach ((Employee,Payroll) tuple in EmployeeList.emp)
            {
                Employee employee = tuple.Item1;
                employees.Add(employee);
            }
         
            cmbEmployees.DataSource = employees;
            cmbEmployees.DisplayMember = "Name";      
            cmbEmployees.ValueMember = "EmployeeID1";  
            cmbEmployees.SelectedIndex = -1;
        }

        private void CmbEmployees_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbEmployees.SelectedValue is string selectedEmployeeID)
            {
                Employee selectedEmployee = null;
                Payroll payroll = null;

                // Tìm kiếm Employee và Payroll theo EmployeeID
                foreach ((Employee emp, Payroll pay) in EmployeeList.emp)
                {
                    if (selectedEmployeeID == emp.EmployeeID1)
                    {
                        selectedEmployee = emp;
                        payroll = pay;
                        break;
                    }
                }

                // Hiển thị thông tin lương cơ bản và ngày công
                if (selectedEmployee != null)
                {
                    if (payroll != null)
                    {
                        txtBaseSalary.Text = payroll.BaseSalary.ToString("C0") + " VND";
                    }
                    else
                    {
                        txtBaseSalary.Text = "Chưa có bảng lương";
                    }
                    txtAttendanceDay.Text = selectedEmployee.Attendances.Count.ToString();
                }
                else
                {
                    txtBaseSalary.Clear();
                    txtAttendanceDay.Clear();
                }
            }
        }

        private void InitializeComponent()
        {
            this.dgvPayrolls = new System.Windows.Forms.DataGridView();
            this.cmbEmployees = new System.Windows.Forms.ComboBox();
            this.txtBaseSalary = new System.Windows.Forms.TextBox();
            this.txtAttendanceDay = new System.Windows.Forms.TextBox();
            this.btnAddPayroll = new System.Windows.Forms.Button();
            this.btnUpdatePayroll = new System.Windows.Forms.Button();
            this.btnDeletePayroll = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvPayrolls)).BeginInit();
            this.SuspendLayout();
            // 
            // dgvPayrolls
            // 
            this.dgvPayrolls.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvPayrolls.Location = new System.Drawing.Point(12, 12);
            this.dgvPayrolls.Name = "dgvPayrolls";
            this.dgvPayrolls.RowHeadersWidth = 51;
            this.dgvPayrolls.Size = new System.Drawing.Size(800, 300);
            this.dgvPayrolls.TabIndex = 0;
            this.dgvPayrolls.SelectionChanged += new System.EventHandler(this.dgvPayrolls_SelectionChanged);
            // 
            // cmbEmployees
            // 
            this.cmbEmployees.Location = new System.Drawing.Point(12, 320);
            this.cmbEmployees.Name = "cmbEmployees";
            this.cmbEmployees.Size = new System.Drawing.Size(200, 28);
            this.cmbEmployees.TabIndex = 1;
            // 
            // txtBaseSalary
            // 
            this.txtBaseSalary.Location = new System.Drawing.Point(112, 347);
            this.txtBaseSalary.Name = "txtBaseSalary";
            this.txtBaseSalary.Size = new System.Drawing.Size(100, 26);
            this.txtBaseSalary.TabIndex = 2;
            // 
            // txtAttendanceDay
            // 
            this.txtAttendanceDay.Location = new System.Drawing.Point(112, 373);
            this.txtAttendanceDay.Name = "txtAttendanceDay";
            this.txtAttendanceDay.Size = new System.Drawing.Size(100, 26);
            this.txtAttendanceDay.TabIndex = 3;
            // 
            // btnAddPayroll
            // 
            this.btnAddPayroll.Location = new System.Drawing.Point(12, 410);
            this.btnAddPayroll.Name = "btnAddPayroll";
            this.btnAddPayroll.Size = new System.Drawing.Size(153, 59);
            this.btnAddPayroll.TabIndex = 4;
            this.btnAddPayroll.Text = "Add Payroll";
            this.btnAddPayroll.Click += new System.EventHandler(this.btnAddPayroll_Click);
            // 
            // btnUpdatePayroll
            // 
            this.btnUpdatePayroll.Location = new System.Drawing.Point(181, 410);
            this.btnUpdatePayroll.Name = "btnUpdatePayroll";
            this.btnUpdatePayroll.Size = new System.Drawing.Size(153, 59);
            this.btnUpdatePayroll.TabIndex = 5;
            this.btnUpdatePayroll.Text = "Update Payroll";
            this.btnUpdatePayroll.Click += new System.EventHandler(this.btnUpdatePayroll_Click);
            // 
            // btnDeletePayroll
            // 
            this.btnDeletePayroll.Location = new System.Drawing.Point(349, 410);
            this.btnDeletePayroll.Name = "btnDeletePayroll";
            this.btnDeletePayroll.Size = new System.Drawing.Size(153, 59);
            this.btnDeletePayroll.TabIndex = 6;
            this.btnDeletePayroll.Text = "Delete Payroll";
            this.btnDeletePayroll.Click += new System.EventHandler(this.btnDeletePayroll_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(30, 350);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(106, 20);
            this.label1.TabIndex = 14;
            this.label1.Text = "Lương cơ bản";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(7, 376);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(143, 20);
            this.label2.TabIndex = 15;
            this.label2.Text = "Tổng số ngày công";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(658, 414);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(154, 55);
            this.button1.TabIndex = 16;
            this.button1.Text = "Back";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // PayrollForm
            // 
            this.ClientSize = new System.Drawing.Size(826, 477);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.dgvPayrolls);
            this.Controls.Add(this.cmbEmployees);
            this.Controls.Add(this.txtBaseSalary);
            this.Controls.Add(this.txtAttendanceDay);
            this.Controls.Add(this.btnAddPayroll);
            this.Controls.Add(this.btnUpdatePayroll);
            this.Controls.Add(this.btnDeletePayroll);
            this.Name = "PayrollForm";
            this.Text = "Payroll Form";
            ((System.ComponentModel.ISupportInitialize)(this.dgvPayrolls)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        private void ClearInputs()
        {
            txtBaseSalary.Clear();
            txtAttendanceDay.Clear();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Employee ee = UserManager.GetEmployee();
            MainForm mainform = new MainForm(ee);
            mainform.Show();
            this.Close();
        }
    }
}