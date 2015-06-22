using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Closedown.Example.csharp
{
    public partial class closedownExample : Form
    {
        //��ũ��� ���Խ� �߱޹��� ��ũ���̵�
        private string LinkID = "TESTER";
        //��ũ��� ���Խ� �߱޹��� ���Ű
        private string SecretKey = "SwWxqU+0TErBXy/9TVjIPEnI0VTUMMSQZtJf3Ed8q3I=";

        private static ClosedownChecker closedownChecker;

        private const string CRLF = "\r\n";

        public closedownExample()
        {
            InitializeComponent();
            
            // �������ȸ Ŭ���� �ʱ�ȭ
            closedownChecker = new ClosedownChecker(LinkID, SecretKey);
        }

        //��ȸ �ܰ� Ȯ��
        private void btnUnitCost_Click(object sender, EventArgs e)
        {
            try
            {
                float unitCost = closedownChecker.GetUnitCost();

                MessageBox.Show("��ȸ�ܰ� : " + unitCost.ToString());

            }
            catch (ClosedownException ex)
            {
                MessageBox.Show(ex.code.ToString() + " | " + ex.Message);

            }
        }
        
        //�ܿ�����Ʈ Ȯ��
        private void btnGetBalance_Click(object sender, EventArgs e)
        {
            try
            {
                double remainPoint = closedownChecker.GetBalance();

                MessageBox.Show("�ܿ�����Ʈ : " + remainPoint.ToString());

            }
            catch (ClosedownException ex)
            {
                MessageBox.Show(ex.code.ToString() + " | " + ex.Message);
            }
        }

        //�������ȸ - �ܰ�
        private void btnCheckCorpNum_Click(object sender, EventArgs e)
        {
            try
            {
                String tmp;

                CorpState corpState = closedownChecker.checkCorpNum(txtCorpNum.Text);

                tmp = "corpNum : " + corpState.corpNum + CRLF;
                tmp += "state : " + corpState.state + CRLF;
                tmp += "type : " + corpState.type + CRLF;
                tmp += "stateDate(���������) : " + corpState.stateDate + CRLF;
                tmp += "checkDate(����ûȮ������) : " + corpState.checkDate + CRLF + CRLF;

                tmp += "* state (���������) : null-�˼�����, 0-��ϵ��� ���� ����ڹ�ȣ, 1-�����, 2-���, 3-�޾�" + CRLF;
                tmp += "* type (��� ����) : null-�˼�����, 1-�Ϲݰ�����, 2-�鼼������, 3-���̰�����, 4-�񿵸�����, �������" + CRLF;
                MessageBox.Show(tmp);
            
            }
            catch (ClosedownException ex)
            {
                MessageBox.Show(ex.code.ToString() + " | " + ex.Message);

            }
        }

        //�������ȸ - �뷮
        private void btnCheckCorpNums_Click(object sender, EventArgs e)
        {
            List<String> CorpNumList = new List<string>();
            
            //��ȸ�� ����ڹ�ȣ �迭, �ִ� 1000��
            CorpNumList.Add("1234567890");
            CorpNumList.Add("4352343543");
            CorpNumList.Add("4108600477");

            try
            {
                String tmp = null;

                List<CorpState> corpStateList = closedownChecker.checkCorpNums(CorpNumList);

                tmp += "* state (���������) : null-�˼�����, 0-��ϵ��� ���� ����ڹ�ȣ, 1-�����, 2-���, 3-�޾�" + CRLF;
                tmp += "* type (��� ����) : null-�˼�����, 1-�Ϲݰ�����, 2-�鼼������, 3-���̰�����, 4-�񿵸�����, �������" + CRLF + CRLF;

                for (int i = 0; i < corpStateList.Count; i++)
                {
                    tmp += "corpNum : " + corpStateList[i].corpNum + CRLF;
                    tmp += "state : " + corpStateList[i].state + CRLF;
                    tmp += "type : " + corpStateList[i].type + CRLF;
                    tmp += "stateDate(�����1����) : " + corpStateList[i].stateDate + CRLF;
                    tmp += "checkDate(����ûȮ������) : " + corpStateList[i].checkDate + CRLF + CRLF;
                }

                MessageBox.Show(tmp);

            }
            catch (ClosedownException ex)
            {
                MessageBox.Show(ex.code.ToString() + " | " + ex.Message);

            }

        }

        private void txtCorpNum_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnCheckCorpNum.PerformClick();
            }
            else
            {
                return;
            }
        }
    }
}