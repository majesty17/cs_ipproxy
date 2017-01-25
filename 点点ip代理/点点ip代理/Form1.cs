using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using HtmlAgilityPack;

namespace 点点ip代理
{
    public partial class Form1 : Form
    {
        private System.Net.WebProxy proxy = null;
        private String url = null;
        private String data_str = null;

        public Form1()
        {
            InitializeComponent();
        }

        //选择模式1
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            show_proxy1();
        }
        //翻页1
        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            show_proxy1();
        }
        //显示代理1
        private void show_proxy1() {
            WebClient web = new WebClient();
            int index = comboBox1.SelectedIndex;
            int page = Convert.ToInt32(numericUpDown1.Value);
            byte[] data = null;

            try
            {
                switch (index)
                {
                    case 0:
                        data = web.DownloadData("http://www.kuaidaili.com/free/inha/" + page + "/");
                        break;
                    case 1:
                        data = web.DownloadData("http://www.kuaidaili.com/free/intr/" + page + "/");
                        break;
                    case 2:
                        data = web.DownloadData("http://www.kuaidaili.com/free/outha/" + page + "/");
                        break;
                    case 3:
                        data = web.DownloadData("http://www.kuaidaili.com/free/outtr/" + page + "/");
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("数据抓取错误！请清除当前代理再试。");
                return;
            }
            String data_str = System.Text.Encoding.UTF8.GetString(data);
            listView1.Items.Clear();
            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(data_str);
            HtmlNodeCollection nodes = doc.DocumentNode.SelectSingleNode("//tbody").SelectNodes("tr");// .SelectNodes("//tbody");
            foreach (HtmlNode anode in nodes)
            {
                string ip = anode.SelectSingleNode("td[@data-title='IP']").InnerText;
                string port = anode.SelectSingleNode("td[@data-title='PORT']").InnerText;
                string hide = anode.SelectSingleNode("td[@data-title='匿名度']").InnerText;
                string type = anode.SelectSingleNode("td[@data-title='类型']").InnerText;
                string pos = anode.SelectSingleNode("td[@data-title='位置']").InnerText;
                string speed = anode.SelectSingleNode("td[@data-title='响应速度']").InnerText;
                string utime = anode.SelectSingleNode("td[@data-title='最后验证时间']").InnerText;

                ListViewItem lvi = new ListViewItem(ip);
                lvi.SubItems.Add(port);
                lvi.SubItems.Add(hide);
                lvi.SubItems.Add(type);
                lvi.SubItems.Add(pos);
                lvi.SubItems.Add(speed);
                lvi.SubItems.Add(utime);
                listView1.Items.Add(lvi);
            }
        }
        //双击选中
        private void listView1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (listView1.SelectedItems.Count <= 0) return;
            String ip = listView1.SelectedItems[0].SubItems[0].Text;
            String port = listView1.SelectedItems[0].SubItems[1].Text;

            textBox_testip.Text=ip;
            textBox_testport.Text=port;
            tabControl1.SelectedIndex=2;
        }
        
        
        //代理测试
        private void button_test_Click(object sender, EventArgs e)
        {
            String ip=textBox_testip.Text;
            String port=textBox_testport.Text;
            url=textBox_testurl.Text;
            proxy=new WebProxy(ip,Convert.ToInt32(port));
            System.Threading.Thread thd=new System.Threading.Thread(new System.Threading.ThreadStart(ThreadFunc));
            thd.Start();
        }

        //测试进程
        private void ThreadFunc() { 
            WebClient web=new WebClient();
			web.Proxy=this.proxy;
			System.DateTime start=System.DateTime.Now;
            byte[] data;
            try{
                data=web.DownloadData(this.url);
                this.data_str=System.Text.Encoding.Default.GetString(data);
            }
            catch(Exception ex){
                MessageBox.Show("此代理不可用，请更换代理或者更换网址再试。");
                return;
            }
            System.DateTime end=System.DateTime.Now;
            System.TimeSpan ts=end-start;
            //MessageBox::Show(IPProxy::Form1::_data);
            MessageBox.Show("延迟：" + (ts.Seconds * 1000 + ts.Milliseconds).ToString() + "毫秒");
        }

    }
}
