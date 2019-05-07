using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace DownLoadDB
{
	public partial class Form1 : Form
	{
		public Form1()
		{
			InitializeComponent();
		}
		public void BindDB()
		{
			string connStr = @"Data Source="+textBox1.Text+";Initial Catalog=master;Persist Security Info=True;User ID="+textBox2.Text+";Password="+textBox3.Text+"";
			try
			{
				string strSql = @"select name AS id, name AS text From dbo.sysdatabases";
				List<string> listDB = GetList(connStr, strSql).ToList();
				comboBox1.DataSource = listDB;
				comboBox1.DisplayMember = "text";
			}
		   catch(Exception ex)
			{
				MessageBox.Show(""+ex.Message+"！");
			}
		}

		private void TextBox3_Leave(object sender, EventArgs e)
		{
			BindDB();
		}
		
		/* select name from sysobjects where xtype='u' ---
C = CHECK 约束
D = 默认值或 DEFAULT 约束
F = FOREIGN KEY 约束
L = 日志
FN = 标量函数
IF = 内嵌表函数
P = 存储过程
PK = PRIMARY KEY 约束（类型是 K）
RF = 复制筛选存储过程
S = 系统表
TF = 表函数
TR = 触发器
U = 用户表
UQ = UNIQUE 约束（类型是 K）
V = 视图
X = 扩展存储过程
*/
		public string[] GetTableList(string connString)
		{
			string strSql = "SELECT name FROM sysobjects WHERE xtype='U' AND name  <>  'dtproperties' order by name asc";
			return GetList(connString,strSql);
		}
		public string[] GetPROList(string connString)
		{
			string strSql = "SELECT name FROM sysobjects WHERE xtype='P' AND name  <>  'dtproperties' order by name asc";
			return GetList(connString, strSql);
		}
		private string[] GetList(string connString, string sql)
		{
			if (String.IsNullOrEmpty(connString)) return null;
			string connStr = connString;
			SqlConnection conn = new SqlConnection(connStr);
			SqlCommand cmd = new SqlCommand(sql, conn);
			cmd.CommandType = CommandType.Text;
			cmd.CommandTimeout = 600;
			try
			{
				conn.Open();
				List<string> ret = new List<string>();
				using (SqlDataReader MyReader = cmd.ExecuteReader())
				{
					while (MyReader.Read())
					{
						ret.Add(MyReader[0].ToString());
					}
				}
				if (ret.Count > 0) return ret.ToArray();
				return null;
			}
			finally
			{
				conn.Close();
			}
		}

		private void ComboBox1_SelectedIndexChanged(object sender, EventArgs e)
		{
			string strDB = comboBox1.Text;
			string connStr = @"Data Source=" + textBox1.Text + ";Initial Catalog=" + strDB + ";Persist Security Info=True;User ID=" + textBox2.Text + ";Password=" + textBox3.Text + "";
			try
			{
				List<string> listTable = GetTableList(connStr).ToList();
				List<string> listPRO = GetPROList(connStr).ToList();
				comboBox2.DataSource = listTable;
				comboBox3.DataSource = listPRO;
			}
			catch (Exception ex)
			{
				MessageBox.Show("" + ex.Message + "！");
			}
		}
		
		private void ComboBox3_SelectedIndexChanged(object sender, EventArgs e)
		{
			string strDB = comboBox1.Text;
			string connStr = @"Data Source=" + textBox1.Text + ";Initial Catalog=" + strDB + ";Persist Security Info=True;User ID=" + textBox2.Text + ";Password=" + textBox3.Text + "";
			SqlConnection conn = new SqlConnection(connStr);
			SqlCommand cmd = new SqlCommand();
			cmd.Connection = conn;
			cmd.CommandText = "" + comboBox3.Text + "";
			cmd.CommandType = CommandType.StoredProcedure;
			try
			{
				conn.Open();
				SqlCommandBuilder.DeriveParameters(cmd);
				int i = 0;
				foreach (Control item in groupBox1.Controls)
				{
					groupBox1.Controls.Remove(item);
				}
				
				foreach (SqlParameter var in cmd.Parameters)
				{
					if (cmd.Parameters.IndexOf(var) == 0) continue;//Skip return value

					Label label = new Label();
					label.Text = var.ParameterName.Replace("@","");
					label.Name = var.ParameterName;
					label.Location = new Point(30+i, 50);
					TextBox textBox = new TextBox();
					textBox.Name = "txt" + var.ParameterName.Replace("@", "");
					textBox.Location = new Point(100+i, 50);
					groupBox1.Controls.Add(label);
					groupBox1.Controls.Add(textBox);
					i += 100;
				}
			}
			finally
			{
				conn.Close();
			}
		}
	}
}
