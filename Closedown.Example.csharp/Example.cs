using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Closedown.Example.csharp
{
    public partial class closedownExample : Form
    {
        //링크허브 가입시 발급받은 링크아이디
        private string LinkID = "TESTER";
        //링크허브 가입시 발급받은 비밀키
        private string SecretKey = "SwWxqU+0TErBXy/9TVjIPEnI0VTUMMSQZtJf3Ed8q3I=";

        private static ClosedownChecker closedownChecker;

        private const string CRLF = "\r\n";

        public closedownExample()
        {
            InitializeComponent();
            
            // 휴폐업조회 클래스 초기화
            closedownChecker = new ClosedownChecker(LinkID, SecretKey);
        }

        //조회 단가 확인
        private void btnUnitCost_Click(object sender, EventArgs e)
        {
            try
            {
                float unitCost = closedownChecker.GetUnitCost();

                MessageBox.Show("조회단가 : " + unitCost.ToString());

            }
            catch (ClosedownException ex)
            {
                MessageBox.Show(ex.code.ToString() + " | " + ex.Message);

            }
        }
        
        //잔여포인트 확인
        private void btnGetBalance_Click(object sender, EventArgs e)
        {
            try
            {
                double remainPoint = closedownChecker.GetBalance();

                MessageBox.Show("잔여포인트 : " + remainPoint.ToString());

            }
            catch (ClosedownException ex)
            {
                MessageBox.Show(ex.code.ToString() + " | " + ex.Message);
            }
        }

        //휴폐업조회 - 단건
        private void btnCheckCorpNum_Click(object sender, EventArgs e)
        {
            try
            {
                String tmp;

                CorpState corpState = closedownChecker.checkCorpNum(txtCorpNum.Text);

                tmp = "corpNum : " + corpState.corpNum + CRLF;
                tmp += "state : " + corpState.state + CRLF;
                tmp += "type : " + corpState.type + CRLF;
                tmp += "stateDate(휴폐업일자) : " + corpState.stateDate + CRLF;
                tmp += "checkDate(국세청확인일자) : " + corpState.checkDate + CRLF + CRLF;

                tmp += "* state (휴폐업상태) : null-알수없음, 0-등록되지 않은 사업자번호, 1-사업중, 2-폐업, 3-휴업" + CRLF;
                tmp += "* type (사업 유형) : null-알수없음, 1-일반과세자, 2-면세과세자, 3-간이과세자, 4-비영리법인, 국가기관" + CRLF;
                MessageBox.Show(tmp);
            
            }
            catch (ClosedownException ex)
            {
                MessageBox.Show(ex.code.ToString() + " | " + ex.Message);

            }
        }

        //휴폐업조회 - 대량
        private void btnCheckCorpNums_Click(object sender, EventArgs e)
        {
            List<String> CorpNumList = new List<string>();
            
            //조회할 사업자번호 배열, 최대 1000건
            CorpNumList.Add("1234567890");
            CorpNumList.Add("4352343543");
            CorpNumList.Add("4108600477");

            try
            {
                String tmp = null;

                List<CorpState> corpStateList = closedownChecker.checkCorpNums(CorpNumList);

                tmp += "* state (휴폐업상태) : null-알수없음, 0-등록되지 않은 사업자번호, 1-사업중, 2-폐업, 3-휴업" + CRLF;
                tmp += "* type (사업 유형) : null-알수없음, 1-일반과세자, 2-면세과세자, 3-간이과세자, 4-비영리법인, 국가기관" + CRLF + CRLF;

                for (int i = 0; i < corpStateList.Count; i++)
                {
                    tmp += "corpNum : " + corpStateList[i].corpNum + CRLF;
                    tmp += "state : " + corpStateList[i].state + CRLF;
                    tmp += "type : " + corpStateList[i].type + CRLF;
                    tmp += "stateDate(휴폐업1일자) : " + corpStateList[i].stateDate + CRLF;
                    tmp += "checkDate(국세청확인일자) : " + corpStateList[i].checkDate + CRLF + CRLF;
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