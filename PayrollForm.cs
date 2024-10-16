using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Project_KTMH
{
    public partial class PayrollForm : Form
    {
        private List<Employee> employees;
        private List<Payroll> payrolls;
        private string payrollFilePath = "payrolls.json"; // Đổi tên file lưu trữ thành JSON

        public PayrollForm()
        {
            InitializeComponent();
            LoadData();
            LoadEmployees();
            LoadPayrolls();
        }

        // Phương Thức Tải Dữ Liệu từ File
        public void LoadData()
        {
            employees = LoadEmployeesFromFile("employees.json"); // Sử dụng JSON cho employees
            payrolls = LoadPayrollsFromFile(payrollFilePath);
        }

        // Phương Thức Tải Danh Sách Nhân Viên từ File JSON
        public List<Employee> LoadEmployeesFromFile(string filePath)
        {
            if (!File.Exists(filePath))
                return new List<Employee>();

            string json = File.ReadAllText(filePath);
            return JsonSerializer.Deserialize<List<Employee>>(json);
        }

        // Phương Thức Tải Danh Sách Lương từ File JSON
        public List<Payroll> LoadPayrollsFromFile(string filePath)
        {
            if (!File.Exists(filePath))
                return new List<Payroll>();

            string json = File.ReadAllText(filePath);
            return JsonSerializer.Deserialize<List<Payroll>>(json);
        }

        // Phương Thức Lưu Danh Sách Lương vào File JSON
        public void SavePayrollsToFile(string filePath)
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            string json = JsonSerializer.Serialize(payrolls, options);
            File.WriteAllText(filePath, json);
        }

        // Phương Thức Tải Danh Sách Nhân Viên vào ComboBox
        public void LoadEmployees()
        {
            cmbEmployees.DataSource = employees;
            cmbEmployees.DisplayMember = "Name";
            cmbEmployees.ValueMember = "EmployeeID";
            cmbEmployees.SelectedIndex = -1;
        }

        public void LoadPayrolls()
        {
            dgvPayrolls.DataSource = null;
            dgvPayrolls.DataSource = payrolls.Select(p => new
            {
                p.PayrollID,
                p.EmployeeID,
                p.EmployeeName,
                p.DepartmentID,
                p.SalaryCoefficient,
                p.SalaryCoefficientDepartment,
                p.SalaryCoefficientPosition,
                p.BaseSalary,
                p.OvertimeSalary,
                p.TotalSalary,
                p.AttendanceDay,
                p.BHXH,
                p.BHYT,
                p.Bonus,
                p.Tax,
                p.TotalDeductions,
                p.CalculationDate
            }).ToList();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // PayrollForm
            // 
            this.ClientSize = new System.Drawing.Size(1441, 517);
            this.Name = "PayrollForm";
            this.ResumeLayout(false);

        }
    }
}
