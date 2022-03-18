using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Calculator
{
    public partial class MainPage : ContentPage
    {
        List<string> ScreenValue = new List<string> { "0" };
        List<string> val1 = new List<string> { "0" };
        List<string> val2 = new List<string> { "0" };
        private string temp = "";

        private bool op_state = false;
        private bool hasCalc = false;
        private bool hasSet = false;
        private bool isStart = true;
        private bool hasDecimal = false;
        private string operand = "";

        public MainPage()
        {
            InitializeComponent();
            InitializeCalc();
        }

        private void InitializeCalc()
        {
            GenerateNumberBtn();
            GenerateOperandBtn();
            GenerateFunctionBtns();
            calcScroll.HorizontalScrollBarVisibility = ScrollBarVisibility.Never;
        }

        private Button CreateButton(bool type, string val)
        {
            Button b = new Button
            {
                Text = val,
                FontSize = 25,
                BackgroundColor = Color.Gray,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.FillAndExpand,
            };

            if (!type)
            {
                b.TextColor = Color.White;
                b.BackgroundColor = Color.Orange;
            }

            return b;
        }

        private void GenerateNumberBtn()
        {
            string[] numbers = { "7", "8", "9", "4", "5", "6", "1", "2", "3" };
            for (int i = 3, y = 0; i < 6; i++)
            {
                for (int x = 0; x < 3; x++, y++)
                {
                    //Create Button Background
                    BoxView bv = new BoxView { BackgroundColor = Color.Gray };
                    calcGrid.Children.Add(bv, x, i);
                    //Create Number Button
                    Button btn = CreateButton(true, numbers[y]);
                    //Create Button Function
                    btn.Clicked += (sender, args) =>
                    {
                        string UserInput = (sender as Button).Text;
                        SetInputValue(UserInput);
                    };
                    calcGrid.Children.Add(btn, x, i);
                }
            }
            //For the Zero button
            BoxView zbv = new BoxView { BackgroundColor = Color.Gray };
            calcGrid.Children.Add(zbv, 0, 6);
            Grid.SetColumnSpan(zbv, 2);

            Button zbtn = CreateButton(true, "0");
            zbtn.HorizontalOptions = LayoutOptions.FillAndExpand;
            zbtn.Clicked += (sender, args) =>
            {
                string UserInput = (sender as Button).Text;
                SetInputValue(UserInput);
            };
            calcGrid.Children.Add(zbtn, 0, 6);
            Grid.SetColumnSpan(zbtn, 2);
        }

        private void GenerateOperandBtn()
        {
            string[] operands = { "/", "X", "-", "+", "="};
            for(int i = 0; i < 5; i++)
            {
                BoxView bv = new BoxView { BackgroundColor = Color.Orange};
                calcGrid.Children.Add(bv, 3, i + 2);

                Button btn = CreateButton(false, operands[i]);
                btn.Clicked += (sender, args) =>
                {
                    string UserInput = (sender as Button).Text;
                    SetOperandValue(UserInput);
                };

                calcGrid.Children.Add(btn, 3, i + 2);
            }
        }

        private void GenerateFunctionBtns()
        {
            //Clear Button
            BoxView cbv = new BoxView { BackgroundColor = Color.Gray };
            calcGrid.Children.Add(cbv, 0, 2);
            Grid.SetColumnSpan(cbv, 2);
            Button cbtn = CreateButton(true ,"C");
            cbtn.HorizontalOptions = LayoutOptions.FillAndExpand;
            cbtn.Clicked += (sender, args) => 
            {
                val1.Clear();
                val2.Clear();
                ScreenValue.Clear();
                operand = "";
                val1.Add("0");
                val2.Add("0");
                isStart = true;
                hasSet = false;
                hasCalc = false;
                hasDecimal = false;
                UpdateScreenValue(val1);
                miniScreen.Text = "";
            };
            calcGrid.Children.Add(cbtn, 0, 2);
            Grid.SetColumnSpan(cbtn, 2);

            //Decimal Button
            BoxView dbv = new BoxView { BackgroundColor = Color.Gray };
            calcGrid.Children.Add(dbv, 2, 6);
            Button dbtn = CreateButton(true, ".");
            dbtn.Clicked += (sender, args) =>
            {
                string UserInput = (sender as Button).Text;
                SetInputValue(UserInput);
            };
            calcGrid.Children.Add(dbtn, 2, 6);

            //Delete Button
            BoxView delbv = new BoxView { BackgroundColor = Color.Gray };
            calcGrid.Children.Add(delbv, 2, 2);
            Button delbtn = CreateButton(true, "DEL");
            delbtn.Clicked += (sender, args) => { DelButton(); };
            calcGrid.Children.Add(delbtn, 2, 2);
        }

        private void DelButton()
        {
            if (!hasSet)
                DeleteOperation(val1);
            else
                DeleteOperation(val2);
        }

        private void DeleteOperation(List<string> val)
        {
            int loc = val.Count;
            if (val.Any() && !isStart)
            {
                if (string.Equals(val.Last(), "."))
                {
                    hasDecimal = false;
                    if (string.Equals(val.First(), "0"))
                        isStart = true;
                }
                val.RemoveAt(loc - 1);
            }
            if (!val.Any())
            {
                val.Add("0");
                ResetValues();
            }
            if(!(op_state && isStart))   
                UpdateScreenValue(val);
            if (!(op_state && isStart) && hasCalc)
                miniScreen.Text = "";
        }

        private void ResetValues()
        {
            isStart = true;
            hasDecimal = false;
        }

        private void SetOperandValue(string value)
        {
            bool isEqual = string.Equals(value, "=");

            if (isEqual)
            {
                double result = GetResult();
                UpdateScreenValue(TranslateToList(result));
            }
            else
            {
                temp = GetListValue(val1).ToString();

                if (!isEqual && hasSet && !op_state)
                    UpdateOperand();

                if (!isEqual)
                    operand = value;

                hasSet = true;
                op_state = true;
            }

            isStart = true;
            hasDecimal = false;

            UpdateMiniScreen();
        }

        private void UpdateOperand()
        {
            double result = GetResult();
            temp = result.ToString();
            UpdateScreenValue(TranslateToList(result));
            UpdateMiniScreen();
        }

        private double GetResult()
        {
            if(hasSet && isStart)
            {
                CopyList(val2, val1);
            }

            double result = 0;
            double num1 = GetListValue(val1);
            double num2 = GetListValue(val2);
            hasCalc = true;
            hasDecimal = false;

            switch (operand)
            {
                case "/":
                    result = num1 / num2;
                    break;
                case "X":
                    result = num1 * num2;
                    break;
                case "-":
                    result = num1 - num2;
                    break;
                case "+":
                    result = num1 + num2;
                    break;
                default:
                    break;
            }
 
            return result;
        }

        private void UpdateMiniScreen()
        {
            double num = GetListValue(val2);
            if (!hasSet && !hasCalc)
                miniScreen.Text = "";
            else if (hasCalc && !hasSet)
            {
                miniScreen.Text = temp + " " + operand + " " + num.ToString() + " =";
            }
            else
                miniScreen.Text = temp + " " + operand;
        }

        //Decides which val to update
        private void SetInputValue(string value)
        {
            if (!hasSet)
                UpdateInput(val1, value);
            else
                UpdateInput(val2, value);
        }

        //Updates the value of val
        private void UpdateInput(List<string> val, string number)
        {
            if(string.Equals(number,"0") &&  isStart) { val.Clear(); val.Add(number); }
            else
            {
                if (isStart)
                {
                    if(string.Equals(number, ".") && (hasCalc || val.Count > 1) )
                    {
                        val.Clear();
                        val.Add("0");
                    }
                    if (string.Equals(number, ".")) { }
                    else
                        val.Clear();
                    isStart = false;
                }
                if(string.Equals(number, ".") && hasDecimal) { }
                else
                {   
                    op_state = false;
                    hasCalc = false;
                    if (string.Equals(number, "."))
                        hasDecimal = true;
                    val.Add(number);
                }
            }
            UpdateScreenValue(val);
            OnScrollViewScrolled();
        }

        private void OnScrollViewScrolled()
        {
            calcScroll.ScrollToAsync(calcScreen, ScrollToPosition.End, false);
        }

        private void UpdateScreenValue(List<string> val)
        {
            CopyList(ScreenValue, val);
            UpdateScreen();
            UpdateMiniScreen();
        }

        private void UpdateScreen()
        {
            int count = ScreenValue.Count;

            if (ScreenValue.Any())
            {
                if (count >= 8)
                    calcScreen.FontSize = 60;
                else
                    calcScreen.FontSize = 90;
                calcScreen.Text = " ";
                foreach (string item in ScreenValue)
                    calcScreen.Text = string.Concat(calcScreen.Text, item);
            }
            else
                calcScreen.Text = "0";
        }

        private double GetListValue(List<string> number)
        {
            string sValue = "";

            for (int i = 0; i < number.Count; i++)
                sValue = String.Concat(sValue, number[i]);

            double val = double.Parse(sValue);

            return val;
        }

        private void CopyList(List<string> dest, List<string> source)
        {
            dest.Clear();
            foreach (string item in source)
                dest.Add(item);
        }

        private List<string> TranslateToList(double val)
        {
            string number = val.ToString();
            List<string> translatedList = number.Select(x => x.ToString()).ToList();

            CopyList(val1, translatedList);
            hasCalc = true;
            hasSet = false;
            return translatedList;
        }
    }
}
