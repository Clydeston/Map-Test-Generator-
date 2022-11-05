using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace MapTestGen
{
    public partial class Form1 : Form
    {
        private List<Grid> Grid_List;
        private int Question_Amount;
        private QUESTION_TYPE eQuestion_Type;
        private BackgroundWorker backgroundWorker1;
        private List<ConventionalSign> convetinal_signs;

        public Form1()
        {
            InitializeComponent();
            InitializeWorkerThread();

            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.AutoScaleMode = AutoScaleMode.Dpi;
            this.

            // tabs 
            tabPage1.Text = "Question Generator";
            tabPage2.Text = "Grid Refernces";
            tabPage3.Text = "Resections";
            this.groupBox1.Controls.Add(this.radioButton2);
            this.groupBox1.Controls.Add(this.radioButton1);
            this.groupBox1.Text = "Amount";
            this.checkBox1.Text = "Distance";
            this.checkBox2.Text = "Bearing";
            this.checkBox3.Text = "At This Grid";
            this.radioButton1.Text = "10";
            this.radioButton2.Text = "20";

            comboBox1.DisplayMember = "Text";
            comboBox1.ValueMember = "Value";
            comboBox2.DisplayMember = "Text";
            comboBox2.ValueMember = "Value";

            comboBox1.Items.Add(new { Text = "SX", Value = "SX"});
            comboBox1.Items.Add(new { Text = "SY", Value = "SY" });

            foreach (ConventionalSigns sign in (ConventionalSigns[])Enum.GetValues(typeof(ConventionalSigns)))
            {
                comboBox2.Items.Add(new { Text = $"{sign}", Value = $"{sign}" });
            }

            comboBox1.SelectedIndex = 1;
            comboBox2.SelectedIndex = 1;
            comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox2.DropDownStyle = ComboBoxStyle.DropDownList;

            // list grids 
            ConventionalSign cg = new ConventionalSign();
            cg.dgv = dataGridView1;
            convetinal_signs = cg.GetConventionalSigns();
            if(convetinal_signs != null)
            {
                foreach (var item in convetinal_signs)
                {
                    try
                    {
                        DataGridViewRow row = (DataGridViewRow)dataGridView1.Rows[0].Clone();
                        row.Cells[0].Value = item.Grid_Reference.grid_s;
                        row.Cells[1].Value = Enum.GetName(typeof(ConventionalSigns), item.Type);
                        row.Cells[3].Value = item;
                        row.Cells[2].Value = item.Comment;

                        dataGridView1.Rows.Add(row);
                    }
                    catch
                    {
                        continue;
                    }
                }
            }

            DataGridViewButtonColumn btn = new DataGridViewButtonColumn();
            btn.HeaderText = "";
            btn.Text = "Delete";
            btn.Name = "del-btn";
            btn.UseColumnTextForButtonValue = true;
            btn.Width = 105;
            dataGridView1.Columns.Add(btn);
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.RowHeadersVisible = false;

            oGrid grid = new oGrid();
            Grid_List = grid.GenerateGridSquares();
        }

        private void InitializeWorkerThread()
        {
            backgroundWorker1 = new BackgroundWorker();
            backgroundWorker1.WorkerReportsProgress = true;
            backgroundWorker1.ProgressChanged += backgroundWorker1_ProgressChanged;
            backgroundWorker1.DoWork += backgroundWorker1_DoWork;
            backgroundWorker1.RunWorkerCompleted += backgroundWorker1_RunWorkerCompleted;
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            Question distance = new Question();
            distance.type = this.eQuestion_Type;
            distance.GenerateQuestion(this.Question_Amount, this.Grid_List, convetinal_signs);
        }
        void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            
        }

        void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            button1.Enabled = true;
        }

        private void checkedListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(!checkBox1.Checked && !checkBox2.Checked && !checkBox3.Checked)
            {
                MessageBox.Show("Please select a question type!");
                return;
            }

            if (!radioButton1.Checked && !radioButton2.Checked)
            {
                MessageBox.Show("Please select an amount!");
                return;
            }

            if(checkBox1.Checked)
            {
                if (checkBox3.Checked && checkBox2.Checked)
                {
                    this.eQuestion_Type = QUESTION_TYPE.BEARING_DISTANCE_SIGN;
                }
                else
                {
                    if(checkBox2.Checked)
                    {
                        this.eQuestion_Type = QUESTION_TYPE.DISTANCE_BEARING;
                    }
                    else
                    {
                        if(checkBox3.Checked)
                        {
                            this.eQuestion_Type = QUESTION_TYPE.DISTANCE_SIGN;
                        }
                        else
                        {
                            this.eQuestion_Type = QUESTION_TYPE.DISTANCE;
                        }                        
                    }                   
                }
            }

            if(checkBox2.Checked && !checkBox1.Checked)
            {
                if(checkBox3.Checked)
                {
                    this.eQuestion_Type = QUESTION_TYPE.BEARING_SIGN;
                }
                else
                {
                    this.eQuestion_Type = QUESTION_TYPE.BEARING;
                }
            }
            
            if(checkBox3.Checked && !checkBox2.Checked && !checkBox1.Checked)
            {
                this.eQuestion_Type = QUESTION_TYPE.CONVENTIONAL_SIGN;
            }

            button1.Enabled = false;
            backgroundWorker1.RunWorkerAsync();
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            this.Question_Amount = 10;
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            this.Question_Amount = 20;
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            if(textBox1.Text == "")
            {
                MessageBox.Show("Please enter a grid reference");
                return;
            }

            if(textBox1.Text.Length > 8 || textBox1.Text.Length < 8)
            {
                MessageBox.Show("Please enter an 8 figure grid reference");
                return;
            }
            
            foreach (char c in textBox1.Text)
            {
                if (c < '0' || c > '9')
                {
                    MessageBox.Show("Grid Reference Can Only Contain Numbers!");
                    return;
                }                    
            }

            Grid grid = new Grid
            {
                first_half = Convert.ToInt32(textBox1.Text.Substring(0, 4)),
                second_half = Convert.ToInt32(textBox1.Text.Substring(textBox1.TextLength / 2, textBox1.TextLength / 2)),
                full_grid = Convert.ToInt32(textBox1.Text),
                designation = comboBox1.Text,
                grid_s = comboBox1.Text + textBox1.Text,
            };

            ConventionalSign new_sign = new ConventionalSign()
            {
                Filename = comboBox1.Text + "_" + textBox1.Text,
                Type = (ConventionalSigns)Enum.Parse(typeof(ConventionalSigns), comboBox2.Text),
                Grid_Reference = grid, 
                File_Path = Path.Combine(Environment.CurrentDirectory, "Grids"),
                dgv = dataGridView1
            };

            new_sign.CreateNewEntry(convetinal_signs);
        }

        private void tabPage1_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            ConventionalSign cg = new ConventionalSign();
            cg.GetConventionalSigns();
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if(e.ColumnIndex == 3)
            {
                MessageBox.Show("yoo");
            }
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if(e.ColumnIndex == 4 && e.RowIndex != -1)
            {
                ConventionalSign cs = (ConventionalSign)dataGridView1.Rows[e.RowIndex].Cells[3].Value;
                cs.dgv = dataGridView1;
                cs.RemoveEntry(dataGridView1.Rows[e.RowIndex], convetinal_signs);                
            }

            if(e.ColumnIndex == 2 && e.RowIndex != -1) 
            {
                ConventionalSign cs = (ConventionalSign)dataGridView1.Rows[e.RowIndex].Cells[3].Value;
                cs.dgv = dataGridView1;
                
            }
        }

        private void button3_Click_1(object sender, EventArgs e)
        {

        }

        private void button3_Click_2(object sender, EventArgs e)
        {
            ConventionalSign cg = new ConventionalSign();
            cg.dgv = dataGridView1;

            cg.UpdateComments();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Form2 f2 = new Form2();
            f2.Show();
        }

        private void label7_Click(object sender, EventArgs e)
        {

        }

        private void label8_Click(object sender, EventArgs e)
        {

        }
    }
}
