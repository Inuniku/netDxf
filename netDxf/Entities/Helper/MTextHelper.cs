using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace netDxf.Entities.Helper
{
    public static class MTextHelper
    {
        public static string MTextToPlainText(string mtext)
        {
            if (string.IsNullOrEmpty(mtext))
                return string.Empty;

            string txt = mtext;

            //text = text.Replace("%%c", "Ø");
            //text = text.Replace("%%d", "°");
            //text = text.Replace("%%p", "±");

            StringBuilder rawText = new StringBuilder();

            using (CharEnumerator chars = txt.GetEnumerator())
            {
                while (chars.MoveNext())
                {
                    char token = chars.Current;
                    if (token == '\\') // is a formatting command
                    {
                        if (chars.MoveNext())
                            token = chars.Current;
                        else
                            return rawText.ToString(); // premature end of text

                        if (token == '\\' | token == '{' | token == '}') // escape chars
                            rawText.Append(token);
                        else if (token == 'L' | token == 'l' | token == 'O' | token == 'o' | token == 'K' | token == 'k')
                        {
                            /* discard one char commands */
                        }
                        else if (token == 'P' | token == 'X')
                            rawText.Append(Environment.NewLine); // replace the end paragraph command with the standard new line, carriage return code "\r\n".
                        else if (token == 'S')
                        {
                            if (chars.MoveNext())
                                token = chars.Current;
                            else
                                return rawText.ToString(); // premature end of text

                            // we want to preserve the text under the stacking command
                            StringBuilder data = new StringBuilder();

                            while (token != ';')
                            {
                                if (token == '\\')
                                {
                                    if (chars.MoveNext())
                                        token = chars.Current;
                                    else
                                        return rawText.ToString(); // premature end of text

                                    data.Append(token);
                                }
                                else if (token == '^')
                                {
                                    if (chars.MoveNext())
                                        token = chars.Current;
                                    else
                                        return rawText.ToString(); // premature end of text

                                    // discard the code "^ " that defines super and subscript texts
                                    if (token != ' ') data.Append("^" + token);
                                }
                                else
                                {
                                    // replace the '#' stacking command by '/'
                                    // non command characters '#' are written as '\#'
                                    data.Append(token == '#' ? '/' : token);
                                }

                                if (chars.MoveNext())
                                    token = chars.Current;
                                else
                                    return rawText.ToString(); // premature end of text
                            }

                            rawText.Append(data);
                        }
                        else
                        {
                            // formatting commands of more than one character always terminate in ';'
                            // discard all information
                            while (token != ';')
                            {
                                if (chars.MoveNext())
                                    token = chars.Current;
                                else
                                    return rawText.ToString(); // premature end of text
                            }
                        }
                    }
                    else if (token == '{' | token == '}')
                    {
                        /* discard group markers */
                    }
                    else // char is what it is, a character
                        rawText.Append(token);
                }
            }

            return rawText.ToString();
        }
    }
}
