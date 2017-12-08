using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
namespace RichTextBoxExtension
{
    static class RichTextBoxExtension
    {
        public static void ChangeFontSize(this RichTextBox richTextBox, float amount)
        {
            int start = richTextBox.SelectionStart;
            int end = start + richTextBox.SelectionLength;
            for (int i = start; i < end; i++)
            {
                if (richTextBox.SelectionFont.Size + amount < 1 || richTextBox.SelectionFont.Size > Single.MaxValue) { continue; }
                richTextBox.SelectionStart = i;
                richTextBox.SelectionLength = 1;
                FontStyle style = richTextBox.SelectionFont.Style;
                richTextBox.SelectionFont = new Font(richTextBox.SelectionFont.FontFamily, richTextBox.SelectionFont.Size + amount);
                richTextBox.SelectionFont = new Font(richTextBox.SelectionFont, style);
            }
            richTextBox.SelectionStart = start;
            richTextBox.SelectionLength = end - start;
        }

        public static void ToggleFontStyle(this RichTextBox richTextBox, FontStyle style)
        {
            int start = richTextBox.SelectionStart;
            int end = start + richTextBox.SelectionLength;
            for (int i = start; i < end; i++)
            {
                richTextBox.SelectionStart = i;
                richTextBox.SelectionLength = 1;
                FontStyle newStyle = richTextBox.SelectionFont.Style;
                if (richTextBox.SelectionFont.Style.HasFlag(style))
                {
                    newStyle = richTextBox.SelectionFont.Style & ~style;
                }
                else
                {
                    newStyle = richTextBox.SelectionFont.Style | style;
                }
                richTextBox.SelectionFont = new Font(richTextBox.SelectionFont, newStyle);
            }
            richTextBox.SelectionStart = start;
            richTextBox.SelectionLength = end - start;
        }
    }
}
