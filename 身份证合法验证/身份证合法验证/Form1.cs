using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace 身份证合法验证
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        string ID;      //定义身份证号

        private void button1_Click(object sender, EventArgs e)
        {
            ID = this.textBox1.Text;    //读取身份证输入
            //this.label1.Text = ID;    //测试输入

            bool result1 = formatCheck(ID);
            if (result1)
            {
                label1.Text = "身份证格式正确";
            }
            else
            {
                label1.Text = "！身份证格式不正确！";
            }
            
            bool result2 = CheckChinaIDCardNumberFormat(ID);           
            if (result2)
            {
                label2.Text = "身份证最后一位有效位合法";
            }
            else
            {
                label2.Text = "！身份证最后一位有效位不合法！";
            }
        }


        public static bool formatCheck(string idCardNumber)
        {
            string idNumber = idCardNumber;
            bool result = true;     //定义判断结果

            System.Text.RegularExpressions.Regex rgX = new System.Text.RegularExpressions.Regex(@"^\d{17}X$");
            System.Text.RegularExpressions.Regex rgN = new System.Text.RegularExpressions.Regex(@"^\d{18}$");
            System.Text.RegularExpressions.Match mc1 = rgX.Match(idNumber);
            System.Text.RegularExpressions.Match mc2 = rgN.Match(idNumber);

            if (!mc1.Success && !mc2.Success)   //两种格式都不符合
            {
                result = false;
            }

            return result;

        }

        public static bool CheckChinaIDCardNumberFormat(string idCardNumber)
        {
            string idNumber = idCardNumber;
            bool result = true;     //定义判断结果
            try
            {
                if (idNumber.Length != 18)  //输入身份证长度
                {
                    return false;
                }

                //数字验证 
                long n = 0;
                if (long.TryParse(idNumber.Remove(17), out n) == false
                    || n < Math.Pow(10, 16) || long.TryParse(idNumber.Replace('x', '0').Replace('X', '0'), out n) == false)
                {
                    return false;    
                }

                //省份验证  
                string address = "11x22x35x44x53x12x23x36x45x54x13x31x37x46x61x14x32x41x50x62x15x33x42x51x63x21x34x43x52x64x65x71x81x82x91";
                if (address.IndexOf(idNumber.Remove(2)) == -1)
                {
                    return false;   
                }

                //生日验证 
                string birth = idNumber.Substring(6, 8).Insert(6, "-").Insert(4, "-");
                DateTime time = new DateTime();
                if (DateTime.TryParse(birth, out time) == false)
                {
                    return false;    
                }

                //校验码验证  
                string[] arrVarifyCode = ("1,0,x,9,8,7,6,5,4,3,2").Split(',');
                string[] Wi = ("7,9,10,5,8,4,2,1,6,3,7,9,10,5,8,4,2").Split(',');   //从高位到低位相对应的加权因子值
                char[] Ai = idNumber.Remove(17).ToCharArray();
                int sum = 0;
                for (int i = 0; i < 17; i++)
                {
                    sum += int.Parse(Wi[i]) * int.Parse(Ai[i].ToString());
                }
                int y = -1;
                Math.DivRem(sum, 11, out y);
                if (arrVarifyCode[y] != idNumber.Substring(17, 1).ToLower())
                {
                    return false;   
                }
                return true;//符合GB11643-1999标准 

            }
            catch (Exception ex)
            {
                result = false;
            }
            return result;
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            //使用红色虚线绘制边框
            Pen pen1 = new Pen(Color.Red, 1);
            pen1.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
            pen1.DashPattern = new float[] { 4f, 2f };
            e.Graphics.DrawRectangle(pen1, 0, 0, this.panel1.Width - 1, this.panel1.Height - 1);
        }
        /*
        Boolean textboxHasText = false;     //判断输入框是否有文本
        
        //textbox获得焦点
        private void Textbox_Enter(object sender, EventArgs e)
        {
            if (textboxHasText == false)
                textBox1.Text = "";

            textBox1.ForeColor = Color.Black;
        }
        
        //textbox失去焦点
        private void Textbox_Leave(object sender, EventArgs e)
        {
            if (textBox1.Text == "")
            {
                textBox1.Text = "请输入身份证号";
                textBox1.ForeColor = Color.LightGray;
                textboxHasText = false;
            }
            else
                textboxHasText = true;
        }
        */
        private void txtID_Enter(object sender, EventArgs e)
        {
            if (textBox1.Text.Trim() == "请输入身份证号")
            {
                textBox1.ForeColor = ColorTranslator.FromHtml("#333333");
                textBox1.Text = "";
                textBox1.PasswordChar = '*';
            }
        }

        private void txtID_Leave(object sender, EventArgs e)
        {
            if (textBox1.Text.Trim().Length == 0)
            {
                textBox1.ForeColor = ColorTranslator.FromHtml("#999999");
                textBox1.PasswordChar = '\0';
                textBox1.Text = "请输入身份证号";
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            txtID_Enter(null, EventArgs.Empty);
            txtID_Leave(null, EventArgs.Empty);
        }
    }
}
