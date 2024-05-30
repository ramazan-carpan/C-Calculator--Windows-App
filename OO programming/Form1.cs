using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;

namespace OO_programming
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            LoadEmployees();
        }

        private void LoadEmployees()
        {
            try
            {
                var employees = new List<PaySlip>();
                using (var reader = new StreamReader("../../../employee.csv"))
                {

                    while (!reader.EndOfStream)
                    {
                        var line = reader.ReadLine();
                        var values = line.Split(',');

                        if (values.Length >= 5)
                        {
                            var employee = new PaySlip
                            {
                                EmployeeID = int.Parse(values[0]),
                                FirstName = values[1],
                                LastName = values[2],
                                HourlyRate = decimal.Parse(values[3]),
                                TaxThreshold = values[4]
                            };

                            employees.Add(employee);
                        }
                    }
                }

                listBox1.DataSource = employees;
                listBox1.DisplayMember = "ToString";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading CSV file: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedItem is PaySlip selectedEmployee)
            {
                selectedEmployee.WeekHours = decimal.Parse(textBox1.Text);

                PayCalculator payCalculator;
                if (selectedEmployee.TaxThreshold.ToLower() == "y")
                {
                    payCalculator = new PayCalculatorWithThreshold("../../../taxrate-withthreshold.csv");
                }
                else
                {
                    payCalculator = new PayCalculatorNoThreshold("../../../taxrate-nothreshold.csv");
                }

                selectedEmployee.CalculatePay(payCalculator);

                textBox2.Clear();
                textBox2.AppendText($"Employee ID: {selectedEmployee.EmployeeID}\n");
                textBox2.AppendText($"Name: {selectedEmployee.FirstName} {selectedEmployee.LastName}\n");
                textBox2.AppendText($"Gross Pay: {selectedEmployee.GrossPay:C}\n");
                textBox2.AppendText($"Tax: {selectedEmployee.Tax:C}\n");
                textBox2.AppendText($"Net Pay: {selectedEmployee.NetPay:C}\n");
                textBox2.AppendText($"Superannuation: {selectedEmployee.Superannuation:C}\n");
            }
        }


        private void button2_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedItem is PaySlip selectedEmployee)
            {
                string fileName = $"../../../Pay_{selectedEmployee.FirstName}_{selectedEmployee.LastName}_{DateTime.Now:yyyyMMddHHmmss}.csv";
                var lines = new List<string>
        {
            "EmployeeID,FullName,HoursWorked,HourlyRate,TaxThreshold,GrossPay,Tax,NetPay,Superannuation"
        };

                PayCalculator payCalculator;
                if (selectedEmployee.TaxThreshold.ToLower() == "y")
                {
                    payCalculator = new PayCalculatorWithThreshold("../../../taxrate-withthreshold.csv");
                }
                else
                {
                    payCalculator = new PayCalculatorNoThreshold("../../../taxrate-nothreshold.csv");
                }

                selectedEmployee.CalculatePay(payCalculator);

                string line = $"{selectedEmployee.EmployeeID},{selectedEmployee.FirstName} {selectedEmployee.LastName}," +
                              $"{selectedEmployee.WeekHours},{selectedEmployee.HourlyRate},{selectedEmployee.TaxThreshold}," +
                              $"{selectedEmployee.GrossPay},{selectedEmployee.Tax},{selectedEmployee.NetPay},{selectedEmployee.Superannuation}";

                lines.Add(line);
                try
                {
                    File.WriteAllLines(fileName, lines);
                    MessageBox.Show($"Data saved to {fileName}", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error saving data: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                };
            }
        }
    }
}
