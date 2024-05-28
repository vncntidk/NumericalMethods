using System;
using System.Windows.Forms;
//a nuGet package that was recommended for parsing/reading the functions, and also it can be easily understood for me, unlike other packages that contains Math class
using NCalc;

    namespace NumericalMethods
    {
        public partial class Form1 : Form
        {
            public Form1()
            {
                InitializeComponent();

                //define the columns of the dgv first, so that the output on the iterations will display
                guna2DataGridView1.Columns.Add("Iteration", "Iteration");
                guna2DataGridView1.Columns.Add("x0", "x0");
                guna2DataGridView1.Columns.Add("x1", "x1");
                guna2DataGridView1.Columns.Add("f(x0)", "f(x0)");
                guna2DataGridView1.Columns.Add("f(x1)", "f(x1)");
                guna2DataGridView1.Columns.Add("Xnew", "Xnew");
                guna2DataGridView1.Columns.Add("Error", "Error");
            }

            //this will clear all entries that the user previously inputted
            private void btnClear_Click(object sender, EventArgs e)
            {
                txtFunction.Text = "";
                txtError.Text = "";
                txtX0.Text = "";
                txtX1.Text = "";
                txtIterations.Text = "";
                lblFinal.Visible = false;
                guna2DataGridView1.Rows.Clear();
            }

            private void btnCalculate_Click(object sender, EventArgs e)
            {
                string functionInput = txtFunction.Text;
                double x0, x1, tolerance;
                int iterations;

                //check if the inputted digits are valid
                if (!double.TryParse(txtX0.Text, out x0) || !double.TryParse(txtX1.Text, out x1) || !double.TryParse(txtError.Text, out tolerance) || !int.TryParse(txtIterations.Text, out iterations))
                {
                    MessageBox.Show("Please input valid numbers!");
                    return;
                }

                //this will replace all trigonometric function that the user inputted into a syntax that the c# or NCalc can read or process
                functionInput = functionInput.Replace("sin", "Sin").Replace("cos", "Cos").Replace("exp", "Exp").Replace("log", "Log");

                //this is the same as previous line, it will handle exponents or powers that the NCalc can process
                functionInput = System.Text.RegularExpressions.Regex.Replace(functionInput, @"(\d+)?x\^(\d+)", m =>
                {
                    //this process checks if there is a coefficient before the variables
                    string coefficient = m.Groups[1].Value == "" ? "1" : m.Groups[1].Value;
                    string exponent = m.Groups[2].Value;
                    return $"{coefficient} * Pow(x, {exponent})"; //if there is it will return as 2(or actual number) * x^3, if not, it will just set as 1 as the value of the coefficient
                });

                //this is a method that will read the equation or function that the user inputted, and also that will fit NCalc(same on the previous two lines)
                Func<double, double> f = (double x) =>
                {
                    var expression = new Expression(functionInput.Replace("x", x.ToString()));
                    return Convert.ToDouble(expression.Evaluate());
                };

                double x2 = 0;
                double f0, f1, f2;
                int iteration = 0;

                //incase if the user did not press clear, so that it will clear the previous result then add the new one
                guna2DataGridView1.Rows.Clear();

                //iterations in do while loop
                do
                {
                    //calls the method above and passing x0 and x1 as parameters then directly assigning it to f0 and f1
                    f0 = f(x0);
                    f1 = f(x1);

                    if (f1 - f0 == 0)
                    {
                        MessageBox.Show("x0 and x1 should not be the same");
                        return;
                    }

                    //Secant method formula
                    x2 = x1 - (f1 * (x1 - x0)) / (f1 - f0);
                    //this calculates the f(Xnew) 
                    f2 = f(x2);

                    //update x0 and x1
                    x0 = x1;
                    x1 = x2;    

                    //this makes sure that the percentage error is not negative
                    double error = Math.Abs(f2);

                    //this will add the outputs in dgv
                    guna2DataGridView1.Rows.Add(iteration + 1, x0.ToString("F4"), x1.ToString("F4"), f0.ToString("F4"), f1.ToString("F4"), x2.ToString("F4"), error.ToString("F4"));
                    iteration++;

                    if (error < tolerance)
                    {
                        //this shows the final root in messagebox first
                        MessageBox.Show($"Final root: x = {x2:F4}");
                        //then set the visibility of the label to true, then it will replace the text of the final root
                        lblFinal.Visible = true;
                        lblFinal.Text = x2.ToString("F4");
                        return;
                    }

                } while (iteration < iterations); //break if the iteration reached the maximum iteration that the user inputted

                //if the estimated error is not reached yet but it already reached the maximum iterations, this message will pop up
                MessageBox.Show($"Maximum iterations reached. Last approximation: x = {x2:F4}");
                lblFinal.Visible = true;
                lblFinal.Text = x2.ToString("F4");
            }
        }
    }
