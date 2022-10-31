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
        public Form1()
        {
            InitializeComponent();

            // tabs 
            tabPage1.Text = "Question Generator";
            tabPage2.Text = "Grid Refernces";

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
            List<ConventionalSign> convetinal_signs = cg.GetConventionalSigns();
            if(convetinal_signs != null)
            {
                foreach (var item in convetinal_signs)
                {
                    DataGridViewRow row = (DataGridViewRow)dataGridView1.Rows[0].Clone();
                    row.Cells[0].Value = item.Grid_Reference.grid_s;
                    row.Cells[1].Value = Enum.GetName(typeof(ConventionalSigns), item.Type);
                    //row.Cells["Column3"].Value = Enum.GetName(typeof(ConventionalSigns), item.Type);
                   
                    dataGridView1.Rows.Add(row);
                }
            }

            oGrid grid = new oGrid();
            Grid_List = grid.GenerateGridSquares();
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

            //MessageBox.Show(this.eQuestion_Type.ToString());
            Question distance = new Question();
            distance.type = this.eQuestion_Type;
            distance.GenerateQuestion(this.Question_Amount, this.Grid_List);
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
                File_Path = Path.Combine(Environment.CurrentDirectory, "Grids")
            };

            new_sign.CreateNewEntry();
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

        }
    }
}
