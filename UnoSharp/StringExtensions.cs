using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnoSharp
{
    public static class StringExtensions
    {
        public static Card ToCard(this string source)
        {
            CardColor? color = null;
            CardValue? value = null;
            var s = source.ToUpper();
            foreach (var specialCard in Card.SpecialCards.Where(specialCard => s == specialCard.ShortName))
                return specialCard;
            
            switch (s.Length) {
                case 2:
                    var first = s.Substring(0, s.Length - 1);
                    var last = s.Substring(1, s.Length - 1);
                    Set(first, last);
                    break;
                case 3:
                    var f1 = s.Substring(0, 1);
                    var l1 = s.Substring(1, 2);
                    Set(f1, l1);
                    if (color == null || value == null)
                    {
                        color = null;
                        value = null;
                    }

                    var f2 = s.Substring(0, 2);
                    var l2 = s.Substring(2, 1);
                    Set(f2, l2);
                    break;
                default:
                    return null;
            }

            if (color == null || value == null) {
                return null;
            } else {
                return new Card(value.Value, color.Value);
            }

            void Set(string first, string last)
            {
                if (first.Contains("R")) {
                    SetOne(last, false);
                    SetOne(first, true);
                    return;
                }

//                if (last.Contains("R")) {
//                    SetOne(first, true);
//                    SetOne(last, false);
//                    return;
//                }
                SetOne(first, true);
                SetOne(last, false);
            }

            void SetOne(string str, bool isFirst)
            {
                if (SetValue(str, isFirst))
                    return;
                SetColor(str);
            }

            bool SetColor(string str)
            {
                if (color != null)
                    return false;
                switch (str) {
                    case "绿":
                    case "G":
                        color = CardColor.Green;
                        return true;
                    case "蓝":
                    case "B":
                        color = CardColor.Blue;
                        return true;
                    case "红":
                    case "R":
                        color = CardColor.Red;
                        return true;
                    case "黄":
                    case "Y":
                        color = CardColor.Yellow;
                        return true;
                }

                return false;
            }

            bool SetValue(string str, bool isFirst)
            {
                if (value != null)
                    return false;
                switch (str)
                {
                    case "W":
                        value = CardValue.Wild;
                        return true;
                    case "S":
                    case "禁":
                        value = CardValue.Skip;
                        return true;
                    case "R":
                        value = CardValue.Reverse;
                        return true;
                    case "+2":
                        value = CardValue.DrawTwo;
                        return true;
                    case "+4":
                        value = CardValue.DrawFour;
                        return true;
                    case "转":
                        value = isFirst ? CardValue.Wild : CardValue.Reverse;
                        return true;
                }

                if (str.Length == 1 && int.TryParse(str, out var num))
                {
                    value = (CardValue) num;
                    return true;
                }

                return false;
            }
        }
    }
}
